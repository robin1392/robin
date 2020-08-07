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
        public PlayerController controller;
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

        protected int targetLayer => 1 << LayerMask.NameToLayer(_isBottomPlayer ? "TopPlayer" : "BottomPlayer");

        private void Awake()
        {
            _poad = GetComponent<PoolObjectAutoDeactivate>();
        }

        public virtual void Initialize(int pTargetId, float pDamage, bool pIsMine, bool pIsBottomPlayer, UnityAction pCallback = null)
        {
            _isTarget = true;
            _damage = pDamage;
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

        public void Initialize(Vector3 pTargetPos, float pDamage, bool pIsMine, bool pIsBottomPlayer, UnityAction pCallback = null)
        {
            _isTarget = false;
            _damage = pDamage;
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

            var mr = GetComponentsInChildren<MeshRenderer>();
            foreach (var m in mr)
            {
                m.material = arrMaterial[isBlue ? 0 : 1];
            }
            var smr = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var m in smr)
            {
                m.material = arrMaterial[isBlue ? 0 : 1];
            }
        }

        protected virtual IEnumerator Move()
        {
            var startPos = transform.position;
            var endPos = _isTarget ? _target.ts_HitPos.position : _targetPos;
            var distance = Vector3.Distance(startPos, endPos);
            moveTime = distance / moveSpeed;

            if (_isTarget) transform.LookAt(_target.ts_HitPos);
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

            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && _isMine)
            {
                if (_target != null)
                    controller.targetPlayer.SendPlayer(RpcTarget.All, E_PTDefine.PT_HITMINION,_target.id, _damage, 0f);
                //controller.targetPlayer.photonView.RPC("HitDamageMinion", RpcTarget.All, _target.id, _damage, 0f);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                if (_target != null)
                    controller.targetPlayer.HitDamageMinion(_target.id, _damage, 0f);
            }

            _poad.Deactive();
        }
    }
}