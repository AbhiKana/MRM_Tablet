using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

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

    public static UnityAction OnConnect;
    public static UnityAction OnServerDisconnected;
    public static UnityAction<string> OnMessageReceived;
    

    public void _Initialze()
    {
        Debug.Log("Client Started");
        _status = GetComponent<ClientStatus_ClientSide>();
        startAs = FindFirstObjectByType<StartAs>();
        connectViaInput = FindFirstObjectByType<ConnectViaInput>();
        //ConnectToServer();
        StartCoroutine(ConnectToServer_New());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SendMessage("Check connection");
        }

        /*if (_IsMessageReceived)
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
        }*/

        //Close TCP connections
        /*if (!isRunning)
        {
            Thread.Sleep(100);
            if (tcpClient != null && tcpClient.Connected)
            {
                tcpClient.Close();
                Debug.Log("Server stopped listening.");
            }
        }*/
    }

    //Connect to server
    private void ConnectToServer()
    {
        string ip = connectViaInput?.ipKey ?? startAs?.ipKey;
        int port = 8052;

        if (!string.IsNullOrEmpty(ip) && Connect(ip, port))
        {
            try
            {
                clientThread = new Thread(new ThreadStart(AttemptConnection));
                clientThread.IsBackground = true;
                clientThread.Start();
            }
            catch (Exception e)
            {
                Debug.LogError("Client thread start exception: " + e);
            }
        }
        else
        {
            Debug.LogWarning("IP is null or connection failed.");
        }
    }

    private IEnumerator ConnectToServer_New()
    {
        //StopClient();
        yield return new WaitForSeconds(0.1f);
        string ip = connectViaInput?.ipKey ?? startAs?.ipKey;
        int port = 8052;

        if (!string.IsNullOrEmpty(ip))
        {
            ConnectWithTimeout(ip, port, 5f, (success) =>
            {
                if (!success)
                {
                    Debug.LogWarning("Connection failed to: " + ip);
                }
            });
        }
        else
        {
            Debug.LogWarning("IP is null or empty");
        }
    }

    private bool Connect(string ip, int port)
    {
        try
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(ip, port);
            Debug.Log("Connected to server: " + ip + ":" + port);

            UnityMainThreadDispatcher.Enqueue(() =>
            {
                OnConnect?.Invoke();
            });

            return true;
        }
        catch (SocketException ex)
        {
            Debug.LogError("Connection failed: " + ex.Message);
            return false;
        }
    }

    //Send request to server to connect
    private void AttemptConnection()
    {
        myIP = GetIPAddress();
        Debug.Log("MY IP: " + myIP);

        byte[] buffer = new byte[1024];
        isRunning = true;

        try
        {
            using (NetworkStream networkStream = tcpClient.GetStream())
            {
                int byteLength;

                while (isRunning && networkStream.CanRead)
                {
                    if ((byteLength = networkStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        if (byteLength == 0)
                        {
                            HandleServerDisconnection(byteLength);
                            break;
                        }

                        byte[] dataToBeCopy = new byte[byteLength];
                        Array.Copy(buffer, 0, dataToBeCopy, 0, byteLength);
                        string serverMessage = Encoding.ASCII.GetString(dataToBeCopy);
                        Debug.Log("Message from server: " + serverMessage);

                        UnityMainThreadDispatcher.Enqueue(() =>
                        {
                            OnMessageReceived?.Invoke(serverMessage);
                        });
                    }
                }
            }
        }
        catch (SocketException e)
        {
            socketException = e.ToString();
            Debug.Log("OnConnect: " + e.ToString());
        }
        finally
        {
            tcpClient?.Close();
            isRunning = false;
        }
    }

    private void HandleServerDisconnection(int byteLength)
    {
        Debug.LogWarning("Server disconnected or closed.");
        isRunning = false;

        //UnityMainThreadDispatcher.Enqueue(() =>
        //{
        //    //OnServerDisconnected?.Invoke();
        //});
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

    #region Connection_New
    private void StartMessageListeningThread()
    {
        if (clientThread == null || !clientThread.IsAlive)
        {
            clientThread = new Thread(AttemptConnection);
            clientThread.IsBackground = true;
            clientThread.Start();
        }
    }

    public void ConnectWithTimeout(string ip, int port, float timeoutSeconds, Action<bool> callback)
    {
        StartCoroutine(ConnectCoroutine(ip, port, timeoutSeconds, callback));
    }

    private IEnumerator ConnectCoroutine(string ip, int port, float timeoutSeconds, Action<bool> callback)
    {
        bool connected = false;
        bool timedOut = false;
        bool validated = false;
        float startTime = Time.time;

        Thread connectThread = new Thread(() =>
        {
            try
            {
                tcpClient = new TcpClient();
                var result = tcpClient.BeginConnect(ip, port, null, null);

                // Wait for connection or timeout
                connected = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(timeoutSeconds));

                if (connected)
                {
                    tcpClient.EndConnect(result);

                    // NEW: Verify connection is actually usable
                    validated = VerifyConnectionActive(tcpClient);

                    if (validated)
                    {
                        serverIP = ip;
                        Debug.Log("Connected to server: " + ip + ":" + port);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Connection error: " + e.Message);
                //In case if server is not running
                StopClient();
            }
        });

        connectThread.IsBackground = true;
        connectThread.Start();

        while (connectThread.IsAlive && Time.time - startTime < timeoutSeconds)
        {
            yield return null;
        }

        if (connectThread.IsAlive)
        {
            Debug.Log("Is Connection Problem");
            tcpClient?.Close();
            connectThread.Abort();
            timedOut = true;
        }

        UnityMainThreadDispatcher.Enqueue(() =>
        {
            if (connected && validated) // CHANGED: Check both flags
            {
                OnConnect?.Invoke();
                StartMessageListeningThread();
                callback?.Invoke(true);
            }
            else
            {
                callback?.Invoke(false);
                Debug.Log(timedOut ? "Connection timed out" : (connected ? "Connection failed validation" : "Connection failed"));

                //Destroy(gameObject);
            }
        });
    }

    private bool VerifyConnectionActive(TcpClient client)
    {
        try
        {
            if (!client.Connected) return false;

            // Test 2: Small read/write test
            var stream = client.GetStream();
            if (!stream.CanWrite || !stream.CanRead) return false;

            // Test 3: Send ping if your protocol supports it
            // (Implement protocol-specific verification here)

            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion

    //Close TCP connections

    public void StopClient()
    {
        Debug.Log("Client Disconnect");
        isRunning = false;

        try
        {
            // First stop the thread
            if (clientThread != null && clientThread.IsAlive)
            {
                clientThread.Abort();
                clientThread = null;
            }

            // Then close the TCP connection
            if (tcpClient != null)
            {
                if (tcpClient.Connected)
                {
                    var stream = tcpClient.GetStream();
                    if (stream != null)
                    {
                        stream.Close();
                        Thread.Sleep(50); // Small delay to ensure stream closure
                    }
                    tcpClient.Close();
                }
                tcpClient.Dispose();
                tcpClient = null;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error during client shutdown: " + e.Message);
        }

        Debug.Log("Client socket connection closed");
    }

    private void OnApplicationQuit()
    {
        StopClient();
    }
}

