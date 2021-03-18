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
        logger.Log(
            $"[MergeDice] sourceDiceFieldIndex:{sourceDiceFieldIndex} targetDiceFieldIndex{targetDiceFieldIndex}");

        var playerState = GetPlayerState();

        var targetFieldDice = playerState.Field[targetDiceFieldIndex];
        if (targetFieldDice.IsEmpty)
        {
            logger.LogError($"필드에 주사위가 존재하지 않습니다.: playerId:{playerState.userId}, fieldIndex:{targetDiceFieldIndex}");
            return;
        }

        // 인게임 주사위의 최대 등급 여부를 체크한다.
        short MaxInGameUp = 6;
        if (targetFieldDice.diceScale >= MaxInGameUp)
        {
            logger.LogError($"주사위 눈금이 최대치입니다.: playerId:{playerState.userId}, fieldIndex:{targetDiceFieldIndex}");
            return;
        }

        var sourceFieldDice = playerState.Field[sourceDiceFieldIndex];
        if (sourceFieldDice.IsEmpty)
        {
            logger.LogError($"필드에 주사위가 존재하지 않습니다.: playerId:{playerState.userId}, fieldIndex:{sourceDiceFieldIndex}");
            return;
        }

        // 주사위 아이디 체크
        if (sourceFieldDice.diceId != targetFieldDice.diceId)
        {
            logger.LogError(
                $"병합하려는 주사위의 아이디가 다릅니다.: playerId:{playerState.userId}, source:{sourceFieldDice.diceId} target:{targetFieldDice.diceId}");
            return;
        }

        if (sourceFieldDice.diceScale != targetFieldDice.diceScale)
        {
            logger.LogError($"필드에 주사위가 존재하지 않습니다.: playerId:{playerState.userId}, fieldIndex:{sourceDiceFieldIndex}");
            return;
        }

        // Deck에서 랜덤으로 주사위를 선택한다
        int randDeckIndex = Random.Range(0, playerState.Deck.Count);
        var selectedDeck = playerState.Deck[randDeckIndex];
        if (selectedDeck.IsEmpty)
        {
            logger.LogError($"덱에 주사위가 존재하지 않습니다.: playerId:{playerState.userId}, selectedDeckIndex:{randDeckIndex}");
            return;
        }

        playerState.Field[targetDiceFieldIndex] = new FieldDice()
        {
            diceId = selectedDeck.diceId,
            diceScale = ++sourceFieldDice.diceScale
        };

        // 선택 주사위는 제거한다.
        playerState.Field[sourceDiceFieldIndex] = FieldDice.Empty;
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
        logger.Log($"[UpgradeIngameLevel] diceId:{diceId}");

        var playerState = GetPlayerState();

        var deckDice = playerState.GetDeckDice(diceId);
        if (deckDice.IsEmpty)
        {
            logger.LogError($"덱에 주사위가 존재하지 않습니다.: playerId:{playerState.userId}, diceId:{diceId}");
            return;
        }

        byte MaxInGameUp = 6;
        if (deckDice.inGameLevel >= MaxInGameUp)
        {
            logger.LogError($"덱 주사위 레벨이 최대치입니다.: playerId:{playerState.userId}, diceId:{diceId}");
            return;
        }

        // 필요한 SP를 구한다.
        int needSp = TableManager.Get().Vsmode
            .KeyValues[(int) EVsmodeKey.DicePowerUpCost01 + deckDice.inGameLevel].value;
        // 플레이어 SP를 업데이트 한다.
        if (playerState.sp < needSp)
        {
            logger.LogError(
                $"덱 주사위 업그레이드를 위한 SP가 모자랍니다.: playerId:{playerState.userId}, diceId:{diceId}, sp:{playerState.sp} 팔요sp:{needSp}");
            return;
        }

        playerState.sp -= needSp;

        var deckIndex = playerState.GetDeckIndex(diceId);
        playerState.Deck[deckIndex] = new DeckDice()
        {
            diceId = deckDice.diceId,
            inGameLevel = ++deckDice.inGameLevel,
            outGameLevel = deckDice.outGameLevel,
        };
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
        logger.Log($"[UpgradeSp]");
        var playerState = GetPlayerState();

        // sp 등급 체크
        int MaxSpGrade = 6;
        if (playerState.spGrade >= MaxSpGrade)
        {
            logger.LogError($"Sp 등급이 최대치입니다.: playerId:{playerState.userId}");
            return;
        }
            
        // SP를 차감한다.
        int needSp = playerState.GetUpradeSpCost();
        if (playerState.sp < needSp)
        {
            logger.LogError($"Sp 업그레이드를 위한 SP가 모자랍니다.: playerId:{playerState.userId} sp:{playerState.sp} 필요sp: {needSp}");
            return;
        }

        playerState.sp -= needSp;
        playerState.spGrade += 1;
    }
}