using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Percent.GameBaseClient;
using Percent.Platform.InAppPurchase;
using Template.Shop.GameBaseShop;
using Template.Shop.GameBaseShop.Common;
using Template.Shop.GameBaseShop.Table;
using UnityEngine;

namespace Percent.Platform.InAppPurchase
{
    public class ShopManager : ManagerSingleton<ShopManager>
    {
        [SerializeField] private Transform transformShopParent;
        
        [SerializeField] private GameObject prefabShop;
        
        private List<Shop> listShop;
        
        protected void Start()
        {
            //InitShop();
        }

        /// <summary>
        /// 상점 정보 불러오기
        /// </summary>
        public void InitShop()
        {
            if (listShop == null || listShop.Count == 0)
            {
                listShop = new List<Shop>();

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
            if (errorCode == GameBaseShopErrorCode.Success)
            {
                foreach (ShopInfo shopInfo in arrayShopInfo)
                {
                    // Shop shop = Instantiate(prefabShop, transformShopParent).GetComponent<Shop>();
                    // listShop.Add(shop);
                    // shop.Initialize(shopInfo);
                    string str = $"ShopID:{shopInfo.shopId}, e:{shopInfo.eventRemainTime}, r:{shopInfo.resetRemainTime}";
                    foreach (var productInfo in shopInfo.arrayProductInfo)
                    {
                        str += $"\nProductID:{productInfo.shopProductId}, BuyCount:{productInfo.buyCount}";
                    }
                    Debug.Log(str);
                }
                return true;
            }
            else
            {
                Debug.LogError("에러 발생");
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
                Debug.LogError("에러 발생");
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
            
                return true;
            }
            else
            {
                Debug.LogError("에러 발생");
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
