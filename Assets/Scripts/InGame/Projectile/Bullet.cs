using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

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
        
        [HideInInspector]
        public PlayerController controller;
        [Space]
        public float moveSpeed = 0.5f;
        [HideInInspector]
        public float moveTime;
        public Material[] arrMaterial;
        
        protected BaseStat _target;
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

        private void Awake()
        {
            _poad = GetComponent<PoolObjectAutoDeactivate>();
        }

        public virtual void Initialize(int pTargetId, float pDamage, float splashRange, bool pIsMine, bool pIsBottomPlayer, UnityAction pCallback = null)
        {
            obj_Bullet.SetActive(true);
            obj_EndEffect.SetActive(false);
            
            _isTarget = true;
            _damage = pDamage;
            _splashRange = splashRange;
            _isMine = pIsMine;
            _isBottomPlayer = pIsBottomPlayer;
            _target = controller.targetPlayer.GetBaseStatFromId(pTargetId);
            
            if (_target)
            {
                this._callback = pCallback;
                SetColor();
                StartCoroutine(Move());
            }
            else
            {
                _poad.Deactive();
            }
        }

        public virtual void Initialize(Vector3 pTargetPos, float pDamage, float splashRange, bool pIsMine, bool pIsBottomPlayer, UnityAction pCallback = null)
        {
            _isTarget = false;
            _damage = pDamage;
            _splashRange = splashRange;
            _isMine = pIsMine;
            _isBottomPlayer = pIsBottomPlayer;
            _targetPos = pTargetPos;
            _callback = pCallback;
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

            _callback?.Invoke();

            if( (InGameManager.IsNetwork && _isMine) || InGameManager.IsNetwork == false )
                controller.AttackEnemyMinionOrMagic(_target.id, _damage, 0f);

            /*
            //if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && _isMine)
            if( InGameManager.IsNetwork && _isMine )
            {
                if (_target != null)
                    controller.HitMinionDamage( true , _target.id , _damage, 0f);
                //controller.targetPlayer.SendPlayer(RpcTarget.All, E_PTDefine.PT_HITMINIONANDMAGIC,_target.id, _damage, 0f);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false)
            {
                if (_target != null)
                    controller.HitMinionDamage( true , _target.id , _damage, 0f);
                //controller.targetPlayer.HitDamageMinionAndMagic(_target.id, _damage, 0f);
            }
            */


            if (obj_EndEffect != null)
            {
                transform.rotation = Quaternion.identity;
                obj_Bullet.SetActive(false);
                obj_EndEffect.SetActive(true);
                yield return new WaitForSeconds(endEffectDuration);
            }

            _poad.Deactive();
        }
    }
}