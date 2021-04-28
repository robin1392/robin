#if mopub_manager
using UnityEngine;

public class VungleNetworkConfig : MoPubNetworkConfig
{
    public override string AdapterConfigurationClassName
    {
        get { return Application.platform == RuntimePlatform.Android
                  ? "com.mopub.mobileads.VungleAdapterConfiguration"
                  : "VungleAdapterConfiguration"; }
    }

    [Tooltip("Enter your app ID to be used to initialize the Vungle SDK.")]
    [Config.Optional]
    public PlatformSpecificString appId;

}
#endif
