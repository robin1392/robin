using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(RectTransform))]
public class UI_FollowSizedelta : MonoBehaviour
{
    public RectTransform rts_Target;

    private void LateUpdate()
    {
        Resize();
    }

    public void Resize()
    {
        ((RectTransform)transform).sizeDelta = rts_Target.sizeDelta;
    }
    
    public void ResizeWidth()
    {
        var size = rts_Target.sizeDelta;
        ((RectTransform)transform).sizeDelta = new Vector2(size.x, ((RectTransform)transform).sizeDelta.y);
    }
    
    public void ResizeHeight()
    {
        var size = rts_Target.sizeDelta;
        ((RectTransform)transform).sizeDelta = new Vector2(((RectTransform)transform).sizeDelta.x, size.y);
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(UI_FollowSizedelta))]
public class UI_FollowSizedeltaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UI_FollowSizedelta script = (UI_FollowSizedelta) target;
        if (GUILayout.Button("Resize Width"))
        {
            script.ResizeWidth();
        }
        if (GUILayout.Button("Resize Height"))
        {
            script.ResizeHeight();
        }
    }
}
#endif