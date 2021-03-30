using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoadAttribute]
public static class StartScene
{
    public const string DisableStartScenePrefsKey = "AutoStartScenePrefsKey";

    static StartScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (EditorPrefs.GetBool(DisableStartScenePrefsKey, false)) return;

        switch (state)
        {
            case PlayModeStateChange.EnteredEditMode:
                EditorSceneManager.OpenScene(File.ReadAllText(".lastScene"));
                break;
            case PlayModeStateChange.ExitingEditMode:
                File.WriteAllText(".lastScene", SceneManager.GetActiveScene().path);
                if (SceneManager.GetActiveScene().isDirty) EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

                EditorSceneManager.OpenScene("Assets/01.Scenes/StartScene.unity");
                EditorApplication.isPlaying = true;
                break;
        }
    }
}

internal static class StartSceneSettingsIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("Project/RandomWars", SettingsScope.Project)
        {
            // By default the last token of the path is used as display name if no label is provided.
            label = "RandomWars",
            // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
            guiHandler = searchContext =>
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    var toggleValue = EditorGUILayout.ToggleLeft("StartScene 자동 로드 비활성화",
                        EditorPrefs.GetBool(StartScene.DisableStartScenePrefsKey));
                    EditorPrefs.SetBool(StartScene.DisableStartScenePrefsKey, toggleValue);
                }
            },

            // Populate the search keywords to enable smart search filtering and label highlighting:
            keywords = new HashSet<string>(new[] {"StartScene"})
        };

        return provider;
    }
}