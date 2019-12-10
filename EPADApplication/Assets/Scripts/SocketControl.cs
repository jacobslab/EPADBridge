using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
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

        IPAddress serverAddress = ClientSocket();

        if(serverAddress != IPAddress.Parse("0.0.0.0"))
        {
            ConnectToServer(serverAddress);
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
            s.Send(bytes);
            Thread.Sleep(2000);
        }

        s.Close();




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
}
