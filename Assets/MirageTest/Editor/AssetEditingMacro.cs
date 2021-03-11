using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using Pathfinding;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Path = System.IO.Path;

namespace MirageTest.Editor
{
    public class AssetEditingMacro : EditorWindow
    {
        private Vector3 _scale;
        private Object _animatorController;
    
        [MenuItem ("RandomWars/AssetEditingMacro")]
        static void Init () {
            AssetEditingMacro window = (AssetEditingMacro)EditorWindow.GetWindow (typeof (AssetEditingMacro));
            window.Show();
        }
	
        void OnGUI () {
        
            GUILayout.Label("선택 폴더 레거시 스크립트 일괄 제거");
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("변경"))
                {
                    RemoveLegacyScript();
                }
            }

            EditorGUILayout.Space();
        }
        
        void RemoveLegacyScript()
        {
            foreach (var assetGuid in Selection.assetGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                var isValidFolder = AssetDatabase.IsValidFolder(assetPath);
                if (!isValidFolder)
                {
                    continue;
                }
        
                var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new string[]{assetPath});
                foreach (var prefabGuid in prefabGuids)
                {
                    var prefabPath = AssetDatabase.GUIDToAssetPath(prefabGuid);
                    var prefab = PrefabUtility.LoadPrefabContents(prefabPath);
                    
                    var aiPath = prefab.GetComponent<AIPath>();
                    if (aiPath != null)
                    {
                        Object.DestroyImmediate(aiPath);    
                    }
                    
                    var seeker = prefab.GetComponent<Seeker>();
                    if (seeker != null)
                    {
                        Object.DestroyImmediate(seeker);    
                    }

                    var rigidbody = prefab.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        Object.DestroyImmediate(rigidbody);    
                    }
                    
                    var blackboard = prefab.GetComponent<Blackboard>();
                    if (blackboard != null)
                    {
                        Object.DestroyImmediate(blackboard);    
                    }
                    
                    var behaviourTreeOwner = prefab.GetComponent<BehaviourTreeOwner>();
                    if (behaviourTreeOwner != null)
                    {
                        Object.DestroyImmediate(behaviourTreeOwner);    
                    }
                    
                    PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
                    PrefabUtility.UnloadPrefabContents(prefab);
                }            
            }
        }

        public void RenameAnimation()
        {
            foreach (var assetGuid in Selection.assetGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                var isValidFolder = AssetDatabase.IsValidFolder(assetPath);
                if (!isValidFolder)
                {
                    continue;
                }

                var aniGuids = AssetDatabase.FindAssets("", new string[] {assetPath});
                foreach (var aniGuid in aniGuids)
                {
                    var aniPath = AssetDatabase.GUIDToAssetPath(aniGuid);
                    var ani = (AnimationClip)AssetDatabase.LoadAssetAtPath(aniPath, typeof(AnimationClip));
                    var fileName = Path.GetFileNameWithoutExtension(aniPath);
                    var importer = (ModelImporter)AssetImporter.GetAtPath(aniPath);
                    RenameAndImport(importer, fileName);
                }
            }
        }

        private void RenameAndImport(ModelImporter asset, string name)
        {
            ModelImporter modelImporter = asset as ModelImporter;
            ModelImporterClipAnimation[] clipAnimations = modelImporter.defaultClipAnimations;

            for (int i = 0; i < clipAnimations.Length; i++)
            {
                clipAnimations[i].name = name;
            }

            modelImporter.clipAnimations = clipAnimations;
            modelImporter.SaveAndReimport();
        }
    }
}