using System;
using Mirage;
using Mirage.Logging;
using UnityEngine;

public class PlayerRegisterer : MonoBehaviour
{
    static readonly ILogger logger = LogFactory.GetLogger(typeof(PlayerRegisterer));

    public NetworkClient[] clients;
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

        clients = FindObjectsOfType<NetworkClient>();
        foreach (var client in clients)
        {
            client.Authenticated.AddListener(OnClientAuthenticated);
            client.GetComponent<ClientObjectManager>().RegisterPrefab(playerPrefab);
        }

        server = FindObjectOfType<NetworkServer>();
        if (server != null)
        {
            server.Connected.AddListener(OnServerConnected);
            server.Authenticated.AddListener(OnServerAuthenticated);
            serverObjectManager = server.GetComponent<ServerObjectManager>();
        }
    }

    private void OnServerConnected(INetworkPlayer connection)
    {
        if (server.authenticator == null)
        {
            connection.RegisterHandler<AddCharacterMessage>(OnServerAddPlayerInternal);    
        }
    }
    
    private void OnServerAuthenticated(INetworkPlayer connection)
    {
        if (server.authenticator != null)
        {
            connection.RegisterHandler<AddCharacterMessage>(OnServerAddPlayerInternal);    
        }
    }

    private void OnClientAuthenticated(INetworkPlayer connection)
    {
        connection.Send(new AddCharacterMessage());
    }

    void OnServerAddPlayerInternal(INetworkPlayer conn, AddCharacterMessage msg)
    {
        logger.Log("NetworkManager.OnServerAddPlayer");

        if (conn.Identity != null)
        {
            throw new InvalidOperationException("There is already a player for this connection.");
        }

        OnServerAddPlayer(conn);
    }

    public void OnServerAddPlayer(INetworkPlayer conn)
    {
        NetworkIdentity player = Instantiate(playerPrefab);
        serverObjectManager.AddCharacter(conn, player.gameObject);
    }
}