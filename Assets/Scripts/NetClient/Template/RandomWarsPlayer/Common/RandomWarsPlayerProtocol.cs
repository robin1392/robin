using System.Collections.Generic;
using System.IO;
using Service.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.Player.RandomWarsPlayer.Common
{
    public enum ERandomWarsPlayerProtocol
    {
        BEGIN = 2000000,
        
        EDIT_NAME_REQ,
        EDIT_NAME_ACK,
        EDIT_NAME_NOTIFY,

        END
    }

    public class RandomWarsPlayerProtocol : MessageControllerBase
    {
        public RandomWarsPlayerProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                 //{(int)ERandomWarsPlayerProtocol.EDIT_NAME_REQ, HttpReceiveEditNameReq},
           };
        }


        // -------------------------------------------------------------------
        // Http Controller 구현부
        // -------------------------------------------------------------------
#region Http Controller 구현부        

        // public bool HttpSendEditNameReq(HttpClient client, string playerGuid, string editName)
        // {
        //     JObject json = new JObject();
        //     json.Add("playerGuid", playerGuid);
        //     json.Add("editName", editName);
        //     client.Send((int)ERandomWarsPlayerProtocol.EDIT_NAME_REQ, "player/editName", json.ToString());
        //     return true;
        // }


        // public delegate (ERandomWarsPlayerErrorCode errorCode, string editName) HttpReceiveEditNameReqDelegate(string playerGuid, string editName);
        // public HttpReceiveEditNameReqDelegate HttpReceiveEditNameReqCallback;
        // public string HttpReceiveEditNameReq(string json)
        // {
        //     JObject jObject = JObject.Parse(json);
        //     var res = HttpReceiveEditNameReqCallback(
        //         jObject["playerGuid"].ToString(),
        //         jObject["editName"].ToString());

        //     return HttpSendEditNameAck(res.errorCode, res.editName);
        // }


        // public string HttpSendEditNameAck(ERandomWarsPlayerErrorCode errorCode, string editName)
        // {
        //     JObject json = new JObject();
        //     json.Add("errorCode", (int)errorCode);
        //     json.Add("editName", editName);
        //     return json.ToString();
        // }


        // public delegate bool HttpReceiveEditNameAckDelegate(ERandomWarsPlayerErrorCode errorCode, string editName);
        // public HttpReceiveEditNameAckDelegate HttpReceiveEditNameAckCallback;
        // public string HttpReceiveEditNameAck(string json)
        // {
        //     JObject jObject = JObject.Parse(json);
        //     HttpReceiveEditNameAckCallback(
        //         (ERandomWarsPlayerErrorCode)(int)jObject["errorCode"], 
        //         jObject["editName"].ToString());

        //     return "";
        // }
    
#endregion

    }
}