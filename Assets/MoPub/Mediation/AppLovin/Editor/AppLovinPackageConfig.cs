using System.Collections.Generic;

public class AppLovinPackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "AppLovin"; }
    }

    public override string Version
    {
        get { return /*UNITY_PACKAGE_VERSION*/"1.6.2"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, /*ANDROID_SDK_VERSION*/"9.14.12" },
                { Platform.IOS, /*IOS_SDK_VERSION*/"6.14.11" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.AppLovin" },
                { Platform.IOS, "AppLovin" }
            };
        }
    }
}
