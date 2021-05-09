using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Quantum;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Views
{
   public class ProjcetileView : QuantumCallbacks
   {
        public EntityView EntityView;
        private EntityViewUpdater _viewUpdater;
        private bool _initialized;
        private PoolableObject _model;
        private Transform _target;
        protected float _targetRadius;
        private Vector3 _startPosition;
        private Vector3 _lastTargetPosition;
        private float _duration;
        private float _elapsedTime;
        private bool _isEntityDestroyed;
        private bool _isProjectileDestroyed;
        private string _hitEffectAssetName;
       
        private void Awake()
        {
            _viewUpdater = FindObjectOfType<EntityViewUpdater>();
        }
       
        public void OnEntityDestroyed(QuantumGame game)
        {
            _isEntityDestroyed = true;
        }

        public override void OnUpdateView(QuantumGame game)
        {
            if (EntityView.EntityRef == EntityRef.None)
            {
                return;
            }

            if (_initialized == false)
            {
                Init(game).Forget();
                return;
            }
            
            if (_isEntityDestroyed && _isProjectileDestroyed)
            {
                Destroy(gameObject);
            }

            if (_isProjectileDestroyed)
            {
                return;
            }

            _elapsedTime += Time.deltaTime;
            var t = _elapsedTime / _duration;
            if (_target != null)
            {
                var targetPosition = new Vector3(_target.position.x, _startPosition.y, _target.position.z);
                _lastTargetPosition = (targetPosition + (_startPosition - targetPosition).normalized * _targetRadius);
            }
            
            transform.LookAt(_lastTargetPosition);
            transform.position = Vector3.Lerp(_startPosition, _lastTargetPosition, t);

            if (t >= 1)
            {
                _isProjectileDestroyed = true;
                Pool.Push(_model);
                _model = null;
                
                ResourceManager.LoadGameObjectAsyncAndReseveDeacivate(
                    _hitEffectAssetName,
                    transform.position,
                    Quaternion.identity).Forget();
            }
        }
        
        async UniTask Init(QuantumGame game)
        {
            var f = game.Frames.Verified;
            
            var projectile = f.Get<Projectile>(EntityView.EntityRef);
            _target = _viewUpdater.GetView(projectile.Defender)?.transform;
            if (_target == null)
            {
                _isProjectileDestroyed = true;
                return;
            }

            var attackerView = _viewUpdater.GetView(projectile.Attacker)?.GetComponent<ActorView>();
            //TODO: view가 널 일 수 있다. EntityView ManualDispose보다 늦게 호출되도록 해야한다.
            if (attackerView == null)
            {
                _isProjectileDestroyed = true;
                return;
            }

            var currentTime = f.Number * f.DeltaTime;
            var endTime = projectile.HitTime;
            _duration = (endTime - currentTime).AsFloat;
            _hitEffectAssetName = projectile.HitEffect;
            _targetRadius = f.Get<PhysicsCollider2D>(projectile.Defender).Shape.Circle.Radius.AsFloat;
            _startPosition = attackerView.ActorModel.ShootingPosition.transform.position;
            transform.position = _startPosition;
            
            _initialized = true;
            
            _model = await ResourceManager.LoadPoolableAsync<PoolableObject>(projectile.Model, Vector3.zero, Quaternion.identity);
            _model.transform.SetParent(transform, false);
        }
   }
}