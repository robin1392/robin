using System.Collections;
using System.Collections.Generic;
using Service.Net;

public class NetClientGameSession : GameSessionClient
{
    public NetClientPlayer GamePlayer { get; set; }


    public NetClientGameSession()
    {
        GamePlayer = new NetClientPlayer();
    }


    protected override void OnConnectClient(ClientSession clientSession)
    {
        GamePlayer.SetClientSession(clientSession);
    }

    protected override void OnReconnectClient(ClientSession clientSession)
    {
    }

    protected override void OnOfflineClient(ClientSession clientSession)
    {
    }

    protected override void OnDisconnectClient(ClientSession clientSession)
    {
    }
}
