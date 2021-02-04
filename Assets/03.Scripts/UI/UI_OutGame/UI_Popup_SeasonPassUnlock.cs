using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Popup_SeasonPassUnlock : UI_Popup
{
    public Text text_Title;
    public Text text_Message;
    public Text text_Cost;
    public Button btn_Buy;

    public void Initialize(int cost, UnityAction callback = null)
    {
        gameObject.SetActive(true);

        text_Cost.text = cost.ToString();
        btn_Buy.interactable = UserInfoManager.Get().GetUserInfo().diamond >= cost;
        
        btn_Buy.onClick.RemoveAllListeners();
        if (callback != null)
        {
            btn_Buy.onClick.AddListener(callback);
        }
    }
}
