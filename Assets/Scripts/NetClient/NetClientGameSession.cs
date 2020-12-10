using System.Collections;
using System.Collections.Generic;
using Service.Net;

public class NetClientGameSession : GameSessionClient
{
    public NetClientPlayer Player { get; set; }


    public NetClientGameSession()
    {
        Player = new NetClientPlayer();
    }


    protected override void OnConnectClient(ClientSession clientSession)
    {
        Player.SetClientSession(clientSession);
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
