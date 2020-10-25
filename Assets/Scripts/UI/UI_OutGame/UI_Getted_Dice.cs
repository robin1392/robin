#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ED
{
    public class UI_Getted_Dice : MonoBehaviour
    {
        public Image image_Icon;
        public Image image_Eye;
        public Button button_Use;
        public Button button_Info;
        public GameObject obj_Selected;
        public Transform ts_Move;
        public int slotNum;

        //private Data_Dice _data;
        private DiceInfoData _data;
        
        private UI_Panel_Dice _panelDice;
        private Transform _grandParent;

        public Image image_GradeBG;
        public Sprite[] arrSprite_GradeBG;
        public Material mtl_Grayscale;

        private bool isUngetted;

        private void Awake()
        {
            _panelDice = FindObjectOfType<UI_Panel_Dice>();
            _grandParent = transform.parent.parent;

            if (image_GradeBG == null)
            {
                image_GradeBG = this.transform.Find("Parent/Image").GetComponent<Image>();
            }
        }

        public void Initialize(DiceInfoData pData)
        {
            this._data = pData;
            //if(FileHelper.GetIcon( pData.iconName ) == null)
                //print("eoroerejorjagasjdf   " + pData.iconName);
            image_Icon.sprite = FileHelper.GetIcon( pData.iconName );
            image_Eye.color = FileHelper.GetColor(pData.color);
            button_Use.onClick.AddListener(() => { _panelDice.Click_Dice_Use(pData.id); });
            button_Info.onClick.AddListener(()=>{_panelDice.Click_Dice_Info(pData.id);});

            image_GradeBG.sprite = arrSprite_GradeBG[pData.grade];
        }

        public void Click_Dice()
        {
            _grandParent.BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);
            obj_Selected.SetActive(true);
            ts_Move.SetParent(_grandParent);

            var parent = transform.parent.parent;
            ((RectTransform)parent).DOAnchorPosY(
                Mathf.Clamp(Mathf.Abs((((RectTransform)transform.parent).anchoredPosition.y + 980) + ((RectTransform)transform).anchoredPosition.y - 200), 0,
                    ((RectTransform)parent).sizeDelta.y - ((RectTransform)parent.parent).rect.height), 0.3f);
        }

        public void DeactivateSelectedObject()
        {
            obj_Selected.SetActive(false);
            ts_Move.SetParent(transform);
        }

        public void SetGrayscale()
        {
            isUngetted = true;

            var images = GetComponentsInChildren<Image>();
            foreach(var item in images)
            {
                item.material = mtl_Grayscale;
            }
        }
    }
}