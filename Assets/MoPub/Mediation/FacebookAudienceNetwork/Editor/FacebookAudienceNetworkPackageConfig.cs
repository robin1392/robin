using System.Collections.Generic;

public class FacebookAudienceNetworkPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "FacebookAudienceNetwork"; }
    }

    public override string Version
    {
        get { return /*UNITY_PACKAGE_VERSION*/"1.5.14"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get
        {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, /*ANDROID_SDK_VERSION*/"5.11.0" },
                { Platform.IOS, /*IOS_SDK_VERSION*/"5.10.1" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get
        {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.FacebookAudienceNetwork" },
                { Platform.IOS, "FacebookAudienceNetwork" }
            };
        }
    }
}
