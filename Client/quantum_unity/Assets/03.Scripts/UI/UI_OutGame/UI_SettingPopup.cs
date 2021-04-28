using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;
using Debug = ED.Debug;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using Template.Account.GameBaseAccount.Common;

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

    [Header("Account")] 
    public Image image_Account_Icon;
    public Text text_Account;

    [Header("Quality")] 
    public Toggle[] arrToggle_Quality;

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
        SetAccountButton();

        switch (ObscuredPrefs.GetInt("Quality", 1))
        {
            case 0:
                arrToggle_Quality[0].isOn = true;
                arrToggle_Quality[1].isOn = false;
                break;
            case 1:
                arrToggle_Quality[0].isOn = false;
                arrToggle_Quality[1].isOn = true;
                break;
        }
    }

    private void SetAccountButton()
    {
#if UNITY_ANDROID
        image_Account_Icon.sprite = FileHelper.GetIcon("icon_aos");
#elif UNITY_IOS
        image_Account_Icon.sprite = FileHelper.GetIcon("icon_ios");
#endif

        if (ObscuredPrefs.GetInt("PlatformType", (int)EPlatformType.Guest) == (int)EPlatformType.Android
        || ObscuredPrefs.GetInt("PlatformType", (int)EPlatformType.Guest) == (int)EPlatformType.IOS)
        {
            text_Account.text = LocalizationManager.GetLangDesc("Option_Logout");
        }
        else
        {
            text_Account.text = LocalizationManager.GetLangDesc("Option_Login");
        }
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
        string str = isZero ? $"OFF" : $"{slider_BGM.value * 10:F0}";
        if (String.CompareOrdinal(text_BGM_Volume.text, str) != 0)
        {
            text_BGM_Volume.transform.localScale = Vector3.one;
            text_BGM_Volume.transform.DOKill();
            text_BGM_Volume.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f);
            text_BGM_Volume.text = str;
            text_BGM_Volume.color = isZero ? Color.red : Color.white;
            image_BGM_Icon.color = isZero ? Color.red : Color.white;
        }
    }

    public void SFX_SliderValueChanged(float value)
    {
        float p = value % 0.1f;
        slider_SFX.value = value + (p < 0.05f ? -p : -p + 0.1f);
        bool isZero = slider_SFX.value == 0;
        ObscuredPrefs.SetFloat("SFX_Volume", slider_SFX.value);
        SoundManager.instance.SFXVolume = slider_SFX.value;
        string str = isZero ? "OFF" : $"{slider_SFX.value * 10:F0}";
        if (string.CompareOrdinal(text_SFX_Volume.text, str) != 0)
        {
            text_SFX_Volume.transform.localScale = Vector3.one;
            text_SFX_Volume.transform.DOKill();
            text_SFX_Volume.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f);
            text_SFX_Volume.text = str;
            text_SFX_Volume.color = isZero ? Color.red : Color.white;
            image_SFX_Icon.color = isZero ? Color.red : Color.white;

            SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BUTTON);
        }
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
            {
                if (ObscuredPrefs.GetInt("PlatformType", (int)EPlatformType.Guest) == (int)EPlatformType.Android
                    || ObscuredPrefs.GetInt("PlatformType", (int)EPlatformType.Guest) == (int)EPlatformType.IOS)  // 로그아웃
                {
                    UI_Main.Get().commonMessageBoxPopup.Initialize(
                        LocalizationManager.GetLangDesc("Option_Logout"),
                        LocalizationManager.GetLangDesc("Option_Logout"),
                        "OK", null, () =>
                        {
                            AuthManager.Get().Logout();
                            GameStateManager.Get().ChangeScene(Global.E_GAMESTATE.STATE_START);  
                        });
                }
                else                // 로그인
                {
                    UI_Main.Get().commonMessageBoxPopup.Initialize("Account Link", "link?", "OK", null, () =>
                    {
                        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
                        AuthManager.Get().LinkPlatform(false, PlatformLinkCallback);
                    });
                }
            }
                break;
            case SETTING_SUBMENU.LOCALIZATION:
                UI_Main.Get().languagePopup.gameObject.SetActive(true);
                break;
            case SETTING_SUBMENU.SUPPORT:
                UI_Main.Get().Click_Helpshift_Button();
                break;
            case SETTING_SUBMENU.YOUTUBE:
                Application.OpenURL("https://www.youtube.com/channel/UCSKvHQ-fuH-LLFzeCH7zt6A");
                break;
            case SETTING_SUBMENU.APPSTORE:
#if UNITY_ANDROID
                Application.OpenURL("https://play.google.com/store/apps/dev?id=7769366979601471884");
#elif UNITY_IOS
		        Application.OpenURL("https://itunes.apple.com/developer/id1060433596");
#endif
                break;
            case SETTING_SUBMENU.CREDIT:
                break;
        }
    }

    public bool PlatformLinkCallback(EGameBaseAccountErrorCode errorCode, AccountInfo accountInfo, bool needConfirm)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        
        if (needConfirm)
        {
            UI_Main.Get().commonMessageBoxPopup.Initialize("Account Link", "link override?", "Override", null, () =>
            {
                UI_Main.Get().obj_IndicatorPopup.SetActive(true);
                AuthManager.Get().LinkPlatform(true, PlatformLinkCallback);
            });
            return false;
        }
        else
        {
            if (errorCode == EGameBaseAccountErrorCode.Success)
            {
                ObscuredPrefs.SetInt("PlatformType", accountInfo.PlatformType);
                UserInfoManager.Get().GetUserInfo().SetPlatformID(accountInfo.PlatformId);
                SetAccountButton();
                return true;
            }
        }

        return false;
    }

    public void Toggle_HighQuality(bool isOn)
    {
        if (isOn)
        {
            ObscuredPrefs.SetInt("Quality", 1);
            Application.targetFrameRate = 60;
            //Screen.SetResolution(Screen.width, Screen.height, true);
            QualitySettings.SetQualityLevel(5, true);
        }
    }
    
    public void Toggle_LowQuality(bool isOn)
    {
        if (isOn)
        {
            ObscuredPrefs.SetInt("Quality", 0);
            Application.targetFrameRate = 30;
            //Screen.SetResolution(Screen.width / 2, Screen.height / 2, true);
            QualitySettings.SetQualityLevel(0, true);
        }
    }
}
