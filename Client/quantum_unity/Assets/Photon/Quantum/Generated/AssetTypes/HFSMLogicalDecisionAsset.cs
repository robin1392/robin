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

public abstract partial class HFSMLogicalDecisionAsset : HFSMDecisionAsset {

}

public static partial class HFSMLogicalDecisionAssetExts {
  public static HFSMLogicalDecisionAsset GetUnityAsset(this HFSMLogicalDecision data) {
    return data == null ? null : UnityDB.FindAsset<HFSMLogicalDecisionAsset>(data);
  }
}