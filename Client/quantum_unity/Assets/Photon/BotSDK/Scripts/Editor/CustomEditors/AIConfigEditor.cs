using UnityEditor;
using UnityEngine;

namespace Quantum.Editor
{
  [CustomEditor(typeof(AIConfigAsset), true)]
  public class AIConfigEditor : UnityEditor.Editor
  {
    private long _guid;
    private AIConfigAsset _defaultConfig;
    private AssetGuid _defaultConfigGUID;

    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      // This draws all fields except the "Settings" property.
      CustomEditorsHelper.DrawDefaultInspector(serializedObject, new string[] { "Settings" });

      // As we show only the content of "Settings" add a headline here to make it clear that this is not the default asset layout.
      CustomEditorsHelper.DrawHeadline(target != null ? target.GetType().Name : "Quantum Asset Inspector");

      // We know the data is called "Settings" so we can go ahead and only render the content of "Settings".
      CustomEditorsHelper.BeginBox();

      var aiConfigProperty = serializedObject.FindProperty("Settings");
      _guid = aiConfigProperty.FindPropertyRelative("Identifier").FindPropertyRelative("Guid").FindPropertyRelative("Value").longValue;

      if (_defaultConfig != null && IsDefaultConfig() == false)
      {
        // This draws all fields except the "Settings" property. But only if it is not default (generated) config.
        CustomEditorsHelper.DrawDefaultInspector(serializedObject, "Settings", new string[] { "Settings.KeyValuePairs", "Settings.DefaultConfig" }, false);
      }

      DrawDefaultConfig(aiConfigProperty.FindPropertyRelative("DefaultConfig"));
      DrawValues(aiConfigProperty.FindPropertyRelative("KeyValuePairs"));

      if (_defaultConfig != null && IsDefaultConfig() == false)
      {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Update Config") == true)
        {
          Undo.RecordObject(serializedObject.targetObject, "Updated Config Values");
          UpdateConfig();
        }

        if (GUILayout.Button("Reset to Default") == true)
        {
          Undo.RecordObject(serializedObject.targetObject, "Reset Config to Default");
          ResetConfig();
        }

        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
      }

      if (serializedObject.hasModifiedProperties)
      {
        serializedObject.ApplyModifiedProperties();
      }

      CustomEditorsHelper.EndBox();
    }

    private void DrawDefaultConfig(SerializedProperty defaultConfigProperty)
    {
      var defaultConfigGUID = defaultConfigProperty.FindPropertyRelative("Id").FindPropertyRelative("Value").longValue;

      // Config changed
      if (defaultConfigGUID != _defaultConfigGUID.Value)
      {
        _defaultConfig = null;

        if (defaultConfigGUID != default)
        {
          //UnityDB.Init(true);
          _defaultConfig = UnityDB.FindAsset<AIConfigAsset>(defaultConfigGUID);
        }

        _defaultConfigGUID.Value = _defaultConfig != null ? _defaultConfig.Settings.Guid.Value : 0;
      }

      // Draw default config property only if this object is not default config
      if (IsDefaultConfig() == false)
      {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Config Setup", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(defaultConfigProperty);
      }
    }

    private void DrawValues(SerializedProperty values)
    {
      if (values == null)
        return;

      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Values", EditorStyles.boldLabel);

      EditorGUI.indentLevel++;

      for (int i = 0; i < values.arraySize; i++)
      {
        EditorGUILayout.PropertyField(values.GetArrayElementAtIndex(i));
      }

      EditorGUI.indentLevel--;
    }

    private void UpdateConfig()
    {
      var configAsset = serializedObject.targetObject as AIConfigAsset;
      var configValues = configAsset.Settings.KeyValuePairs;
      var defaultConfigValues = _defaultConfig.Settings.KeyValuePairs;

      // Remove old values
      for (int i = configValues.Count - 1; i >= 0; i--)
      {
        var value = configValues[i];
        var defaultValue = defaultConfigValues.Find(t => t.Key == value.Key);

        if (defaultValue == null || defaultValue.Type != value.Type)
        {
          configValues.RemoveAt(i);
        }
      }

      // Add missing values
      for (int i = 0; i < defaultConfigValues.Count; i++)
      {
        var defaultValue = defaultConfigValues[i];

        if (configValues.Find(t => t.Key == defaultValue.Key) == null)
        {
          configValues.Add(new AIConfig.KeyValuePair
          {
            Key = defaultValue.Key,
            Type = defaultValue.Type,
            Value = defaultValue.Value,
          });
        }
      }

      configValues.Sort((a, b) => a.Key.CompareTo(b.Key));

      serializedObject.Update();
    }

    private void ResetConfig()
    {
      var configAsset = serializedObject.targetObject as AIConfigAsset;
      var configValues = configAsset.Settings.KeyValuePairs;
      var defaultConfigValues = _defaultConfig.Settings.KeyValuePairs;

      configValues.Clear();

      for (int i = 0; i < defaultConfigValues.Count; i++)
      {
        var defaultValue = defaultConfigValues[i];

        configValues.Add(new AIConfig.KeyValuePair
        {
          Key = defaultValue.Key,
          Type = defaultValue.Type,
          Value = defaultValue.Value,
        });
      }

      serializedObject.Update();
    }

    private bool IsDefaultConfig()
    {
      return _defaultConfigGUID.Value == _guid/* && serializedObject.targetObject.name == _guid*/;
    }
  }
}
