using System;
using Mirage;
using UnityEngine;

public class PlayerRegisterer : MonoBehaviour
{
    static readonly ILogger logger = LogFactory.GetLogger(typeof(PlayerRegisterer));

    public NetworkClient[] clients;
    public NetworkServer server;
    public NetworkIdentity playerPrefab;
    public ServerObjectManager serverObjectManager;

    // Start is called before the first frame update
    public virtual void Start()
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
            server.Authenticated.AddListener(OnServerAuthenticated);
            serverObjectManager = server.GetComponent<ServerObjectManager>();
        }
    }

    private void OnServerAuthenticated(INetworkConnection connection)
    {
        connection.RegisterHandler<AddPlayerMessage>(OnServerAddPlayerInternal);
    }

    private void OnClientAuthenticated(INetworkConnection connection)
    {
        connection.Send(new AddPlayerMessage());
    }

    void OnServerAddPlayerInternal(INetworkConnection conn, AddPlayerMessage msg)
    {
        logger.Log("NetworkManager.OnServerAddPlayer");

        if (conn.Identity != null)
        {
            throw new InvalidOperationException("There is already a player for this connection.");
        }

        OnServerAddPlayer(conn);
    }

    public void OnServerAddPlayer(INetworkConnection conn)
    {
        NetworkIdentity player = Instantiate(playerPrefab);
        serverObjectManager.AddPlayerForConnection(conn, player.gameObject);
    }
}