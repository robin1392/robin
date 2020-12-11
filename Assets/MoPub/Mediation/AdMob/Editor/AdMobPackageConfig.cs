using System.Collections.Generic;

public class AdMobPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "AdMob"; }
    }

    public override string Version
    {
        get { return /*UNITY_PACKAGE_VERSION*/"1.8.1"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get
        {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, /*ANDROID_SDK_VERSION*/"19.5.0" },
                { Platform.IOS, /*IOS_SDK_VERSION*/"7.68.0" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get
        {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.GooglePlayServices" },
                { Platform.IOS, "MPGoogleAdMob" }
            };
        }
    }
}
