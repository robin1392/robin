using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Scripts.Resourcing;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
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

        void Init(QuantumGame game)
        {
            var f = game.Frames.Verified;
            if (f.Exists(EntityView.EntityRef) == false)
            {
                return;
            }
            
            OnInit(game);
            OnAfterInit();
            
            _initialized = true;
        }

        private void OnAfterInit()
        {
            if (string.IsNullOrWhiteSpace(_animationTriggerPending) == false)
            {
                AnimationTrigger(_animationTriggerPending);
                _animationTriggerPending = null;
            }

            if (ActorModel != null)
            {
                if (ActorModel.Animator != null)
                {
                    ActorModel?.Animator?.SetFloat("MoveSpeed", 1.0f);    
                }
            }
            
            if (_animationSpeedPending.HasValue)
            {
                SetAttackSpeed(_animationSpeedPending.Value);
                _animationSpeedPending = null;
            }
        }

        protected virtual void OnInit(QuantumGame game)
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
                if (ActorModel?.ShootingPosition != null)
                {
                    ResourceManager.LoadGameObjectAsyncAndReseveDeacivate(
                        AssetNames.EffectArrowHit, 
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
                var ad = PreloadedResourceManager.LoadPoolable<PoolableObject>(resource ,GetEffectPosition(effectLocation), Quaternion.identity);
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
                AnimationTrigger(callback.State.ToString());
            }
        }
        
        //TODO: 기본공격 속도만 제어한다. 네이밍 수정
        private void OnActionChangedWithSpeed(EventActionChangedWithSpeed callback)
        {
            if (EntityView.EntityRef.Equals(callback.Entity))
            {
                AnimationTrigger(callback.State.ToString());
                SetAttackSpeed(callback.Speed.AsFloat);
            }
        }

        private string _animationTriggerPending;
        private float? _animationSpeedPending;
        void AnimationTrigger(string trigger)
        {
            if (ActorModel == null)
            {
                _animationTriggerPending = trigger;
                return;
            }

            if (trigger == "Walk")
            {
                ActorModel.Animator.SetFloat("MoveSpeed", 1.0f);
            }
            else if(trigger == "Idle")
            {
                ActorModel.Animator.SetFloat("MoveSpeed", 0.0f);
            }
            else
            {
                ActorModel.Animator.SetTrigger(trigger);
            }
        }

        void SetAttackSpeed(float attackSpeed)
        {
            if (ActorModel == null)
            {
                _animationSpeedPending = attackSpeed;
                return;
            }
            
            ActorModel.Animator.SetFloat("AttackSpeed", attackSpeed);
        }

        public void OnEntityDestroyed(QuantumGame game)
        {
            OnEntityDestroyedInternal(game);
        }

        protected virtual void OnEntityDestroyedInternal(QuantumGame game)
        {
            EntityView.EntityRef = EntityRef.None;
            Destroy(gameObject);
        }

        public override void OnUpdateView(QuantumGame game)
        {
            if (EntityView.EntityRef == EntityRef.None)
            {
                return;
            }

            if (_initialized == false)
            {
                Init(game);
            }

            OnUpdateViewAfterInit(game);
        }
        
        protected virtual void OnUpdateViewAfterInit(QuantumGame game)
        {
        }

        protected void TiltActorModel()
        {
            if (ActorModel == null)
            {
                return;
            }
            
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
