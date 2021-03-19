using System;
using Mirage;
using Mirage.Logging;
using MirageTest.Scripts;
using MirageTest.Scripts.Messages;
using UnityEngine;
using NetworkPlayer = Mirage.NetworkPlayer;

public class ServerPlayerProxySpawner : MonoBehaviour
{
    static readonly ILogger logger = LogFactory.GetLogger(typeof(ServerPlayerProxySpawner));
    public NetworkServer server;
    public NetworkIdentity playerPrefab;
    public ServerObjectManager serverObjectManager;

    // Start is called before the first frame update
    public void Awake()
    {
        if (playerPrefab == null)
        {
            throw new InvalidOperationException("Assign a player in the PlayerSpawner");
        }
        
        server.Connected.AddListener(OnServerConnected);
        server.Authenticated.AddListener(OnServerAuthenticated);
        serverObjectManager = server.GetComponent<ServerObjectManager>();
    }

    private void OnServerConnected(INetworkPlayer connection)
    {
        if (server.authenticator == null)
        {
            connection.RegisterHandler<AddPlayerProxyMessage>(OnServerAddPlayerInternal);    
        }
    }
    
    private void OnServerAuthenticated(INetworkPlayer connection)
    {
        if (server.authenticator != null)
        {
            connection.RegisterHandler<AddPlayerProxyMessage>(OnServerAddPlayerInternal);    
        }
    }

    void OnServerAddPlayerInternal(INetworkPlayer conn, AddPlayerProxyMessage msg)
    {
        logger.Log("NetworkManager.OnServerAddPlayer");

        if (conn.Identity != null)
        {
            throw new InvalidOperationException("There is already a player for this connection.");
        }

        OnServerAddPlayer(conn, msg.userId);
    }

    public void OnServerAddPlayer(INetworkPlayer conn, string userId)
    {
        var  player = Instantiate(playerPrefab);
        player.GetComponent<PlayerProxy>().userId = userId;
        serverObjectManager.AddCharacter(conn, player.gameObject);
    }
}