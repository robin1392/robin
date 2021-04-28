using UnityEditor;
using Circuit;
using UnityEngine;

[CustomEditor(typeof(SettingsDatabase))]
public class SettingsDatabaseEditor : Editor
{
  public override void OnInspectorGUI()
  {
    base.OnInspectorGUI();

    GUILayout.BeginHorizontal();

    if (GUILayout.Button("Change Folder", EditorStyles.miniButton))
    {
      EditorApplication.delayCall += () =>
      {
        var path = UnityEditor.EditorUtility.OpenFolderPanel("Select search path", "Assets", "");
        path = path.Replace(Application.dataPath, "Assets");
        if (AssetDatabase.IsValidFolder(path))
        {
          serializedObject.FindProperty("BotSDKOutputFolder").SetValue(path);
          serializedObject.Update();
        }
      };
    }

    if (GUILayout.Button("Reset Folder", EditorStyles.miniButton))
    {
      serializedObject.FindProperty("BotSDKOutputFolder").SetValue("Assets/Resources/DB");
      serializedObject.Update();
    }

    GUILayout.EndHorizontal();

    EditorGUILayout.HelpBox("Please make sure that the chosen location is included in Quantum's Asset Database paths, " +
      "otherwise the output will not be loaded by the DB.", MessageType.Info);
  }
}
