using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using UnityEngine;
using UnityEngine.Events;

namespace ED
{
    [RequireComponent(typeof(PoolObjectAutoDeactivate))]
    public class Bullet : MonoBehaviour
    {
        [Header("Bullet")]
        public GameObject obj_Bullet;
        [Header("End Effect")]
        public GameObject obj_EndEffect;
        public float endEffectDuration = 1f;
        
        public RWNetworkClient client;
        [Space]
        public float moveSpeed = 0.5f;
        [HideInInspector]
        public float moveTime;
        public Material[] arrMaterial;

        [Header("Audio")]
        public AudioSource audio_Explosion;
        
        protected BaseEntity _target;
        protected Vector3 _targetPos;
        protected UnityAction _callback;
        protected bool _isTarget;
        protected bool _isMine;
        protected bool _isBottomPlayer;
        protected PoolObjectAutoDeactivate _poad;
        protected float _damage;
        protected float _splashRange;

        protected int targetLayer => 1 << LayerMask.NameToLayer(_isBottomPlayer ? "TopPlayer" : "BottomPlayer");
        protected MeshRenderer[] _arrMeshRenderer;
        protected SkinnedMeshRenderer[] _arrSkinnedMeshRenderers;
        protected E_BulletType _bulletType;
        private float _effect;

        private void Awake()
        {
            _poad = GetComponent<PoolObjectAutoDeactivate>();
        }

        public virtual void Initialize(E_BulletType bulletType, BaseEntity target, float pDamage, float splashRange,
            bool pIsMine, bool pIsBottomPlayer, float effect)
        {
            obj_Bullet.SetActive(true);
            if (obj_EndEffect != null) obj_EndEffect.SetActive(false);

            _bulletType = bulletType;
            _isTarget = true;
            _damage = pDamage;
            _splashRange = splashRange;
            _isMine = pIsMine;
            _isBottomPlayer = pIsBottomPlayer;
            _target =target;
            _effect = effect;

            if (_target)
            {
                SetColor();
                StartCoroutine(Move());
            }
            else
            {
                _poad.Deactive();
            }
        }

        public virtual void Initialize(Vector3 pTargetPos, float pDamage, float splashRange, bool pIsMine, bool pIsBottomPlayer)
        {
            _isTarget = false;
            _damage = pDamage;
            _splashRange = splashRange;
            _isMine = pIsMine;
            _isBottomPlayer = pIsBottomPlayer;
            _targetPos = pTargetPos;
            SetColor();
            StartCoroutine(Move());
        }

        private void SetColor()
        {
            var isBlue = _isMine;
            if (InGameManager.Get().playType == PLAY_TYPE.CO_OP)
            {
                isBlue = _isBottomPlayer;
            }

            if (_arrMeshRenderer == null)
            {
                _arrMeshRenderer = GetComponentsInChildren<MeshRenderer>();
            }
            foreach (var m in _arrMeshRenderer)
            {
                m.material = arrMaterial[isBlue ? 0 : 1];
            }

            if (_arrSkinnedMeshRenderers == null)
            {
                _arrSkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }
            foreach (var m in _arrSkinnedMeshRenderers)
            {
                m.material = arrMaterial[isBlue ? 0 : 1];
            }
        }

        protected virtual IEnumerator Move()
        {
            var startPos = transform.position;
            var endPos = (_isTarget && _target != null) ? _target.ts_HitPos.position : _targetPos;
            var distance = Vector3.Distance(startPos, endPos);
            moveTime = distance / moveSpeed;

            if (_isTarget && _target != null) transform.LookAt(_target.ts_HitPos);
            else transform.LookAt(_targetPos);

            float t = 0;
            while (t < moveTime)
            {
                transform.position = Vector3.Lerp(startPos, (_isTarget && _target != null) ? _target.ts_HitPos.position : _targetPos, t / moveTime);
                if (_isTarget && _target != null) transform.LookAt(_target.ts_HitPos);
                t += Time.deltaTime;
                yield return null;
            }

            if (client.IsPlayingAI)
            {
                //TODO: 발사체가 날아가는 사이 공격자가 죽을 수 있기때문에 직접 히트를 부른다.
                //      액터 사망 시 유예시간을 두어서 공격자 액터프락시가 존재하지 않는 상황이 없도록 하는 방법을 고려해본다.
                if (_target != null && _target.ActorProxy != null)
                {
                    _target.ActorProxy.HitDamage(_damage);
                    if (_bulletType == E_BulletType.ICE_FREEZE_BULLET && _target != null && _target.ActorProxy != null)
                    {
                        _target.ActorProxy.AddBuff(BuffInfos.IceFreeze, _effect);
                        PoolManager.instance.ActivateObject("Effect_Ice_Freeze", _target.ActorProxy.transform.position);
                    }   
                }
            }

            if (obj_EndEffect != null)
            {
                transform.rotation = Quaternion.identity;
                obj_Bullet.SetActive(false);
                obj_EndEffect.SetActive(true);
                yield return new WaitForSeconds(endEffectDuration);
            }

            if (audio_Explosion != null) audio_Explosion.Play();
            _poad.Deactive();
        }
    }
}