using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirageTest.Scripts;
using MirageTest.Scripts.GameMode;
using Photon.Deterministic;
using Quantum;
using Quantum.Commands;
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
    [Range(0, 14)]
    public int FieldIndex;
    public FPVector2 Position;
    
    [TableList]
    public List<DiceElement> DiceInfos;
    [TableList]
    public List<DiceElement> GuardianInfos;
    [TableList]
    public List<DiceElement> BossnInfos;

    private QuantumGame _game;
    protected override void OnGUI()
    {
        if (_game != null)
        {
            base.OnGUI();
            return;   
        }
        
        _game = QuantumRunner.Default?.Game;
        if (_game != null)
        {
            DiceInfos = TableManager.Get().DiceInfo.Values
                .Select(d => new DiceElement(SpawnMine, SpawnEnemys, d.id, d.prefabName)).ToList();
            GuardianInfos = TableManager.Get().GuardianInfo.Values
                .Select(d => new DiceElement(SpawnMyGuardian, SpawnEnemyGuardian, d.id, d.prefabName)).ToList();
            BossnInfos = TableManager.Get().CoopModeBossInfo.Values
                .Select(d => new DiceElement(SpawnMyBoss, SpawnEnemyBoss, d.id, d.prefabName)).ToList();
        }
        else
        {
            EditorGUILayout.HelpBox("Quantum 인게임 씬 실행 중에만 동작합니다.", MessageType.Info);
        }
    }

    void SpawnMine(int diceId)
    {
        Spawn(0, diceId);
    }

    void Spawn(int playerIndex, int diceId)
    {
        var command = new CreateActorCommand();
        command.ActorType = ActorType.Dice;
        command.DataId = diceId;
        command.IngameLevel = inGameLevel;
        command.OutgameLevel = outGameLevel;
        command.DiceScale = diceScale;
        command.FieldIndex = FieldIndex;
        command.Position = Position;
        _game.SendCommand(QuantumRunner.Default.Game.GetLocalPlayers()[playerIndex], command);
    }
    
    void SpawnEnemys(int diceId)
    {
        Spawn(1, diceId);
    }
    
    void SpawnMyGuardian(int diceId)
    {
        // var playerState = _gameMode.PlayerState1;
        // _server.CreateActorWithGuardianId(diceId, playerState.ownerTag, playerState.team, Vector3.zero);
    }
    
    void SpawnEnemyGuardian(int diceId)
    {
        // var playerState = _gameMode.PlayerState2;
        // _server.CreateActorWithGuardianId(diceId, playerState.ownerTag, playerState.team, Vector3.zero);
    }
    
    void SpawnMyBoss(int id)
    {
        // var playerState = _gameMode.PlayerState1;
        // _server.CreateActorWithBossId(id, playerState.ownerTag, playerState.team, Vector3.zero, 0, 0, true);
    }
    
    void SpawnEnemyBoss(int id)
    {
        // var playerState = _gameMode.PlayerState2;
        // _server.CreateActorWithBossId(id, playerState.ownerTag, playerState.team, Vector3.zero, 0, 0, true);
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

