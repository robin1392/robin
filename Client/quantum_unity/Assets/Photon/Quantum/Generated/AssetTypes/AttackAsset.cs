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

[CreateAssetMenu(menuName = "Quantum/BTNode/BTLeaf/Attack", order = Quantum.EditorDefines.AssetMenuPriorityStart + 26)]
public partial class AttackAsset : BTLeafAsset {
  public Quantum.Attack Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.Attack();
    }
    base.Reset();
  }
}

public static partial class AttackAssetExts {
  public static AttackAsset GetUnityAsset(this Attack data) {
    return data == null ? null : UnityDB.FindAsset<AttackAsset>(data);
  }
}
