using System.Collections;
using System.Collections.Generic;
using Percent.Platform;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InAppManager))]
public class InAppManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        InAppManager inappManager = (InAppManager) target;

        if (GUILayout.Button("Long String Parsing"))
        {
            inappManager.LongStringParsing();
        }
    }
}
