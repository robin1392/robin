using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirageTest.Scripts;
using MirageTest.Scripts.GameMode;
using RandomWarsResource.Data;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class ActorDevModeEditorWindow : OdinEditorWindow
{
    [MenuItem("RandomWars/ActorDevModeEditorWindow")]
    private static void OpenWindow()
    {
        GetWindow<ActorDevModeEditorWindow>().Show();
    }

    protected override void Initialize()
    {
        // DiceInfos = new List<DiceElement>() {new DiceElement(SpawnMine, SpawnEnemys, 1), new DiceElement(SpawnMine, SpawnEnemys, 2)};
    }

    public int inGameLevel;
    public int outGameLevel;
    public int diceScale = 0;
    
    [TableList]
    public List<DiceElement> DiceInfos;
    [TableList]
    public List<DiceElement> GuardianInfos;

    private GameModeBase _gameMode;
    private RWNetworkServer _server;
    protected override void OnGUI()
    {
        if (_gameMode != null)
        {
            base.OnGUI();
            return;   
        }
    
        var serverGameLogic = FindObjectOfType<ServerGameLogic>();
        if (serverGameLogic != null)
        {
            if (serverGameLogic._gameMode != null)
            {
                _server = serverGameLogic.server;
                _gameMode = serverGameLogic._gameMode;
                DiceInfos = TableManager.Get().DiceInfo.Values
                    .Select(d => new DiceElement(SpawnMine, SpawnEnemys, d.id, d.prefabName)).ToList();
                GuardianInfos = TableManager.Get().GuardianInfo.Values
                    .Select(d => new DiceElement(SpawnMyGuardian, SpawnEnemyGuardian, d.id, d.prefabName)).ToList();
            }
        }
    
        if (_gameMode == null)
        {
            EditorGUILayout.HelpBox("InGameMirageActorDev 씬 실행 중에만 동작합니다.", MessageType.Info);
        }
    }

    void SpawnMine(int diceId)
    {
        _gameMode.SpawnMyMinion(diceId, (byte)inGameLevel, (byte)outGameLevel, (byte)diceScale);
    }
    
    void SpawnEnemys(int diceId)
    {
        _gameMode.SpawnEnemyMinion(diceId, (byte)inGameLevel, (byte)outGameLevel, (byte)diceScale);
    }
    
    void SpawnMyGuardian(int diceId)
    {
        var playerState = _gameMode.PlayerState1;
        _server.CreateActorWithGuardianId(diceId, playerState.ownerTag, playerState.team, Vector3.zero);
    }
    
    void SpawnEnemyGuardian(int diceId)
    {
        var playerState = _gameMode.PlayerState2;
        _server.CreateActorWithGuardianId(diceId, playerState.ownerTag, playerState.team, Vector3.zero);
    }
    
    public class DiceElement
    {
        [NonSerialized]
        private Action<int> spawnMine;
        [NonSerialized]
        private Action<int> spawnEnemys;
        
        public int DiceId;
        public string Prefab;
        
        public DiceElement(Action<int> spawnMine, Action<int> spawnEnemys, int diceId, string prefab)
        {
            this.spawnMine = spawnMine;
            this.spawnEnemys = spawnEnemys;
            DiceId = diceId;
            Prefab = prefab;
        }

        [Button]
        public void SpawnMine()
        {
            spawnMine?.Invoke(DiceId);
        }
        
        [Button]
        public void SpawnEnemys()
        {
            spawnEnemys?.Invoke(DiceId);
        }
    }
}

