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

    [Header("Button")] 
    public Button btn_Close;
    public Button btn_OK;
    public Text text_Button;
    
    private Action closeCallback;
    private Action buttonCallback;

    protected override void Open()
    {
        base.Open();
        
        btn_Close.gameObject.SetActive(isBgButtonEnable);
    }

    public void Initialize(string title, string message, Action closeCallback = null)
    {
        text_Title.text = title;
        text_Message.text = message;
        this.closeCallback = closeCallback;
        
        btn_OK.gameObject.SetActive(false);

        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(text_Message.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_Frame);
    }

    public void Initialize(string title, string message, string buttonText, Action closeCallback = null, Action buttonCallback = null, bool isBgButtonEnable = true)
    {
        btn_OK.gameObject.SetActive(true);
        
        text_Title.text = title;
        text_Message.text = message;
        this.closeCallback = closeCallback;
        this.buttonCallback = buttonCallback;
        this.isBgButtonEnable = isBgButtonEnable;
        text_Button.text = buttonText;
        
        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(text_Message.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_Frame);
    }

    public void Click_OK()
    {
        Close();
        
        buttonCallback?.Invoke();
        buttonCallback = null;
    }
    
    public override void Close()
    {
        base.Close();
        
        closeCallback?.Invoke();

        closeCallback = null;
    }
}
