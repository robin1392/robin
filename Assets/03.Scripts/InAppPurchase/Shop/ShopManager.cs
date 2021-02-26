using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DG.Tweening;
using ED;
using Percent.GameBaseClient;
using Percent.Platform.InAppPurchase;
//using RandomWarsProtocol;
using Service.Core;
using Template.Shop.GameBaseShop;
using Template.Shop.GameBaseShop.Common;
using Template.Shop.GameBaseShop.Table;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace Percent.Platform.InAppPurchase
{
    public class ShopManager : SingletonDestroy<ShopManager>
    {
        [SerializeField] private RectTransform rts_ScrollView;
        [SerializeField] private Transform transformShopParent;
        
        [SerializeField] private GameObject prefabShop;
        
        [SerializeField]
        private List<Shop> listShop = new List<Shop>();

        public RectTransform rts_TargetDiashop;
        public RectTransform rts_TargetGoldshop;
        
        private bool isInitialized;
        private bool isShowDiamondShop;
        private bool isShowGoldShop;
        
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

                NetworkManager.session.ShopTemplate.ShopInfoReq(NetworkManager.session.HttpClient, SetAllShop);
            }
            else
            {
                if (isShowGoldShop) Invoke("ScrollToGoldShop", 0.1f);
                else if (isShowDiamondShop) Invoke("ScrollToDiamondShop", 0.1f);
            }
        }
        
        /// <summary>
        /// 상점 정보 초기화하기
        /// </summary>
        public void RefreshShop()
        {
            //listShop = new List<Shop>();
            
            NetworkManager.session.ShopTemplate.ShopInfoReq(NetworkManager.session.HttpClient, RefreshAllShop);    
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
                    string str = $"ShopID:{arrayShopInfo[i].shopId}, r:{arrayShopInfo[i].resetRemainTime}";
                    foreach (var productInfo in arrayShopInfo[i].arrayProductInfo)
                    {
                        str += $"\nProductID:{productInfo.shopProductId}, BuyCount:{productInfo.buyCount}";
                    }
                    Debug.Log(str);
                }

                // int count = 0;
                // for (int i = 0; i < listShop.Count; i++)
                // {
                //     if (i + 1 == arrayShopInfo[count].shopId)
                //     {
                //         listShop[i].EnableContent();
                //         listShop[i].Initialize(arrayShopInfo[count]);
                //         count++;
                //     }
                //     else listShop[i].DisableContent();
                // }
                for (int i = 0; i < arrayShopInfo.Length; i++)
                {
                    var shop = listShop.Find(s => s.shopID == arrayShopInfo[i].shopId);
                    shop.EnableContent();
                    shop.Initialize(arrayShopInfo[i]);
                }
                
                if (isShowGoldShop) Invoke("ScrollToGoldShop", 0.1f);
                else if (isShowDiamondShop) Invoke("ScrollToDiamondShop", 0.1f);
                
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
        public bool ShowBuyResult(GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopProductInfo changeProductInfo, ItemBaseInfo payItemInfo, ItemBaseInfo[] arrayRewardItemInfo, QuestData[] arrayQuestData)
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
                    switch (payItemInfo.ItemId)
                    {
                        case 1:
                            type = ITEM_TYPE.GOLD;
                            UserInfoManager.Get().GetUserInfo().gold += payItemInfo.Value;
                            break;
                        case 2:
                            type = ITEM_TYPE.DIAMOND;
                            UserInfoManager.Get().GetUserInfo().diamond += payItemInfo.Value;
                            break;
                        case 11:
                            type = ITEM_TYPE.KEY;
                            UserInfoManager.Get().GetUserInfo().key += payItemInfo.Value;
                            break;
                        default:
                            type = ITEM_TYPE.NONE;
                            break;
                    }
                    UI_GetProduction.Get().RefreshProduct(type);
                }
            
                //구매한 상품에 대한 결과 값
                //arrayRewardItemInfo
                ItemBaseInfo[] arr = new ItemBaseInfo[arrayRewardItemInfo.Length];
                for (int i = 0; i < arrayRewardItemInfo.Length; i++)
                {
                    Debug.Log($"GET == ID:{arrayRewardItemInfo[i].ItemId}, Value:{arrayRewardItemInfo[i].Value}");
                    arr[i] = new ItemBaseInfo();
                    arr[i].ItemId = arrayRewardItemInfo[i].ItemId;
                    arr[i].Value = arrayRewardItemInfo[i].Value;
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
        /// <param name="arrayItemBaseInfo"></param>
        /// <returns></returns>
        public bool ShowPurchaseResult(GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopProductInfo changeProductInfo, ItemBaseInfo payItemInfo, ItemBaseInfo[] arrayRewardItemInfo, QuestData[] arrayQuestData)
        {
            return ShowBuyResult(errorCode, shopId, shopProductInfo, changeProductInfo, payItemInfo, arrayRewardItemInfo, arrayQuestData);
        }

        public void ShowGoldShop()
        {
            isShowGoldShop = true;
            UI_Main.Get().Click_MainButton(0);
        }

        private void ScrollToGoldShop()
        {
            //((RectTransform) transformShopParent).DOAnchorPosY(5800, 0.4f).SetDelay(0.1f);
            isShowGoldShop = false;
            
            RectTransform target = rts_TargetGoldshop;

            Vector2 point = (Vector2)rts_ScrollView.InverseTransformPoint(transformShopParent.position)
                            - (Vector2)rts_ScrollView.InverseTransformPoint(target.position)
                            - new Vector2(0f, target.sizeDelta.y / 2f);

            point.x = 0f;
            point.y = Mathf.Clamp(point.y, 0, ((RectTransform) transformShopParent).sizeDelta.y
                                              - (GetComponentInParent<CanvasScaler>().referenceResolution.y
                                                 - (Mathf.Abs(rts_ScrollView.offsetMax.y) + Mathf.Abs(rts_ScrollView.offsetMin.y))));
            ((RectTransform) transformShopParent).DOAnchorPosY(point.y, 0.4f);
        }

        public void ShowDiamondShop()
        {
            isShowDiamondShop = true;
            UI_Main.Get().Click_MainButton(0);
        }

        private void ScrollToDiamondShop()
        {
            //((RectTransform) transformShopParent).DOAnchorPosY(5100, 0.4f).SetDelay(0.1f);
            isShowDiamondShop = false;
            
            RectTransform target = rts_TargetDiashop;

            Vector2 point = (Vector2)rts_ScrollView.InverseTransformPoint(transformShopParent.position)
                            - (Vector2)rts_ScrollView.InverseTransformPoint(target.position)
                            - new Vector2(0f, target.sizeDelta.y / 2f);

            point.x = 0f;
            point.y = Mathf.Clamp(point.y, 0, ((RectTransform) transformShopParent).sizeDelta.y
                                              - (GetComponentInParent<CanvasScaler>().referenceResolution.y
                                                 - (Mathf.Abs(rts_ScrollView.offsetMax.y) + Mathf.Abs(rts_ScrollView.offsetMin.y))));
            ((RectTransform) transformShopParent).DOAnchorPosY(point.y, 0.4f);
            //rts_ScrollView.GetComponent<ScrollView>().scrollOffset
        }
    }
}
