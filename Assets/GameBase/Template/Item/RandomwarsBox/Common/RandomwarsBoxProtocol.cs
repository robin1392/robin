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
        Begin = 20000,

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

        public delegate (ERandomwarsBoxErrorCode errorCode, MsgUserBox boxInfo, MsgReward[] arrayRewardInfo, MsgQuestData[] arrayQuestData, int updateKey) ReceiveBoxOpenReqDelegate(string accessToken, int boxId);
        public ReceiveBoxOpenReqDelegate ReceiveBoxOpenReqHandler;
        public bool ReceiveBoxOpenReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int boxId = (int)jObject["boxId"];
            var res = ReceiveBoxOpenReqHandler(accessToken, boxId);
            return BoxOpenAck(sender, res.errorCode, res.boxInfo, res.arrayRewardInfo, res.arrayQuestData, res.updateKey);
        }

        public bool BoxOpenAck(ISender sender, ERandomwarsBoxErrorCode errorCode, MsgUserBox boxInfo, MsgReward[] arrayRewardInfo, MsgQuestData[] arrayQuestData, int updateKey)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("boxInfo", JsonConvert.SerializeObject(boxInfo));
            json.Add("arrayRewardInfo", JsonConvert.SerializeObject(arrayRewardInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            json.Add("updateKey", updateKey);
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveBoxOpenAckDelegate(ERandomwarsBoxErrorCode errorCode, MsgUserBox boxInfo, MsgReward[] arrayRewardInfo, MsgQuestData[] arrayQuestData, int updateKey);
        public ReceiveBoxOpenAckDelegate ReceiveBoxOpenAckHandler;
        public bool ReceiveBoxOpenAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsBoxErrorCode errorCode = (ERandomwarsBoxErrorCode)(int)jObject["errorCode"];
            MsgUserBox boxInfo = JsonConvert.DeserializeObject<MsgUserBox>(jObject["boxInfo"].ToString());
            MsgReward[] arrayRewardInfo = JsonConvert.DeserializeObject<MsgReward[]>(jObject["arrayRewardInfo"].ToString());
            MsgQuestData[] arrayQuestData = JsonConvert.DeserializeObject<MsgQuestData[]>(jObject["arrayQuestData"].ToString());
            int updateKey = (int)jObject["updateKey"];
            return ReceiveBoxOpenAckHandler(errorCode, boxInfo, arrayRewardInfo, arrayQuestData, updateKey);
        }
        #endregion    
    }
}