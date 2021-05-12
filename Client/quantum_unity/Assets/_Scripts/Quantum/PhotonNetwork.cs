using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using _Scripts;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon;
using Photon.Realtime;
using Quantum;
using Quantum.Demo;
using UnityEngine;
using UnityEngine.SceneManagement;
using Input = UnityEngine.Input;

public class PhotonNetwork : GameObjectSingleton<PhotonNetwork>
{
    public byte MaxPlayers = 2;
    public bool WaitForAll = true;

    public MapAsset BattleMap;
    public MapAsset CoopMap;

    public LoadBalancingClient LocalBalancingClient;
    private PhotonTaskNetwork _photonTaskNetwork;

    private string IntoGameScenePropertyKey = "SCENE";
    private string StartPropertyKey = "START";
    private string MapPropertyKey = "MAP";
    private string ReadyPropertyKey = "READY";

    public enum State
    {
        Connecting,
        JoinOrCreating,
        WaitingForPlayers,
        SceneChanged,
        Started
    }

    #region Properties

    public State _State
    {
        get { return _state; }
        set
        {
            _state = value;
            Debug.Log("Setting UIJoinRandom state to " + _state.ToString());
        }
    }

    public bool Online { get; set; }

    private State _state;

    #endregion


    protected override void OnAwake()
    {
        base.OnAwake();
        var serverSettings = PhotonServerSettings.Instance;
        if (string.IsNullOrEmpty(serverSettings.AppSettings.AppIdRealtime))
        {
            Debug.LogError("AppId not set");
            return;
        }
        
        LocalBalancingClient = new LoadBalancingClient();
        LocalBalancingClient.AppId = serverSettings.AppSettings.AppIdRealtime;
        LocalBalancingClient.AppVersion = serverSettings.AppSettings.AppVersion;

        var userInfo = UserInfoManager.Get().GetUserInfo();
        var id = userInfo.userID ?? Guid.NewGuid().ToString(); 
        var nickName =  userInfo.userNickName ?? id;
        LocalBalancingClient.UserId = id;
        LocalBalancingClient.NickName = nickName;
        _photonTaskNetwork = new PhotonTaskNetwork(LocalBalancingClient);
    }

    #region UnityCallbacks

    public async UniTask JoinBattleModeByMatching(CancellationToken token = default)
    {
        Online = true;
        _State = State.Connecting;
        await _photonTaskNetwork.ConnectToRegionAsync(PhotonServerSettings.Instance.AppSettings.FixedRegion, token);
        _State = State.JoinOrCreating;

        var userInfo = UserInfoManager.Get().GetUserInfo();
        var userDeck = userInfo.GetActiveDeck;
        var diceDeck = userDeck.Take(5).ToArray();
        var guadianId = userDeck[5];
        
        var localMatchPlayer = new MatchPlayer()
        {
            UserId = userInfo.userID,
            NickName = userInfo.userNickName,
            Trophy = 0,
            WinStreak = 0,
            Deck = new DeckInfo(guadianId, diceDeck, userInfo.GetOutGameLevels(diceDeck)),
            EnableAI = false
        };
        LocalBalancingClient.LocalPlayer.SetCustomProperties(localMatchPlayer.ToPlayerCustomProperty());
        
        Debug.Log($"~~{BattleMap.AssetObject.Guid.Value}");
        await _photonTaskNetwork.JoinRandomOrCreateRoom((MapPropertyKey, BattleMap.AssetObject.Guid.Value), token);
        _State = State.WaitingForPlayers;
        Debug.Log(LocalBalancingClient.CurrentRoom.Name);
    }
    
    public async UniTask JoinBattleModeByCode(string roomName, CancellationToken token = default)
    {
        Online = true;
        _State = State.Connecting;
        await _photonTaskNetwork.ConnectToRegionAsync(PhotonServerSettings.Instance.AppSettings.FixedRegion, token);
        _State = State.JoinOrCreating;

        var userInfo = UserInfoManager.Get().GetUserInfo();
        var userDeck = userInfo.GetActiveDeck;
        var diceDeck = userDeck.Take(5).ToArray();
        var guadianId = userDeck[5];
        
        var localMatchPlayer = new MatchPlayer()
        {
            UserId = userInfo.userID,
            NickName = userInfo.userNickName,
            Trophy = 0,
            WinStreak = 0,
            Deck = new DeckInfo(guadianId, diceDeck, userInfo.GetOutGameLevels(diceDeck)),
            EnableAI = false
        };
        LocalBalancingClient.LocalPlayer.SetCustomProperties(localMatchPlayer.ToPlayerCustomProperty());
        
        await _photonTaskNetwork.JoinRoomAsync(roomName, new Hashtable()
        {
            { MapPropertyKey, BattleMap.AssetObject.Guid.Value }
        }, token);
        _State = State.WaitingForPlayers;
        Debug.Log(LocalBalancingClient.CurrentRoom.Name);
    }
    
    public async UniTask<string> CreateBattleRoom(CancellationToken token = default)
    {
        Online = true;
        _State = State.Connecting;
        await _photonTaskNetwork.ConnectToRegionAsync(PhotonServerSettings.Instance.AppSettings.FixedRegion, token);
        _State = State.JoinOrCreating;

        var userInfo = UserInfoManager.Get().GetUserInfo();
        var userDeck = userInfo.GetActiveDeck;
        var diceDeck = userDeck.Take(5).ToArray();
        var guadianId = userDeck[5];
        
        var localMatchPlayer = new MatchPlayer()
        {
            UserId = userInfo.userID,
            NickName = userInfo.userNickName,
            Trophy = 0,
            WinStreak = 0,
            Deck = new DeckInfo(guadianId, diceDeck, userInfo.GetOutGameLevels(diceDeck)),
            EnableAI = false
        };
        LocalBalancingClient.LocalPlayer.SetCustomProperties(localMatchPlayer.ToPlayerCustomProperty());

        var roomGuid = System.Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        await _photonTaskNetwork.CreateRoomAsync(roomGuid, (MapPropertyKey, BattleMap.AssetObject.Guid.Value), token);
        _State = State.WaitingForPlayers;
        Debug.Log(LocalBalancingClient.CurrentRoom.Name);
        return roomGuid;
    }

    public AssetGuid GetMapGuid()
    {
        if (LocalBalancingClient.CurrentRoom.CustomProperties.TryGetValue(MapPropertyKey, out var mapId) == false)
        {
            Debug.LogError($"룸 프로퍼티에 맵 아이디가 업습니다.");
            return AssetGuid.Invalid;
        }

        return new AssetGuid((long)mapId);
    }
    
    public void Ready()
    {
        var ht = new Hashtable() {{ReadyPropertyKey, true}};
        LocalBalancingClient.LocalPlayer.SetCustomProperties(ht);
    }

        
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log(LocalBalancingClient.State.ToString());
        }
        
        LocalBalancingClient?.Service();

        if (_State == State.Started)
            return;

        if (LocalBalancingClient != null && LocalBalancingClient.InRoom)
        {
            var hasStarted = LocalBalancingClient.CurrentRoom.CustomProperties.TryGetValue(StartPropertyKey, out var start) && (bool) start;
            var sceneChanged = LocalBalancingClient.CurrentRoom.CustomProperties.TryGetValue(IntoGameScenePropertyKey, out var sceneChange) && (bool) sceneChange;

            if (sceneChanged && _State != State.SceneChanged)
            {
                var mapGuid =
                    (AssetGuid) (LocalBalancingClient.CurrentRoom.CustomProperties.TryGetValue(MapPropertyKey, out var guid)
                        ? (long) guid
                        : 0L);
                var mapAsset = UnityDB.DefaultResourceManager.GetAsset(mapGuid, true) as Map;
                _State = State.SceneChanged;
                SceneManager.LoadScene(mapAsset.Scene);
                return;
            }

            // Only admin posts properties into the room
            if (LocalBalancingClient.LocalPlayer.IsMasterClient)
            {
                if (!hasStarted && !sceneChanged && (!WaitForAll || LocalBalancingClient.CurrentRoom.PlayerCount >= MaxPlayers))
                {
                    var ht = new Hashtable();
                    ht.Add(IntoGameScenePropertyKey, true);
                    LocalBalancingClient.CurrentRoom.SetCustomProperties(ht);
                }
                
                if (!hasStarted && sceneChanged)
                {
                    if (LocalBalancingClient.CurrentRoom.Players.Count(p => 
                        p.Value.CustomProperties.TryGetValue(ReadyPropertyKey, out var ready) && (bool) ready) >= MaxPlayers)
                    {
                        var ht = new Hashtable();
                        ht.Add(StartPropertyKey, true);
                        LocalBalancingClient.CurrentRoom.IsVisible = false;
                        LocalBalancingClient.CurrentRoom.SetCustomProperties(ht);
                    }
                }
            }

            // Everyone is listening for map and start properties
            if (hasStarted)
            {
                _State = State.Started;
            }
        }
    }

    #endregion
}
