// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
//
// If you need to extend the classes generated to add
// fields or methods to them, please create partial  
// declarations in another file.
// </auto-generated>

using Quantum;
using UnityEngine;

[CreateAssetMenu(menuName = "Quantum/BTNode/BTLeaf/WaitForTrigger", order = Quantum.EditorDefines.AssetMenuPriorityStart + 48)]
public partial class WaitForTriggerAsset : BTLeafAsset {
  public Quantum.WaitForTrigger Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.WaitForTrigger();
    }
    base.Reset();
  }
}

public static partial class WaitForTriggerAssetExts {
  public static WaitForTriggerAsset GetUnityAsset(this WaitForTrigger data) {
    return data == null ? null : UnityDB.FindAsset<WaitForTriggerAsset>(data);
  }
}