#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ED
{
    public class UI_Getted_Dice : MonoBehaviour
    {
        public Image image_Icon;
        public Button button_Use;
        public Button button_Info;
        public GameObject obj_Selected;
        public Transform ts_Move;
        public int slotNum;

        private Data_Dice _data;
        private UI_Panel_Dice _panelDice;
        private Transform _grandParent;

        private void Awake()
        {
            _panelDice = FindObjectOfType<UI_Panel_Dice>();
            _grandParent = transform.parent.parent;
        }

        public void Initialize(Data_Dice pData)
        {
            this._data = pData;
            image_Icon.sprite = pData.icon;
            button_Use.onClick.AddListener(() => { _panelDice.Click_Dice_Use(pData.id); });
            button_Info.onClick.AddListener(()=>{_panelDice.Click_Dice_Info(pData.id);});
        }

        public void Click_Dice()
        {
            _grandParent.BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);
            obj_Selected.SetActive(true);
            ts_Move.SetParent(_grandParent);

            var parent = transform.parent.parent;
            ((RectTransform) parent).DOAnchorPosY(
                Mathf.Clamp(Mathf.Abs(((RectTransform) transform).anchoredPosition.y - 200), 0,
                    (((RectTransform) parent).sizeDelta.y - ((float) Screen.height - 440f))) *
                ((RectTransform) parent.parent.parent).anchorMax.y, 0.3f);
        }

        public void DeactivateSelectedObject()
        {
            obj_Selected.SetActive(false);
            ts_Move.SetParent(transform);
        }
    }
}