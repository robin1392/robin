using ParrelSync;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CloneProjectConfiguration
{
    static CloneProjectConfiguration()
    {
        if (!ClonesManager.IsClone())
        {
            return;
        }
            
        var arg = ClonesManager.GetArgument();
        if (PlayerSettings.productName.Contains(arg))
        {
            return;
        }

        PlayerSettings.productName += $"_{arg}";
    }
}