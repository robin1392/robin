using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using Percent.Platform.InAppPurchase;
using RandomWarsResource.Data;
using Service.Template;
using Template.Shop.GameBaseShop;
using Template.Shop.GameBaseShop.Common;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using EBuyTypeKey = RandomWarsResource.Data.EBuyTypeKey;
using TDataShopInfo = RandomWarsResource.Data.TDataShopInfo;

namespace Percent.Platform
{
    public class Shop : MonoBehaviour
    {
        public bool isInitialized { get; private set; }

        public int shopID;
        [SerializeField] private Transform transformShopItemGrid;
        [SerializeField] private GameObject prefabShopItem;
        private List<ShopItem> listShopItem;
        [SerializeField] private int poolSize;
        [SerializeField] private Text textLeftTime;
        [SerializeField] private Button btn_Reset;

        private ShopInfo info;

        private void Start()
        {
            DisableContent();
        }

        public void EnableContent()
        {
            gameObject.SetActive(true);
        }

        public void DisableContent()
        {
            gameObject.SetActive(false);
        }
        
        public virtual void Initialize(ShopInfo shopInfo)
        {
            if (shopInfo == null)
            {
                gameObject.SetActive(false);
                return;
            }

            listShopItem = new List<ShopItem>();

            if (prefabShopItem != null)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    ShopItem shopItemBase = Instantiate(prefabShopItem, transformShopItemGrid).GetComponent<ShopItem>();
                    shopItemBase.Initialize(0);
                    listShopItem.Add(shopItemBase);
                }
            }

            shopID = shopInfo.shopId;
            UpdateContent(shopInfo);
            isInitialized = true;
        }

        /// <summary>
        /// 상품 전개
        /// </summary>
        public void UpdateContent(ShopInfo shopInfo)
        {
            if (prefabShopItem == null)
            {
                int childCount = transformShopItemGrid.childCount;
                for (int i = childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(transformShopItemGrid.GetChild(i).gameObject);
                }

                for (int i = 0; i < shopInfo.listProductInfo.Count; i++)
                {
                    TDataShopProductList data;
                    if (TableManager.Get().ShopProductList
                        .GetData(shopInfo.listProductInfo[i].shopProductId, out data))
                    {
                        var shopItemBase = Instantiate(FileHelper.LoadShopUIPrefab(data.shopImage), transformShopItemGrid).GetComponent<ShopItemBig>();
                        shopItemBase.Initialize(shopInfo.listProductInfo[i].shopProductId);
                        listShopItem.Add(shopItemBase);
                        shopItemBase.UpdateContent(shopInfo, shopInfo.listProductInfo[i]);
                    }
                }
            }
            
            this.info = shopInfo;
            //textLeftTime.text = shopInfo.resetRemainTime.ToString();
            if (shopInfo.resetRemainTime > 0)
            {
                StopAllCoroutines();
                StartCoroutine(TimeleftCoroutine(shopInfo.resetRemainTime));
            }
            
            // if(poolSize<shopInfo.listProductInfo.Length)
            //     Debug.LogError("풀 사이즈보다 표시해야하는 상품이 많은 경우 별도로 처리 필요");
            
            if (prefabShopItem != null)
            {
                for (int i = 0; i < poolSize; i++)
                {
                    ShopItem shopItemBase = listShopItem[i];
                    if (i < shopInfo.listProductInfo.Count)
                    {
                        shopItemBase.UpdateContent(shopInfo, shopInfo.listProductInfo[i]);
                        listShopItem.Add(shopItemBase);    
                        shopItemBase.EnableContent();
                    }
                    else
                    {
                        shopItemBase.DisableContent();
                    }
                }
            }

            if (btn_Reset != null)
            {
                TDataShopInfo data;
                if (TableManager.Get().ShopInfo.GetData(sinfo => sinfo.id == shopID, out data))
                {
                    if (data.isReset)
                    {
                        int maxADCount = data.resetAdValue;
                        int remainADCount = maxADCount - info.adResetCount;
                        int maxPointCount = data.resetBuyValue.Length;
                        int remainPointCount = maxPointCount - info.pointResetCount;

                        btn_Reset.interactable = remainADCount > 0 || remainPointCount > 0;
                    }
                }
            }
        }

        IEnumerator TimeleftCoroutine(int remainTime)
        {
            DateTime resetDate = DateTime.Now.AddSeconds(remainTime);
            TimeSpan subTime = resetDate.Subtract(DateTime.Now);

            while (subTime.TotalSeconds > 0)
            {
                string str = string.Empty;
                if (subTime.Days > 0) str += $"{subTime.Days:D2}일 ";
                if (subTime.Hours > 0) str += $"{subTime.Hours:D2}시간 ";
                if (subTime.Minutes > 0) str += $"{subTime.Minutes:D2}분 ";
                //str += $"{subTime.Seconds:D2}초";
                textLeftTime.text = str;
                
                yield return new WaitForSeconds(1f);
                
                subTime = resetDate.Subtract(DateTime.Now);
            }

            TDataShopInfo data;
            if (TableManager.Get().ShopInfo.GetData(shopID, out data))
            {
                if (data.isReset)
                {
                    ResetShop(EBuyTypeKey.None);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        public void Click_ResetShopButton()
        {
            TDataShopInfo data;
            if (TableManager.Get().ShopInfo.GetData(sinfo => sinfo.id == shopID, out data))
            {
                if (data.isReset)
                {
                    int maxADCount = data.resetAdValue;
                    int remainADCount = maxADCount - info.adResetCount;
                    int maxPointCount = data.resetBuyValue.Length;
                    int remainPointCount = maxPointCount - info.pointResetCount;
                    int cost = data.resetBuyValue[info.pointResetCount];
                    
                    UI_Main.Get().dailyShopResetPopup.Initialize(maxADCount, remainADCount, maxPointCount, remainPointCount, cost, ResetShop);
                }
            }
        }
        
        /// <summary>
        /// 개별 상점 리셋
        /// </summary>
        /// <param name="shopID">리셋할 상점 ID</param>
        public void ResetShop(RandomWarsResource.Data.EBuyTypeKey type)
        {
            int typeNum = (int) type;
            if (typeNum < 0) typeNum = 0;
            NetworkManager.session.ShopTemplate.ShopResetReq(NetworkManager.session.HttpClient, shopID, typeNum, Reset);
            UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        }

        
        public bool Reset(GameBaseShopErrorCode errorCode, ShopInfo shopInfo, ItemBaseInfo payItemInfo)
        {
            UI_Main.Get().obj_IndicatorPopup.SetActive(false);
            
            if (errorCode == GameBaseShopErrorCode.Success)
            {
                if (payItemInfo != null)
                {
                    //리셋을 위해서 사용한 재화 연출 처리
                    //payItemInfo   
                    switch ((ITEM_TYPE)payItemInfo.ItemId)
                    {
                        case ITEM_TYPE.GOLD:
                        {
                            UserInfoManager.Get().GetUserInfo().gold += payItemInfo.Value;
                        }
                            break;
                        case ITEM_TYPE.DIAMOND:
                        {
                            UserInfoManager.Get().GetUserInfo().diamond += payItemInfo.Value;
                        }
                            break;
                    }

                    UI_Main.Get().RefreshUserInfoUI();
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
