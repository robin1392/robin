using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ED
{
    public class UI_UpgradeButton : MonoBehaviour
    {
        public int num;
        public Button btn;
        public Image icon;
        public Image image_sp;
        public Text text_Price;
        public Text text_Level;

        private int level;
        private Data_Dice data;
        private readonly int[] arrPrice = { 100, 200, 400, 700, 1100 };

        public void Initialize(Data_Dice dataDice, int level)
        {
            this.data = dataDice;
            this.level = level;
            icon.sprite = dataDice.icon;
            Refresh();

            InGameManager.Instance.event_SP_Edit.AddListener(EditSpCallback);
        }

        private void EditSpCallback(int sp)
        {
            if (this.level < 5)
            {
                btn.interactable = sp >= arrPrice[level];
            }
            else
            {
                btn.interactable = false;
            }

            var color = btn.interactable ? Color.white : Color.gray;
            text_Price.color = color;
            text_Level.color = Color.black;
            image_sp.color = color;
        }

        public void Click()
        {
            level = InGameManager.Instance.playerController.DiceUpgrade(num);
            InGameManager.Instance.playerController.AddSp(-arrPrice[level - 1]);
            Refresh();
        }

        private void Refresh()
        {
            text_Price.text = level < 5 ? arrPrice[level].ToString() : string.Empty;
            text_Level.text = $"Lv.{(level < 5 ? (level + 1).ToString() : "MAX")}";
            if (level >= 5)
            {
                image_sp.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
