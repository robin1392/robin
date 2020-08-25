using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : Singleton<NetworkManager>
{
    
    
    #region net variable
    
    // web
    public WebNetworkCommon webNetCommon { get; private set; }
    public WebPacket webPacket { get; private set; }

    // socket
    private SocketManager _clientSocket = null;
    
    //
    private SocketSendPacket _packetSend;
    public SocketSendPacket SendSocket
    {
        get => _packetSend;
        private set => _packetSend = value;
    }
    
    #endregion
    
    
    
    #region unity base

    public override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        InitNetwork();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSocket();
    }

    public override void OnDestroy()
    {
        DestroyNetwork();
        
        base.OnDestroy();
    }

    #endregion
    
    
    #region net add

    public void InitNetwork()
    {
        webNetCommon = this.gameObject.AddComponent<WebNetworkCommon>();
        webPacket = this.gameObject.AddComponent<WebPacket>();
        
        
        _clientSocket = new SocketManager();
        _clientSocket.Init(new SocketRecvPacket());
        _packetSend = new SocketSendPacket(_clientSocket);
    }

    
    public void DestroyNetwork()
    {
        GameObject.Destroy(webPacket);
        GameObject.Destroy(webNetCommon);

        _packetSend = null;
        _clientSocket = null;
    }
    #endregion
    
    
    #region socket

    public void UpdateSocket()
    {
        if(_clientSocket != null)
            _clientSocket.Update();
    }

    public void DisconnectSocket()
    {
        _clientSocket.Disconnect();
    }

    public bool IsConnect()
    {
        return _clientSocket.IsConnected();
    }
    #endregion
    
}

