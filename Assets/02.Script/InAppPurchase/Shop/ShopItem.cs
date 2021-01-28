using System;
using System.Collections;
using System.Collections.Generic;
using Percent.Platform.InAppPurchase;
using RandomWarsResource.Data;
using Template.Shop.GameBaseShop.Common;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;


namespace Percent.Platform
{
    public class ShopItem : MonoBehaviour
    {
        //상품 정보
        [SerializeField] private Text textPItemId;
        [SerializeField] private Text textPItemBuyCount;

        private Button buttonShopItem;
        private ShopInfo info;
        private string productId;
        private string imageName;
        private BuyType buyType;

        private void Awake()
        {
            buttonShopItem = GetComponent<Button>();
        }

        public void EnableContent()
        {
            transform.localScale = Vector3.one;
        }

        public void DisableContent()
        {
            transform.localScale = Vector3.zero;
        }
        public virtual void UpdateContent(ShopInfo shopInfo,ShopProductInfo shopProductInfo)
        {
            info = shopInfo;

            switch (shopInfo.shopId)
            {
                case 1:     // 이벤트 상품
                {
                    TDataEventShopList data;
                    if (TableManager.Get().EventShopList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        imageName = data.shopImage;
#if UNITY_ANDROID
                        productId = data.googleProductId;
#elif UNITY_IOS
                        productID = data.appleProductId;
#endif
                    }
                }
                    break;
                case 2:     // 패키지 상품
                {
                    TDataPackageShopList data;
                    if (TableManager.Get().PackageShopList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        imageName = data.shopImage;
#if UNITY_ANDROID
                        productId = data.googleProductId;
#elif UNITY_IOS
                        productID = data.appleProductId;
#endif
                    }
                }
                    break;
                case 3:     // 일일 상품
                {
                    TDataOnedayShopList data;
                    if (TableManager.Get().OnedayShopList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        textPItemBuyCount.text = $"{data.buyType}:{data.buyPrice}";
                    }
                }
                    break;
                case 4:     // 박스
                {
                    TDataBoxShopList data;
                    if (TableManager.Get().BoxShopList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        textPItemBuyCount.text = $"{data.buyType}:{data.buyPrice}";
                    }
                }
                    break;
                case 5:     // 프리미엄
                {
                    TDataPremiumShopList data;
                    if (TableManager.Get().PremiumShopList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        imageName = data.shopImage;
#if UNITY_ANDROID
                        productId = data.googleProductId;
#elif UNITY_IOS
                        productID = data.appleProductId;
#endif
                    }
                }
                    break;
                case 6:     // 이모티콘
                {
                    TDataEmotionShopList data;
                    if (TableManager.Get().EmotionShopList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        textPItemBuyCount.text = $"{data.buyType}:{data.buyPrice}";
                    }
                }
                    break;
                case 7:     // 다이아
                {
                    TDataDiaShopList data;
                    if (TableManager.Get().DiaShopList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        imageName = data.goodsImage;
#if UNITY_ANDROID
                        productId = data.googleProductId;
#elif UNITY_IOS
                        productID = data.appleProductId;
#endif
                    }
                }
                    break;
                case 8:     // 골드
                {
                    TDataGoldShopList data;
                    if (TableManager.Get().GoldShopList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        imageName = data.goodsImage;
                        textPItemBuyCount.text = $"{data.buyType}:{data.buyPrice}";
                    }
                }
                    break;
            }
            
            buttonShopItem.onClick = new Button.ButtonClickedEvent();

            //shopProductInfo.shopProductId 값으로 테이블에 존재하는 상품 데이터 조회
            //string productId = "mhl_package_crystal_150";
            
            textPItemId.text = shopProductInfo.shopProductId.ToString();
            if (buyType == BuyType.cash)
                textPItemBuyCount.text = InAppManager.Instance.GetIDToProduct(productId)?.metadata.localizedPriceString;
            //else textPItemBuyCount.text = string.Empty;

            //아이템의 buyType에 따라서 구매 버튼 눌렀을때 다르게 처리하기
            if (buyType == BuyType.cash)
            {
                buttonShopItem.onClick.AddListener(() =>
                {
#if UNITY_EDITOR
                    //개발자 테스트용
                    NetworkManager.session.ShopTemplate.ShopPurchaseTestReq(NetworkManager.session.HttpClient, UserInfoManager.Get().GetUserInfo().userID, shopInfo.shopId, shopProductInfo.shopProductId, null, ShopManager.Instance.ShowPurchaseResult);
#else
                    //실제 결제
                    InAppManager.Instance.BuyProductID(productId, UserInfoManager.Get().GetUserInfo().userID, shopInfo.shopId, shopProductInfo.shopProductId, ShopManager.Instance.ShowPurchaseResult);
#endif
                });
            }
            else
            {
                buttonShopItem.onClick.AddListener(() =>
                {  
                    //인앱 재화로 상품 구매하는 경우
                    NetworkManager.session.ShopTemplate.ShopBuyReq(NetworkManager.session.HttpClient, UserInfoManager.Get().GetUserInfo().userID,
                        shopInfo.shopId, shopProductInfo.shopProductId, ShopManager.Instance.ShowBuyResult);
                });
            }
        }

        public virtual void Initialize()
        {
            //Instantiate해줄것, 기타 설정해주기
        }

        public void Buy()
        {
            //상품 재화 소모 연출
            Debug.Log("구매!");
            //InAppManager.Instance.BuyProductID();
        }

        public void OnPurchaseSuccess()
        {
            //상품 받아서 처리 연출
        }

        public void OnPurchaseFail()
        {
            
        }
    }
}
