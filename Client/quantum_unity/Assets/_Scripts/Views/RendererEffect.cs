using System.Linq;
using UnityEngine;

namespace _Scripts.Quantum.Views
{
    public class RendererEffect
    {
        private readonly Renderer[] _renderers;
        private readonly Material[][] _materials;
        private Material[][] _iceMaterials;
        private static Material _iceMaterialResource;
        
        private const string IceResourceName = "DiceState_Ice";
        private readonly int _tintColorProperty = Shader.PropertyToID("_TintColor");
        private readonly int _tintFactorProperty = Shader.PropertyToID("_TintFactor");

        private bool _isTinting = false;
        private Color _tintColor;
        private float _tintValue;

        public RendererEffect(GameObject gameObject)
        {
            var meshRenderer = gameObject.GetComponentsInChildren<MeshRenderer>() as Renderer[];
            var skinnedMeshRenderer =  gameObject.GetComponentsInChildren<SkinnedMeshRenderer>() as Renderer[];

            _renderers = meshRenderer.Union(skinnedMeshRenderer).ToArray();
            _materials = _renderers.Select(r => r.materials).ToArray();
        }

        public void Reset()
        {
            ResetToOriginal();
            ResetTint();
        }

        public void ResetToOriginal()
        {
            for (var i = 0; i < _renderers.Length; ++i)
            {
                var renderer = _renderers[i];
                renderer.materials = _materials[i];

                if (_isTinting)
                {
                    SetTintColor(renderer, _tintColor, _tintValue);
                }
                else
                {
                    ResetTint();
                }
            }
        }

        public void ChangeToIceMaterial()
        {
            if (_iceMaterialResource == null)
            {
                _iceMaterialResource = Resources.Load<Material>(IceResourceName);
            }

            if (_iceMaterials == null)
            {
                _iceMaterials = new Material[_renderers.Length][];
            }
            
            for (var i = 0; i < _renderers.Length; ++i)
            {
                var renderer = _renderers[i];

                var originalMaterials = renderer.materials;
                if (_iceMaterials[i] == null)
                {
                    _iceMaterials[i] = new Material[originalMaterials.Length]; 
                }
                
                for (var materialIndex = 0; materialIndex < originalMaterials.Length; ++materialIndex)
                {
                    if (_iceMaterials[i][materialIndex] == null)
                    {
                        _iceMaterials[i][materialIndex] = Object.Instantiate(_iceMaterialResource);
                        _iceMaterials[i][materialIndex].mainTexture = originalMaterials[materialIndex].mainTexture;
                    }
                }

                renderer.materials = _iceMaterials[i];

                if (_isTinting)
                {
                    SetTintColor(renderer, _tintColor, _tintValue);
                }
                else
                {
                    ResetTint();
                }
            }
        }

        public void SetTintColor(Color color, float value = 1.0f)
        {
            _tintColor = color;
            _isTinting = true;
            _tintValue = value;
            for (var i = 0; i < _renderers.Length; ++i)
            {
                var renderer = _renderers[i];
                SetTintColor(renderer, color, value);
            }
        }

        public void ResetTint()
        {
            _isTinting = false;
            for (var i = 0; i < _renderers.Length; ++i)
            {
                var renderer = _renderers[i];
                for (var materialIndex = 0; materialIndex < renderer.materials.Length; ++materialIndex)
                {
                    ResetTintColor(renderer);
                }
            }
        }

        void SetTintColor(Renderer renderer, Color color, float value)
        {
            var materials = renderer.materials;
            for (var materialIndex = 0; materialIndex < materials.Length; ++materialIndex)
            {
                materials[materialIndex].SetFloat(_tintFactorProperty, value);
                materials[materialIndex].SetColor(_tintColorProperty, color);   
            }
        }
        
        void ResetTintColor(Renderer renderer)
        {
            var materials = renderer.materials;
            for (var materialIndex = 0; materialIndex < materials.Length; ++materialIndex)
            {
                materials[materialIndex].SetFloat(_tintFactorProperty, 0);
            }
        }
    }
}