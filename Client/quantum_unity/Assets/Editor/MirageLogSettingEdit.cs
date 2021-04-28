using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class MirageLogSettingEdit
{
    [MenuItem ("RandomWars/ResetMirageLogSetting")]
    public static void Reset () {
        EditorPrefs.DeleteKey("Log Levels");
    }
}
