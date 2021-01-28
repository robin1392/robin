using System;
using System.Collections;
using System.Collections.Generic;
using Percent.Platform.InAppPurchase;
using Template.Shop.GameBaseShop.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Percent.Platform
{
    public class Shop : MonoBehaviour
    {
        public int shopID;
        [SerializeField] private Transform transformShopItemGrid;
        [SerializeField] private GameObject prefabShopItem;
        private List<ShopItem> listShopItem;
        [SerializeField] private int poolSize;
        [SerializeField] private Text textLeftTime;

        public virtual void Initialize(ShopInfo shopInfo)
        {
            if (shopInfo == null)
            {
                gameObject.SetActive(false);
                return;
            }
            
            listShopItem = new List<ShopItem>();

            for (int i = 0; i < poolSize; i++)
            {
                ShopItem shopItemBase = Instantiate(prefabShopItem, transformShopItemGrid).GetComponent<ShopItem>();
                shopItemBase.Initialize();
                listShopItem.Add(shopItemBase);
            }
            shopID = shopInfo.shopId;
            UpdateContent(shopInfo);
        }

        /// <summary>
        /// 상품 전개
        /// </summary>
        public void UpdateContent(ShopInfo shopInfo)
        {
            textLeftTime.text = shopInfo.resetRemainTime.ToString();
            if(poolSize<shopInfo.arrayProductInfo.Length)
                Debug.LogError("풀 사이즈보다 표시해야하는 상품이 많은 경우 별도로 처리 필요");
            
            for (int i = 0; i < poolSize; i++)
            {
                ShopItem shopItemBase = listShopItem[i];
                if (i < shopInfo.arrayProductInfo.Length)
                {
                    shopItemBase.UpdateContent(shopInfo, shopInfo.arrayProductInfo[i]);
                    listShopItem.Add(shopItemBase);    
                    shopItemBase.EnableContent();
                }
                else
                {
                    shopItemBase.DisableContent();
                }
            }
        }
        
        /// <summary>
        /// 개별 상점 리셋
        /// </summary>
        /// <param name="shopID">리셋할 상점 ID</param>
        public void ResetShop()
        {
            NetworkManager.session.ShopTemplate.ShopResetReq(NetworkManager.session.HttpClient, UserInfoManager.Get().GetUserInfo().userID, shopID,1, Reset);
        }

        
        public bool Reset(GameBaseShopErrorCode errorCode, ShopInfo shopInfo, ShopItemInfo payItemInfo)
        {
            if (errorCode == GameBaseShopErrorCode.Success)
            {
                if (payItemInfo != null)
                {
                    //리셋을 위해서 사용한 재화 연출 처리
                    //payItemInfo    
                }

                UpdateContent(shopInfo);
                return true;
            }
            else
            {
                Debug.LogError("에러발생");
                return false;
            }
        }
    }
}
