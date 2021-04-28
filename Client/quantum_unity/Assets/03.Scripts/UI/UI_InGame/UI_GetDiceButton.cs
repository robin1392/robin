using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ED
{
    public class UI_GetDiceButton : MonoBehaviour
    {
        public Button button;
        public Image[] arrImage;
        public Text[] arrText;

        private void Start()
        {
            // InGameManager.Get().event_SP_Edit.AddListener(EditSpCallback);
        }

        /// <summary>
        /// 버튼 활성화 여부를 판단 후 SetImageAndText 호출 할 것
        /// </summary>
        /// <param name="sp"></param>
        public void EditSpCallback(bool enable)
        {
            button.interactable = enable;
            SetImageAndText();
        }

        protected void SetImageAndText()
        {
            Color color = button.interactable ? Color.white : Color.gray;
            foreach (var image in arrImage)
            {
                image.color = color;
            }

            foreach (var text in arrText)
            {
                text.color = color;
            }
        }


        public void SetInteracterButton(bool interactive)
        {
            button.interactable = interactive;
            SetImageAndText();
        }
    }
}