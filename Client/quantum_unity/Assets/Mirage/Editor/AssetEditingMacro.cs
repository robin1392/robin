using Pathfinding;
using UnityEditor;
using UnityEngine;
using Path = System.IO.Path;

namespace MirageTest.Editor
{
    public class AssetEditingMacro : EditorWindow
    {
        private Vector3 _scale;
        private Object _animatorController;

        public GameObject shadowPrefab;
    
        [MenuItem ("RandomWars/AssetEditingMacro")]
        static void Init () {
            AssetEditingMacro window = (AssetEditingMacro)GetWindow (typeof (AssetEditingMacro));
            window.Show();
        }
	
        void OnGUI () {
        
            GUILayout.Label("선택 폴더 레거시 스크립트 일괄 제거");
            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("변경"))
                {
                    Do();
                }
            }

            EditorGUILayout.Space();
        }

        void Do()
        {
            var oldShader = Shader.Find("Toony Colors Pro 2/User/RWOutline");
            var newShader = Shader.Find("Shader Graphs/DefaultBody");
            
            foreach (var assetGuid in Selection.assetGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                var isValidFolder = AssetDatabase.IsValidFolder(assetPath);
                if (!isValidFolder)
                {
                    continue;
                }
        
                var materials = AssetDatabase.FindAssets("t:Material", new string[]{assetPath});
                foreach (var materialGuid in materials)
                {
                    var materialPath = AssetDatabase.GUIDToAssetPath(materialGuid);
                    var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                    var texture = material.GetTexture("_BaseMap");
                    if (material.shader != oldShader)
                    {
                        continue;
                    }
                    
                    material.shader = newShader;
                    material.SetTexture("_MainTex", texture);

                    Debug.Log($"{materialPath} Changed");
                }            
            }
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
                        DestroyImmediate(aiPath);    
                    }
                    
                    var seeker = prefab.GetComponent<Seeker>();
                    if (seeker != null)
                    {
                        DestroyImmediate(seeker);    
                    }

                    // var rigidbody = prefab.GetComponent<Rigidbody>();
                    // if (rigidbody != null)
                    // {
                    //     Object.DestroyImmediate(rigidbody);    
                    // }

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