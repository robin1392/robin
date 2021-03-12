using System;
using System.Collections.Generic;
using RandomWarsService.Network.Http;
using RandomWarsProtocol.Msg;
using Newtonsoft.Json;

namespace RandomWarsProtocol
{
    public class HttpSender
    {
        HttpClient _httpService;


        public HttpSender(HttpClient httpService)
        {
            _httpService = httpService;
        }


        //public void AuthUserReq(MsgUserAuthReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.AUTH_USER_REQ, "userauth", JsonConvert.SerializeObject(msg));
        //}


        //public string AuthUserAck(MsgUserAuthAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}


        //public void EditUserNameReq(MsgEditUserNameReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.EDIT_USER_NAME_REQ, "usernameedit", JsonConvert.SerializeObject(msg));
        //}


        //public string EditUserNameAck(MsgEditUserNameAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}


        //public void UpdateDeckReq(MsgUpdateDeckReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.UPDATE_DECK_REQ, "deckupdate", JsonConvert.SerializeObject(msg));
        //}


        //public string UpdateDeckAck(MsgUpdateDeckAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}


        //public void EndTutorialReq(MsgEndTutorialReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.END_TUTORIAL_REQ, "tutorialend", JsonConvert.SerializeObject(msg));
        //}


        //public string EndTutorialAck(MsgEndTutorialAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}


        public void StartMatchReq(MsgStartMatchReq msg)
        {
            _httpService.Send((int)GameProtocol.START_MATCH_REQ, "matchrequest", JsonConvert.SerializeObject(msg));
        }


        public string StartMatchAck(MsgStartMatchAck msg)
        {
            return JsonConvert.SerializeObject(msg);
        }


        public void StatusMatchReq(MsgStatusMatchReq msg)
        {
            _httpService.Send((int)GameProtocol.STATUS_MATCH_REQ, "matchstatus", JsonConvert.SerializeObject(msg));
        }


        public string StatusMatchAck(MsgStatusMatchAck msg)
        {
            return JsonConvert.SerializeObject(msg);
        }

        public void StopMatchReq(MsgStopMatchReq msg)
        {
            _httpService.Send((int)GameProtocol.STOP_MATCH_REQ, "matchstop", JsonConvert.SerializeObject(msg));
        }


        public string StopMatchAck(MsgStopMatchAck msg)
        {
            return JsonConvert.SerializeObject(msg);
        }


        //public void OpenBoxReq(MsgOpenBoxReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.OPEN_BOX_REQ, "boxopen", JsonConvert.SerializeObject(msg));
        //}


        //public string OpenBoxAck(MsgOpenBoxAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}

        //public void LevelUpDiceReq(MsgLevelUpDiceReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.LEVELUP_DICE_REQ, "dicelevelup", JsonConvert.SerializeObject(msg));
        //}


        //public string LevelUpDiceAck(MsgLevelUpDiceAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}

        //public void SeasonInfoReq(MsgSeasonInfoReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.SEASON_INFO_REQ, "seasoninfo", JsonConvert.SerializeObject(msg));
        //}


        //public string SeasonInfoAck(MsgSeasonInfoAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}

        //public void SeasonResetReq(MsgSeasonResetReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.SEASON_RESET_REQ, "seasonreset", JsonConvert.SerializeObject(msg));
        //}


        //public string SeasonResetAck(MsgSeasonResetAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}


        //public void GetRankReq(MsgGetRankReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.GET_RANK_REQ, "rankget", JsonConvert.SerializeObject(msg));
        //}


        //public string GetRankAck(MsgGetRankAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}

        //public void SeasonPassInfoReq(MsgSeasonPassInfoReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.SEASON_PASS_INFO_REQ, "seasonpassinfo", JsonConvert.SerializeObject(msg));
        //}


        //public string SeasonPassInfoAck(MsgSeasonPassInfoAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}


        //public void SeasonPassRewardStepReq(MsgSeasonPassRewardStepReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.SEASON_PASS_REWARD_STEP_REQ, "seasonpassrewardstep", JsonConvert.SerializeObject(msg));
        //}


        //public string SeasonPassRewardStepAck(MsgSeasonPassRewardStepAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}


        //public void GetSeasonPassRewardReq(MsgGetSeasonPassRewardReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.GET_SEASON_PASS_REWARD_REQ, "seasonpassreward ", JsonConvert.SerializeObject(msg));
        //}


        //public string GetSeasonPassRewardAck(MsgGetSeasonPassRewardAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}


        //public void ClassRewardInfoReq(MsgClassRewardInfoReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.CLASS_REWARD_INFO_REQ, "classrewardinfo", JsonConvert.SerializeObject(msg));
        //}


        //public string ClassRewardInfoAck(MsgClassRewardInfoAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}


        //public void GetClassRewardReq(MsgGetClassRewardReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.GET_CLASS_REWARD_REQ, "classrewardget", JsonConvert.SerializeObject(msg));
        //}


        //public string GetClassRewardAck(MsgGetClassRewardAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}


        //public void QuestInfoReq(MsgQuestInfoReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.QUEST_INFO_REQ, "questinfo", JsonConvert.SerializeObject(msg));
        //}


        //public string QuestInfoAck(MsgQuestInfoAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}


        //public void QuestRewardReq(MsgQuestRewardReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.QUEST_REWARD_REQ, "questreward", JsonConvert.SerializeObject(msg));
        //}


        //public string QuestRewardAck(MsgQuestRewardAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}

        //public void QuestDayRewardReq(MsgQuestDayRewardReq msg)
        //{
        //    _httpService.Send((int)GameProtocol.QUEST_DAY_REWARD_REQ, "questdayreward", JsonConvert.SerializeObject(msg));
        //}


        //public string QuestDayRewardAck(MsgQuestDayRewardAck msg)
        //{
        //    return JsonConvert.SerializeObject(msg);
        //}
    }
}
