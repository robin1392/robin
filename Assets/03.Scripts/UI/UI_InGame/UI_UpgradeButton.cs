using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MirageTest.Scripts;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace ED
{
    public class UI_UpgradeButton : MonoBehaviour
    {
        public int index;
        public Button btn;
        public Image image_Icon;
        public Image image_SP;
        public Text text_Price;
        public Text text_Level;

        private int level;
        private RandomWarsResource.Data.TDataDiceInfo pData;
        
        public async UniTask Initialize(RandomWarsResource.Data.TDataDiceInfo dataDice, int level, int index)
        {
            this.index = index;
            this.pData = dataDice;
            this.level = level;
            image_Icon.sprite = await Addressables.LoadAssetAsync<Sprite>(dataDice.iconName);
            image_Icon.SetNativeSize();
            Refresh();
            
            // InGameManager.Get().event_SP_Edit.AddListener(EditSpCallback);
        }
        
        private RWNetworkClient _client;
        public void InitClient(RWNetworkClient client)
        {
            _client = client;
        }

        public void EditSpCallback(int sp)
        {
            if (this.level < 5)
            {
                btn.interactable = sp >= GetUpgradeCost();
            }
            else
            {
                btn.interactable = false;
            }

            var color = btn.interactable ? Color.white : Color.gray;
            text_Price.color = color;
            text_Level.color = color;
            image_SP.color = color;
        }

        int GetUpgradeCost()
        {
            int needSp = TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.DicePowerUpCost01 + level].value;
            return needSp;
        }

        public void Click()
        {
            // sp 작으면 리턴
            var localPlayerState = _client.GetLocalPlayerState();
            if (localPlayerState == null)
            {
                return;
            }
            
            if (localPlayerState.sp < GetUpgradeCost())
                return;

            var localPlayerProxy = _client.GetLocalPlayerProxy();
            localPlayerProxy.UpgradeIngameLevel(index);
            
            SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_UI_DICE_LEVEL_UP);
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
            text_Price.text = level < 5 ? GetUpgradeCost().ToString() : string.Empty;
            text_Level.text = $"Lv.{(level < 5 ? (level + 1).ToString() : "MAX")}";
            text_Level.color = level < 5 ? Color.white : Color.red;
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
