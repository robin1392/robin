/**
 * This code was auto-generated by a tool, every time
 * the tool executes this code will be reset.
 *
 * If you need to extend the classes generated to add 
 * fields or methods to them, please create partial 
 * declarations in another file.
 **/

using Quantum;
using UnityEngine;


public partial class CheckBlackboardIntAsset : HFSMDecisionAsset {
  public Quantum.CheckBlackboardInt Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
}

public static partial class CheckBlackboardIntAssetExts {
  public static CheckBlackboardIntAsset GetUnityAsset(this CheckBlackboardInt data) {
    return data == null ? null : UnityDB.FindAsset<CheckBlackboardIntAsset>(data);
  }
}
