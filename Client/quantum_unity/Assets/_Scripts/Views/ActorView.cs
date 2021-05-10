using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Scripts.Resourcing;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ED;
using Quantum;
using UnityEngine;
using Debug = ED.Debug;

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
        protected AnimationSpeed _animationSpeed;

        public class AnimationSpeed
        {
            private float _freezeSpeed = 1;
            private float _actionSpeed = 1;
            private Animator _animator;

            public AnimationSpeed(Animator animator)
            {
                _animator = animator;
                UpdateSpeed();
            }
            public void SetFreeze(float val)
            {
                _freezeSpeed = val;
                UpdateSpeed();
            }
            
            public void SetActionSpeed(float val)
            {
                _actionSpeed = val;
                UpdateSpeed();
            }

            void UpdateSpeed()
            {
                _animator.speed = _freezeSpeed * _actionSpeed;
            }
        }

        private void Start()
        {
            QuantumEvent.Subscribe<EventActionChanged>(this, OnActionChanged);
            QuantumEvent.Subscribe<EventActionChangedWithSpeed>(this, OnActionChangedWithSpeed);
            QuantumEvent.Subscribe<EventActorHitted>(this, OnActorHitted);
            QuantumEvent.Subscribe<EventPlayCasterEffect>(this, OnPlayCasterEffect);
            QuantumEvent.Subscribe<EventBuffStateChanged>(this, OnBuffStateChanged);
            QuantumEvent.Subscribe<EventPlaySound>(this, OnPlaySound);
        }

        async UniTask Init(QuantumGame game)
        {
            _initializing = true;
        
            await OnInit(game);
            OnAfterInit();
            
            _initialized = true;
            _initializing = false;
        }

        private void OnAfterInit()
        {
            if (string.IsNullOrWhiteSpace(_animationTriggerPending) == false)
            {
                AnimationTrigger(_animationTriggerPending, _animationSpeedPending);
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
        
        private void OnBuffStateChanged(EventBuffStateChanged callback)
        {
            if (EntityView.EntityRef.Equals(callback.Entity))
            {
                var buffState = (BuffType) callback.BuffState;
                // EnableBuffEffect((BuffType)callback.BuffState, BuffType.Shield, "Shield", EffectLocation.Bottom);
                // EnableBuffEffect(buffType, BuffType.Stun, "Effect_Sturn", EffectLocation.Top);
                EnableFreezeEffect((buffState & BuffType.Freeze) != 0);
                EnableBuffEffect(buffState, BuffType.Freeze, AssetNames.EffectIceState, EffectLocation.Bottom);
                // EnableBuffEffect(buffType, BuffType.Taunted, "Effect_Taunted", EffectLocation.Top);
            }
        }
        
        public Dictionary<BuffType, PoolableObject> _dicEffectPool =
            new Dictionary<BuffType, PoolableObject>();
        
        private void EnableFreezeEffect(bool b)
        {
            if (b)
            {
                _animationSpeed.SetFreeze(0);
                ActorModel.RendererEffect.ChangeToIceMaterial();
            }
            else
            {
                _animationSpeed.SetFreeze(1);
                ActorModel.RendererEffect.ResetToOriginal();
            }
        }
        
        private void OnPlaySound(EventPlaySound callback)
        {
            if (EntityView.EntityRef.Equals(callback.Actor))
            {
                PlaySound(callback.AssetName).Forget();
            }
        }
        
        protected async UniTask PlaySound(string clipName)
        {
            if (string.IsNullOrEmpty(clipName))
            {
                return;
            }

            var clip = await ResourceManager.LoadClip(clipName);
            SoundManager.instance.Play(clip);
        }
        
        void EnableBuffEffect(BuffType buffState, BuffType buffType, string resource, EffectLocation effectLocation)
        {
            if ((buffState & buffType) != 0)
            {
                EnableBuffEffect(buffType, resource, effectLocation);    
            }
            else
            {
                DisableBuffEffect(buffType);
            }
        }

        public void EnableBuffEffect(BuffType buffType, string resource,  EffectLocation effectLocation)
        {
            if (_dicEffectPool.ContainsKey(buffType) == false)
            {
                var ad = PreloadedResouceManager.LoadPoolable<PoolableObject>(resource ,GetEffectPosition(effectLocation), Quaternion.identity);
                ad.transform.SetParent(transform);
                _dicEffectPool.Add(buffType, ad);
            }
        }

        public void DisableBuffEffect(BuffType buffType)
        {
            if (_dicEffectPool.TryGetValue(buffType, out var ad))
            {
                _dicEffectPool.Remove(buffType);
                Pool.Push(ad);
            }
        }
        
        public Vector3 GetEffectPosition(EffectLocation effectLocation)
        {
            switch (effectLocation)
            {
                case EffectLocation.Top:
                    if (ActorModel.TopEffectPosition != null)
                    {
                        return ActorModel.TopEffectPosition.transform.position;
                    }
                    
                    return ActorModel.HitPosition.transform.position + new Vector3(0, 0.65f, 0);
                
                case EffectLocation.Mid:
                    return ActorModel.HitPosition.transform.position;
                case EffectLocation.Bottom:
                    return ActorModel.transform.position;
            }
            
            return ActorModel.transform.position;
        }

        private void OnActionChanged(EventActionChanged callback)
        {
            if (EntityView.EntityRef.Equals(callback.Entity))
            {
                AnimationTrigger(callback.State.ToString(), 1);
            }
        }
        
        private void OnActionChangedWithSpeed(EventActionChangedWithSpeed callback)
        {
            if (EntityView.EntityRef.Equals(callback.Entity))
            {
                AnimationTrigger(callback.State.ToString(), callback.Speed.AsFloat);
            }
        }

        private string _animationTriggerPending;
        private float _animationSpeedPending;
        void AnimationTrigger(string trigger, float speed)
        {
            if (ActorModel == null)
            {
                _animationTriggerPending = trigger;
                _animationSpeedPending = speed;
                return;
            }
            
            ActorModel.Animator.SetTrigger(trigger);
            _animationSpeed.SetActionSpeed(speed);
        }

        public void OnEntityDestroyed(QuantumGame game)
        {
            OnEntityDestroyedInternal(game);
        }

        protected virtual void OnEntityDestroyedInternal(QuantumGame game)
        {
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

        protected void TiltActorModel()
        {
            var modelTransform = ActorModel.transform;
            var tilt =  modelTransform.worldToLocalMatrix * new Vector3(20, 0, 0);
            modelTransform.transform.localPosition = new Vector3(0, 0.1f, 0);
            
            if (CameraController.IsBottomOrientation)
            {
                ActorModel.transform.localEulerAngles = tilt;
            }
            else
            {
                ActorModel.transform.localEulerAngles = -tilt;
            }
        }
    }
    
    public enum EffectLocation
    {
        Top,
        Mid,
        Bottom
    }
}