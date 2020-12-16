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

        private Dictionary<int, GameObject[]> dicAura = new Dictionary<int, GameObject[]>();

    private void Start()
        {
            if (arrAni_Model == null) arrAni_Model = new Animator[arrTs_SpawnPos.Length];
            //StartCoroutine(Idle2Coroutine());
            for (int i = 0; i < arrTs_SpawnPos.Length; i++)
            {
                dicAura.Add(i, new GameObject[4]);
                dicAura[i][0] = arrTs_SpawnPos[i].GetChild(0).gameObject;
                dicAura[i][1] = arrTs_SpawnPos[i].GetChild(1).gameObject;
                dicAura[i][2] = arrTs_SpawnPos[i].GetChild(2).gameObject;
                dicAura[i][3] = arrTs_SpawnPos[i].GetChild(3).gameObject;
            }
            Set();
        }

        public void Set()
        {
            //Debug.Log("Main stage set");
            if (arrAni_Model == null) arrAni_Model = new Animator[arrTs_SpawnPos.Length];
            
            int active = UserInfoManager.Get().GetActiveDeckIndex();
            var deck = UserInfoManager.Get().GetSelectDeck(active);
            
            //var splitDeck = deck.Split('/');

            for (var i = 0; i < arrAni_Model.Length; i++)
            {
                if (arrAni_Model[i] != null) Destroy(arrAni_Model[i].gameObject);

                //var num = int.Parse(splitDeck[i]);
                Table.Data.TDataDiceInfo dataDiceInfo;
                if (TableManager.Get().DiceInfo.GetData(deck[i], out dataDiceInfo) == false)
                {
                    return;
                }
                
                var obj = FileHelper.LoadPrefab(dataDiceInfo.modelName, dataDiceInfo.loadType == 0 ? Global.E_LOADTYPE.LOAD_MAIN_MINION : Global.E_LOADTYPE.LOAD_MAIN_MAGIC);

                if (obj != null)
                {
                    var insObj = Instantiate(obj);
                    arrAni_Model[i] = insObj.GetComponent<Animator>();
                    arrAni_Model[i].transform.parent = arrTs_SpawnPos[i];
                    arrAni_Model[i].transform.localScale = Vector3.one;
                    arrAni_Model[i].transform.localPosition = Vector3.zero;
                    arrAni_Model[i].transform.localRotation = Quaternion.identity;
                    arrAni_Model[i].SetTrigger(Set1);

                    if (dicAura.Count > 0)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            dicAura[i][j].SetActive(j == dataDiceInfo.grade);
                        }
                    }
                }

                if (dataDiceInfo.moveType == 1)
                {
                    arrAni_Model[i].transform.localPosition += Vector3.up;
                }

                var particles = arrAni_Model[i].GetComponentsInChildren<ParticleSystem>();
                for (int j = 0; j < particles.Length; j++)
                {
                    particles[j].Clear();
                }
            }
        }

        IEnumerator Idle2Coroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(1f, 8f));

                var rnd = Random.Range(0, arrAni_Model.Length);
                if (arrAni_Model != null && arrAni_Model[rnd] != null)
                {
                    arrAni_Model[rnd].SetTrigger(Idle2);
                }
            }
        }

        public void Click_MainDiceIcon(int num)
        {
            arrAni_Model[num].SetTrigger(Idle2);
        }
    }
}
