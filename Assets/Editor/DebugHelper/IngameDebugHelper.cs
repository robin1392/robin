using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ED;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

public class IngameDebugHelperWindow : OdinEditorWindow
{
    [MenuItem("RandomWars/Ingame/BSList")]
    private static void OpenWindow()
    {
        GetWindow<IngameDebugHelperWindow>().Show();
    }

    protected override void OnGUI()
    {
        base.OnGUI();
        
        var ingameManager =InGameManager.Get();
        if (ingameManager == null)
        {
            return;
        }

        var bottomPlayerController = ingameManager.playerController.isBottomCamp
            ? ingameManager.playerController
            : ingameManager.playerController.targetPlayer;

        var topPlayerController = ingameManager.playerController.isBottomCamp
            ? ingameManager.playerController.targetPlayer
            : ingameManager.playerController;
        
        if (ingameManager.playType == Global.PLAY_TYPE.BATTLE)
        {
            
        }

        EditorGUILayout.LabelField($"하단 플레이어 미니언: {ingameManager.ListBottomPlayer.Count()}");
        foreach (var baseStat in ingameManager.ListBottomPlayer)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(baseStat.name);
                EditorGUILayout.LabelField(baseStat.id.ToString());
                EditorGUILayout.ObjectField(baseStat.gameObject, typeof(GameObject));
                EditorGUILayout.LabelField(baseStat.gameObject.GetInstanceID().ToString());
            }
        }
        
        EditorGUILayout.LabelField($"하단 플레이어 컨트롤 미니언: {bottomPlayerController.listMinion.Count}");
        foreach (var baseStat in bottomPlayerController.listMinion)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(baseStat.name);
                EditorGUILayout.LabelField(baseStat.id.ToString());
                EditorGUILayout.ObjectField(baseStat.gameObject, typeof(GameObject));
                EditorGUILayout.LabelField(baseStat.gameObject.GetInstanceID().ToString());
            }
        }

        EditorGUILayout.LabelField($"상단 플레이어 미니언: {ingameManager.ListTopPlayer.Count()}");
        foreach (var baseStat in ingameManager.ListTopPlayer)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(baseStat.name);   
                EditorGUILayout.LabelField(baseStat.id.ToString());
                EditorGUILayout.ObjectField(baseStat.gameObject, typeof(GameObject));
                EditorGUILayout.LabelField(baseStat.gameObject.GetInstanceID().ToString());
            }
        }
        
        EditorGUILayout.LabelField($"상단 플레이어 컨트롤 미니언: {topPlayerController.listMinion.Count}");
        foreach (var baseStat in topPlayerController.listMinion)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(baseStat.name);
                EditorGUILayout.LabelField(baseStat.id.ToString());
                EditorGUILayout.ObjectField(baseStat.gameObject, typeof(GameObject));
                EditorGUILayout.LabelField(baseStat.gameObject.GetInstanceID().ToString());
            }
        }
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}
