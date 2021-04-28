using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.Purchasing;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine.Networking;
using Percent.Boomlagoon.JSON;
using Percent.Platform.InAppPurchase;
using RandomWarsResource.Data;
using Template.Shop.GameBaseShop.Common;

// ReSharper disable All

namespace Percent.Platform
{

    [System.Serializable]
    public struct ProductData
    {
        public string IosStrID;
        public string AndroidStrID;
        public ProductType type;

        public ProductData(string and, string io, ProductType t)// : this()
        {
            AndroidStrID = and;
            IosStrID = io;
            type = t;
        }

        public ProductData(string all, ProductType t)// : this()
        {
            AndroidStrID = all;
            IosStrID = all;
            type = t;
        }
    }

    public class InAppManager : ManagerSingleton<InAppManager>, IStoreListener
    {
        [SerializeField] bool isLog = false;
        
        private bool isPurchaseInProgress = false;

        private IStoreController storeController = null;
        private IExtensionProvider extensionProvider = null;

        System.Action inappViewLockCallback = null;
        System.Action inappViewUnLockCallback = null;

        GameBaseShopProtocol.ReceiveShopPurchaseAckDelegate buyCallback = null;
        GameBaseShopProtocol.ReceiveShopPurchaseAckDelegate restoreCallback = null;

        private Dictionary<string, PurchaseEventArgs> dictPendingProducts = new Dictionary<string, PurchaseEventArgs>();
        
        [SerializeField] private List<ProductData> listProductDatas = new List<ProductData>();
        
        [SerializeField] private string consumableParsing;
        [SerializeField] private string nonConsumalbleParsing;

        public bool GetPurchaseProgressState()
        {
            return isPurchaseInProgress;
        }

        public void SetViewLockEvents(System.Action lockEvent, System.Action unlockEvent)
        {
            inappViewLockCallback = lockEvent;
            inappViewUnLockCallback = unlockEvent;
        }

        public string GetPriceText(string productID)
        {
            if (GetInitialized())
            {
                var product = storeController.products.WithID(productID);
                if (product != null)
                    return storeController.products.WithID(productID).metadata.localizedPriceString;
            }

            return "None";
        }

        public Product GetIDToProduct(string productId)
        {
            if (GetInitialized())
            {
                Product p = storeController.products.WithID(productId);

                if (!p.availableToPurchase) return null;
                return p;
            }

            return null;
        }

        public void LongStringParsing()
        {
            listProductDatas.Clear();
            splitLongString(consumableParsing, ProductType.Consumable);
            splitLongString(nonConsumalbleParsing, ProductType.NonConsumable);

            consumableParsing = "";
            nonConsumalbleParsing = "";
        }

        private void splitLongString(string str, ProductType type)
        {
            string[] split = str.Split(',', '\n', ';', ' ');

            int count = split.Length;
            for (int i = 0; i < count; i++)
            {
                if( !string.IsNullOrEmpty(split[i]) )
                    listProductDatas.Add( new ProductData(split[i], type) );
            }
        }
        
        private void Start()
        {
            InitializePurchasing();
        }
        
        public bool GetInitialized()
        {
            return (storeController != null && extensionProvider != null);
        }

        public void InitializePurchasing()
        {
            if (GetInitialized())
                return;
            
            if(isLog)
                Debug.Log("Percent [IAP] InitializePurchasing!");

            var module = StandardPurchasingModule.Instance();

            ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
            
            {
                ProductType type = ProductType.Consumable;
                // Event
                var keys = TableManager.Get().ShopProductList.Keys;
                TDataShopProductList dataEvent;
                foreach (var key in keys)
                {
                    if (TableManager.Get().ShopProductList.GetData(key, out dataEvent))
                    {
#if UNITY_IOS
                        builder.AddProduct(dataEvent.appleProductId, type);
#elif UNITY_ANDROID
                        builder.AddProduct(dataEvent.googleProductId, type);
#endif
                    }
                }
            }
            UnityPurchasing.Initialize(this, builder);
        }

        private string playerGuid;
        private int shopId, shopProductId;
        //인앱구매 시작 
        public void BuyProductID(string productId, string playerGuid, int shopId, int shopProductId, GameBaseShopProtocol.ReceiveShopPurchaseAckDelegate callback)
        {
            this.playerGuid = playerGuid;
            this.shopId = shopId;
            this.shopProductId = shopProductId;
            
            if (buyCallback != null || isPurchaseInProgress == true)
            {
                // 이미 구매가 진행중
                if(isLog)
                    Debug.Log("Percent [IAP] Please wait, purchase in progress");
                if (callback != null) 
                    callback(GameBaseShopErrorCode.PurchaseAlreadyBuyProcess, this.shopId, null, null, null, null, null);
                return;
            }
            
            if (inappViewLockCallback != null) inappViewLockCallback();

            isPurchaseInProgress = true;

            buyCallback = null;
            restoreCallback = null;
            
            buyCallback = callback;
            
            try
            {
                if (GetInitialized())
                {
                    Product p = storeController.products.WithID(productId);

                    if (p != null && p.availableToPurchase)
                    {
                        // 구매 진행
                        if(isLog)
                            Debug.Log(string.Format("Percent [IAP] Purchasing product asychronously: '{0}'", p.definition.id));
                        
                        storeController.InitiatePurchase(p);
                    }
                    else
                    {
                        // 식별 불가 상품
                        if(isLog)
                            Debug.Log("Percent [IAP] BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                        
                        if (buyCallback != null) buyCallback(GameBaseShopErrorCode.PurchaseAlreadyBuyProcess, this.shopId, null, null, null, null, null);
                        buyCallback = null;
                        ResultPurchaseFailed();
                        if (inappViewUnLockCallback != null) inappViewUnLockCallback();
                    }
                }
                else
                {
                    // 초기화 불가능, 스토어 로그인을 체크하거나 인터넷 체크
                    if(isLog)
                        Debug.Log("Percent [IAP] BuyProductID FAIL. Not initialized.");
                    
                    if (buyCallback != null) buyCallback(GameBaseShopErrorCode.PurchaseAlreadyBuyProcess, this.shopId, null, null, null, null, null);
                    buyCallback = null;
                    ResultPurchaseFailed();
                    if (inappViewUnLockCallback != null) inappViewUnLockCallback();
                }
            }
            catch (Exception e)
            {
                //코드로직 충돌
                if(isLog)
                    Debug.Log("Percent [IAP] BuyProductID: FAIL. Exception during purchase. " + e);
                
                if (buyCallback != null) buyCallback(GameBaseShopErrorCode.PurchaseAlreadyBuyProcess, this.shopId, null, null, null, null, null);
                buyCallback = null;
                ResultPurchaseFailed();
                if (inappViewUnLockCallback != null) inappViewUnLockCallback();
            }
        }
        
        // 리스토어 (구매 상품 재구매)
        public void RestorePurchase(GameBaseShopProtocol.ReceiveShopPurchaseAckDelegate callback)
        {
            if (inappViewLockCallback != null) inappViewLockCallback();

            restoreCallback = callback;

            if (!GetInitialized())
            {
                if(isLog)
                    Debug.Log("Percent [IAP] RestorePurchases FAIL. Not initialized.");
                
                ResultPurchaseFailed();
                if (restoreCallback != null) restoreCallback(GameBaseShopErrorCode.PurchaseInitError, shopId, null, null, null, null, null);
                if (inappViewUnLockCallback != null) inappViewUnLockCallback();
                return;
            }

            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                if(isLog)
                    Debug.Log("Percent [IAP] RestorePurchases started ...");

                var apple = extensionProvider.GetExtension<IAppleExtensions>();

                apple.RestoreTransactions
                    (
                        (result) => { if(isLog) Debug.Log("Percent [IAP] RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore."); }
                    );
            }
            else
            {
                if(isLog)
                    Debug.Log("Percent [IAP] RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
                
                ResultPurchaseFailed();
                if (restoreCallback != null) restoreCallback(GameBaseShopErrorCode.PurchaseNotSupportOS, shopId, null, null, null, null, null);
                if (inappViewUnLockCallback != null) inappViewUnLockCallback();
            }
        }

        public void OnInitialized(IStoreController sc, IExtensionProvider ep)
        {
            if(isLog)
                Debug.Log("Percent [IAP] OnInitialized : PASS");

            storeController = sc;
            extensionProvider = ep;
        }

        public void OnInitializeFailed(InitializationFailureReason reason)
        {
            if(isLog)
                Debug.Log("Percent [IAP] OnInitializeFailed InitializationFailureReason:" + reason);
        }

        // 결제 시 호출
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            if(isLog)
                Debug.Log("Percent [IAP] ProcessPurchase: " + args.purchasedProduct.definition.id);

            if (isPurchaseInProgress)
            {
                isPurchaseInProgress = true;
                dictPendingProducts.Add(args.purchasedProduct.transactionID, args);
                PurchaseValidation(args);    
            }
            
            
            return PurchaseProcessingResult.Pending;
        }

        
        // pending product 에 등록 된 결제 내역이 있는지 체크 
        public void ProcessPendingPurchase(System.Action<PurchaseEventArgs> actionPending)
        {
            if (dictPendingProducts.Count == 0) return;
            PurchaseEventArgs args = dictPendingProducts.First().Value;

            if (args == null) 
                return;
            
            if(isLog)
                Debug.Log("Percent [IAP] ProcessPendingPurchase: " + args.purchasedProduct.definition.id);
            
            PurchaseValidation(args, actionPending);
        }

        public void PlayStorePointPayment(PurchaseEventArgs args, GameBaseShopProtocol.ReceiveShopPurchaseAckDelegate callback)
        {
            if (inappViewLockCallback != null) inappViewLockCallback();
            this.buyCallback = callback;
            SendPostReceipt(args);
        }

        private void PurchaseValidation(PurchaseEventArgs args, System.Action<PurchaseEventArgs> actionPending = null)
        {
            isPurchaseInProgress = true;

            if(isLog)
                Debug.Log("Percent [IAP] ProcessValidation: " + args.purchasedProduct.definition.id);

            if(actionPending != null) 
                actionPending(args);
            
            if (!args.purchasedProduct.definition.id.Contains(".rew"))
            {
                // 영수증 검증 실행
                SendPostReceipt(args);
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            if(isLog)
                Debug.Log(string.Format("Percent [IAP] OnPurchaseFailed: FAIL. Product: {0}, PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));

            isPurchaseInProgress = false;

            if (buyCallback != null) buyCallback(GameBaseShopErrorCode.PurchaseFailed, shopId, null, null, null, null, null);
            buyCallback = null;
            if (inappViewUnLockCallback != null) inappViewUnLockCallback();
        }

        private void SendPostReceipt(PurchaseEventArgs args)
        {
            if(isLog) 
                Debug.Log("Percent [IAP] Start receipt validation: " + args.purchasedProduct.definition.id);
            
            dictPendingProducts.Remove(args.purchasedProduct.transactionID);
            storeController.ConfirmPendingPurchase(args.purchasedProduct);

            ResultPurchaseSuccessed(args);
        }

        private void ResultPurchaseSuccessed(PurchaseEventArgs args)
        {
            
            string payload = null;
#if UNITY_ANDROID
            payload = GetPayload(args).ToString();
#elif UNITY_IOS
            payload = args.purchasedProduct.receipt.ToString();
#endif
            
            
            if(isLog)
                Debug.Log(string.Format("Percent [IAP] Purchase Successed: Product: {0}", args.purchasedProduct.definition.id));

            isPurchaseInProgress = false;

            if (buyCallback == null && restoreCallback == null)
            {
                // start auto restore
            }

            if (buyCallback != null) NetworkManager.session.ShopTemplate.ShopPurchaseReq(NetworkManager.session.HttpClient, shopId,shopProductId, payload, buyCallback);
            buyCallback = null;

            if (restoreCallback != null) NetworkManager.session.ShopTemplate.ShopPurchaseReq(NetworkManager.session.HttpClient, shopId,shopProductId, payload, restoreCallback);

            if (inappViewUnLockCallback != null) inappViewUnLockCallback();

            if (args != null) storeController.ConfirmPendingPurchase(args.purchasedProduct);
        }
        
        private JObject GetPayload(PurchaseEventArgs args)
        {
            string token = "";
            try
            {
                JObject purchaseResult = JObject.Parse(args.purchasedProduct.receipt);
                JObject purchaseResult2 = JObject.Parse(purchaseResult["Payload"].ToString());
                JObject purchaseResult3 = JObject.Parse(purchaseResult2["json"].ToString());
                token = (string)purchaseResult3["purchaseToken"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            JObject payload = new JObject();
            payload.Add("token",token);
            payload.Add("packageName", Application.identifier);
            payload.Add("productId",args.purchasedProduct.definition.id);
            Debug.Log(payload.ToString());
            return payload;
        }
        
        private string JsEval(string json)
        {
            for(int i = 0; i < json.Length; i++)
                if(string.Equals("\\", json[i].ToString()))
                    json = json.Remove(i, 1);
            return json;
        }

        private void ResultPurchaseFailed()
        {
            isPurchaseInProgress = false;

            if (buyCallback != null) buyCallback(GameBaseShopErrorCode.PurchaseFailed, shopId, null, null, null, null, null);
            buyCallback = null;

            if (restoreCallback != null) restoreCallback(GameBaseShopErrorCode.PurchaseFailed, shopId, null, null, null, null, null);
            restoreCallback = null;

            if (inappViewUnLockCallback != null) inappViewUnLockCallback();
        }
    }
}

