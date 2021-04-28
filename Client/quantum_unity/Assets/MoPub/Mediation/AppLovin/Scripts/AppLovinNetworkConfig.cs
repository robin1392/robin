#if mopub_manager
using UnityEngine;

public class AppLovinNetworkConfig : MoPubNetworkConfig
{
    public override string AdapterConfigurationClassName
    {
        get { return Application.platform == RuntimePlatform.Android
                  ? "com.mopub.mobileads.AppLovinAdapterConfiguration"
                  : "AppLovinAdapterConfiguration"; }
    }

    [Tooltip("Enter your SDK Key to be used to configure the AppLovin SDK. You can get your SDK Key from your AppLovin dashboard.")]
    [Config.Optional("sdk_key")]
    public string sdkKey;
}
#endif
