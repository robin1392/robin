﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using DG.Tweening;

namespace ED
{
    public class UI_BattleFieldDiceSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        public Image image_Icon;
        public GameObject[] arrEyes;
        public Transform ts_Canvas;

        UI_DiceField ui_DiceField;
        [SerializeField] private Dice dice;
        static bool isDraging;
        static Dice dragDice;
        public Animator ani_Blind;
        public UI_Card ui_Card;
        public ParticleSystem ps;
        public Animator ani;

        private void Awake()
        {
            ui_DiceField = GetComponentInParent<UI_DiceField>();
        }

        public void SetIcon(float alpha = 1f)
        {
            if (dice != null && dice.id >= 0)
            {
                image_Icon.sprite = dice.GetIcon();
                image_Icon.enabled = true;
                var c = Color.white;
                c.a = alpha;
                image_Icon.color = c;
                for (var i = 0; i < arrEyes.Length; i++)
                {
                    arrEyes[i].SetActive(i == dice.eyeLevel);

                    if (arrEyes[i].activeSelf)
                    {
                        var images = arrEyes[i].GetComponentsInChildren<Image>();
                        foreach (var img in images)
                        {
                            img.color = FileHelper.GetColor(dice.diceData.color);
                        }
                    }
                }
            }
            else
            {
                Clear();
            }
        }

        public void SetDice(Dice pDice)
        {
            this.dice = pDice;
        }

        private void Clear()
        {
            this.dice = null;
            image_Icon.sprite = null;
            image_Icon.enabled = false;
            
            foreach (var eye in arrEyes)
            {
                eye.SetActive(false);
            }
        }

        private Vector2 _mouseDownPos;
        private static readonly int BBoing = Animator.StringToHash("BBoing");

        public void OnPointerDown(PointerEventData eventData)
        {
            if (dice != null && dice.id >= 0)
            {
                isDraging = true;
                _mouseDownPos = eventData.position;
                dragDice = dice;
                ui_DiceField.BroadcastMessage("SetHighlight", dice);
                ui_Card.gameObject.SetActive(true);
                ui_Card.Initialize(dice.diceData);
                DetachIcon();
            }
            else
            {
                isDraging = false;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isDraging)
            {
                ui_DiceField.BroadcastMessage("AttachIcon");

                if (ui_Card.gameObject.activeSelf && ui_Card.isAnimationRunning == false)
                {
                    ui_Card.Off();
                }
            }
            isDraging = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isDraging)
            {
                if (ui_Card.gameObject.activeSelf && ui_Card.isAnimationRunning == false)
                {
                    var distance = Vector2.Distance(eventData.position, _mouseDownPos);
                    
                    if (distance > 80f)
                    {
                        ui_Card.Off();
                    }
                }

                if (Camera.main != null)
                {
                    var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    image_Icon.transform.position = pos;
                }

                image_Icon.transform.localPosition += Vector3.forward * 100f;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isDraging)
            {
                ui_DiceField.BroadcastMessage("AttachIcon");

                if (ui_Card.gameObject.activeSelf && ui_Card.isAnimationRunning == false)
                {
                    ui_Card.Off();
                }
            }
            isDraging = false;
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (dice != null && dragDice != null && dice != dragDice && dice.id == dragDice.id && dice.eyeLevel == dragDice.eyeLevel)
            {
                if (dice.LevelUp(InGameManager.Get().playerController.arrDiceDeck))
                {
                    dragDice.Reset();
                    ui_DiceField.RefreshField();
                        
                    //if (PhotonNetwork.IsConnected)
                    if(InGameManager.IsNetwork == true)
                    {
                        //InGameManager.Get().playerController.SendPlayer(RpcTarget.Others , E_PTDefine.PT_LEVELUPDICE , dragDice.diceFieldNum, dice.diceFieldNum, dice.diceData.id, dice.eyeLevel);
                        //InGameManager.Get().playerController.SendPlayer(RpcTarget.Others , E_PTDefine.PT_LEVELUPDICE ,dragDice.diceFieldNum, dice.diceFieldNum, dice.diceData.id, dice.level);
                        InGameManager.Get().SendDiceLevelUp(dragDice.diceFieldNum, dice.diceFieldNum);
                    }
                    else
                    {
                        
                    }
                    
                    ani.SetTrigger(BBoing);
                }
            }
        }

        private void DetachIcon()
        {
            image_Icon.transform.SetParent(ts_Canvas);
        }

        public void AttachIcon()
        {
            image_Icon.transform.SetParent(transform);
            image_Icon.transform.localPosition = Vector3.zero;
            var arrImage = image_Icon.GetComponentsInChildren<Image>();
            
            if (image_Icon.color != Color.white)
            {
                for (var i = 0; i < arrImage.Length; ++i)
                {
                    //arrImage[i].DOColor(i == 0 ? Color.white : dice.data.color, 0.3f);
                    arrImage[i].DOColor(i == 0 ? Color.white : FileHelper.GetColor(dice.diceData.color), 0.3f);
                }
            }
        }

        public void SetHighlight(Dice d)
        {
            if (dice != null && d != dice && (d.diceData != dice.diceData || d.eyeLevel != dice.eyeLevel))
            {
                var arrImage = image_Icon.GetComponentsInChildren<Image>();
                foreach (var item in arrImage)
                {
                    item.DOColor(Color.white / 2.5f, 0.3f);
                }
            }
        }
    }
}