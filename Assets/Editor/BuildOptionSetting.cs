using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;



public class BuildOptionSetting : EditorWindow
{
    static Configuration configuration;
    static Configuration nowConfiguration;
    Vector2 pos = Vector2.zero;
    
    [MenuItem("Build/BuildManager")]
    static void Open()
    {
        nowConfiguration = configuration = Configuration.GetConfigFile();
        GetWindow<BuildOptionSetting>("Build Setting Window");
    }
    
    
    GUIStyle TitleStyle()
    {
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 14;
        bool hasPro = UnityEditorInternal.InternalEditorUtility.HasPro();
        titleStyle.normal.textColor = hasPro ? Color.white : Color.black;
        return titleStyle;
    }

    private void OnGUI()
    {
        if(configuration == null)
        {
            nowConfiguration = configuration = Configuration.GetConfigFile();
        }
        
        pos = GUILayout.BeginScrollView(pos);
        GUILayout.BeginVertical();
        
        Top();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        Center();
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        Bottom();
        
        
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    void Top()
    {
        EditorGUILayout.LabelField("Build Setting", TitleStyle());
        EditorGUILayout.Space();
        
        configuration.eGameServiceMode = (E_GameServiceMode)EditorGUILayout.Popup("Service Build Type" , (int)configuration.eGameServiceMode, Enum.GetNames(typeof(E_GameServiceMode)));
        EditorGUILayout.Space();
        configuration.eGamePlayMode = (E_GamePlayMode)EditorGUILayout.Popup("Data Play Type" , (int)configuration.eGamePlayMode, Enum.GetNames(typeof(E_GamePlayMode)));
        
        EditorGUILayout.Space();
    }

    void Center()
    {
        
    }

    void Bottom()
    {
        //EditorGUILayout.LabelField("Build File Name", CustomBuildPipeline.BuildFileName(configuration));
        GUILayout.BeginHorizontal();
        bool isOk = GUILayout.Button("Save");
        if (isOk)
        {
            Debug.Log("Save");
            Configuration.SaveConfigFile(configuration);
            //Configuration.SetPlayerSetting(configuration);
        }
        
        /*isOk = GL.Button("Build");
        if (isOk)
        {
            Debug.Log("Build");
            Configuration.SaveConfigFile(configuration);
            CustomBuildPipeline.Build(EditorUserBuildSettings.activeBuildTarget, CustomBuildPipeline.BuildFileName(configuration));
            
            Close();
            GUIUtility.ExitGUI();
        }*/
        
        GUILayout.EndHorizontal();
    }
    
    
}
