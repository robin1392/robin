using System.Collections.Generic;
using System.Text;
using Service.Core;
using Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Item.RandomwarsItem.Common
{
    public enum ERandomwarsItemProtocol
    {
        Begin = 40000,

        BoxOpenReq,
        BoxOpenAck,

        EmotionEquipReq,
        EmotionEquipAck,

        End,
    }


    public class RandomwarsItemProtocol : MessageControllerBase
    {
        public RandomwarsItemProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomwarsItemProtocol.BoxOpenReq, ReceiveBoxOpenReq},
                {(int)ERandomwarsItemProtocol.BoxOpenAck, ReceiveBoxOpenAck},
                {(int)ERandomwarsItemProtocol.EmotionEquipReq, ReceiveEmotionEquipReq},
                {(int)ERandomwarsItemProtocol.EmotionEquipAck, ReceiveEmotionEquipAck},
            };         
        }


        #region BoxOpen ---------------------------------------------------------------------
        public bool BoxOpenReq(ISender sender, int boxId, ReceiveBoxOpenAckDelegate callback)
        {
            ReceiveBoxOpenAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("boxId", boxId);
            return sender.SendHttpPost((int)ERandomwarsItemProtocol.BoxOpenReq, "boxopen", json.ToString());
        }

        public delegate (ERandomwarsItemErrorCode errorCode, ItemBaseInfo[] arrayDeleteItemInfo, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData) ReceiveBoxOpenReqDelegate(string accessToken, int boxId);
        public ReceiveBoxOpenReqDelegate ReceiveBoxOpenReqHandler;
        public bool ReceiveBoxOpenReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int boxId = (int)jObject["boxId"];
            var res = ReceiveBoxOpenReqHandler(accessToken, boxId);
            return BoxOpenAck(sender, res.errorCode, res.arrayDeleteItemInfo, res.arrayRewardInfo, res.arrayQuestData);     
        }        

        public bool BoxOpenAck(ISender sender, ERandomwarsItemErrorCode errorCode, ItemBaseInfo[] arrayDeleteItemInfo, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayDeleteItemInfo", JsonConvert.SerializeObject(arrayDeleteItemInfo));
            json.Add("arrayRewardInfo", JsonConvert.SerializeObject(arrayRewardInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveBoxOpenAckDelegate(ERandomwarsItemErrorCode errorCode, ItemBaseInfo[] arrayDeleteItemInfo, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData);
        public ReceiveBoxOpenAckDelegate ReceiveBoxOpenAckHandler;
        public bool ReceiveBoxOpenAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsItemErrorCode errorCode = (ERandomwarsItemErrorCode)(int)jObject["errorCode"];
            ItemBaseInfo[] arrayDeleteItemInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayDeleteItemInfo"].ToString());
            ItemBaseInfo[] arrayRewardInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayRewardInfo"].ToString());
            QuestData[] arrayQuestData = JsonConvert.DeserializeObject<QuestData[]>(jObject["arrayQuestData"].ToString());
            return ReceiveBoxOpenAckHandler(errorCode, arrayDeleteItemInfo, arrayRewardInfo, arrayQuestData);
        }
        #endregion    

        #region EmotionEquip ---------------------------------------------------------------------
        public bool EmotionEquipReq(ISender sender, List<int> listItemId, ReceiveEmotionEquipAckDelegate callback)
        {
            ReceiveEmotionEquipAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("listItemId", JsonConvert.SerializeObject(listItemId));
            return sender.SendHttpPost((int)ERandomwarsItemProtocol.EmotionEquipReq, "emotionequip", json.ToString());
        }

        public delegate (ERandomwarsItemErrorCode errorCode, List<int> listItemId) ReceiveEmotionEquipReqDelegate(string accessToken, List<int> listItemId);
        public ReceiveEmotionEquipReqDelegate ReceiveEmotionEquipReqHandler;
        public bool ReceiveEmotionEquipReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            List<int> listItemId = JsonConvert.DeserializeObject<List<int>>(jObject["listItemId"].ToString());
            var res = ReceiveEmotionEquipReqHandler(accessToken, listItemId);
            return EmotionEquipAck(sender, res.errorCode, res.listItemId);     
        }        

        public bool EmotionEquipAck(ISender sender, ERandomwarsItemErrorCode errorCode, List<int> listItemId)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("listItemId", JsonConvert.SerializeObject(listItemId));
            return sender.SendHttpResult(json.ToString());
        }        


        public delegate bool ReceiveEmotionEquipAckDelegate(ERandomwarsItemErrorCode errorCode, List<int> listItemId);
        public ReceiveEmotionEquipAckDelegate ReceiveEmotionEquipAckHandler;
        public bool ReceiveEmotionEquipAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsItemErrorCode errorCode = (ERandomwarsItemErrorCode)(int)jObject["errorCode"];
            List<int> listItemId = JsonConvert.DeserializeObject<List<int>>(jObject["listItemId"].ToString());
            return ReceiveEmotionEquipAckHandler(errorCode, listItemId);
        }
        #endregion            
    }
}