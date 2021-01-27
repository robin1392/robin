using System;
using System.Collections;
using System.Collections.Generic;
using Percent.Platform.InAppPurchase;
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
        [SerializeField] private TextMeshProUGUI textPItemId;
        [SerializeField] private TextMeshProUGUI textPItemBuyCount;

        private Button buttonShopItem;

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
            buttonShopItem.onClick = new Button.ButtonClickedEvent();

            //shopProductInfo.shopProductId 값으로 테이블에 존재하는 상품 데이터 조회
            string productId = "mhl_package_crystal_150";
            
            textPItemId.text = shopProductInfo.shopProductId.ToString();
            textPItemBuyCount.text = shopProductInfo.buyCount.ToString();
            bool isBuyType3 = true;
            
            //아이템의 buyType에 따라서 구매 버튼 눌렀을때 다르게 처리하기
            if (isBuyType3)
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
