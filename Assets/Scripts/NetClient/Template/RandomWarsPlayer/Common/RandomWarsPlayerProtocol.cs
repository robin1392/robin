using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Service.Net;

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
                {(int)ERandomWarsPlayerProtocol.EDIT_NAME_REQ, ReceiveEditNameReq},
                {(int)ERandomWarsPlayerProtocol.EDIT_NAME_ACK, ReceiveEditNameAck},
            };
        }


        // -------------------------------------------------------------------
        // Http Controller 구현부
        // -------------------------------------------------------------------
        #region Http Controller 구현부        
        public bool SendEditNameReq(ISender sender, string playerGuid, string editName)
        {
            JObject json = new JObject();
            json.Add("playerGuid", playerGuid);
            json.Add("editName", editName);
            return sender.SendHttpPost((int)ERandomWarsPlayerProtocol.EDIT_NAME_REQ, "editname", json.ToString());
        }


        public delegate (ERandomWarsPlayerErrorCode errorCode, string editName) ReceiveEditNameReqDelegate(string playerGuid, string editName);
        public ReceiveEditNameReqDelegate ReceiveEditNameReqCallback;
        public bool ReceiveEditNameReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string playerGuid = (string)jObject["playerGuid"];
            string editName = (string)jObject["editName"];
            var res = ReceiveEditNameReqCallback(playerGuid, editName);
            return SendEditNameAck(sender, res.errorCode, res.editName);
        }


        public bool SendEditNameAck(ISender sender, ERandomWarsPlayerErrorCode errorCode, string editName)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("editName", editName);
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveEditNameAckDelegate(ERandomWarsPlayerErrorCode errorCode, string editName);
        public ReceiveEditNameAckDelegate ReceiveEditNameAckCallback;
        public bool ReceiveEditNameAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomWarsPlayerErrorCode errorCode = (ERandomWarsPlayerErrorCode)(int)jObject["errorCode"];
            string editName = (string)jObject["editName"];
            return ReceiveEditNameAckCallback(errorCode, editName);
        }
        #endregion
    }
}