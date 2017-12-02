using HoloToolkit.Unity;
using kadmium_sacn_core;
using System;


using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;



#if !UNITY_EDITOR
using Windows.Networking.Sockets;
using Windows.Networking;
#else
using System.Net;
using System.Net.Sockets;
#endif

public class NetworkHandler : Singleton<NetworkHandler> {

    public int port;
    public string ip;
    [Tooltip("The UnityEvent to be invoked when the Message is recieved.")]
    public StageEventManager stagemanager;
    private Byte[] buffer;
    private Stack<byte[]> stack = new Stack<byte[]>();
    private object stackLock = new object();

#if !UNITY_EDITOR
    private DatagramSocket client;
    private HostName localEp;
     
#else
    private UdpClient client;
    private IPEndPoint localEp;
    private IPAddress multicastaddress;


#endif


    // Use this for initialization
#if !UNITY_EDITOR
     async void Start () {
        try
        {
         
            client = new DatagramSocket();
            
            client.Control.MulticastOnly = true;
             
            client.MessageReceived += OnMessageReceived;

            await client.BindServiceNameAsync(port.ToString());
            client.JoinMulticastGroup(new HostName(ip));

#else
    void Start()
    {
        try
        {
            client = new UdpClient();
            localEp = new IPEndPoint(IPAddress.Any, port);

            client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            client.ExclusiveAddressUse = false;
            client.Client.Bind(localEp);

            multicastaddress = IPAddress.Parse(ip);
            client.JoinMulticastGroup(multicastaddress);

            client.BeginReceive(new AsyncCallback(OnUdpData), client);

#endif


            Debug.Log("Network ok");
        }catch(Exception ex)
        {
            Debug.Log("Fehler beim connecten");
#if !UNITY_EDITOR
            client.Dispose();
#else
            client.Close();
#endif
            client = null;
            Debug.LogException(ex);
        }

    }

    // Update is called once per frame
    void Update () {
        byte[] data = null;
        lock (stackLock)
        {
            if (stack.Count > 0)
            {
                data = stack.Pop();
                stack.Clear();
            }
        }

        if (data != null)
        {
            SACNPacket packet = null;
            try
            {
                 packet = SACNPacket.Parse(data);
                
            }catch(Exception ex)
            {
                Debug.Log("Error Parsing Message from Network");
                return;
            }
            stagemanager.OnMessageReceived(packet);

        }
            
        

    }


#if !UNITY_EDITOR
    private void OnMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
    {
        var length = args.GetDataReader().UnconsumedBufferLength;
        byte[] message = new byte[length];
        args.GetDataReader().ReadBytes(message);

        lock (stackLock)
        {
            stack.Push(message);
        }

    }
#else
    private void OnUdpData(IAsyncResult result)
    {
        // this is what had been passed into BeginReceive as the second parameter:
        UdpClient socket = result.AsyncState as UdpClient;
        // points towards whoever had sent the message:
 
        // get the actual message and fill out the source:
        Byte[] message = socket.EndReceive(result, ref localEp);
        lock (stackLock)
        {
            stack.Push(message);
        }

        // schedule the next receive operation once reading is done:
        socket.BeginReceive(new AsyncCallback(OnUdpData), socket);
    }
#endif
    override protected void OnDestroy()
    {
        base.OnDestroy();
        if(client != null)
#if !UNITY_EDITOR
            client.Dispose();
#else
            client.Close();
#endif
    }
}
