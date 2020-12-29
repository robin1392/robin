using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ED
{
    public class UI_UpgradeButton : MonoBehaviour
    {
        public int num;
        public Button btn;
        public Image image_Icon;
        public Image image_SP;
        public Text text_Price;
        public Text text_Level;

        private int level;
        //private Data_Dice data;
        private RandomWarsResource.Data.TDataDiceInfo pData;
        private readonly int[] arrPrice = { 100, 200, 400, 700, 1100 };
    
        //public void Initialize(Data_Dice dataDice, int level)
        public void Initialize(RandomWarsResource.Data.TDataDiceInfo dataDice, int level)
        {
            this.pData = dataDice;
            this.level = level;
            image_Icon.sprite = FileHelper.GetIcon(dataDice.iconName);
            image_Icon.SetNativeSize();
            Refresh();

            InGameManager.Get().event_SP_Edit.AddListener(EditSpCallback);
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
            image_SP.color = color;
        }

        public void Click()
        {
            // sp 작으면 리턴
            if (InGameManager.Get().playerController.sp < arrPrice[level])
                return;
            
            if( InGameManager.IsNetwork == true )
                InGameManager.Get().SendInGameUpgrade(pData.id , num);
            else
            {
                level = InGameManager.Get().playerController.DiceUpgrade(num);
                InGameManager.Get().playerController.AddSp(-arrPrice[level - 1]);
                Refresh();
            }
        }

        public int GetDeckDiceId()
        {
            return pData.id;
        }
        public void RefreshLevel(int level)
        {
            this.level = level;
        }

        public void Refresh()
        {
            text_Price.text = level < 5 ? arrPrice[level].ToString() : string.Empty;
            text_Level.text = $"Lv.{(level < 5 ? (level + 1).ToString() : "MAX")}";
            image_SP.transform.parent.gameObject.SetActive(level < 5);
        }

        public void SetIconAlpha(float alpha)
        {
            image_Icon.DOFade(alpha, 0);
            image_SP.DOFade(alpha, 0);
            text_Level.DOFade(alpha * 1.5f, 0);
            text_Price.DOFade(alpha * 1.5f, 0);
        }
    }
}
