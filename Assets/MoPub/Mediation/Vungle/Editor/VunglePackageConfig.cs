using System.Collections.Generic;

public class VunglePackageConfig : PackageConfig
{
    public override string Name
    {
        get { return "Vungle"; }
    }

    public override string Version
    {
        get { return /*UNITY_PACKAGE_VERSION*/"1.2.4"; }
    }

    public override Dictionary<Platform, string> NetworkSdkVersions
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, /*ANDROID_SDK_VERSION*/"6.7.0" },
                { Platform.IOS, /*IOS_SDK_VERSION*/"6.7.0" }
            };
        }
    }

    public override Dictionary<Platform, string> AdapterClassNames
    {
        get {
            return new Dictionary<Platform, string> {
                { Platform.ANDROID, "com.mopub.mobileads.Vungle" },
                { Platform.IOS, "Vungle" }
            };
        }
    }
}
