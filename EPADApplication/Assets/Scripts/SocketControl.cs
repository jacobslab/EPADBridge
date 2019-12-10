using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using UnityEngine;


public class ServerThread : ThreadedJob
{
    private bool clientFound = false;
    public ServerThread()
    {
    }

    protected override void ThreadFunction()
    {

        ServerSocket();
    }

    protected override void OnFinished()
    {
    }

    void ServerSocket()
    {
        var Server = new UdpClient(9999);
        var ResponseData = Encoding.ASCII.GetBytes("SomeResponseData");

        while (!clientFound)
        {
            Debug.Log("waiting for client to be found");
            var ClientEp = new IPEndPoint(IPAddress.Any, 0);
            var ClientRequestData = Server.Receive(ref ClientEp);
            var ClientRequest = Encoding.ASCII.GetString(ClientRequestData);

            UnityEngine.Debug.Log("Received " + ClientRequest + " from " + ClientEp.Address.ToString() + " sending response");
            Server.Send(ResponseData, ResponseData.Length, ClientEp);
        }

    }

    

    public virtual void close()
    {

    }


}

public class ClientThread : ThreadedJob
{
    public ClientThread()
    {

    }

    protected override void ThreadFunction()
    {

        ClientSocket();
    }

    protected override void OnFinished()
    {
    }

    void ClientSocket()
    {
        var Client = new UdpClient();
        var RequestData = Encoding.ASCII.GetBytes("SomeRequestData");
        var ServerEp = new IPEndPoint(IPAddress.Any, 0);

        Client.EnableBroadcast = true;
        Client.Send(RequestData, RequestData.Length, new IPEndPoint(IPAddress.Broadcast, 9999));
        Debug.Log("waiting to receive as client");
        var ServerResponseData = Client.Receive(ref ServerEp);
        var ServerResponse = Encoding.ASCII.GetString(ServerResponseData);
        UnityEngine.Debug.Log("Recived " + ServerResponse + " from " + ServerEp.Address.ToString());
        Client.Close();

    }
}

public class SocketControl : MonoBehaviour
{
    ServerThread _server;
    ClientThread _client;
    // Start is called before the first frame update
    void Start()
    {
        _server = new ServerThread();
        _client = new ClientThread();

        
        _server.Start();
        _client.Start();
        Debug.Log("starting thread");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
