using System.Collections.Generic;
using System.Text;
using Service.Core;
using Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RandomWarsProtocol;

namespace Template.Shop.GameBaseShop.Common
{
    public enum EGameBaseShopProtocol
    {
        Begin = 40000,

        ShopInfoReq = 40001,
        ShopInfoAck,
        ShopInfoNotify,

        ShopBuyReq,
        ShopBuyAck,
        ShopBuyNotify,

        ShopPurchaseReq,
        ShopPurchaseAck,
        ShopPurchaseNotify,

        ShopPurchaseTestReq,
        ShopPurchaseTestAck,
        ShopPurchaseTestNotify,

        ShopResetReq,
        ShopResetAck,
        ShopResetNotify,

        ShopRefreshReq,
        ShopRefreshAck,
        ShopRefreshNotify,


        End,
    }


    public class GameBaseShopProtocol : MessageControllerBase
    {
        public GameBaseShopProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)EGameBaseShopProtocol.ShopInfoReq, ReceiveShopInfoReq},
                {(int)EGameBaseShopProtocol.ShopInfoAck, ReceiveShopInfoAck},

                {(int)EGameBaseShopProtocol.ShopBuyReq, ReceiveShopBuyReq},
                {(int)EGameBaseShopProtocol.ShopBuyAck, ReceiveShopBuyAck},

                {(int)EGameBaseShopProtocol. ShopPurchaseReq, ReceiveShopPurchaseReq},
                {(int)EGameBaseShopProtocol. ShopPurchaseAck, ReceiveShopPurchaseAck},

                {(int)EGameBaseShopProtocol. ShopPurchaseTestReq, ReceiveShopPurchaseTestReq},
                {(int)EGameBaseShopProtocol. ShopPurchaseTestAck, ReceiveShopPurchaseTestAck},

                {(int)EGameBaseShopProtocol.ShopResetReq, ReceiveShopResetReq},
                {(int)EGameBaseShopProtocol.ShopResetAck, ReceiveShopResetAck},

                {(int)EGameBaseShopProtocol.ShopRefreshReq, ReceiveShopRefreshReq},
                {(int)EGameBaseShopProtocol.ShopRefreshAck, ReceiveShopRefreshAck},
            };
        }


        #region ShopInfo ---------------------------------------------------------------------
        public bool ShopInfoReq(ISender sender, string playerGuid, ReceiveShopInfoAckDelegate callback)
        {
            ReceiveShopInfoAckHandler = callback;
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            return sender.SendHttpPost((int)EGameBaseShopProtocol.ShopInfoReq, "shopinfo", json.ToString());
        }

        public delegate (GameBaseShopErrorCode errorCode, ShopInfo[] arrayShopInfo) ReceiveShopInfoReqDelegate(string playerGuid);
        public ReceiveShopInfoReqDelegate ReceiveShopInfoReqHandler;
        public bool ReceiveShopInfoReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string playerGuid = (string)jObject["playerGuid"];
            var res = ReceiveShopInfoReqHandler(playerGuid);
            return ShopInfoAck(sender, res.errorCode, res.arrayShopInfo);
        }

        public bool ShopInfoAck(ISender sender, GameBaseShopErrorCode errorCode, ShopInfo[] arrayShopInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayShopInfo", JsonConvert.SerializeObject(arrayShopInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveShopInfoAckDelegate(GameBaseShopErrorCode errorCode, ShopInfo[] arrayShopInfo);
        public ReceiveShopInfoAckDelegate ReceiveShopInfoAckHandler;
        public bool ReceiveShopInfoAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseShopErrorCode errorCode = (GameBaseShopErrorCode)(int)jObject["errorCode"];
            ShopInfo[] arrayShopInfo = JsonConvert.DeserializeObject<ShopInfo[]>(jObject["arrayShopInfo"].ToString());
            return ReceiveShopInfoAckHandler(errorCode, arrayShopInfo);
        }
        #endregion


        #region ShopBuy ---------------------------------------------------------------------
        public bool ShopBuyReq(ISender sender, string playerGuid, int shopId, int shopProductId, ReceiveShopBuyAckDelegate callback)
        {
            ReceiveShopBuyAckHandler = callback;
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("shopId", shopId);
            json.Add("productId", shopProductId);
            return sender.SendHttpPost((int)EGameBaseShopProtocol.ShopBuyReq, "shopbuy", json.ToString());
        }

        public delegate (GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo, MsgQuestData[] arrayQuestData) ReceiveShopBuyReqDelegate(string playerGuid, int shopId, int productId);
        public ReceiveShopBuyReqDelegate ReceiveShopBuyReqHandler;
        public bool ReceiveShopBuyReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string playerGuid = (string)jObject["playerGuid"];
            int shopId = (int)jObject["shopId"];
            int productId = (int)jObject["productId"];
            var res = ReceiveShopBuyReqHandler(playerGuid, shopId, productId);
            return ShopBuyAck(sender, res.errorCode, res.shopId, res.shopProductInfo, res.payItemInfo, res.arrayRewardItemInfo, res.arrayQuestData);
        }

        public bool ShopBuyAck(ISender sender, GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo, MsgQuestData[] arrayQuestData)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("shopId", shopId);
            json.Add("shopProductInfo", JsonConvert.SerializeObject(shopProductInfo));
            json.Add("payItemInfo", JsonConvert.SerializeObject(payItemInfo));
            json.Add("arrayRewardItemInfo", JsonConvert.SerializeObject(arrayRewardItemInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveShopBuyAckDelegate(GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo, MsgQuestData[] arrayQuestData);
        public ReceiveShopBuyAckDelegate ReceiveShopBuyAckHandler;
        public bool ReceiveShopBuyAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseShopErrorCode errorCode = (GameBaseShopErrorCode)(int)jObject["errorCode"];
            int shopId = (int)jObject["shopId"];
            ShopProductInfo shopProductInfo = JsonConvert.DeserializeObject<ShopProductInfo>(jObject["shopProductInfo"].ToString());
            ShopItemInfo payItemInfo = JsonConvert.DeserializeObject<ShopItemInfo>(jObject["payItemInfo"].ToString());
            ShopItemInfo[] arrayRewardItemInfo = JsonConvert.DeserializeObject<ShopItemInfo[]>(jObject["arrayRewardItemInfo"].ToString());
            MsgQuestData[] arrayQuestData = JsonConvert.DeserializeObject<MsgQuestData[]>(jObject["arrayQuestData"].ToString());
            return ReceiveShopBuyAckHandler(errorCode, shopId, shopProductInfo, payItemInfo, arrayRewardItemInfo, arrayQuestData);
        }
        #endregion        


        #region ShopPurchase ---------------------------------------------------------------------
        public bool ShopPurchaseReq(ISender sender, string playerGuid, int shopId, int productId, string receiptInfo, ReceiveShopPurchaseAckDelegate callback)
        {
            ReceiveShopPurchaseAckHandler = callback;
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("shopId", shopId);
            json.Add("productId", productId);
            json.Add("receiptInfo", receiptInfo);
            return sender.SendHttpPost((int)EGameBaseShopProtocol.ShopPurchaseReq, "shoppurchase", json.ToString());
        }

        public delegate (GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo, MsgQuestData[] arrayQuestData) ReceiveShopPurchaseReqDelegate(string playerGuid, int shopId, int productId, string purchaseInfo);
        public ReceiveShopPurchaseReqDelegate ReceiveShopPurchaseReqHandler;
        public bool ReceiveShopPurchaseReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string playerGuid = (string)jObject["playerGuid"];
            int shopId = (int)jObject["shopId"];
            int productId = (int)jObject["productId"];
            string receiptInfo = (string)jObject["receiptInfo"];
            var res = ReceiveShopPurchaseReqHandler(playerGuid, shopId, productId, receiptInfo);
            return ShopPurchaseAck(sender, res.errorCode, res.shopId, res.shopProductInfo, res.payItemInfo, res.arrayRewardItemInfo, res.arrayQuestData);
        }

        public bool ShopPurchaseAck(ISender sender, GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo, MsgQuestData[] arrayQuestData)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("shopId", shopId);
            json.Add("shopProductInfo", JsonConvert.SerializeObject(shopProductInfo));
            json.Add("payItemInfo", JsonConvert.SerializeObject(payItemInfo));
            json.Add("arrayRewardItemInfo", JsonConvert.SerializeObject(arrayRewardItemInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveShopPurchaseAckDelegate(GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo, MsgQuestData[] arrayQuestData);
        public ReceiveShopPurchaseAckDelegate ReceiveShopPurchaseAckHandler;
        public bool ReceiveShopPurchaseAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseShopErrorCode errorCode = (GameBaseShopErrorCode)(int)jObject["errorCode"];
            int shopId = (int)jObject["shopId"];
            ShopProductInfo shopProductInfo = JsonConvert.DeserializeObject<ShopProductInfo>(jObject["shopProductInfo"].ToString());
            ShopItemInfo payItemInfo = JsonConvert.DeserializeObject<ShopItemInfo>(jObject["payItemInfo"].ToString());
            ShopItemInfo[] arrayRewardItemInfo = JsonConvert.DeserializeObject<ShopItemInfo[]>(jObject["arrayRewardItemInfo"].ToString());
            MsgQuestData[] arrayQuestData = JsonConvert.DeserializeObject<MsgQuestData[]>(jObject["arrayQuestData"].ToString());
            return ReceiveShopPurchaseAckHandler(errorCode, shopId, shopProductInfo, payItemInfo, arrayRewardItemInfo, arrayQuestData);
        }
        #endregion        


        #region ShopPurchaseTest ---------------------------------------------------------------------
        public bool ShopPurchaseTestReq(ISender sender, string playerGuid, int shopId, int shopProductId, string purchaseInfo, ReceiveShopPurchaseTestAckDelegate callback)
        {
            ReceiveShopPurchaseTestAckHandler = callback;
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("shopId", shopId);
            json.Add("productId", shopProductId);
            json.Add("purchaseInfo", purchaseInfo);
            return sender.SendHttpPost((int)EGameBaseShopProtocol.ShopPurchaseTestReq, "shoppurchasetest", json.ToString());
        }

        public delegate (GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo) ReceiveShopPurchaseTestReqDelegate(string playerGuid, int shopId, int shopProductId);
        public ReceiveShopPurchaseTestReqDelegate ReceiveShopPurchaseTestReqHandler;
        public bool ReceiveShopPurchaseTestReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string playerGuid = (string)jObject["playerGuid"];
            int shopId = (int)jObject["shopId"];
            int shopProductId = (int)jObject["productId"];
            var res = ReceiveShopPurchaseTestReqHandler(playerGuid, shopId, shopProductId);
            return ShopPurchaseTestAck(sender, res.errorCode, res.shopId, res.shopProductInfo, res.payItemInfo, res.arrayRewardItemInfo);
        }

        public bool ShopPurchaseTestAck(ISender sender, GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("shopId", shopId);
            json.Add("shopProductInfo", JsonConvert.SerializeObject(shopProductInfo));
            json.Add("payItemInfo", JsonConvert.SerializeObject(payItemInfo));
            json.Add("arrayRewardItemInfo", JsonConvert.SerializeObject(arrayRewardItemInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveShopPurchaseTestAckDelegate(GameBaseShopErrorCode errorCode, int shopId, ShopProductInfo shopProductInfo, ShopItemInfo payItemInfo, ShopItemInfo[] arrayRewardItemInfo, MsgQuestData[] arrayQuestData);
        public ReceiveShopPurchaseTestAckDelegate ReceiveShopPurchaseTestAckHandler;
        public bool ReceiveShopPurchaseTestAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseShopErrorCode errorCode = (GameBaseShopErrorCode)(int)jObject["errorCode"];
            int shopId = (int)jObject["shopId"];
            ShopProductInfo shopProductInfo = JsonConvert.DeserializeObject<ShopProductInfo>(jObject["shopProductInfo"].ToString());
            ShopItemInfo payItemInfo = JsonConvert.DeserializeObject<ShopItemInfo>(jObject["payItemInfo"].ToString());
            ShopItemInfo[] arrayRewardItemInfo = JsonConvert.DeserializeObject<ShopItemInfo[]>(jObject["arrayRewardItemInfo"].ToString());
            MsgQuestData[] arrayQuestData = JsonConvert.DeserializeObject<MsgQuestData[]>(jObject["arrayQuestData"].ToString());
            return ReceiveShopPurchaseTestAckHandler(errorCode, shopId, shopProductInfo, payItemInfo, arrayRewardItemInfo, arrayQuestData);
        }
        #endregion    


        #region ShopReset ---------------------------------------------------------------------
        public bool ShopResetReq(ISender sender, string playerGuid, int shopId, int resetType, ReceiveShopResetAckDelegate callback)
        {
            ReceiveShopResetAckHandler = callback;
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("shopId", shopId);
            json.Add("resetType", resetType);
            return sender.SendHttpPost((int)EGameBaseShopProtocol.ShopResetReq, "shopreset", json.ToString());
        }

        public delegate (GameBaseShopErrorCode errorCode, ShopInfo shopInfo, ShopItemInfo payItemInfo) ReceiveShopResetReqDelegate(string playerGuid, int shopId, int resetType);
        public ReceiveShopResetReqDelegate ReceiveShopResetReqHandler;
        public bool ReceiveShopResetReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string playerGuid = (string)jObject["playerGuid"];
            int shopId = (int)jObject["shopId"];
            int resetType = (int)jObject["resetType"];
            var res = ReceiveShopResetReqHandler(playerGuid, shopId, resetType);
            return ShopResetAck(sender, res.errorCode, res.shopInfo, res.payItemInfo);
        }

        public bool ShopResetAck(ISender sender, GameBaseShopErrorCode errorCode, ShopInfo shopInfo, ShopItemInfo payItemInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("shopInfo", JsonConvert.SerializeObject(shopInfo));
            json.Add("payItemInfo", JsonConvert.SerializeObject(payItemInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveShopResetAckDelegate(GameBaseShopErrorCode errorCode, ShopInfo shopInfo, ShopItemInfo payItemInfo);
        public ReceiveShopResetAckDelegate ReceiveShopResetAckHandler;
        public bool ReceiveShopResetAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseShopErrorCode errorCode = (GameBaseShopErrorCode)(int)jObject["errorCode"];
            ShopInfo shopInfo = JsonConvert.DeserializeObject<ShopInfo>(jObject["shopInfo"].ToString());
            ShopItemInfo payItemInfo = JsonConvert.DeserializeObject<ShopItemInfo>(jObject["payItemInfo"].ToString());
            return ReceiveShopResetAckHandler(errorCode, shopInfo, payItemInfo);
        }
        #endregion



        #region ShopRefresh ---------------------------------------------------------------------
        public bool ShopRefreshReq(ISender sender, string playerGuid, int shopId, int resetType, ReceiveShopRefreshAckDelegate callback)
        {
            ReceiveShopRefreshAckHandler = callback;
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            return sender.SendHttpPost((int)EGameBaseShopProtocol.ShopRefreshReq, "shoprefresh", json.ToString());
        }

        public delegate (GameBaseShopErrorCode errorCode, ShopInfo[] arrayShopInfo) ReceiveShopRefreshReqDelegate(string playerGuid);
        public ReceiveShopRefreshReqDelegate ReceiveShopRefreshReqHandler;
        public bool ReceiveShopRefreshReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string playerGuid = (string)jObject["playerGuid"];
            var res = ReceiveShopRefreshReqHandler(playerGuid);
            return ShopRefreshAck(sender, res.errorCode, res.arrayShopInfo);
        }

        public bool ShopRefreshAck(ISender sender, GameBaseShopErrorCode errorCode, ShopInfo[] arrayShopInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayShopInfo", JsonConvert.SerializeObject(arrayShopInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveShopRefreshAckDelegate(GameBaseShopErrorCode errorCode, ShopInfo[] arrayShopInfo);
        public ReceiveShopRefreshAckDelegate ReceiveShopRefreshAckHandler;
        public bool ReceiveShopRefreshAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            GameBaseShopErrorCode errorCode = (GameBaseShopErrorCode)(int)jObject["errorCode"];
            ShopInfo[] arrayShopInfo = JsonConvert.DeserializeObject<ShopInfo[]>(jObject["arrayShopInfo"].ToString());
            return ReceiveShopRefreshAckHandler(errorCode, arrayShopInfo);
        }
        #endregion


    }
}