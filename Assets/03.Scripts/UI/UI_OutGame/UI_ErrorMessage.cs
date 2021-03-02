using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_ErrorMessage : SingletonDestroy<UI_ErrorMessage>
{
    public GameObject pref_Message;
    public RectTransform rts_Stack;

    public void ShowMessage(string message)
    {
        if (rts_Stack.childCount < 5)
        {
            Text text = Instantiate(pref_Message, Vector3.zero, Quaternion.identity, rts_Stack).GetComponent<Text>();
            text.text = message;

            text.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f);
            text.DOFade(1f, 0.5f);
            text.DOFade(0f, 0.5f).SetDelay(1f).OnComplete(() => { Destroy(text.gameObject); });
        }
    }
}
