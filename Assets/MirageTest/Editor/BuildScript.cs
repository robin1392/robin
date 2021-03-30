using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using Debug = UnityEngine.Debug;

public class BuildScript
{
    public static string BuildRoot = "/build";
    public static string BinaryParentFormat = "game_{0}";
    public const string Name = "game";
    public static string BuildPath => $"{BuildRoot}/{string.Format(BinaryParentFormat, 0)}/{Name}";

    public static HashSet<string> ExceptPath = new HashSet<string>
    {
        "Assets/HeadlessBuilder/Resources",
    };

    public const string AssetsFolder = "Assets";

    [MenuItem("Build/LogPath")]
    public static void LogPath()
    {
        Debug.Log(BuildPath);
    }

    [MenuItem("Build/Linux Server Build")]
    public static void BuildLinuxServer()
    {
        IgnoreUnusedAssetForServer();
        string[] levels = new string[] {"Assets/01.Scenes/MirageServerScene.Unity"};
        BuildPipeline.BuildPlayer(levels, BuildPath, BuildTarget.StandaloneLinux64, BuildOptions.EnableHeadlessMode);

        System.IO.File.WriteAllText($"{BuildRoot}/install.sh",
            @"for var in {1..19}
    do
    cp -r game_0 game_${var}
done");
    }

    [MenuItem("Build/IgnoreUnusedAssetForServer")]
    public static void IgnoreUnusedAssetForServer()
    {
        DisableResourcesFolder(AssetsFolder);
        
        var guids = AssetDatabase.FindAssets("t:spriteAtlas");
        foreach (string guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
            atlas.SetIncludeInBuild(false);
        }

        AssetDatabase.Refresh();
    }

    static void DisableResourcesFolder(string folder)
    {
        var resourcesFolder = "Resources";
        var subFolders = AssetDatabase.GetSubFolders(folder);
        foreach (var sub in subFolders)
        {
            if (IsExceptPath(sub))
            {
                continue;
            }
            
            if (System.IO.Path.GetFileName(sub) == resourcesFolder)
            {
                var replace = sub.Replace(resourcesFolder, $"~{resourcesFolder}");
                Debug.Log($"{AssetDatabase.MoveAsset(sub, replace)}");
            }
            else
            {
                DisableResourcesFolder(sub);
            }
        }
    }

    static bool IsExceptPath(string path)
    {
        foreach (var except in ExceptPath)
        {
            if (path.Contains(except))
            {
                return true;
            }
        }

        return false;
    }
}

[HeadlessCallbacks]
public static class ServerBuildHeadlessCallbacks
{
    public static void HeadlessBuildBefore()
    {
        ED.Debug.Log("HeadlessBuildBefore");
        BuildScript.IgnoreUnusedAssetForServer();
        System.IO.File.WriteAllText($"{BuildScript.BuildRoot}/install.sh",
            @"for var in {1..19}
    do
    cp -r game_0 game_${var}
done");
    }

    public static void HeadlessBuildSuccess()
    {
        ED.Debug.Log("HeadlessBuildSuccess");
    }
    
}

