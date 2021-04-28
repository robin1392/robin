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

[CreateAssetMenu(menuName = "Quantum/BTNode/BTLeaf/SetNearestEnemyToTarget", order = Quantum.EditorDefines.AssetMenuPriorityStart + 44)]
public partial class SetNearestEnemyToTargetAsset : BTLeafAsset {
  public Quantum.SetNearestEnemyToTarget Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.SetNearestEnemyToTarget();
    }
    base.Reset();
  }
}

public static partial class SetNearestEnemyToTargetAssetExts {
  public static SetNearestEnemyToTargetAsset GetUnityAsset(this SetNearestEnemyToTarget data) {
    return data == null ? null : UnityDB.FindAsset<SetNearestEnemyToTargetAsset>(data);
  }
}
