using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ED;
using Percent.GameBaseClient;
using Percent.Platform.InAppPurchase;
using Template.Shop.GameBaseShop;
using Template.Shop.GameBaseShop.Common;
using Template.Shop.GameBaseShop.Table;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Percent.Platform.InAppPurchase
{
    public class ShopManager : ManagerSingleton<ShopManager>
    {
        [SerializeField] private Transform transformShopParent;
        
        [SerializeField] private GameObject prefabShop;
        
        [SerializeField]
        private List<Shop> listShop = new List<Shop>();

        private bool isInitialized;
        
        protected void Start()
        {
            //InitShop();
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
            listShop = new List<Shop>();
            
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
        public bool ShowBuyResult(GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo)
        {
            if (errorCode == GameBaseShopErrorCode.Success)
            {
                //구매한 상품에 대한 정보
                //shopProductInfo
            
                if (payItemInfo != null)
                {
                    //소모한 재화에 대한 연출 처리
                    //payItemInfo
                }
            
                //구매한 상품에 대한 결과 값
                //arrayRewardItemInfo
                for (int i = 0; i < arrayRewardItemInfo.Length; i++)
                {
                    Debug.Log($"GET == ID:{arrayRewardItemInfo[i].itemId}, Value:{arrayRewardItemInfo[i].value}");
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
        /// 상점 현금 결제 처리
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="shopId"></param>
        /// <param name="shopProductInfo"></param>
        /// <param name="arrayShopItemInfo"></param>
        /// <returns></returns>
        public bool ShowPurchaseResult(GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo)
        {
            return ShowBuyResult(errorCode, shopId, shopProductInfo, payItemInfo, arrayRewardItemInfo);
        }
    }
}
