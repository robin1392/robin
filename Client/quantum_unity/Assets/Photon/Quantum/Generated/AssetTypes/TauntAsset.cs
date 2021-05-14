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

[CreateAssetMenu(menuName = "Quantum/BTNode/BTLeaf/Taunt", order = Quantum.EditorDefines.AssetMenuPriorityStart + 45)]
public partial class TauntAsset : BTLeafAsset {
  public Quantum.Taunt Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.Taunt();
    }
    base.Reset();
  }
}

public static partial class TauntAssetExts {
  public static TauntAsset GetUnityAsset(this Taunt data) {
    return data == null ? null : UnityDB.FindAsset<TauntAsset>(data);
  }
}