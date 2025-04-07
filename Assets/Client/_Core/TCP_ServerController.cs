using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class ClientInfo
{
    public TcpClient Client { get; set; }
    public NetworkStream Stream { get; set; }
}
public class TCP_ServerController : MonoBehaviour
{
    private TcpListener _listener;
    private Thread thread;
    public List<ClientInfo> _connectedClients = new List<ClientInfo>();
    ClientStatus_ServerSide _status;
    
    public string IPs = "";
    string MsgFromClient;
    public string messageReception;
    bool _IsMessageReceived;
    bool isRunning = true;
    public void _Initialize()
    {
        Debug.Log("Server Started");
        _status = GetComponent<ClientStatus_ServerSide>();
        _status._Initialize();
        ServerStart();
    }

    private void Update()
    {
        MessageReceivedSuccessfully();
        StopThread();
    }

    //Do some stuff after receiving message
    private void MessageReceivedSuccessfully()
    {
        if (_IsMessageReceived)
        {
            if (!string.IsNullOrEmpty(MsgFromClient))
            {
                Debug.Log("Message Received: "+MsgFromClient);
                MsgFromClient = "";
                _IsMessageReceived = false;
            }
        }
    }
     public string GetMessage()
    {
        return MsgFromClient;
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
        Debug.Log("My IP: " + localIP);
        return localIP;
    }

    //Start Server
    private void ServerStart()
    {
        thread = new Thread(new ThreadStart(ListeningOfAttemptedRequest));
        thread.IsBackground = true;
        thread.Start();
    }

    private void ListeningOfAttemptedRequest()
    {
        try 
        {
            _listener = new TcpListener(IPAddress.Parse(GetIPAddress()) , 8052);

            _listener.Start();
            ThreadPool.QueueUserWorkItem(ListenerWorker, null);
        }
        catch(SocketException socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    //Server is listening, waiting for the client's request or message
    private void ListenerWorker(object token)
    {
        Debug.Log("Waiting for the clients to connect");
        while (_listener != null)
        {
            var client = _listener.AcceptTcpClient();
            if (client.Connected) 
            {
                var clientInfo = new ClientInfo
                {
                    Client = client,
                    Stream = client.GetStream()
                };
                _connectedClients.Add(clientInfo);
                string clientIP = clientInfo.Client.Client.RemoteEndPoint.ToString();
                
                Debug.Log(_connectedClients.Count + " Client Connected");
                ThreadPool.QueueUserWorkItem(this.HandleClientsWorker, clientInfo);
                _status.AddStringToList(clientIP);
                SendIpToEveryClients();
            }
            else
            {
                Debug.Log("TcpClient is not connected. Skipping...");
            }
        }
    }

    //Handling multiple clients
    private void HandleClientsWorker(object token)
    {
        byte[] buffer = new byte[1024];
        var clientInfo = token as ClientInfo;
        var client = clientInfo.Client;
        var stream = clientInfo.Stream;
        
        try
        {
            while (client.Connected)
            {
                string clientIP = client.Client.RemoteEndPoint.ToString();
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Debug.Log("Client disconnected: " + client.Client.RemoteEndPoint);
                    _connectedClients.Remove(clientInfo);
                    Debug.Log(clientIP + " :CLIENT IP");
                    _status.RemoveStringFromList(clientIP);

                    SendIpToEveryClients();
                    Debug.Log("Disconnected Clinet");
                    break;
                }

                var incommingData = new byte[bytesRead];
                Array.Copy(buffer, 0, incommingData, 0, bytesRead);
                string clientMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                messageReception = clientIP + ">>>";
                MsgFromClient = clientMessage;
                _IsMessageReceived = true;
                Debug.Log("Received from " + clientIP + ": " + clientMessage);
                Array.Clear(buffer, 0, buffer.Length);
            }
        }
        catch (SocketException socketException)
        {
            Debug.Log(socketException.ToString());  
        } 
    }

    //Send message to anyone
    public void SendMessageFromServer(TcpClient token, string msg)
    {
        TcpClient client = token;
       
            if (client != null && client.Connected)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    if (stream.CanWrite)
                    {             
                        byte[] serverMessageAsByteArray = Encoding.ASCII.GetBytes(msg);
                        // Write byte array to socketConnection stream.            
                        stream.Write(serverMessageAsByteArray, 0, serverMessageAsByteArray.Length);
                        Debug.Log("Server sent his message - should be received by client");
                    }
                }
                catch (SocketException socketException)
                {
                    Debug.Log("Socket exception: " + socketException);
                    return;
                }
            }
            else
            {
                Debug.Log("Problem connectedTCPClient null");
                return;
            }
        //}
    }

    //Send available IP to connected clients
    public void SendIpToEveryClients()
    {
        Debug.Log("Send IP");
        var allClient = _status.allClients;
        if (_connectedClients != null)
        {
            for (int i = 0; i < _connectedClients.Count; i++)
            {
                for (int j = 0; j < allClient.Count; j++)
                {
                    if (!IPs.Contains(allClient[j]))
                    {
                        IPs += allClient[j] + "\n";
                    }
                }
                SendMessageFromServer(_connectedClients[i].Client, IPs);
            }
        }
    }

    private void OnDisable()
    {
        StopThreading();
    }

    public void StopThreading()
    {
        isRunning = false;
        Thread.Sleep(100);

        if (_listener != null)
        {
            _listener = null;
            Debug.Log("Server stopped listening...");
        }
        thread?.Abort();
    }
    private void StopThread()
    {
        if (!isRunning)
        {
            Thread.Sleep(100);
            if (_listener != null)
            {
                _listener.Stop();
                Debug.Log("Listener stopped");
            }
        }
    }
}

