using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpshift;

public class HelpshiftManager : Singleton<HelpshiftManager>
{
    [Header("Keys")]
    public string api_key;
    public string domain_name;
    public string app_id_android;
    public string app_id_ios;

    private HelpshiftSdk help;

    public override void Awake()
    {
        base.Awake();
#if !UNITY_EDITOR
        Initialize();
#endif
    }

    public void Initialize()
    {
        if (string.IsNullOrEmpty(api_key) == false && string.IsNullOrEmpty(domain_name) == false)
        {
            help = HelpshiftSdk.getInstance();
            var configMap = new Dictionary<string, object>();
#if UNITY_ANDROID
            if (string.IsNullOrEmpty(app_id_android) == false)
            {
                help.install(api_key, domain_name, app_id_android, configMap);
            }
#elif UNITY_IOS
            if (string.IsNullOrEmpty(app_id_ios) == false)
            {
                help.install(api_key, domain_name, app_id_ios, configMap);                
            }
#endif
        }
    }

    public void ShowHelpshift()
    {
        if (help != null)
        {
            var configMap = new Dictionary<string, object>();
            help.showConversation(configMap);
        }
    }
}
