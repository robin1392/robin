using System.Collections.Generic;
using System.IO;
using Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Player.RandomWarsPlayer.Common
{
    public enum ERandomWarsPlayerProtocol
    {
        BEGIN = 20000000,
        
        EDIT_NAME_REQ,
        EDIT_NAME_ACK,
        EDIT_NAME_NOTIFY,

        OPEN_BOX_REQ,
        OPEN_BOX_ACK,
        OPEN_BOX_NOTIFY,

        END
    }

    public class RandomWarsPlayerProtocol : BaseProtocol
    {
        public RandomWarsPlayerProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
            };

            HttpMessageControllers = new Dictionary<int, HttpControllerDelegate>
            {
                {(int)ERandomWarsPlayerProtocol.EDIT_NAME_REQ, HttpReceiveEditNameReq},
                {(int)ERandomWarsPlayerProtocol.OPEN_BOX_REQ, HttpReceiveOpenBoxReq},
                {(int)ERandomWarsPlayerProtocol.OPEN_BOX_ACK, HttpReceiveOpenBoxAck},
          };            
        }


        // -------------------------------------------------------------------
        // Http Controller 구현부
        // -------------------------------------------------------------------
#region Http Controller 구현부        

        public bool HttpSendEditNameReq(HttpClient client, string playerGuid, string editName)
        {
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("editName", editName);
            client.Send((int)ERandomWarsPlayerProtocol.EDIT_NAME_REQ, "player/editName", json.ToString());
            return true;
        }


        public delegate (ERandomWarsPlayerErrorCode errorCode, string editName) HttpReceiveEditNameReqDelegate(string playerGuid, string editName);
        public HttpReceiveEditNameReqDelegate HttpReceiveEditNameReqCallback;
        public string HttpReceiveEditNameReq(string json)
        {
            JObject jObject = JObject.Parse(json);
            var res = HttpReceiveEditNameReqCallback(
                jObject["playerGuid"].ToString(),
                jObject["editName"].ToString());

            return HttpSendEditNameAck(res.errorCode, res.editName);
        }


        public string HttpSendEditNameAck(ERandomWarsPlayerErrorCode errorCode, string editName)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("editName", editName);
            return json.ToString();
        }


        public delegate bool HttpReceiveEditNameAckDelegate(ERandomWarsPlayerErrorCode errorCode, string editName);
        public HttpReceiveEditNameAckDelegate HttpReceiveEditNameAckCallback;
        public string HttpReceiveEditNameAck(string json)
        {
            JObject jObject = new JObject(json);
            HttpReceiveEditNameAckCallback(
                (ERandomWarsPlayerErrorCode)(int)jObject["errorCode"], 
                jObject["editName"].ToString());

            return "";
        }
        

        // openBox
        public bool HttpSendOpenBoxReq(HttpClient client, string playerGuid, int boxId)
        {
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("boxId", boxId);
            client.Send((int)ERandomWarsPlayerProtocol.OPEN_BOX_REQ, "player/openBox", json.ToString());
            return true;
        }


        public delegate (ERandomWarsPlayerErrorCode errorCode, OpenBoxReward[] rewardInfo) HttpReceiveOpenBoxReqDelegate(string playerGuid, int boxId);
        public HttpReceiveOpenBoxReqDelegate HttpReceiveOpenBoxReqCallback;
        public string HttpReceiveOpenBoxReq(string json)
        {
            JObject jObject = JObject.Parse(json);
            var res = HttpReceiveOpenBoxReqCallback(
                jObject["playerGuid"].ToString(),
                (int)jObject["boxId"]);

            return HttpSendOpenBoxAck(res.errorCode, res.rewardInfo);
        }


        public string HttpSendOpenBoxAck(ERandomWarsPlayerErrorCode errorCode, OpenBoxReward[] rewardInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("rewardInfo", JsonConvert.SerializeObject(rewardInfo));
            return json.ToString();
        }


        public delegate bool HttpReceiveOpenBoxAckDelegate(ERandomWarsPlayerErrorCode errorCode, OpenBoxReward[] rewardInfo);
        public HttpReceiveOpenBoxAckDelegate HttpReceiveOpenBoxAckCallback;
        public string HttpReceiveOpenBoxAck(string json)
        {
            JObject jObject = JObject.Parse(json);
            HttpReceiveOpenBoxAckCallback(
                (ERandomWarsPlayerErrorCode)(int)jObject["errorCode"], 
                JsonConvert.DeserializeObject<OpenBoxReward[]>(jObject["rewardInfo"].ToString()));


            return "";
        }
#endregion

    }
}