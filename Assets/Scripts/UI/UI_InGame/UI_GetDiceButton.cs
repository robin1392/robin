using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ED
{
    public class UI_GetDiceButton : MonoBehaviour
    {
        public Button button;
        public Image image_Button;
        public Text text_SP;

        private void Start()
        {
            InGameManager.Instance.event_SP_Edit.AddListener(EditSpCallback);
        }

        private void EditSpCallback(int sp)
        {
            button.interactable = sp >= InGameManager.Instance.getDiceCost;
            image_Button.color = button.interactable ? Color.white : Color.gray;
            text_SP.color = image_Button.color;
        }
    }
}