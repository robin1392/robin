using System;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using ED;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_Language : UI_Popup
{
    public Sprite[] arrSprite_ButtonBG;
    public Image[] arrImage_ButtonBG;

    public void Start()
    {
        int current = (int)Enum.Parse(typeof(Global.COUNTRYCODE), ObscuredPrefs.GetString("CountryCode"));

        for (int i = 0; i < arrImage_ButtonBG.Length; i++)
        {
            arrImage_ButtonBG[i].sprite = arrSprite_ButtonBG[i == current ? 1 : 0];
        }
    }

    public void Click_Button(int num)
    {
        Global.COUNTRYCODE c = (Global.COUNTRYCODE) num;

        if (c.ToString() == ObscuredPrefs.GetString("CountryCode")) return;
        
        UI_Main.Get().commonMessageBoxPopup.Initialize("Language change", "Change and restart game?", "Change", null, () =>
        {
            ObscuredPrefs.SetString("CountryCode", c.ToString());
            GameStateManager.Get().ChangeScene(Global.E_GAMESTATE.STATE_START);
        });
    }
}
