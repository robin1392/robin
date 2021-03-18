using Mirage;
using Mirage.Logging;
using MirageTest.Scripts;
using MirageTest.Scripts.Entities;
using Percent.Platform;
using RandomWarsResource.Data;
using UnityEngine;

public class PlayerProxy : NetworkBehaviour
{
    static readonly ILogger logger = LogFactory.GetLogger(typeof(PlayerProxy));

    private void Awake()
    {
        if (NetIdentity == null)
        {
            return;
        }

        NetIdentity.OnStartClient.AddListener(OnStartClient);
        NetIdentity.OnStopClient.AddListener(OnStopClient);
        NetIdentity.OnStartLocalPlayer.AddListener(OnStartLocalPlayer);
        
        NetIdentity.OnStartServer.AddListener(OnStartServer);
        NetIdentity.OnStopServer.AddListener(OnStopServer);
    }

    private void OnStopServer()
    {
        var server = Server as RWNetworkServer;
        server.RemovePlayerProxy(this);
        
        if (Server.LocalClientActive)
        {
            OnStopClient();
        }
    }

    private void OnStartServer()
    {
        var server = Server as RWNetworkServer;
        server?.AddPlayerProxy(this);
        
        if (Server.LocalClientActive)
        {
            OnStartClient();
            
            var client = Client as RWNetworkClient;
            client.localPlayerId = UserInfoManager.Get().GetUserInfo().userID;
        }
    }

    private void OnStartClient()
    {
        var client = Client as RWNetworkClient;
        client.AddPlayerProxy(this);
    }

    private void OnStopClient()
    {
        var client = Client as RWNetworkClient;
        client.RemovePlayerProxy(this);
    }

    private void OnStartLocalPlayer()
    {
        if (IsLocalPlayer)
        {
            var client = Client as RWNetworkClient;
            var authData = (ConnectionToServer.AuthenticationData as AuthDataForConnection);
            client.localPlayerId = authData.PlayerId;
        }
    }
    
    public void MergeDice(int sourceDiceFieldIndex, int targetDiceFieldIndex)
    {
        if (IsLocalClient)
        {
            MergeDiceInternal(sourceDiceFieldIndex, targetDiceFieldIndex);
            return;
        }
        
        MergeDiceOnServer(sourceDiceFieldIndex, targetDiceFieldIndex);
    }
    
    [ServerRpc]
    public void MergeDiceOnServer(int sourceDiceFieldIndex, int targetDiceFieldIndex)
    {
        MergeDiceInternal(sourceDiceFieldIndex, targetDiceFieldIndex);
    }

    void MergeDiceInternal(int sourceDiceFieldIndex, int targetDiceFieldIndex)
    {
        var playerState = GetPlayerState();
        playerState.MergeDice(sourceDiceFieldIndex, targetDiceFieldIndex);
    }
    
    public void UpgradeIngameLevel(int diceId)
    {
        if (IsLocalClient)
        {
            UpgradeIngameLevelInternal(diceId);
            return;
        }
        
        UpgradeIngameLevelOnServer(diceId);
    }

    [ServerRpc]
    public  void UpgradeIngameLevelOnServer(int diceId)
    {
        UpgradeIngameLevelInternal(diceId);
    }

    void UpgradeIngameLevelInternal(int diceId)
    {
        var playerState = GetPlayerState();
        playerState.UpgradeIngameLevel(diceId);
    }
    
    public void GetRandomDice()
    {
        if (IsLocalClient)
        {
            GetRandomDiceOnServerInternal();
            return;
        }
        
        GetRandomDiceOnServer();
    }

    [ServerRpc]
    public void GetRandomDiceOnServer()
    {
        GetRandomDiceOnServerInternal();
    }

    void GetRandomDiceOnServerInternal()
    {
        logger.Log($"[GetDice]");
        var playerState = GetPlayerState();
        playerState.GetDice();
    }
    
    public void GetDice(int deckIndex, int fieldIndex)
    {
        GetDiceOnServer(deckIndex, fieldIndex);
    }

    [Server]
    PlayerState GetPlayerState()
    {
        var server = Server as RWNetworkServer;
        if (Server.LocalClientActive)
        {
            var playerId = UserInfoManager.Get().GetUserInfo().userID;
            return server.serverGameLogic.GetPlayerState(playerId);   
        }
        else
        {
            var auth = ConnectionToClient.AuthenticationData as AuthDataForConnection;
            var playerId = auth.PlayerId;    
            return server.serverGameLogic.GetPlayerState(playerId);
        }
    }
    
    [ServerRpc]
    public void GetDiceOnServer(int deckIndex, int fieldIndex)
    {
        logger.Log($"[GetDice]");
        var playerState = GetPlayerState();
        playerState.GetDice(deckIndex, fieldIndex);
    }
    
    public void UpgradeSp()
    {
        if (IsLocalClient)
        {
            UpgradeSpInternal();
            return;
        }
        UpgradeSpOnServer();
    }

    [ServerRpc]
    public void UpgradeSpOnServer()
    {
        UpgradeSpInternal();
    }

    void UpgradeSpInternal()
    {
        var playerState = GetPlayerState();
        playerState.UpgradSp();
    }
}