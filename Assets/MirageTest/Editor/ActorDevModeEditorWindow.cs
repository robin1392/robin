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
    public int diceScale;
    
    [TableList]
    public List<DiceElement> DiceInfos;

    private ActorDevMode _actorDevMode;
    protected override void OnGUI()
    {
        if (_actorDevMode != null)
        {
            base.OnGUI();
            return;   
        }
    
        var serverGameLogic = FindObjectOfType<ServerGameLogic>();
        if (serverGameLogic != null)
        {
            if (serverGameLogic._gameMode is ActorDevMode actorDevMode)
            {
                _actorDevMode = actorDevMode;
                DiceInfos = TableManager.Get().DiceInfo.Values
                    .Select(d => new DiceElement(SpawnMine, SpawnEnemys, d.id, d.prefabName)).ToList();
            }
        }
    
        if (_actorDevMode == null)
        {
            EditorGUILayout.HelpBox("InGameMirageActorDev 씬 실행 중에만 동작합니다.", MessageType.Info);
        }
    }

    void SpawnMine(int diceId)
    {
        _actorDevMode.SpawnMyMinion(diceId, (byte)inGameLevel, (byte)outGameLevel, (byte)diceScale);
    }
    
    void SpawnEnemys(int diceId)
    {
        _actorDevMode.SpawnEnemyMinion(diceId, (byte)inGameLevel, (byte)outGameLevel, (byte)diceScale);
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

