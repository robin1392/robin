#if !UNITY_WINDOW

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using Percent.Boomlagoon.JSON;

namespace Percent.Build
{
    public static class XcodeOption
    {
        [PostProcessBuild(999)]
        public static void OnPostProcessBuild( BuildTarget buildTarget, string path)
        {
            if(buildTarget == BuildTarget.iOS)
            {
                // 여기서 xcode 프로젝트의 설정을 추가합니다.
                // 참고 사이트 : https://xcodebuildsettings.com/
                {
                    string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
                    PBXProject pbxProject = new PBXProject();
                    pbxProject.ReadFromFile(projectPath);

                    // xcode main target 을 가져옵니다 ======================
                    string target = pbxProject.GetUnityMainTargetGuid();
                    pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
                    pbxProject.SetBuildProperty(target, "VALIDATE_WORKSPACE", "YES");

                    // 파이어베이스 7.0.2 버전과 페이스북 8.0 이상 버전 사용시 충돌나는 문제 해결하는 코드
                    //string podDatas = PercentEditorUtils.GetFileData(path + "/Podfile");
                    // if(podDatas.Contains("pod 'FBSDKCoreKit', '~> 8.0'") && podDatas.Contains("pod 'Firebase/Core', '7.0.0'"))
                    //     pbxProject.AddFileToBuild(target, pbxProject.AddFile("GoogleService-Info.plist", "GoogleService-Info.plist"));
                    // ===================================================

                    // unity framework target 을 가져옵니다 =================
                    string unityTarget = pbxProject.GetUnityFrameworkTargetGuid();
                    pbxProject.SetBuildProperty(unityTarget, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
                    // ===================================================


                    pbxProject.WriteToFile (projectPath);
                }

                // 여기서 info.plist 파일의 설정을 추가합니다.
                {
                    string infoPlistPath = path + "/Info.plist";

                    PlistDocument plistDoc = new PlistDocument();
                    plistDoc.ReadFromFile(infoPlistPath);
                    if (plistDoc.root != null) 
                    {
                        PlistElementDict root = plistDoc.root;

                        // SKADNetwork 설정
                        PlistElementArray array = root.CreateArray("SKAdNetworkItems");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "22mmun2rn5.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "238da6jt44.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "24t9a8vw3c.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "2u9pt9hc89.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "3qy4746246.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "3rd42ekr43.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "3sh42y64q3.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "424m5254lk.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "4468km3ulz.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "44jx6755aq.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "44n7hlldy6.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "488r3q3dtq.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "4dzt52r2t5.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "4fzdc2evr5.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "4pfyvq9l8r.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "523jb4fst2.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "52fl2v3hgk.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "578prtvx9j.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "5a6flpkh64.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "5l3tpt7t6e.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "5lm9lj6jb7.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "737z793b9f.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "7953jerfzd.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "7rz58n8ntl.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "7ug5zh24hu.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "8s468mfl3y.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "97r2b46745.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "9rd848q2bz.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "9t245vhmpl.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "9yg77x724h.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "av6w8kgt66.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "bvpn9ufa9b.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "c6k4g5qg8m.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "cg4yq2srnc.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "cj5566h2ga.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "cstr6suwn9.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "ejvt5qm6ak.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "f38h382jlk.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "f73kdq92p3.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "g28c52eehv.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "ggvn48r87g.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "glqzh8vgby.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "gta9lk7p23.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "gvmwg8q7h5.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "hs6bdukanm.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "kbd757ywx3.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "klf5c3l5u5.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "lr83yxwka7.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "ludvb6z3bs.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "m8dbw4sv7c.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "mlmmfzh3r3.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "mls7yz5dvl.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "mtkv5xtk9e.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "n38lu8286q.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "n66cz3y3bx.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "n9x2a789qt.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "nzq8sh4pbs.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "p78axxw29g.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "ppxm28t8ap.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "prcb7njmu6.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "pu4na253f3.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "s39g8k73mm.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "t38b2kh725.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "tl55sbb4fm.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "u679fj5vs4.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "uw77j35x4d.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "v72qych5uu.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "v79kvwwj4g.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "v9wttpbfk9.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "w9q455wk68.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "wg4vff78zm.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "wzmmz9fp6w.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "xy9t38ct57.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "yclnxrl5pm.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "ydx93a7ass.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "yrqqpx2mcb.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "z4gj7hsk7h.skadnetwork");
                        AddArrayDict(ref array, "SKAdNetworkIdentifier", "zmvfpc5aq8.skadnetwork");

                        root.SetString("AppLovinSdkKey", "CR4KmCdD0TUPNE-ARH4HtdfmokMLxvZ8KdS4q5IqZnEx0C5STAkKoO5a6Rw2LTAg6dlxk2mHp-CqAHGdR_zEO3");
                        root.SetString("GADApplicationIdentifier", "ca-app-pub-9932267989523399~6796722927");

                        plistDoc.WriteToFile(infoPlistPath);
                    }
                    else 
                    {
                        Debug.LogError("Percent [Build] ERROR: Can't open " + infoPlistPath);
                    }
                }
            }
        }

        static void AddArrayDict(ref PlistElementArray array, string key, string value)
        {
            PlistElementDict dict = array.AddDict();
            dict.SetString(key, value);
        }

    }
}

#endif