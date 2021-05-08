using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Quantum;
using UnityEngine;

namespace _Scripts.Views
{
    public class ActorView : QuantumCallbacks
    {
        //TODO: 게임 시작 전에 비동기로 모델을 비롯한 리소스를 모두 프리로딩하고 동기로 사용한다.
        public ActorModel ActorModel;
        public EntityView EntityView;
        private EntityViewUpdater _viewUpdater;
        protected bool _initializing = false;
        protected bool _initialized = false;

        private void Start()
        {
            QuantumEvent.Subscribe<EventActionChanged>(this, OnActionChanged);
            QuantumEvent.Subscribe<EventActorHitted>(this, OnActorHitted);
            QuantumEvent.Subscribe<EventActorDeath>(this, OnActorDeath);
            QuantumEvent.Subscribe<EventPlayCasterEffect>(this, OnPlayCasterEffect);
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

        private void OnPlayCasterEffect(EventPlayCasterEffect callback)
        {
            if (ActorModel == null)
            {
                return;
            }

            if (EntityView.EntityRef.Equals(callback.Caster))
            {
                ResourceManager.LoadGameObjectAsyncAndReseveDeacivate(
                    callback.AssetName, 
                    transform.position,
                    Quaternion.identity).Forget();
            }
        }

        private void OnActorHitted(EventActorHitted callback)
        {
            if (ActorModel == null)
            {
                return;
            }
            
            if (EntityView.EntityRef.Equals(callback.Attacker))
            {
                if (ActorModel != null)
                {
                    ResourceManager.LoadGameObjectAsyncAndReseveDeacivate(
                        "Effect_ArrowHit", 
                        ActorModel.ShootingPosition.position,
                        Quaternion.identity).Forget();
                }
            }

            if (EntityView.EntityRef.Equals(callback.Victim))
            {
                if (callback.HitColor == HitColor.Fire)
                {
                    PlayRendererHitEffect();
                }
            }
        }
        
        public void PlayRendererHitEffect()
        {
            var amount = 0.0f;
            var targetValue = 0.7f;
            var duration = 0.2f;
            Tweener tweener = DOTween.To(() => amount, x => amount = x, targetValue, duration);
            tweener.OnUpdate(() => { ActorModel.RendererEffect.SetTintColor(Color.red, amount); });
            tweener.SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuad);
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
        
        private unsafe void OnActorDeath(EventActorDeath callback)
        {
            if (EntityView.EntityRef.Equals(callback.Victim) == false)
            {
                return;
            }

            OnActorDeathInternal(callback);
        }

        protected virtual void OnActorDeathInternal(EventActorDeath callback)
        {
        }
    }
}