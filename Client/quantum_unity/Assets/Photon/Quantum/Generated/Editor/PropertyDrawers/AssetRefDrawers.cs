using Quantum;
using UnityEngine;
using UnityEditor;
namespace Quantum.Editor {

[CustomPropertyDrawer(typeof(AssetRefAIAction))]
public class AssetRefAIActionPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(AIActionAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefAIBlackboard))]
public class AssetRefAIBlackboardPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(AIBlackboardAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefAIBlackboardInitializer))]
public class AssetRefAIBlackboardInitializerPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(AIBlackboardInitializerAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefAIConfig))]
public class AssetRefAIConfigPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(AIConfigAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefBTComposite))]
public class AssetRefBTCompositePropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(BTCompositeAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefBTDecorator))]
public class AssetRefBTDecoratorPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(BTDecoratorAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefBTNode))]
public class AssetRefBTNodePropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(BTNodeAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefBTRoot))]
public class AssetRefBTRootPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(BTRootAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefBTService))]
public class AssetRefBTServicePropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(BTServiceAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefGOAPRoot))]
public class AssetRefGOAPRootPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(GOAPRootAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefGOAPTask))]
public class AssetRefGOAPTaskPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(GOAPTaskAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefHFSMDecision))]
public class AssetRefHFSMDecisionPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(HFSMDecisionAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefHFSMRoot))]
public class AssetRefHFSMRootPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(HFSMRootAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefHFSMState))]
public class AssetRefHFSMStatePropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(HFSMStateAsset));
  }
}


[CustomPropertyDrawer(typeof(AssetRefHFSMTransitionSet))]
public class AssetRefHFSMTransitionSetPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(HFSMTransitionSetAsset));
  }
}

}
