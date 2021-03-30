using System;
using System.Linq;
using ED;
using Mirage;
using Mirage.Logging;
using MirageTest.Aws;
using MirageTest.Scripts;
using MirageTest.Scripts.Entities;
using Percent.Platform;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using Service.Template;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerProxy : NetworkBehaviour
{
    static readonly ILogger logger = LogFactory.GetLogger(typeof(PlayerProxy));

    [SyncVar] public bool ready;
    [SyncVar] public string userId;

    private void Awake()
    {
        if (NetIdentity == null)
        {
            return;
        }

        NetIdentity.OnStartClient.AddListener(OnStartClient);
        NetIdentity.OnStartLocalPlayer.AddListener(OnStartLocalPlayer);
        NetIdentity.OnStopClient.AddListener(OnStopClient);

        NetIdentity.OnStartServer.AddListener(OnStartServer);
        NetIdentity.OnStopServer.AddListener(OnStopServer);
    }

    private void OnStartLocalPlayer()
    {
        var client = Client as RWNetworkClient;
        if (client.enableActor && client.GetLocalPlayerState().team == GameConstants.TopCamp)
        {
            InGameManager.Get().RotateTopCampObject();
        }
        
        ClientReady();
        BindLocalDeckUI();
    }

    private void BindLocalDeckUI()
    {
        var client = Client as RWNetworkClient;
        var localPlayerState = client.GetLocalPlayerState();
        client.BindDeckUI(localPlayerState.userId);
    }

    private void OnStopServer()
    {
        var server = Server as RWNetworkServer;
        server.RemovePlayerProxy(this);

        logger.Log($"Client Disconnect: {userId}");
        
        if (Server.LocalClientActive)
        {
            OnStopClient();
        }
    }

    private void OnStartServer()
    {
        var server = Server as RWNetworkServer;
        //재접속
        if (server.serverGameLogic._gameMode.GameState.state == EGameState.Playing)
        {
            var master = server.PlayerProxies.FirstOrDefault();
            if (master != null)
            {
                master.SyncAllPosition();
            }
        }

        server?.AddPlayerProxy(this);
        var gameState = server?.serverGameLogic?._gameMode?.GameState;
        if(gameState != null)
        {
            if (gameState.masterOwnerTag == GameConstants.ServerTag)
            {
                var playerState = server.serverGameLogic.GetPlayerState(userId);
                if (playerState != null)
                {
                    gameState.masterOwnerTag = playerState.ownerTag;
                }
            }
        }

        if (Server.LocalClientActive)
        {
            OnStartClient();
            BindLocalDeckUI();
        }
    }

    [ClientRpc]
    private void SyncAllPosition()
    {
        var client = Client as RWNetworkClient;
        foreach (var actorProxy in client.ActorProxies)
        {
            actorProxy.SyncPosition(true);
        }
    }

    private void OnStartClient()
    {
        var client = Client as RWNetworkClient;
        client.AddPlayerProxy(this);

        if (IsLocalClient)
        {
            //OnStartLocalPlayer가 호출되지 않아 여기서 부름
            ClientReady();
        }
    }

    private void OnStopClient()
    {
        var client = Client as RWNetworkClient;
        client.RemovePlayerProxy(this);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            ClientPause();
        }
    }

    public void ClientPause()
    {
        if (IsLocalClient)
        {
            return;
        }

        ClientPauseOnServer();
    }
    
    
    [ServerRpc(requireAuthority = false)]
    void ClientPauseOnServer()
    {
        var server = Server as RWNetworkServer;
        server.OnClientPause(userId);
    }

    public void ClientReady()
    {
        if (IsLocalClient)
        {
            ClientReadyInternal();
            return;
        }

        ClientReadyOnServer();
    }

    [ServerRpc(requireAuthority = false)]
    void ClientReadyOnServer()
    {
        ClientReadyInternal();
    }

    void ClientReadyInternal()
    {
        ready = true;
    }

    public void MergeDice(int sourceDiceFieldIndex, int targetDiceFieldIndex)
    {
        if (IsLocalClient)
        {
            MergeDiceInternal((byte)sourceDiceFieldIndex, (byte)targetDiceFieldIndex);
            return;
        }
        
        MergeDiceOnServer((byte)sourceDiceFieldIndex, (byte)targetDiceFieldIndex);
    }
    
    [ServerRpc]
    public void MergeDiceOnServer(byte sourceDiceFieldIndex, byte targetDiceFieldIndex)
    {
        MergeDiceInternal(sourceDiceFieldIndex, targetDiceFieldIndex);
    }

    void MergeDiceInternal(byte sourceDiceFieldIndex, byte targetDiceFieldIndex)
    {
        var playerState = GetPlayerState();
        playerState.MergeDice(sourceDiceFieldIndex, targetDiceFieldIndex);
    }
    
    public void UpgradeIngameLevel(int deckIndex)
    {
        if (IsLocalClient)
        {
            UpgradeIngameLevelInternal(deckIndex);
            return;
        }
        
        UpgradeIngameLevelOnServer(deckIndex);
    }

    [ServerRpc]
    public  void UpgradeIngameLevelOnServer(int deckIndex)
    {
        UpgradeIngameLevelInternal(deckIndex);
    }

    void UpgradeIngameLevelInternal(int deckIndex)
    {
        var playerState = GetPlayerState();
        playerState.UpgradeIngameLevel(deckIndex);
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
        if (IsLocalClient)
        {
            GetDiceOnServerInternal((byte)deckIndex, (byte)fieldIndex);
            return;
        }
        
        GetDiceOnServer((byte)deckIndex, (byte)fieldIndex);
    }

    [Server]
    PlayerState GetPlayerState()
    {
        var server = Server as RWNetworkServer;
        return server.serverGameLogic.GetPlayerState(userId);
    }
    
    [ServerRpc]
    public void GetDiceOnServer(byte deckIndex, byte fieldIndex)
    {
        GetDiceOnServerInternal(deckIndex, fieldIndex);
    }
    
    void GetDiceOnServerInternal(byte deckIndex, byte fieldIndex)
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
        playerState.UpgradeSp();
    }

    public void EndGame(INetworkPlayer player, MatchReport result)
    {
        if (IsLocalClient)
        {
            EndGameInternal(result);
            return;
        }

        EndGameOnClient(player, result);
    }

    [ClientRpc(target = Mirage.Client.Player)]
    public void EndGameOnClient(INetworkPlayer player, MatchReport result)
    {
        EndGameInternal(result);
    }

    void EndGameInternal(MatchReport endNoti)
    {
        var client = Client as RWNetworkClient;
        if (client.enableUI == false)
        {
            return;
        }
        
        foreach (var actorProxy in client.ActorProxies)
        {
            actorProxy?.baseStat?.StopAllAction();
        }

        UI_InGamePopup.Get().SetViewWaiting(false);
        if (UI_InGamePopup.Get().IsIndicatorActive() == true)
        {
            UI_InGamePopup.Get().ViewGameIndicator(false);
        }
        
        SoundManager.instance.StopBGM();
        UI_InGame.Get().ClearUI();
        InGameManager.Get().EndGame(client.PlayType, client.LocalMatchPlayer, client.OtherMatchPlayer, endNoti);
    }
    
    public void GiveUp()
    {
        GiveUpOnServer();
        Client.Disconnect();
    }

    [ServerRpc]
    public void GiveUpOnServer()
    {
        var server = Server as RWNetworkServer;
        var playerState = GetPlayerState();
        server.serverGameLogic._gameMode.OnGiveUp(playerState);
    }
}