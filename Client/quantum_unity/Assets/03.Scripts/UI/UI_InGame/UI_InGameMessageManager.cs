using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGameMessageManager : SingletonDestroy<UI_InGameMessageManager>
{
    public GameObject pref_Message;

    public void ShowMessage(string message, float showtime = 3f)
    {
        var obj = Instantiate(pref_Message, Vector3.zero, Quaternion.identity, transform);
        obj.transform.localRotation = Quaternion.identity;
        obj.GetComponent<UI_IngameMessage>().Initialize(message, showtime);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);
    }
}
