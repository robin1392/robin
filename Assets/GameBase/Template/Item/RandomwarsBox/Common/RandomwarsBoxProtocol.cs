using System.Collections.Generic;
using System.Text;
using Service.Core;
using Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Item.RandomwarsBox.Common
{
    public enum ERandomwarsBoxProtocol
    {
        Begin = 40000,

        BoxOpenReq,
        BoxOpenAck,

        End,
    }


    public class RandomwarsBoxProtocol : MessageControllerBase
    {
        public RandomwarsBoxProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomwarsBoxProtocol.BoxOpenReq, ReceiveBoxOpenReq},
                {(int)ERandomwarsBoxProtocol.BoxOpenAck, ReceiveBoxOpenAck},
            };
        }


        #region BoxOpen ---------------------------------------------------------------------
        public bool BoxOpenReq(ISender sender, int boxId, ReceiveBoxOpenAckDelegate callback)
        {
            ReceiveBoxOpenAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("boxId", boxId);
            return sender.SendHttpPost((int)ERandomwarsBoxProtocol.BoxOpenReq, "boxopen", json.ToString());
        }

        public delegate (ERandomwarsBoxErrorCode errorCode, ItemBaseInfo[] arrayDeleteItemInfo, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData) ReceiveBoxOpenReqDelegate(string accessToken, int boxId);
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

        public bool BoxOpenAck(ISender sender, ERandomwarsBoxErrorCode errorCode, ItemBaseInfo[] arrayDeleteItemInfo, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayDeleteItemInfo", JsonConvert.SerializeObject(arrayDeleteItemInfo));
            json.Add("arrayRewardInfo", JsonConvert.SerializeObject(arrayRewardInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveBoxOpenAckDelegate(ERandomwarsBoxErrorCode errorCode, ItemBaseInfo[] arrayDeleteItemInfo, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData);
        public ReceiveBoxOpenAckDelegate ReceiveBoxOpenAckHandler;
        public bool ReceiveBoxOpenAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsBoxErrorCode errorCode = (ERandomwarsBoxErrorCode)(int)jObject["errorCode"];
            ItemBaseInfo[] arrayDeleteItemInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayDeleteItemInfo"].ToString());
            ItemBaseInfo[] arrayRewardInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayRewardInfo"].ToString());
            QuestData[] arrayQuestData = JsonConvert.DeserializeObject<QuestData[]>(jObject["arrayQuestData"].ToString());
            return ReceiveBoxOpenAckHandler(errorCode, arrayDeleteItemInfo, arrayRewardInfo, arrayQuestData);
        }
        #endregion    
    }
}