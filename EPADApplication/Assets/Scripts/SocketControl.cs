using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;


public class ServerThread : ThreadedJob
{
    private bool isConnected = false;
    public ServerThread()
    {
    }

    protected override void ThreadFunction()
    {

        ServerSocket();
        ListeningServer();
    }

    protected override void OnFinished()
    {
    }

    void ListeningServer()
    {
        Debug.Log("starting listening server");
        Socket listenSocket = new Socket(AddressFamily.InterNetwork,
                                       SocketType.Stream,
                                       ProtocolType.Tcp);

        // bind the listening socket to the port
        int port = 9999;
        int backlog = 10;
        IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, port);
        listenSocket.Bind(ep);
        Debug.Log("binded");

        // start listening
        Debug.Log("listening");
        listenSocket.Listen(backlog);

        Debug.Log("about to accept");
        listenSocket.Accept();
        SocketAsyncEventArgs e = new SocketAsyncEventArgs();

        byte[] bytes = new byte[256];
        int byteCount  = listenSocket.Receive(bytes,0,listenSocket.Available, SocketFlags.None);

        if(byteCount > 0)
        {
            Debug.Log(Encoding.UTF8.GetString(bytes));
        }

    }

    void ServerSocket()
    {
        var Server = new UdpClient(9999);
        var ResponseData = Encoding.ASCII.GetBytes("Connected");

        while (!isConnected)
        {
            Debug.Log("waiting for client to be found");
            var ClientEp = new IPEndPoint(IPAddress.Any, 0);
            var ClientRequestData = Server.Receive(ref ClientEp);
            var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);

            UnityEngine.Debug.Log("Received " + ClientRequest + " from " + ClientEp.Address.ToString() + " sending response");
            Server.Send(ResponseData, ResponseData.Length, ClientEp);
            if (ClientRequest == "!")
            {
                isConnected = true;
                Debug.Log("established connection");
            }
        }
        Debug.Log("closing server");
        Server.Close();

    }

    

    public virtual void close()
    {
        isConnected = false;
    }


}

public class ClientThread : ThreadedJob
{

    private bool isConnected = false;
    public ClientThread()
    {

    }

    protected override void ThreadFunction()
    {
        //StartClient();

       
        IPAddress serverAddress = ClientSocket();

        if (serverAddress != IPAddress.Parse("0.0.0.0"))
        {
            //ConnectToServer(serverAddress);
            TCPClient(serverAddress);
        }

    }

    void TCPClient(IPAddress servAddr)
    {
        Debug.Log("outside try");
        try
        {
            Debug.Log("about to create client");
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer 
            // connected to the same address as specified by the server, port
            // combination.
            TcpClient client = new TcpClient(servAddr.ToString(), 9999);
            Debug.Log("created client");
            isConnected = true;
            while (isConnected)
            {
                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes("nah");

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Debug.Log("Sent message");
                //stream.Close();

                Debug.Log("going to sleep");
                Thread.Sleep(1000);

            }

            //close the client
            client.Close();
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
    }

    void ConnectToServer(IPAddress servAddr)
    {
        Debug.Log("establishing connection via socket");
        Socket s = new Socket(AddressFamily.InterNetwork,
         SocketType.Stream,
         ProtocolType.Tcp);
        s.Connect(servAddr,9999);
        Debug.Log("connected");
        isConnected = true;

        while(isConnected)
        {
            string msg = "ok";
            byte[] bytes = new byte[256];
            bytes = Encoding.UTF8.GetBytes(msg.ToCharArray());
            Debug.Log("about to send");
            s.Send(bytes);
            Debug.Log("sent; about to sleep");
            Thread.Sleep(2000);
            Debug.Log("back");
        }

        s.Close();




    }

    public static void StartClient()
    {
        // Data buffer for incoming data.  
        byte[] bytes = new byte[1024];

        // Connect to a remote device.  
        try
        {
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("192.168.0.102"), 9999);

            // Create a TCP/IP  socket.  
            Socket sender = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                sender.Connect(remoteEP);

               Debug.Log("Socket connected to " +
                    sender.RemoteEndPoint.ToString());

                // Encode the data string into a byte array.  
                byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                // Send the data through the socket.  
                int bytesSent = sender.Send(msg);
                Debug.Log("sent byte count " + bytesSent.ToString());
                // Receive the response from the remote device.  
                int bytesRec = sender.Receive(bytes);
                Debug.Log("Echoed test = " +
                    Encoding.ASCII.GetString(bytes, 0, bytesRec));

                // Release the socket.  
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();

            }
            catch (ArgumentNullException ane)
            {
                Debug.Log("ArgumentNullException : " + ane.ToString());
            }
            catch (SocketException se)
            {
               Debug.Log("SocketException : "+ se.ToString());
            }
            catch (Exception e)
            {
                Debug.Log("Unexpected exception : " + e.ToString());
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }


    protected override void OnFinished()
    {
        isConnected = false;
    }

    IPAddress ClientSocket()
    {
        var Client = new UdpClient();
        var RequestData = Encoding.ASCII.GetBytes("!");
        var ServerEp = new IPEndPoint(IPAddress.Any, 0);

        Client.EnableBroadcast = true;
        Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 9999));
        Debug.Log("waiting to receive as client");
        //var ServerResponseData = Client.Receive(ref ServerEp);
        //var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);

        var ServerResponseData = Client.ReceiveAsync();
        var ServerResponse = ServerResponseData.Result.Buffer.ToString();

        IPAddress targetAddr = ServerResponseData.Result.RemoteEndPoint.Address;

        UnityEngine.Debug.Log("Received " + ServerResponse + " from " + ServerResponseData.Result.RemoteEndPoint.Address.ToString());
        Client.Close();

        return targetAddr;

    }

    public virtual void close()
    {
        isConnected = false;
    }
}

public class SocketControl : MonoBehaviour
{
    ServerThread _server;
    ClientThread _client;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine("RunServer");
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine("RunClient");
        }
    }

    IEnumerator RunClient()
    {
        UnityEngine.Debug.Log("starting client thread");
        _client = new ClientThread();
        _client.Start();
        yield return null;
    }

    IEnumerator RunServer()
    {
        UnityEngine.Debug.Log("starting server thread");
        _server = new ServerThread();
        _server.Start();
        yield return null;
    }

    private void OnApplicationQuit()
    {
        if(_client!=null)
        {
            _client.Abort();
        }

        if(_server!=null)
        {
            _server.Abort();
        }
    }
}
