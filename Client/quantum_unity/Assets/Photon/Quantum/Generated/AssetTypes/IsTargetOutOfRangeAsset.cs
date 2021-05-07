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

[CreateAssetMenu(menuName = "Quantum/BTNode/BTDecorator/IsTargetOutOfRange", order = Quantum.EditorDefines.AssetMenuPriorityStart + 34)]
public partial class IsTargetOutOfRangeAsset : BTDecoratorAsset {
  public Quantum.IsTargetOutOfRange Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.IsTargetOutOfRange();
    }
    base.Reset();
  }
}

public static partial class IsTargetOutOfRangeAssetExts {
  public static IsTargetOutOfRangeAsset GetUnityAsset(this IsTargetOutOfRange data) {
    return data == null ? null : UnityDB.FindAsset<IsTargetOutOfRangeAsset>(data);
  }
}
