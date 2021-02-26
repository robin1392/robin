using System.Collections.Generic;
using System.Text;
using Service.Core;
using Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Character.RandomwarsDice.Common
{
    public enum ERandomwarsDiceProtocol
    {
        Begin = 30000,

        DiceInfoReq,
        DiceInfoAck,

        DiceUpgradeReq,
        DiceUpgradeAck,

        DiceChangeDeckReq,
        DiceChangeDeckAck,


        End,
    }


    public class RandomwarsDiceProtocol : MessageControllerBase
    {
        public RandomwarsDiceProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomwarsDiceProtocol.DiceInfoReq, ReceiveDiceInfoReq},
                {(int)ERandomwarsDiceProtocol.DiceInfoAck, ReceiveDiceInfoAck},
                {(int)ERandomwarsDiceProtocol.DiceUpgradeReq, ReceiveDiceUpgradeReq},
                {(int)ERandomwarsDiceProtocol.DiceUpgradeAck, ReceiveDiceUpgradeAck},
                {(int)ERandomwarsDiceProtocol.DiceChangeDeckReq, ReceiveDiceChangeDeckReq},
                {(int)ERandomwarsDiceProtocol.DiceChangeDeckAck, ReceiveDiceChangeDeckAck},
            };         
        }


        #region DiceInfo ---------------------------------------------------------------------
        public bool DiceInfoReq(ISender sender, ReceiveDiceInfoAckDelegate callback)
        {
            ReceiveDiceInfoAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)ERandomwarsDiceProtocol.DiceInfoReq, "diceinfo", json.ToString());
        }

        public delegate (ERandomwarsDiceErrorCode errorCode, MsgDiceInfo[] arrayDiceInfo) ReceiveDiceInfoReqDelegate(string accessToken);
        public ReceiveDiceInfoReqDelegate ReceiveDiceInfoReqHandler;
        public bool ReceiveDiceInfoReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveDiceInfoReqHandler(accessToken);
            return DiceInfoAck(sender, res.errorCode, res.arrayDiceInfo);     
        }        

        public bool DiceInfoAck(ISender sender, ERandomwarsDiceErrorCode errorCode, MsgDiceInfo[] arrayDiceInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayDiceInfo", JsonConvert.SerializeObject(arrayDiceInfo));
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveDiceInfoAckDelegate(ERandomwarsDiceErrorCode errorCode, MsgDiceInfo[] arrayDiceInfo);
        public ReceiveDiceInfoAckDelegate ReceiveDiceInfoAckHandler;
        public bool ReceiveDiceInfoAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsDiceErrorCode errorCode = (ERandomwarsDiceErrorCode)(int)jObject["errorCode"];
            MsgDiceInfo[] arrayDiceInfo = JsonConvert.DeserializeObject<MsgDiceInfo[]>(jObject["arrayDiceInfo"].ToString());
            return ReceiveDiceInfoAckHandler(errorCode, arrayDiceInfo);
        }
        #endregion    

        #region DiceUpgrade ---------------------------------------------------------------------
        public bool DiceUpgradeReq(ISender sender, int diceId, ReceiveDiceUpgradeAckDelegate callback)
        {
            ReceiveDiceUpgradeAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("diceId", diceId);
            return sender.SendHttpPost((int)ERandomwarsDiceProtocol.DiceUpgradeReq, "diceupgrade", json.ToString());
        }

        public delegate (ERandomwarsDiceErrorCode errorCode, MsgDiceInfo diceInfo, QuestData[] arrayQuestData, int updateGold) ReceiveDiceUpgradeReqDelegate(string accessToken, int diceId);
        public ReceiveDiceUpgradeReqDelegate ReceiveDiceUpgradeReqHandler;
        public bool ReceiveDiceUpgradeReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int diceId = (int)jObject["diceId"];
            var res = ReceiveDiceUpgradeReqHandler(accessToken, diceId);
            return DiceUpgradeAck(sender, res.errorCode, res.diceInfo, res.arrayQuestData, res.updateGold);     
        }        

        public bool DiceUpgradeAck(ISender sender, ERandomwarsDiceErrorCode errorCode, MsgDiceInfo diceInfo, QuestData[] arrayQuestData, int updateGold)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("diceInfo", JsonConvert.SerializeObject(diceInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            json.Add("updateGold", updateGold);
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveDiceUpgradeAckDelegate(ERandomwarsDiceErrorCode errorCode, MsgDiceInfo diceInfo, QuestData[] arrayQuestData, int updateGold);
        public ReceiveDiceUpgradeAckDelegate ReceiveDiceUpgradeAckHandler;
        public bool ReceiveDiceUpgradeAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsDiceErrorCode errorCode = (ERandomwarsDiceErrorCode)(int)jObject["errorCode"];
            MsgDiceInfo diceInfo = JsonConvert.DeserializeObject<MsgDiceInfo>(jObject["diceInfo"].ToString());
            QuestData[] arrayQuestData = JsonConvert.DeserializeObject<QuestData[]>(jObject["arrayQuestData"].ToString());
            int updateGold = (int)jObject["updateGold"];
            return ReceiveDiceUpgradeAckHandler(errorCode, diceInfo, arrayQuestData, updateGold);
        }
        #endregion        

        #region DiceChangeDeck ---------------------------------------------------------------------
        public bool DiceChangeDeckReq(ISender sender, int index, int[] arrayDiceId, ReceiveDiceChangeDeckAckDelegate callback)
        {
            ReceiveDiceChangeDeckAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("index", index);
            json.Add("arrayDiceId", JsonConvert.SerializeObject(arrayDiceId));
            return sender.SendHttpPost((int)ERandomwarsDiceProtocol.DiceChangeDeckReq, "dicechangedeck", json.ToString());
        }

        public delegate (ERandomwarsDiceErrorCode errorCode, int index, int[] arrayDiceId) ReceiveDiceChangeDeckReqDelegate(string accessToken, int index, int[] arrayDiceId);
        public ReceiveDiceChangeDeckReqDelegate ReceiveDiceChangeDeckReqHandler;
        public bool ReceiveDiceChangeDeckReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int index = (int)jObject["index"];
            int[] arrayDiceId = JsonConvert.DeserializeObject<int[]>(jObject["arrayDiceId"].ToString());
            var res = ReceiveDiceChangeDeckReqHandler(accessToken, index, arrayDiceId);
            return DiceChangeDeckAck(sender, res.errorCode, res.index, res.arrayDiceId);     
        }        

        public bool DiceChangeDeckAck(ISender sender, ERandomwarsDiceErrorCode errorCode, int index, int[] arrayDiceId)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("index", index);
            json.Add("arrayDiceId", JsonConvert.SerializeObject(arrayDiceId));
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveDiceChangeDeckAckDelegate(ERandomwarsDiceErrorCode errorCode, int index, int[] arrayDiceId);
        public ReceiveDiceChangeDeckAckDelegate ReceiveDiceChangeDeckAckHandler;
        public bool ReceiveDiceChangeDeckAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsDiceErrorCode errorCode = (ERandomwarsDiceErrorCode)(int)jObject["errorCode"];
            int index = (int)jObject["index"];
            int[] arrayDiceId = JsonConvert.DeserializeObject<int[]>(jObject["arrayDiceId"].ToString());
            return ReceiveDiceChangeDeckAckHandler(errorCode, index, arrayDiceId);
        }
        #endregion                  
    }
}