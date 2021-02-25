using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoadAttribute]
public class StartScene : EditorWindow
{
    static StartScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        return;
        switch(state)
        {
        case PlayModeStateChange.EnteredEditMode:
            EditorSceneManager.OpenScene(File.ReadAllText(".lastScene"));
            break;
        case PlayModeStateChange.ExitingEditMode:
            File.WriteAllText(".lastScene", EditorSceneManager.GetActiveScene().path);
            if(EditorSceneManager.GetActiveScene().isDirty)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }
            EditorSceneManager.OpenScene("Assets/01.Scenes/StartScene.unity");
            EditorApplication.isPlaying = true;
            break;
        }
    }
}
