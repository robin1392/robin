using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;
using Debug = ED.Debug;
using CodeStage.AntiCheat.ObscuredTypes;

public class UI_SettingPopup : UI_Popup
{
    [Header("PlayerID")] 
    public Text text_PlayerID;

    [Header("BGM")] 
    public Image image_BGM_Icon;
    public Text text_BGM_Volume;
    public Slider slider_BGM;

    [Header("SFX")] 
    public Image image_SFX_Icon;
    public Text text_SFX_Volume;
    public Slider slider_SFX;

    [Serializable]
    public enum SETTING_SUBMENU
    {
        ACCOUNT,
        LOCALIZATION,
        SUPPORT,
        YOUTUBE,
        APPSTORE,
        CREDIT,
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        
        Initialize();
    }

    public void Initialize()
    {
        text_PlayerID.text = $"PLAYER ID : {UserInfoManager.Get().GetUserInfo().userID}";
        
        BGM_SliderValueChanged(ObscuredPrefs.GetFloat("BGM_Volume", 1f));
        SFX_SliderValueChanged(ObscuredPrefs.GetFloat("SFX_Volume", 1f));
    }

    public void Click_PlayerID()
    {
        UserInfoManager.Get().GetUserInfo().userID.CopyToClipboard();
        
        UI_ErrorMessage.Get().ShowMessage(LocalizationManager.GetLangDesc("Option_Pidcopy"));
    }

    public void BGM_SliderValueChanged(float value)
    {
        float p = value % 0.1f;
        slider_BGM.value = value + (p < 0.05f ? -p : -p + 0.1f);
        bool isZero = slider_BGM.value == 0;
        ObscuredPrefs.SetFloat("BGM_Volume", slider_BGM.value);
        SoundManager.instance.BGMVolume = slider_BGM.value;
        text_BGM_Volume.text = isZero ? $"OFF" : $"{slider_BGM.value * 10:F0}";
        text_BGM_Volume.color = isZero ? Color.red : Color.white;
        image_BGM_Icon.color = isZero ? Color.red : Color.white;
    }

    public void SFX_SliderValueChanged(float value)
    {
        float p = value % 0.1f;
        slider_SFX.value = value + (p < 0.05f ? -p : -p + 0.1f);
        bool isZero = slider_SFX.value == 0;
        ObscuredPrefs.SetFloat("SFX_Volume", slider_SFX.value);
        SoundManager.instance.SFXVolume = slider_SFX.value;
        text_SFX_Volume.text = isZero ? $"OFF" : $"{slider_SFX.value * 10:F0}";
        text_SFX_Volume.color = isZero ? Color.red : Color.white;
        image_SFX_Icon.color = isZero ? Color.red : Color.white;
    }

    public void Click_BGM()
    {
        if (slider_BGM.value > 0)
        {
            ObscuredPrefs.SetFloat("BGM_Volume_Old", slider_BGM.value);
            BGM_SliderValueChanged(0);
        }
        else
        {
            BGM_SliderValueChanged(ObscuredPrefs.GetFloat("BGM_Volume_Old", 1f));
        }
    }

    public void Click_SFX()
    {
        if (slider_SFX.value > 0)
        {
            ObscuredPrefs.SetFloat("SFX_Volume_Old", slider_SFX.value);
            SFX_SliderValueChanged(0);
        }
        else
        {
            SFX_SliderValueChanged(ObscuredPrefs.GetFloat("SFX_Volume_Old", 1f));
        }
    }

    public void Click_SubMenu(int num)
    {
        switch ((SETTING_SUBMENU)num)
        {
            case SETTING_SUBMENU.ACCOUNT:
                break;
            case SETTING_SUBMENU.LOCALIZATION:
                break;
            case SETTING_SUBMENU.SUPPORT:
                UI_Main.Get().Click_Helpshift_Button();
                break;
            case SETTING_SUBMENU.YOUTUBE:
                break;
            case SETTING_SUBMENU.APPSTORE:
                break;
            case SETTING_SUBMENU.CREDIT:
                break;
        }
    }
}
