using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ED;
using Percent.GameBaseClient;
using Percent.Platform.InAppPurchase;
using RandomWarsProtocol;
using Template.Shop.GameBaseShop;
using Template.Shop.GameBaseShop.Common;
using Template.Shop.GameBaseShop.Table;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Percent.Platform.InAppPurchase
{
    public class ShopManager : ManagerSingleton<ShopManager>
    {
        [SerializeField] private RectTransform rts_ScrollView;
        [SerializeField] private Transform transformShopParent;
        
        [SerializeField] private GameObject prefabShop;
        
        [SerializeField]
        private List<Shop> listShop = new List<Shop>();

        private bool isInitialized;
        
        protected void Start()
        {
            var safeArea = Screen.safeArea;
            var canvas = GetComponentInParent<Canvas>();
            
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= canvas.pixelRect.width;
            anchorMin.y /= canvas.pixelRect.height;
            anchorMax.x /= canvas.pixelRect.width;
            anchorMax.y /= canvas.pixelRect.height;
 
            rts_ScrollView.anchorMax = anchorMax;
        }

        /// <summary>
        /// 상점 정보 불러오기
        /// </summary>
        public void InitShop()
        {
            if (isInitialized == false)
            {
                UI_Main.Get().obj_IndicatorPopup.SetActive(true);
                //listShop = new List<Shop>();

                NetworkManager.session.ShopTemplate.ShopInfoReq(NetworkManager.session.HttpClient,
                    UserInfoManager.Get().GetUserInfo().userID, SetAllShop);
            }
        }
        
        /// <summary>
        /// 상점 정보 초기화하기
        /// </summary>
        public void RefreshShop()
        {
            //listShop = new List<Shop>();
            
            NetworkManager.session.ShopTemplate.ShopInfoReq(NetworkManager.session.HttpClient, UserInfoManager.Get().GetUserInfo().userID, RefreshAllShop);    
        }
        
        /// <summary>
        /// 모든 상점 초기화
        /// </summary>
        /// <param name="errorCode">네트워크 결과 값</param>
        /// <param name="arrayShopInfo">상점 결과 값</param>
        /// <returns>네트워크 처리가 정상적으로 됐는지 여부</returns>
        private bool SetAllShop(GameBaseShopErrorCode errorCode, ShopInfo[] arrayShopInfo)
        {
            UI_Main.Get().obj_IndicatorPopup.SetActive(false);
            
            if (errorCode == GameBaseShopErrorCode.Success)
            {
                isInitialized = true;
                
                for (int i = 0; i < arrayShopInfo.Length; ++i)
                {
                    // Shop shop = Instantiate(prefabShop, transformShopParent).GetComponent<Shop>();
                    // listShop.Add(shop);
                    // shop.Initialize(shopInfo);
                    string str = $"ShopID:{arrayShopInfo[i].shopId}, e:{arrayShopInfo[i].eventRemainTime}, r:{arrayShopInfo[i].resetRemainTime}";
                    foreach (var productInfo in arrayShopInfo[i].arrayProductInfo)
                    {
                        str += $"\nProductID:{productInfo.shopProductId}, BuyCount:{productInfo.buyCount}";
                    }
                    Debug.Log(str);
                }

                int count = 0;
                for (int i = 0; i < listShop.Count; i++)
                {
                    if (i + 1 == arrayShopInfo[count].shopId)
                    {
                        listShop[i].EnableContent();
                        listShop[i].Initialize(arrayShopInfo[count]);
                        count++;
                    }
                    else listShop[i].DisableContent();
                }
                return true;
            }
            else
            {
                Debug.Log($"에러 발생 : {errorCode}");
                UI_ErrorMessage.Get().ShowMessage($"Error : {errorCode}");
                return false;
            }
        }
        
        /// <summary>
        /// 모든 상점 초기화
        /// </summary>
        /// <param name="errorCode">네트워크 결과 값</param>
        /// <param name="arrayShopInfo">상점 결과 값</param>
        /// <returns>네트워크 처리가 정상적으로 됐는지 여부</returns>
        private bool RefreshAllShop(GameBaseShopErrorCode errorCode, ShopInfo[] arrayShopInfo)
        {
            if (errorCode == GameBaseShopErrorCode.Success)
            {
                
                foreach (ShopInfo shopInfo in arrayShopInfo)
                {
                    foreach (Shop shop in listShop)
                    {
                        if(shop.shopID==shopInfo.shopId)
                            shop.UpdateContent(shopInfo);
                    }
                }
                return true;
            }
            else
            {
                Debug.Log($"에러 발생 : {errorCode}");
                UI_ErrorMessage.Get().ShowMessage($"Error : {errorCode}");
                return false;
            }
        }
        
        /// <summary>
        /// 상점 인게임 재화 구매 처리
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="shopId"></param>
        /// <param name="shopProductInfo"></param>
        /// <param name="payItemInfo"></param>
        /// <param name="arrayRewardItemInfo"></param>
        /// <returns></returns>
        public bool ShowBuyResult(GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo, MsgQuestData[] arrayQuestData)
        {
            UI_Main.Get().obj_IndicatorPopup.SetActive(false);
            
            if (errorCode == GameBaseShopErrorCode.Success)
            {
                //구매한 상품에 대한 정보
                //shopProductInfo
                switch (shopId)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 5:
                    case 6:
                        RefreshShop();
                        break;
                }
            
                if (payItemInfo != null)
                {
                    //소모한 재화에 대한 연출 처리
                    //payItemInfo
                    ITEM_TYPE type;
                    switch (payItemInfo.itemId)
                    {
                        case 1:
                            type = ITEM_TYPE.GOLD;
                            UserInfoManager.Get().GetUserInfo().gold += payItemInfo.value;
                            break;
                        case 2:
                            type = ITEM_TYPE.DIAMOND;
                            UserInfoManager.Get().GetUserInfo().diamond += payItemInfo.value;
                            break;
                        case 11:
                            type = ITEM_TYPE.KEY;
                            UserInfoManager.Get().GetUserInfo().key += payItemInfo.value;
                            break;
                        default:
                            type = ITEM_TYPE.NONE;
                            break;
                    }
                    UI_GetProduction.Get().RefreshProduct(type);
                }
            
                //구매한 상품에 대한 결과 값
                //arrayRewardItemInfo
                MsgReward[] arr = new MsgReward[arrayRewardItemInfo.Length];
                for (int i = 0; i < arrayRewardItemInfo.Length; i++)
                {
                    Debug.Log($"GET == ID:{arrayRewardItemInfo[i].itemId}, Value:{arrayRewardItemInfo[i].value}");
                    arr[i] = new MsgReward();
                    arr[i].ItemId = arrayRewardItemInfo[i].itemId;
                    arr[i].Value = arrayRewardItemInfo[i].value;
                }
                UI_Main.Get().AddReward(arr, ShopItem.pos);
            
                return true;
            }
            else
            {
                Debug.Log($"에러 발생 : {errorCode}");
                UI_ErrorMessage.Get().ShowMessage($"Error : {errorCode}");
                return false;
            }
        }
        
        /// <summary>
        /// 상점 현금 결제 처리
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="shopId"></param>
        /// <param name="shopProductInfo"></param>
        /// <param name="arrayShopItemInfo"></param>
        /// <returns></returns>
        public bool ShowPurchaseResult(GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo, MsgQuestData[] arrayQuestData)
        {
            return ShowBuyResult(errorCode, shopId, shopProductInfo, payItemInfo, arrayRewardItemInfo, arrayQuestData);
        }
    }
}
