using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TCP_ClientController : MonoBehaviour
{
    private TcpClient tcpClient;
    private Thread clientThread;

    private ClientStatus_ClientSide _status;
    private StartAs startAs;
    private ConnectViaInput connectViaInput;
    string MsgFromServer = "";
    public string myIP, socketException = "";
    public string serverIP = "";
    public bool _IsMessageReceived;
    bool isRunning = true;

    public static UnityAction onConnect;
    public static UnityAction onServerDisconnect;
    public void _Initialze()
    {
        Debug.Log("Client Started");
        _status = GetComponent<ClientStatus_ClientSide>();
        startAs = FindFirstObjectByType<StartAs>();
        connectViaInput = FindFirstObjectByType<ConnectViaInput>();
        ConnectToServer();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessage("Check connection");
        }
        //Do some stuff after receiving message
        if (_IsMessageReceived)
        {
            Debug.Log("client: " + MsgFromServer);
            _status.ConnectedClientList(MsgFromServer);
            if(MsgFromServer.Contains("Server Disconnected"))
            {
                Debug.Log("Try connection again");
                onServerDisconnect?.Invoke();
            }

            MsgFromServer = "";
            _IsMessageReceived = false;
        }

        //Close TCP connections
        if (!isRunning)
        {
            Thread.Sleep(100);
            if (tcpClient != null && tcpClient.Connected)
            {
                tcpClient.Close(); 
                Debug.Log("Server stopped listening.");
            }
        }
    }

    //Connect to server
    private void ConnectToServer()
    {
        try
        {
            clientThread = new Thread(new ThreadStart(AttemptConnection));
            clientThread.IsBackground = true;
            clientThread.Start();
        }
        catch(Exception e)
        {
            Debug.Log("On Client connect exception: " + e);
        }
    }

    //Send request to server to connect
    private void AttemptConnection()
    {
        myIP = GetIPAddress();
        Debug.Log("MY IP: " + myIP);
        try
        {
            if (startAs != null)
            {
                tcpClient = new TcpClient(startAs.ipKey, 8052); 
            }

            if(connectViaInput != null)
            {
                tcpClient = new TcpClient(connectViaInput.ipKey,  8052);
            }

            //Testing purpose
            //tcpClient = new TcpClient(/*ipAddressInputField.text*/ startAs.ipKey, 8052);  //Testing purpose

            Debug.Log("Attempting Connection");

            byte[] buffer = new byte[1024];
            isRunning = true;
            while (isRunning)
            {
                using (NetworkStream networkStream = tcpClient.GetStream())
                {
                    int byteLength;
                    //Debug.Log("Get Stream");
                    while (isRunning && networkStream.CanRead)
                    {
                        if ((byteLength = networkStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            var dataToBeCopy = new byte[byteLength];
                            Array.Copy(buffer, 0, dataToBeCopy, 0, byteLength);
                            string serverMessage = Encoding.ASCII.GetString(dataToBeCopy);
                            MsgFromServer = serverMessage;
                            _IsMessageReceived = true;
                            Debug.Log("Message from server: " + serverMessage);
                            serverMessage = "";
                            onConnect?.Invoke();
                        }
                    }                    
                }
            }
            tcpClient.Close();
        }
        catch(SocketException e)
        {
            socketException = e.ToString();
            //isRunning = false;  
            //DisconnectClient(tcpClient);
            //ConnectToServer();
            Debug.Log(" OnConnect: " +e.ToString());
        }       
    }

    //Close TCP connections
    private void DisconnectClient(TcpClient tcpClient)
    {
        if (tcpClient != null && tcpClient.Connected)
        {
            tcpClient.Close();

            //tcpClient.BeginConnect();
        }
    }

    //Send message to server
    public new void SendMessage(string msg)
    {
        if (tcpClient == null)
        {
            return;
        }
        try
        {
            NetworkStream networkStream = tcpClient.GetStream();
            if (networkStream.CanWrite)
            {                
                //Debug.Log("CLIENT MSG " + clientMessage);
                byte[] buffer = Encoding.ASCII.GetBytes(msg);
                networkStream.Write(buffer, 0, buffer.Length);

                Debug.Log("Client sent his message - should be received by Server: " + msg);               
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e.ToString());
        }
    }

    //Get your device IP
    private string GetIPAddress()
    {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
            }
        }
        return localIP;
    }

    //Use Anywhere with reference
    public string GetMessage()
    {
        return MsgFromServer;
    }

    //Close TCP connections
    public void StopClient()
    {
        isRunning = false;
        Thread.Sleep(100);
        if (tcpClient != null && tcpClient.Connected)
        {

            if (tcpClient.GetStream() != null)
                tcpClient.GetStream().Close();
            tcpClient.Close();
            Debug.Log("Client socket connection closed.....");
        }
        clientThread?.Abort();
        clientThread = null;
    }
    private void OnDisable()
    {
        StopClient();
    }
}

