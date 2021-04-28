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

[CreateAssetMenu(menuName = "Quantum/GOAPRoot", order = Quantum.EditorDefines.AssetMenuPriorityStart + 156)]
public partial class GOAPRootAsset : AssetBase {
  public Quantum.GOAPRoot Settings;

  public override Quantum.AssetObject AssetObject => Settings;
  
  public override void Reset() {
    if (Settings == null) {
      Settings = new Quantum.GOAPRoot();
    }
    base.Reset();
  }
}

public static partial class GOAPRootAssetExts {
  public static GOAPRootAsset GetUnityAsset(this GOAPRoot data) {
    return data == null ? null : UnityDB.FindAsset<GOAPRootAsset>(data);
  }
}
