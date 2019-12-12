using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;
using UnityEngine;

public class ListenerServer:ThreadedJob
{

    public bool isRunning = false;
    public ListenerServer()
    {
        isRunning = true;
    }

   

    protected override void ThreadFunction()
    {
        RunListener();
    }

    public void RunListener()
    {

        TcpListener server = null;
        try
        {
            // Set the TcpListener on specified port.
            Int32 port = 10001; 
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName()); //gets local IP address

            server = new TcpListener(ipHostInfo.AddressList[0], port);
            Debug.Log("iphost info " + ipHostInfo.AddressList[0]);

            // Start listening for client requests.
            server.Start();

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            // Enter the listening loop.
            while (isRunning)
            {
                Debug.Log("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("Connected!");
                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Debug.Log("Received: " + data);



                    // DO SOMETHING WITH THE DATA RECIEVED BY THE SOCKET ON THIS PORT

                    //fire the "DeviceStatusChanged" function by passing a true/false boolean of the device connection status

                    //DeviceStatusChanged(true);

                }

                // Shutdown and end connection
                client.Close();
            }
        }
        catch (SocketException e)
        {
            Debug.Log("SocketException: " + e);
        }
        finally
        {
            // Stop listening for new clients.
            server.Stop();
        }
    }

    public void DeviceStatusChanged(bool deviceConnectionStatus)
    {
        EPADApplication.Instance.UpdateNeuralConnectionStatus(deviceConnectionStatus); //updates the UI o
    }

    protected override void OnFinished()
    {
        
    }

    public virtual void close()
    {
        isRunning = false;
    }
}

public class NeuralDeviceConnectivity : MonoBehaviour
{
    ListenerServer _listenerServer;

    // Start is called before the first frame update
    void Start()
    {

        _listenerServer = new ListenerServer();
        _listenerServer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        if(_listenerServer!=null)
        {
            _listenerServer.close();
        }
    }



}
