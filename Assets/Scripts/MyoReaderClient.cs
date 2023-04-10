using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using TMPro;
using System.Linq;

#if !UNITY_EDITOR
using System.Threading.Tasks;
#else 
using System.Threading;
#endif

public class MyoReaderClient : MonoBehaviour {

#if !UNITY_EDITOR
    private Windows.Networking.Sockets.StreamSocket socket;
    private Task socketListenTask;
#endif

    private int port = 8090;
    private string host = "127.0.0.1";
    private string ipAddress = "192.168.2.51"; // Replace with computer IP
    private string portUWP = "8090";
    public string control = "Starting!";
    public float velocity = 0.0f;
    public int consecutive = 0;
    private StreamReader reader;
    private bool connected = false;
    private string lastControl = "";
    public List<float> velocities = new List<float>();


#if UNITY_EDITOR
    TcpClient socketClient;
#endif

    void Start () {
#if !UNITY_EDITOR
        ConnectSocketUWP();
#else
        ConnectSocketUnity();
        var readThread = new Thread(new ThreadStart(ListenForDataUnity));
        readThread.IsBackground = true;
        readThread.Start();
#endif
    }
    
    void Update () {
#if !UNITY_EDITOR
        if(socketListenTask == null || socketListenTask.IsCompleted)
        {
            socketListenTask = new Task(async() =>{ListenForDataUWP();});
            socketListenTask.Start();
        }
#endif
    }
    
#if UNITY_EDITOR
    void ConnectSocketUnity()
    {
        IPAddress ipAddress = IPAddress.Parse(host);

        socketClient = new TcpClient();
        try
        {
            socketClient.Connect(ipAddress, port);
        }

        catch
        {
            Debug.Log("error when connecting to server socket");
        }
    }
#else
    private async void ConnectSocketUWP()
    {
        try
        {
            socket = new Windows.Networking.Sockets.StreamSocket();
            Windows.Networking.HostName serverHost = new Windows.Networking.HostName(ipAddress);
            await socket.ConnectAsync(serverHost, portUWP);
            Stream streamIn = socket.InputStream.AsStreamForRead();
            reader = new StreamReader(streamIn, Encoding.UTF8);
            connected = true;
            control = "Connected";
        }
        catch (Exception e)
        {
            control = "Connection Error";
        }
    }
#endif

#if UNITY_EDITOR
    void ListenForDataUnity()
    {
        int data;
        while(true){
            byte[] bytes = new byte[socketClient.ReceiveBufferSize];
            NetworkStream stream = socketClient.GetStream();
            data = stream.Read(bytes, 0, socketClient.ReceiveBufferSize);
            string buffer = Encoding.UTF8.GetString(bytes, 0, data).Trim();
            string[] parts = buffer.Split(); 
            string tControl = parts[0];
            // Keep track of consecutive agreements
            if (tControl == lastControl) {
                consecutive += 1; 
            } else {
                consecutive = 0;
                velocities = new List<float>();
            }
            control = tControl;
            lastControl = control;
            velocities.Add(float.Parse(parts[1]));
            velocity = (float) (velocities.Count > 0 ? velocities.Average() : 0.0);
        }
    }
#else
    private void ListenForDataUWP()
    {
        try {
            // Keep track of consecutive agreements
            string buffer = reader.ReadLine().Trim();
            string[] parts = buffer.Split(); 
            string tControl = parts[0];
            if (tControl == lastControl) {
                consecutive += 1; 
            } else {
                consecutive = 0;
                velocities = new List<float>();
            }
            control = tControl;
            lastControl = control;
            velocities.Add(float.Parse(parts[1]));
            velocity = (float) (velocities.Count > 0 ? velocities.Average() : 0.0);
        } catch (Exception e) {
            //Do nothing
        }
    }
#endif
}