using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class UI_MainStage : MonoBehaviour
    {
        public Transform[] arrTs_SpawnPos;

        private Animator[] arrAni_Model;
        private static readonly int Idle2 = Animator.StringToHash("Idle2");
        private static readonly int Set1 = Animator.StringToHash("Set");

        private void Start()
        {
            if (arrAni_Model == null) arrAni_Model = new Animator[arrTs_SpawnPos.Length];
            Set();
        }

        public void Set()
        {
            if (arrAni_Model == null) arrAni_Model = new Animator[arrTs_SpawnPos.Length];
            
            int active = UserInfoManager.Get().GetActiveDeckIndex();
            var deck = UserInfoManager.Get().GetSelectDeck(active);
            
            //var splitDeck = deck.Split('/');

            for (var i = 0; i < arrAni_Model.Length; i++)
            {
                if (arrAni_Model[i] != null)
                {
                    if (arrAni_Model[i].transform.parent == arrTs_SpawnPos[i])
                        Destroy(arrAni_Model[i].gameObject);
                    else
                    {
                        Destroy(arrAni_Model[i].transform.parent.gameObject);
                    }
                }

                if (i == 5)
                {
                    arrAni_Model[i] = LoadGuardianModel(deck[i], arrTs_SpawnPos[i]);
                }
                else
                {
                    arrAni_Model[i] = LoadDiceModel(deck[i], arrTs_SpawnPos[i]);
                }
            }
        }

        private Animator LoadGuardianModel(int id, Transform parent)
        {
            if (TableManager.Get().GuardianInfo.GetData(id, out var guardianInfo) == false)
            {
                return null;
            }
                
            var obj = FileHelper.LoadPrefab(guardianInfo.modelName, Global.E_LOADTYPE.LOAD_MAIN_MINION);

            if (obj == null)
            {
                return null;
            }
            
            var insObj = Instantiate(obj);
            var animator = insObj.GetComponentInChildren<Animator>();
            insObj.transform.SetParent(parent);
            insObj.transform.localPosition = Vector3.zero;
            insObj.transform.localRotation = Quaternion.identity;
            animator.SetTrigger(Set1);
            
            if (guardianInfo.moveType == 1)
            {
                insObj.transform.localPosition += Vector3.up * 0.4f;
            }

            var particles = insObj.GetComponentsInChildren<ParticleSystem>();
            for (int j = 0; j < particles.Length; j++)
            {
                particles[j].Clear();
            }

            return animator;
        }

        Animator LoadDiceModel(int id, Transform parent)
        {
            if (TableManager.Get().DiceInfo.GetData(id, out var dataDiceInfo) == false)
            {
                return null;
            }
                
            var obj = FileHelper.LoadPrefab(dataDiceInfo.modelName, dataDiceInfo.loadType == 0 ? Global.E_LOADTYPE.LOAD_MAIN_MINION : Global.E_LOADTYPE.LOAD_MAIN_MAGIC);

            if (obj == null)
            {
                return null;
            }

            var insObj = Instantiate(obj);
            var animator = insObj.GetComponentInChildren<Animator>();
            insObj.transform.SetParent(parent);
            insObj.transform.localPosition = Vector3.zero;
            insObj.transform.localRotation = Quaternion.identity;
            animator.SetTrigger(Set1);
            
            if (dataDiceInfo.moveType == 1)
            {
                insObj.transform.localPosition += Vector3.up * 0.4f;
            }
            
            var particles = insObj.GetComponentsInChildren<ParticleSystem>();
            for (int j = 0; j < particles.Length; j++)
            {
                particles[j].Clear();
            }
            
            return animator;
        }

        public void Click_MainDiceIcon(int num)
        {
            arrAni_Model[num].SetTrigger(Idle2);
        }
    }
}
