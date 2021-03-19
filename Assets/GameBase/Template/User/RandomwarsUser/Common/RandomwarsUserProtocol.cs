using System.Collections.Generic;
using System.Text;
using Service.Core;
using Service.Net;
using Service.Template;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Template.User.RandomwarsUser.Common
{
    public enum ERandomwarsProtocol
    {
        Begin = ETemplateType.User * 10000,

        UserInfoReq,
        UserInfoAck,

        UserTrophyRewardReq,
        UserTrophyRewardAck,

        UserAdRewardReq,
        UserAdRewardAck,

        UserTutorialEndReq,
        UserTutorialEndAck,

        UserNameInitReq,
        UserNameInitAck,

        UserNameChangeReq,
        UserNameChangeAck,


        End,
    }


    public class RandomwarsUserProtocol : MessageControllerBase
    {
        public RandomwarsUserProtocol()
        {
            MessageControllers = new Dictionary<int, ControllerDelegate>
            {
                {(int)ERandomwarsProtocol.UserInfoReq, ReceiveUserInfoReq},
                {(int)ERandomwarsProtocol.UserInfoAck, ReceiveUserInfoAck},
                {(int)ERandomwarsProtocol.UserTrophyRewardReq, ReceiveUserTrophyRewardReq},
                {(int)ERandomwarsProtocol.UserTrophyRewardAck, ReceiveUserTrophyRewardAck},
                {(int)ERandomwarsProtocol.UserTutorialEndReq, ReceiveUserTutorialEndReq},
                {(int)ERandomwarsProtocol.UserNameInitReq, ReceiveUserNameInitReq},
                {(int)ERandomwarsProtocol.UserNameInitAck, ReceiveUserNameInitAck},
                {(int)ERandomwarsProtocol.UserNameChangeReq, ReceiveUserNameChangeReq},
                {(int)ERandomwarsProtocol.UserNameChangeAck, ReceiveUserNameChangeAck},
                {(int)ERandomwarsProtocol.UserAdRewardReq, ReceiveUserAdRewardReq},
                {(int)ERandomwarsProtocol.UserAdRewardAck, ReceiveUserAdRewardAck},
            };
        }


        #region UserInfo ---------------------------------------------------------------------
        public bool UserInfoReq(ISender sender, ReceiveUserInfoAckDelegate callback)
        {
            ReceiveUserInfoAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)ERandomwarsProtocol.UserInfoReq, "userinfo", json.ToString());
        }

        public delegate (ERandomwarsUserErrorCode errorCode, MsgUserInfo userInfo, UserDeck[] arrayUserDeck, UserDice[] arrayUserDice, UserItemInfo userItemInfo, QuestInfo questInfo, UserSeasonInfo seasonInfo) ReceiveUserInfoReqDelegate(string accessToken);
        public ReceiveUserInfoReqDelegate ReceiveUserInfoReqHandler;
        public bool ReceiveUserInfoReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveUserInfoReqHandler(accessToken);
            return UserInfoAck(sender, res.errorCode, res.userInfo, res.arrayUserDeck, res.arrayUserDice, res.userItemInfo, res.questInfo, res.seasonInfo);
        }

        public bool UserInfoAck(ISender sender, ERandomwarsUserErrorCode errorCode, MsgUserInfo userInfo, UserDeck[] arrayUserDeck, UserDice[] arrayUserDice, UserItemInfo userItemInfo, QuestInfo questInfo, UserSeasonInfo seasonInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("userInfo", JsonConvert.SerializeObject(userInfo));
            json.Add("arrayUserDeck", JsonConvert.SerializeObject(arrayUserDeck));
            json.Add("arrayUserDice", JsonConvert.SerializeObject(arrayUserDice));
            json.Add("userItemInfo", JsonConvert.SerializeObject(userItemInfo));
            json.Add("questInfo", JsonConvert.SerializeObject(questInfo));
            json.Add("seasonInfo", JsonConvert.SerializeObject(seasonInfo));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveUserInfoAckDelegate(ERandomwarsUserErrorCode errorCode, MsgUserInfo userInfo, UserDeck[] arrayUserDeck, UserDice[] arrayUserDice, UserItemInfo userItemInfo, QuestInfo questInfo, UserSeasonInfo seasonInfo);
        public ReceiveUserInfoAckDelegate ReceiveUserInfoAckHandler;
        public bool ReceiveUserInfoAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsUserErrorCode errorCode = (ERandomwarsUserErrorCode)(int)jObject["errorCode"];
            MsgUserInfo userInfo = JsonConvert.DeserializeObject<MsgUserInfo>(jObject["userInfo"].ToString());
            UserDeck[] arrayUserDeck = JsonConvert.DeserializeObject<UserDeck[]>(jObject["arrayUserDeck"].ToString());
            UserDice[] arrayUserDice = JsonConvert.DeserializeObject<UserDice[]>(jObject["arrayUserDice"].ToString());
            UserItemInfo userItemInfo = JsonConvert.DeserializeObject<UserItemInfo>(jObject["userItemInfo"].ToString());
            QuestInfo questInfo = JsonConvert.DeserializeObject<QuestInfo>(jObject["questInfo"].ToString());
            UserSeasonInfo seasonInfo = JsonConvert.DeserializeObject<UserSeasonInfo>(jObject["seasonInfo"].ToString());

            return ReceiveUserInfoAckHandler(errorCode, userInfo, arrayUserDeck, arrayUserDice, userItemInfo, questInfo, seasonInfo);
        }
        #endregion    


        #region UserTrophyReward ---------------------------------------------------------------------
        public bool UserTrophyRewardReq(ISender sender, int rewardId, int targetType, ReceiveUserTrophyRewardAckDelegate callback)
        {
            ReceiveUserTrophyRewardAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("rewardId", rewardId);
            json.Add("targetType", targetType);
            return sender.SendHttpPost((int)ERandomwarsProtocol.UserTrophyRewardReq, "usertrophyreward", json.ToString());
        }


        public delegate (ERandomwarsUserErrorCode errorCode, int[] arrayRewardId, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData) ReceiveUserTrophyRewardReqDelegate(string accessToken, int rewardId, int targetType);
        public ReceiveUserTrophyRewardReqDelegate ReceiveUserTrophyRewardReqHandler;
        public bool ReceiveUserTrophyRewardReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            int rewardId = (int)jObject["rewardId"];
            int targetType = (int)jObject["targetType"];
            var res = ReceiveUserTrophyRewardReqHandler(accessToken, rewardId, targetType);
            return UserTrophyRewardAck(sender, res.errorCode, res.arrayRewardId, res.arrayRewardInfo, res.arrayQuestData);
        }

        public bool UserTrophyRewardAck(ISender sender, ERandomwarsUserErrorCode errorCode, int[] arrayRewardId, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayRewardId", JsonConvert.SerializeObject(arrayRewardId));
            json.Add("arrayRewardInfo", JsonConvert.SerializeObject(arrayRewardInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveUserTrophyRewardAckDelegate(ERandomwarsUserErrorCode errorCode, int[] arrayRewardId, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData);
        public ReceiveUserTrophyRewardAckDelegate ReceiveUserTrophyRewardAckHandler;
        public bool ReceiveUserTrophyRewardAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsUserErrorCode errorCode = (ERandomwarsUserErrorCode)(int)jObject["errorCode"];
            int[] arrayRewardId = JsonConvert.DeserializeObject<int[]>(jObject["arrayRewardId"].ToString());
            ItemBaseInfo[] arrayRewardInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayRewardInfo"].ToString());
            QuestData[] arrayQuestData = JsonConvert.DeserializeObject<QuestData[]>(jObject["arrayQuestData"].ToString());
            return ReceiveUserTrophyRewardAckHandler(errorCode, arrayRewardId, arrayRewardInfo, arrayQuestData);
        }
        #endregion                 


        #region UserAdReward ---------------------------------------------------------------------
        public bool UserAdRewardReq(ISender sender, string rewardId, ReceiveUserAdRewardAckDelegate callback)
        {
            ReceiveUserAdRewardAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("rewardId", rewardId);
            return sender.SendHttpPost((int)ERandomwarsProtocol.UserAdRewardReq, "useradreward", json.ToString());
        }


        public delegate (ERandomwarsUserErrorCode errorCode, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData) ReceiveUserAdRewardReqDelegate(string accessToken, string rewardId);
        public ReceiveUserAdRewardReqDelegate ReceiveUserAdRewardReqHandler;
        public bool ReceiveUserAdRewardReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            string rewardId = (string)jObject["rewardId"];
            var res = ReceiveUserAdRewardReqHandler(accessToken, rewardId);
            return UserAdRewardAck(sender, res.errorCode, res.arrayRewardInfo, res.arrayQuestData);
        }

        public bool UserAdRewardAck(ISender sender, ERandomwarsUserErrorCode errorCode, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("arrayRewardInfo", JsonConvert.SerializeObject(arrayRewardInfo));
            json.Add("arrayQuestData", JsonConvert.SerializeObject(arrayQuestData));
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveUserAdRewardAckDelegate(ERandomwarsUserErrorCode errorCode, ItemBaseInfo[] arrayRewardInfo, QuestData[] arrayQuestData);
        public ReceiveUserAdRewardAckDelegate ReceiveUserAdRewardAckHandler;
        public bool ReceiveUserAdRewardAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsUserErrorCode errorCode = (ERandomwarsUserErrorCode)(int)jObject["errorCode"];
            ItemBaseInfo[] arrayRewardInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayRewardInfo"].ToString());
            QuestData[] arrayQuestData = JsonConvert.DeserializeObject<QuestData[]>(jObject["arrayQuestData"].ToString());
            return ReceiveUserAdRewardAckHandler(errorCode, arrayRewardInfo, arrayQuestData);
        }
        #endregion         

        #region UserTutorialEnd ---------------------------------------------------------------------
        public bool UserTutorialEndReq(ISender sender, ReceiveUserTutorialEndAckDelegate callback)
        {
            ReceiveUserTutorialEndAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            return sender.SendHttpPost((int)ERandomwarsProtocol.UserTutorialEndReq, "usertutorialend", json.ToString());
        }


        public delegate (ERandomwarsUserErrorCode errorCode, bool endTutorial) ReceiveUserTutorialEndReqDelegate(string accessToken);
        public ReceiveUserTutorialEndReqDelegate ReceiveUserTutorialEndReqHandler;
        public bool ReceiveUserTutorialEndReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            var res = ReceiveUserTutorialEndReqHandler(accessToken);
            return UserTutorialEndAck(sender, res.errorCode, res.endTutorial);
        }

        public bool UserTutorialEndAck(ISender sender, ERandomwarsUserErrorCode errorCode, bool endTutorial)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("endTutorial", (bool)endTutorial);
            return sender.SendHttpResult(json.ToString());
        }


        public delegate bool ReceiveUserTutorialEndAckDelegate(ERandomwarsUserErrorCode errorCode, bool endTutorial);
        public ReceiveUserTutorialEndAckDelegate ReceiveUserTutorialEndAckHandler;
        public bool ReceiveUserTutorialEndAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsUserErrorCode errorCode = (ERandomwarsUserErrorCode)(int)jObject["errorCode"];
            bool endTutorial = (bool)jObject["endTutorial"];
            return ReceiveUserTutorialEndAckHandler(errorCode, endTutorial);
        }
        #endregion    
        #region UserNameInit ---------------------------------------------------------------------
        public bool UserNameInitReq(ISender sender, string userName, ReceiveUserNameInitAckDelegate callback)
        {
            ReceiveUserNameInitAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("userName", userName);
            return sender.SendHttpPost((int)ERandomwarsProtocol.UserNameInitReq, "usernameinit", json.ToString());
        }

        public delegate (ERandomwarsUserErrorCode errorCode, string userName, bool isNameInit) ReceiveUserNameInitReqDelegate(string accessToken, string userName);
        public ReceiveUserNameInitReqDelegate ReceiveUserNameInitReqHandler;
        public bool ReceiveUserNameInitReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            string userName = (string)jObject["userName"];
            var res = ReceiveUserNameInitReqHandler(accessToken, userName);
            return UserNameInitAck(sender, res.errorCode, res.userName, res.isNameInit);
        }

        public bool UserNameInitAck(ISender sender, ERandomwarsUserErrorCode errorCode, string userName, bool isNameInit)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("userName", userName);
            json.Add("isNameInit", isNameInit);
            return sender.SendHttpResult(json.ToString());
        }

        public delegate bool ReceiveUserNameInitAckDelegate(ERandomwarsUserErrorCode errorCode, string userName, bool isNameInit);
        public ReceiveUserNameInitAckDelegate ReceiveUserNameInitAckHandler;
        public bool ReceiveUserNameInitAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            ERandomwarsUserErrorCode errorCode = (ERandomwarsUserErrorCode)(int)jObject["errorCode"];
            string userName = (string)jObject["userName"];
            bool isNameInit = (bool)jObject["isNameInit"];
            return ReceiveUserNameInitAckHandler(errorCode, userName, isNameInit);
        }
        #endregion

        #region UserNameChange ---------------------------------------------------------------------
        public bool UserNameChangeReq(ISender sender, string userName, ReceiveUserNameChangeAckDelegate callback)
        {
            ReceiveUserNameChangeAckHandler = callback;
            JObject json = new JObject();
            json.Add("accessToken", sender.GetAccessToken());
            json.Add("userName", userName);
            return sender.SendHttpPost((int)ERandomwarsProtocol.UserNameChangeReq, "usernamechange", json.ToString());
        }

        public delegate (ERandomwarsUserErrorCode errorCode, string userName, ItemBaseInfo[] arrayDeleteItemInfo) ReceiveUserNameChangeReqDelegate(string accessToken, string userName);
        public ReceiveUserNameChangeReqDelegate ReceiveUserNameChangeReqHandler;
        public bool ReceiveUserNameChangeReq(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = JObject.Parse(json);
            string accessToken = (string)jObject["accessToken"];
            string userName = (string)jObject["userName"];
            var res = ReceiveUserNameChangeReqHandler(accessToken, userName);
            return UserNameChangeAck(sender, res.errorCode, res.userName, res.arrayDeleteItemInfo);
        }

        public bool UserNameChangeAck(ISender sender, ERandomwarsUserErrorCode errorCode, string userName, ItemBaseInfo[] arrayDeleteItemInfo)
        {
            JObject json = new JObject();
            json.Add("errorCode", (int)errorCode);
            json.Add("userName", userName);
            json.Add("arrayDeleteItemInfo", JsonConvert.SerializeObject(arrayDeleteItemInfo));
            return sender.SendHttpResult(json.ToString());
        }

        public delegate bool ReceiveUserNameChangeAckDelegate(ERandomwarsUserErrorCode errorCode, string userName, ItemBaseInfo[] arrayDeleteItemInfo);
        public ReceiveUserNameChangeAckDelegate ReceiveUserNameChangeAckHandler;
        public bool ReceiveUserNameChangeAck(ISender sender, byte[] msg, int length)
        {
            string json = Encoding.Default.GetString(msg, 0, length);
            JObject jObject = new JObject();
            ERandomwarsUserErrorCode errorCode = (ERandomwarsUserErrorCode)(int)jObject["errorCode"];
            string userName = (string)jObject["userName"];
            ItemBaseInfo[] arrayItemInfo = JsonConvert.DeserializeObject<ItemBaseInfo[]>(jObject["arrayDeleteItemInfo"].ToString());
            return ReceiveUserNameChangeAckHandler(errorCode, userName, arrayItemInfo);
        }
        #endregion        
    }
}