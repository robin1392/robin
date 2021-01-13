using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class UI_CommonMessageBox : UI_Popup
{
    public Text text_Title;
    public Text text_Message;

    private Action callback;

    public void Initialize(string title, string message, Action callback = null)
    {
        text_Title.text = title;
        text_Message.text = message;
        this.callback = callback;

        gameObject.SetActive(true);
    }
    
    public override void Close()
    {
        callback?.Invoke();
        
        base.Close();
    }
}
