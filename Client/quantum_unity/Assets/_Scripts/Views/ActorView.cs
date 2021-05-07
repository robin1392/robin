using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Quantum;
using UnityEngine;

namespace _Scripts.Views
{
    public class ActorView : QuantumCallbacks
    {
        public ActorModel ActorModel;
        public EntityView EntityView;
        private EntityViewUpdater _viewUpdater;
        protected bool _initializing = false;
        protected bool _initialized = false;

        private void Start()
        {
            QuantumEvent.Subscribe<EventActionChanged>(this, OnActionChanged);
            QuantumEvent.Subscribe<EventActorHitted>(this, OnActorHitted);
            _viewUpdater = FindObjectOfType<EntityViewUpdater>();
        }

        async UniTask Init(QuantumGame game)
        {
            if (_initializing)
            {
                return;
            }
        
            _initializing = true;
        
            try
            {
                await OnInit(game);
                OnAfterInit();
            }
            catch (System.Exception)
            {
                return;
            }
            _initialized = true;
            _initializing = false;
        }

        private void OnAfterInit()
        {
            if (string.IsNullOrWhiteSpace(_animationTriggerPending) == false)
            {
                AnimationTrigger(_animationTriggerPending);
                _animationTriggerPending = null;
            }
        }

        protected virtual async UniTask OnInit(QuantumGame game)
        {
        }

        private void OnActorHitted(EventActorHitted callback)
        {
            if (EntityView.EntityRef.Equals(callback.Attacker))
            {
                if (ActorModel != null)
                {
                    ShowEffect(ActorModel.ShootingPosition.position).Forget();
                }
            }
        }

        async UniTask ShowEffect(Vector3 position)
        {
            var assetName = "Effect_ArrowHit";
            var go = await ResourceManager.LoadGameObjectAsync(assetName, position, Quaternion.identity);
            var poolable = go.GetComponent<PoolableObject>();
            poolable.AssetName = assetName;
            poolable.ReservePushBack();
        }

        private void OnActionChanged(EventActionChanged callback)
        {
            if (EntityView.EntityRef.Equals(callback.Entity))
            {
                AnimationTrigger(callback.State.ToString());
            }
        }

        private string _animationTriggerPending;
        void AnimationTrigger(string trigger)
        {
            if (ActorModel == null)
            {
                _animationTriggerPending = trigger;
                return;
            }
            
            ActorModel.Animator.SetTrigger(trigger);
        }
        
        public override void OnUpdateView(QuantumGame game)
        {
            if (EntityView.EntityRef == EntityRef.None)
            {
                return;
            }

            if (_initialized == false && _initializing == false)
            {
                Init(game).Forget();
                return;
            }

            if (_initialized == false)
            {
                return;
            }

            OnUpdateViewAfterInit(game);
        }
        
        protected virtual void OnUpdateViewAfterInit(QuantumGame game)
        {
        }
    }
}