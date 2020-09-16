﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class UI_MainStage : MonoBehaviour
    {
        public Transform[] arrTs_SpawnPos;

        private Animator[] arrAni_Model;

        private void Start()
        {
            if (arrAni_Model == null) arrAni_Model = new Animator[arrTs_SpawnPos.Length];
            StartCoroutine(Idle2Coroutine());
            Set();
        }

        public void Set()
        {
            if (arrAni_Model == null) arrAni_Model = new Animator[arrTs_SpawnPos.Length];
            
            int active = UserInfoManager.Get().GetActiveDeckIndex();
            var deck = UserInfoManager.Get().GetSelectDeck(active);
            
            var splitDeck = deck.Split('/');

            for (var i = 0; i < arrAni_Model.Length; i++)
            {
                if (arrAni_Model[i] != null) Destroy(arrAni_Model[i].gameObject);
                
                var num = int.Parse(splitDeck[i]);
                var data = JsonDataManager.Get().dataDiceInfo.GetData(num);
                
                var obj = FileHelper.LoadPrefab(data.modelName, data.loadType == 0 ? Global.E_LOADTYPE.LOAD_MAIN_MINION : Global.E_LOADTYPE.LOAD_MAIN_MAGIC);

                if (obj != null)
                {
                    var insObj = Instantiate(obj);
                    arrAni_Model[i] = insObj.GetComponent<Animator>();
                    arrAni_Model[i].transform.parent = arrTs_SpawnPos[i];
                    arrAni_Model[i].transform.localPosition = Vector3.zero;
                    arrAni_Model[i].transform.localRotation = Quaternion.identity;
                }

                if (data.moveType == 1)
                {
                    arrAni_Model[i].transform.localPosition += Vector3.up;
                }
            }
        }

        IEnumerator Idle2Coroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(1f, 10f));

                if (arrAni_Model != null)
                {
                    arrAni_Model[Random.Range(0, arrAni_Model.Length)].SetTrigger("Idle2");
                }
            }
        }
    }
}
