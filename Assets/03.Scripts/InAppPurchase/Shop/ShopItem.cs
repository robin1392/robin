using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using Percent.Platform.InAppPurchase;
//using RandomWarsProtocol;
using Service.Core;
using RandomWarsResource.Data;
using Template.Shop.GameBaseShop.Common;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;


namespace Percent.Platform
{
    public class ShopItem : MonoBehaviour
    {
        public static Vector3 pos;
        
        //상품 정보
        [SerializeField] protected Text textPItemId;
        [SerializeField] protected Text textPItemBuyCount;
        [SerializeField] protected Image imageIcon;
        [SerializeField] protected Image imagePriceIcon;

        [Header("Tab")] public Text textBadge;
        public Text textValue;
        public Text textFirstBuy;

        protected Button buttonShopItem;
        protected ShopInfo shopInfo;
        protected string productId;
        protected string imageName;
        protected BuyType buyType;
        protected int shopId;
        public int shopProductId;

        private void Awake()
        {
            buttonShopItem = GetComponent<Button>();
        }

        public void EnableContent()
        {
            //transform.localScale = Vector3.one;
            gameObject.SetActive(true);
        }

        public void DisableContent()
        {
            //transform.localScale = Vector3.zero;
            gameObject.SetActive(false);
        }
        public virtual void UpdateContent(ShopInfo shopInfo,ShopProductInfo shopProductInfo)
        {
            this.shopInfo = shopInfo;
            int maxBuyCount = 0;
            int tapInfo = 0;
            
            textPItemId.text = shopProductInfo.shopProductId.ToString();
            textPItemId.text += $"  ({shopProductInfo.buyCount}/{maxBuyCount})";
            int getCount = 0;
            TDataShopProductList data = null;
            TDataItemList item = null;

            switch (shopInfo.shopId)
            {
                case 1:     // 이벤트 상품
                {
                    if (TableManager.Get().ShopProductList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        tapInfo = data.tapInfo;
                        imageName = data.shopImage;
                        maxBuyCount = data.buyLimitCnt;
#if UNITY_ANDROID
                        productId = data.googleProductId;
#elif UNITY_IOS
                        productId = data.appleProductId;
#endif
                    }
                }
                    break;
                case 2:     // 패키지 상품
                {
                    if (TableManager.Get().ShopProductList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        tapInfo = data.tapInfo;
                        imageName = data.shopImage;
                        maxBuyCount = data.buyLimitCnt;
#if UNITY_ANDROID
                        productId = data.googleProductId;
#elif UNITY_IOS
                        productId = data.appleProductId;
#endif
                    }
                }
                    break;
                case 3:     // 일일 상품
                {
                    if (TableManager.Get().ShopProductList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        tapInfo = data.tapInfo;
                        switch (buyType)
                        {
                            case BuyType.gold:
                                imagePriceIcon.sprite = FileHelper.GetIcon("icon_gold");
                                textPItemBuyCount.text = data.buyPrice.ToString();
                                break;
                            case BuyType.dia:
                                imagePriceIcon.sprite = FileHelper.GetIcon("icon_dia");
                                textPItemBuyCount.text = data.buyPrice.ToString();
                                break;
                            case BuyType.cash:
                                imagePriceIcon.enabled = false;
                                break;
                            case BuyType.free:
                                imagePriceIcon.enabled = false;
                                textPItemBuyCount.text = LocalizationManager.GetLangDesc("Shop_Buyfree");
                                var pos = textPItemBuyCount.rectTransform.anchoredPosition;
                                pos.x = 0;
                                textPItemBuyCount.rectTransform.anchoredPosition = pos;
                                break;
                            case BuyType.ad:
                                imagePriceIcon.enabled = false;
                                break;
                        }

                        textPItemId.text = $"x{data.itemValue01}";
                    }

                    if (TableManager.Get().ItemList.GetData(itm => itm.id == data.itemId01, out item))
                    {
                        imageIcon.sprite = FileHelper.GetIcon(item.itemIcon);
                    }
                    
                    buttonShopItem.interactable = shopProductInfo.buyCount == 0;
                }
                    break;
                case 4:     // 박스
                {
                    if (TableManager.Get().ShopProductList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        tapInfo = data.tapInfo;
                        switch (buyType)
                        {
                            case BuyType.gold:
                                imagePriceIcon.sprite = FileHelper.GetIcon("icon_gold");
                                textPItemBuyCount.text = data.buyPrice.ToString();
                                break;
                            case BuyType.dia:
                                imagePriceIcon.sprite = FileHelper.GetIcon("icon_dia");
                                textPItemBuyCount.text = data.buyPrice.ToString();
                                break;
                            case BuyType.cash:
                                imagePriceIcon.enabled = false;
                                textPItemBuyCount.text = LocalizationManager.GetLangDesc("Shop_Buyfree");
                                var pos = textPItemBuyCount.rectTransform.anchoredPosition;
                                pos.x = 0;
                                textPItemBuyCount.rectTransform.anchoredPosition = pos;
                                break;
                            case BuyType.free:
                                imagePriceIcon.enabled = false;
                                textPItemBuyCount.text = LocalizationManager.GetLangDesc("Shop_Buyfree");
                                pos = textPItemBuyCount.rectTransform.anchoredPosition;
                                pos.x = 0;
                                textPItemBuyCount.rectTransform.anchoredPosition = pos;
                                break;
                            case BuyType.ad:
                                imagePriceIcon.enabled = false;
                                break;
                        }

                        textPItemId.text = $"x{data.itemValue01}";

                        if (TableManager.Get().ItemList.GetData(itm => itm.id == data.itemId01, out item))
                        {
                            imageIcon.sprite = FileHelper.GetIcon(item.itemIcon);
                            imageIcon.SetNativeSize();
                        }
                        textPItemBuyCount.text = $"{data.buyPrice}";
                    }
                }
                    break;
                case 5:     // 프리미엄
                {
                    if (TableManager.Get().ShopProductList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        tapInfo = data.tapInfo;
                        imageName = data.shopImage;
#if UNITY_ANDROID
                        productId = data.googleProductId;
#elif UNITY_IOS
                        productId = data.appleProductId;
#endif
                    }
                }
                    break;
                case 6:     // 이모티콘
                {
                    if (TableManager.Get().ShopProductList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        tapInfo = data.tapInfo;
                        textPItemBuyCount.text = $"{data.buyType}:{data.buyPrice}";
                    }
                }
                    break;
                case 7:     // 다이아
                {
                    if (TableManager.Get().ShopProductList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        tapInfo = data.tapInfo;
                        imageName = data.shopImage;
#if UNITY_ANDROID
                        productId = data.googleProductId;
#elif UNITY_IOS
                        productId = data.appleProductId;
#endif
                        switch (buyType)
                        {
                            case BuyType.gold:
                                imagePriceIcon.sprite = FileHelper.GetIcon("icon_gold");
                                textPItemBuyCount.text = data.buyPrice.ToString();
                                break;
                            case BuyType.dia:
                                imagePriceIcon.sprite = FileHelper.GetIcon("icon_dia");
                                textPItemBuyCount.text = data.buyPrice.ToString();
                                break;
                            case BuyType.cash:
                                imagePriceIcon.enabled = false;
                                textPItemBuyCount.text = LocalizationManager.GetLangDesc("Shop_Buyfree");
                                var pos = textPItemBuyCount.rectTransform.anchoredPosition;
                                pos.x = 0;
                                textPItemBuyCount.rectTransform.anchoredPosition = pos;
                                break;
                            case BuyType.free:
                                imagePriceIcon.enabled = false;
                                textPItemBuyCount.text = LocalizationManager.GetLangDesc("Shop_Buyfree");
                                pos = textPItemBuyCount.rectTransform.anchoredPosition;
                                pos.x = 0;
                                textPItemBuyCount.rectTransform.anchoredPosition = pos;
                                break;
                            case BuyType.ad:
                                imagePriceIcon.enabled = false;
                                break;
                        }

                        textPItemId.text = $"x{data.itemValue01}";

                        if (TableManager.Get().ItemList.GetData(itm => itm.id == data.itemId01, out item))
                        {
                            imageIcon.sprite = FileHelper.GetIcon(data.shopImage);
                        }
                    }
                }
                    break;
                case 8:     // 골드
                {
                    if (TableManager.Get().ShopProductList.GetData(shopProductInfo.shopProductId, out data))
                    {
                        buyType = (BuyType)data.buyType;
                        tapInfo = data.tapInfo;
                        imageName = data.shopImage;
                        switch (buyType)
                        {
                            case BuyType.gold:
                                imagePriceIcon.sprite = FileHelper.GetIcon("icon_gold");
                                textPItemBuyCount.text = data.buyPrice.ToString();
                                break;
                            case BuyType.dia:
                                imagePriceIcon.sprite = FileHelper.GetIcon("icon_dia");
                                textPItemBuyCount.text = data.buyPrice.ToString();
                                break;
                            case BuyType.cash:
                                imagePriceIcon.enabled = false;
                                textPItemBuyCount.text = LocalizationManager.GetLangDesc("Shop_Buyfree");
                                var pos = textPItemBuyCount.rectTransform.anchoredPosition;
                                pos.x = 0;
                                textPItemBuyCount.rectTransform.anchoredPosition = pos;
                                break;
                            case BuyType.free:
                                imagePriceIcon.enabled = false;
                                textPItemBuyCount.text = LocalizationManager.GetLangDesc("Shop_Buyfree");
                                pos = textPItemBuyCount.rectTransform.anchoredPosition;
                                pos.x = 0;
                                textPItemBuyCount.rectTransform.anchoredPosition = pos;
                                break;
                            case BuyType.ad:
                                imagePriceIcon.enabled = false;
                                break;
                        }

                        textPItemId.text = $"x{data.itemValue01}";

                        if (TableManager.Get().ItemList.GetData(itm => itm.id == data.itemId01, out item))
                        {
                            imageIcon.sprite = FileHelper.GetIcon(data.shopImage);
                        }
                        textPItemBuyCount.text = $"{data.buyPrice}";
                    }
                }
                    break;
            }
            
            // Tap
            if (data != null)
            {
                switch ((ETapInfoKey)data.tapInfo)
                {
                    case ETapInfoKey.tap_hot:
                        textBadge.text = "HOT";
                        textBadge.transform.parent.gameObject.SetActive(true);
                        break;
                    case ETapInfoKey.tap_best:
                        textBadge.text = "BEST";
                        textBadge.transform.parent.gameObject.SetActive(true);
                        break;
                    case ETapInfoKey.tap_new:
                        textBadge.text = "NEW";
                        textBadge.transform.parent.gameObject.SetActive(true);
                        break;
                    case ETapInfoKey.tap_oneplusone:
                        textBadge.text = "1+1";
                        textBadge.transform.parent.gameObject.SetActive(true);
                        textFirstBuy.transform.parent.gameObject.SetActive(true);
                        break;
                    case ETapInfoKey.tap_value:
                        textValue.text = $"{data.tapValue}";
                        textValue.transform.parent.gameObject.SetActive(true);
                        break;
                }
            }

            buttonShopItem.onClick = new Button.ButtonClickedEvent();

            //shopProductInfo.shopProductId 값으로 테이블에 존재하는 상품 데이터 조회
            //string productId = "mhl_package_crystal_150";
            
            if (buyType == BuyType.cash)
                textPItemBuyCount.text = InAppManager.Instance.GetIDToProduct(productId)?.metadata.localizedPriceString;
            //else textPItemBuyCount.text = string.Empty;

            shopProductId = shopProductInfo.shopProductId;

            //아이템의 buyType에 따라서 구매 버튼 눌렀을때 다르게 처리하기
            buttonShopItem.onClick.AddListener(() =>
            {
                if (buyType == BuyType.dia || buyType == BuyType.gold)
                {
                    UI_Main.Get().shopBuyPopup.Initialize(imageIcon.sprite, textPItemId.text, buyType,
                        textPItemBuyCount.text, item, tapInfo, Buy);
                }
                else
                {
                    Buy();
                }
            });
            
            SetColor();
        }

        public bool ShowBuyResult(GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ItemBaseInfo payItemInfo, ItemBaseInfo[] arrayRewardItemInfo, QuestData[] arrayQuestData)
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
                    case 5:
                    case 6:
                        ShopManager.Get().RefreshShop();
                        break;
                    case 3:
                        buttonShopItem.interactable = false;
                        break;
                }
                SetColor();
            
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
                
                // 퀘스트 업데이트
                UI_Popup_Quest.QuestUpdate(arrayQuestData);
            
                return true;
            }
            else
            {
                Debug.Log($"에러 발생 : {errorCode}");
                UI_ErrorMessage.Get().ShowMessage($"Error : {errorCode}");
                return false;
            }
        }

        public virtual void Initialize(int productID)
        {
            //Instantiate해줄것, 기타 설정해주기
            shopProductId = productID;
        }

        public void Buy()
        {
            //상품 재화 소모 연출
            Debug.Log("구매!");
            
            if (buyType == BuyType.cash)
            {
                //buttonShopItem.onClick.AddListener(() =>
                //{
#if UNITY_EDITOR
                    //개발자 테스트용
                //NetworkManager.session.ShopTemplate.ShopPurchaseTestReq(NetworkManager.session.HttpClient, shopInfo.shopId, shopProductId, null, ShowBuyResult);
#else
                    //실제 결제
                InAppManager.Instance.BuyProductID(productId, UserInfoManager.Get().GetUserInfo().userID, shopInfo.shopId, shopProductId, ShowBuyResult);
#endif
                pos = transform.position;
                UI_Main.Get().obj_IndicatorPopup.SetActive(true);
                //});
            }
            else
            {
                //buttonShopItem.onClick.AddListener(() =>
                //{  
                    //인앱 재화로 상품 구매하는 경우
                NetworkManager.session.ShopTemplate.ShopBuyReq(NetworkManager.session.HttpClient,
                    shopInfo.shopId, shopProductId, ShowBuyResult);
                
                pos = transform.position;
                UI_Main.Get().obj_IndicatorPopup.SetActive(true);
                //});
            }
        }

        public void OnPurchaseSuccess()
        {
            //상품 받아서 처리 연출
        }

        public void OnPurchaseFail()
        {
            
        }

        protected void SetColor()
        {
            Color color = Color.white;
            float factor = buttonShopItem.interactable ? 1f : 0.5f;
            var texts = GetComponentsInChildren<Text>();
            var images = GetComponentsInChildren<Image>();

            foreach (var text in texts)
            {
                color.a = text.color.a;
                color.r *= factor;
                color.g *= factor;
                color.b *= factor;
                text.color = color;
            }

            foreach (var image in images)
            {
                color.a = image.color.a;
                color.r *= factor;
                color.g *= factor;
                color.b *= factor;
                image.color = color;
            }
        }
    }
}
