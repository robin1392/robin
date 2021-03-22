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

        if (Server.LocalClientActive)
        {
            OnStartClient();
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
    }

    private void OnStopClient()
    {
        var client = Client as RWNetworkClient;
        client.RemovePlayerProxy(this);
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

    public void EndGame(MatchReport result)
    {
        if (IsLocalClient)
        {
            EndGameInternal(result);
            return;
        }

        EndGameOnServer(result);
    }

    [ClientRpc]
    public void EndGameOnServer(MatchReport result)
    {
        EndGameInternal(result);
    }

    void EndGameInternal(MatchReport endNoti)
    {
        var client = Client as RWNetworkClient;
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
        InGameManager.Get().EndGame(endNoti);
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