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

[CreateAssetMenu(menuName = "Quantum/BTNode/BTLeaf/ApproachToTarget", order = Quantum.EditorDefines.AssetMenuPriorityStart + 26)]
public partial class ApproachToTargetAsset : BTLeafAsset {
  public Quantum.ApproachToTarget Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.ApproachToTarget();
    }
    base.Reset();
  }
}

public static partial class ApproachToTargetAssetExts {
  public static ApproachToTargetAsset GetUnityAsset(this ApproachToTarget data) {
    return data == null ? null : UnityDB.FindAsset<ApproachToTargetAsset>(data);
  }
}