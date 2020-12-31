using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RandomWarsService.Network.Http;
using RandomWarsProtocol.Msg;
using Newtonsoft.Json;

namespace RandomWarsProtocol
{
    public class HttpReceiver : IHttpReceiver
    {
        public delegate Task<string> AuthUserReqDelegate(MsgUserAuthReq msg);
        public AuthUserReqDelegate AuthUserReq;
        public delegate void AuthUserAckDelegate(MsgUserAuthAck msg);
        public AuthUserAckDelegate AuthUserAck;

        public delegate Task<string> EditUserNameReqDelegate(MsgEditUserNameReq msg);
        public EditUserNameReqDelegate EditUserNameReq;
        public delegate void EditUserNameAckDelegate(MsgEditUserNameAck msg);
        public EditUserNameAckDelegate EditUserNameAck;

        public delegate Task<string> UpdateDeckReqDelegate(MsgUpdateDeckReq msg);
        public UpdateDeckReqDelegate UpdateDeckReq;
        public delegate void UpdateDeckAckDelegate(MsgUpdateDeckAck msg);
        public UpdateDeckAckDelegate UpdateDeckAck;

        public delegate Task<string> StartMatchReqDelegate(MsgStartMatchReq msg);
        public StartMatchReqDelegate StartMatchReq;
        public delegate void StartMatchAckDelegate(MsgStartMatchAck msg);
        public StartMatchAckDelegate StartMatchAck;

        public delegate Task<string> StatusMatchReqDelegate(MsgStatusMatchReq msg);
        public StatusMatchReqDelegate StatusMatchReq;
        public delegate void StatusMatchAckDelegate(MsgStatusMatchAck msg);
        public StatusMatchAckDelegate StatusMatchAck;

        public delegate Task<string> StopMatchReqDelegate(MsgStopMatchReq msg);
        public StopMatchReqDelegate StopMatchReq;
        public delegate void StopMatchAckDelegate(MsgStopMatchAck msg);
        public StopMatchAckDelegate StopMatchAck;

        public delegate Task<string> OpenBoxReqDelegate(MsgOpenBoxReq msg);
        public OpenBoxReqDelegate OpenBoxReq;
        public delegate void OpenBoxAckDelegate(MsgOpenBoxAck msg);
        public OpenBoxAckDelegate OpenBoxAck;

        public delegate Task<string> LevelUpDiceReqDelegate(MsgLevelUpDiceReq msg);
        public LevelUpDiceReqDelegate LevelUpDiceReq;
        public delegate void LevelUpDiceAckDelegate(MsgLevelUpDiceAck msg);
        public LevelUpDiceAckDelegate LevelUpDiceAck;

        public delegate Task<string> SeasonInfoReqDelegate(MsgSeasonInfoReq msg);
        public SeasonInfoReqDelegate SeasonInfoReq;
        public delegate void SeasonInfoAckDelegate(MsgSeasonInfoAck msg);
        public SeasonInfoAckDelegate SeasonInfoAck;

        public delegate Task<string> GetRankReqDelegate(MsgGetRankReq msg);
        public GetRankReqDelegate GetRankReq;
        public delegate void GetRankAckDelegate(MsgGetRankAck msg);
        public GetRankAckDelegate GetRankAck;

        public delegate Task<string> SeasonPassInfoReqDelegate(MsgSeasonPassInfoReq msg);
        public SeasonPassInfoReqDelegate SeasonPassInfoReq;
        public delegate void SeasonPassInfoAckDelegate(MsgSeasonPassInfoAck msg);
        public SeasonPassInfoAckDelegate SeasonPassInfoAck;

        public delegate Task<string> GetSeasonPassRewardReqDelegate(MsgGetSeasonPassRewardReq msg);
        public GetSeasonPassRewardReqDelegate GetSeasonPassRewardReq;
        public delegate void GetSeasonPassRewardAckDelegate(MsgGetSeasonPassRewardAck msg);
        public GetSeasonPassRewardAckDelegate GetSeasonPassRewardAck;

        public delegate Task<string> ClassRewardInfoReqDelegate(MsgClassRewardInfoReq msg);
        public ClassRewardInfoReqDelegate ClassRewardInfoReq;
        public delegate void ClassRewardInfoAckDelegate(MsgClassRewardInfoAck msg);
        public ClassRewardInfoAckDelegate ClassRewardInfoAck;

        public delegate Task<string> GetClassRewardReqDelegate(MsgGetClassRewardReq msg);
        public GetClassRewardReqDelegate GetClassRewardReq;
        public delegate void GetClassRewardAckDelegate(MsgGetClassRewardAck msg);
        public GetClassRewardAckDelegate GetClassRewardAck;

        public delegate Task<string> QuestInfoReqDelegate(MsgQuestInfoReq msg);
        public QuestInfoReqDelegate QuestInfoReq;
        public delegate void QuestInfoAckDelegate(MsgQuestInfoAck msg);
        public QuestInfoAckDelegate QuestInfoAck;

        public delegate Task<string> QuestRewardReqDelegate(MsgQuestRewardReq msg);
        public QuestRewardReqDelegate QuestRewardReq;
        public delegate void QuestRewardAckDelegate(MsgQuestRewardAck msg);
        public QuestRewardAckDelegate QuestRewardAck;

        public async Task<string> ProcessAsync(int protocolId, string json)
        {
            string ackJson = string.Empty;
            switch ((GameProtocol)protocolId)
            {
                case GameProtocol.AUTH_USER_REQ:
                    {
                        if (AuthUserReq == null)
                            return ackJson;

                        MsgUserAuthReq msg = JsonConvert.DeserializeObject<MsgUserAuthReq>(json);
                        ackJson = await AuthUserReq(msg);
                    }
                    break;
                case GameProtocol.EDIT_USER_NAME_REQ:
                    {
                        if (EditUserNameReq == null)
                            return ackJson;

                        MsgEditUserNameReq msg = JsonConvert.DeserializeObject<MsgEditUserNameReq>(json);
                        ackJson = await EditUserNameReq(msg);
                    }
                    break;
                case GameProtocol.UPDATE_DECK_REQ:
                    {
                        if (UpdateDeckReq == null)
                            return ackJson;

                        MsgUpdateDeckReq msg = JsonConvert.DeserializeObject<MsgUpdateDeckReq>(json);
                        ackJson = await UpdateDeckReq(msg);
                    }
                    break;
                case GameProtocol.START_MATCH_REQ:
                    {
                        if (StartMatchReq == null)
                            return ackJson;

                        MsgStartMatchReq msg = JsonConvert.DeserializeObject<MsgStartMatchReq>(json);
                        ackJson = await StartMatchReq(msg);
                    }
                    break;
                case GameProtocol.STATUS_MATCH_REQ:
                    {
                        if (StatusMatchReq == null)
                            return ackJson;

                        MsgStatusMatchReq msg = JsonConvert.DeserializeObject<MsgStatusMatchReq>(json);
                        ackJson = await StatusMatchReq(msg);
                    }
                    break;
                case GameProtocol.STOP_MATCH_REQ:
                    {
                        if (StopMatchReq == null)
                            return ackJson;

                        MsgStopMatchReq msg = JsonConvert.DeserializeObject<MsgStopMatchReq>(json);
                        ackJson = await StopMatchReq(msg);
                    }
                    break;
                case GameProtocol.OPEN_BOX_REQ:
                    {
                        if (OpenBoxReq == null)
                            return ackJson;

                        MsgOpenBoxReq msg = JsonConvert.DeserializeObject<MsgOpenBoxReq>(json);
                        ackJson = await OpenBoxReq(msg);
                    }
                    break;
                case GameProtocol.LEVELUP_DICE_REQ:
                    {
                        if (LevelUpDiceReq == null)
                            return ackJson;

                        MsgLevelUpDiceReq msg = JsonConvert.DeserializeObject<MsgLevelUpDiceReq>(json);
                        ackJson = await LevelUpDiceReq(msg);
                    }
                    break;
                case GameProtocol.SEASON_INFO_REQ:
                    {
                        if (SeasonInfoReq == null)
                            return ackJson;

                        MsgSeasonInfoReq msg = JsonConvert.DeserializeObject<MsgSeasonInfoReq>(json);
                        ackJson = await SeasonInfoReq(msg);
                    }
                    break;
                case GameProtocol.GET_RANK_REQ:
                    {
                        if (GetRankReq == null)
                            return ackJson;

                        MsgGetRankReq msg = JsonConvert.DeserializeObject<MsgGetRankReq>(json);
                        ackJson = await GetRankReq(msg);
                    }
                    break;
                case GameProtocol.SEASON_PASS_INFO_REQ:
                    {
                        if (SeasonPassInfoReq == null)
                            return ackJson;

                        MsgSeasonPassInfoReq msg = JsonConvert.DeserializeObject<MsgSeasonPassInfoReq>(json);
                        ackJson = await SeasonPassInfoReq(msg);
                    }
                    break;
                case GameProtocol.GET_SEASON_PASS_REWARD_REQ:
                    {
                        if (GetSeasonPassRewardReq == null)
                            return ackJson;

                        MsgGetSeasonPassRewardReq msg = JsonConvert.DeserializeObject<MsgGetSeasonPassRewardReq>(json);
                        ackJson = await GetSeasonPassRewardReq(msg);
                    }
                    break;
                case GameProtocol.CLASS_REWARD_INFO_REQ:
                    {
                        if (ClassRewardInfoReq == null)
                            return ackJson;

                        MsgClassRewardInfoReq msg = JsonConvert.DeserializeObject<MsgClassRewardInfoReq>(json);
                        ackJson = await ClassRewardInfoReq(msg);
                    }
                    break;
                case GameProtocol.GET_CLASS_REWARD_REQ:
                    {
                        if (GetClassRewardReq == null)
                            return ackJson;

                        MsgGetClassRewardReq msg = JsonConvert.DeserializeObject<MsgGetClassRewardReq>(json);
                        ackJson = await GetClassRewardReq(msg);
                    }
                    break;
                case GameProtocol.QUEST_INFO_REQ:
                    {
                        if (QuestInfoReq == null)
                            return ackJson;

                        MsgQuestInfoReq msg = JsonConvert.DeserializeObject<MsgQuestInfoReq>(json);
                        ackJson = await QuestInfoReq(msg);
                    }
                    break;
                case GameProtocol.QUEST_REWARD_REQ:
                    {
                        if (QuestRewardReq == null)
                            return ackJson;

                        MsgQuestRewardReq msg = JsonConvert.DeserializeObject<MsgQuestRewardReq>(json);
                        ackJson = await QuestRewardReq(msg);
                    }
                    break;
            }

            return ackJson;
        }


        public bool Process(int protocolId, string json)
        {
            switch ((GameProtocol)protocolId)
            {
                case GameProtocol.AUTH_USER_ACK:
                    {
                        if (AuthUserAck == null)
                            return false;

                        MsgUserAuthAck msg = JsonConvert.DeserializeObject<MsgUserAuthAck>(json);
                        AuthUserAck(msg);
                    }
                    break;
                case GameProtocol.EDIT_USER_NAME_ACK:
                    {
                        if (EditUserNameAck == null)
                            return false;

                        MsgEditUserNameAck msg = JsonConvert.DeserializeObject<MsgEditUserNameAck>(json);
                        EditUserNameAck(msg);
                    }
                    break;
                case GameProtocol.UPDATE_DECK_ACK:
                    {
                        if (UpdateDeckAck == null)
                            return false;

                        MsgUpdateDeckAck msg = JsonConvert.DeserializeObject<MsgUpdateDeckAck>(json);
                        UpdateDeckAck(msg);
                    }
                    break;
                case GameProtocol.START_MATCH_ACK:
                    {
                        if (StartMatchAck == null)
                            return false;

                        MsgStartMatchAck msg = JsonConvert.DeserializeObject<MsgStartMatchAck>(json);
                        StartMatchAck(msg);
                    }
                    break;
                case GameProtocol.STATUS_MATCH_ACK:
                    {
                        if (StatusMatchAck == null)
                            return false;

                        MsgStatusMatchAck msg = JsonConvert.DeserializeObject<MsgStatusMatchAck>(json);
                        StatusMatchAck(msg);
                    }
                    break;
                case GameProtocol.STOP_MATCH_ACK:
                    {
                        if (StopMatchAck == null)
                            return false;

                        MsgStopMatchAck msg = JsonConvert.DeserializeObject<MsgStopMatchAck>(json);
                        StopMatchAck(msg);
                    }
                    break;
                case GameProtocol.OPEN_BOX_ACK:
                    {
                        if (OpenBoxAck == null)
                            return false;

                        MsgOpenBoxAck msg = JsonConvert.DeserializeObject<MsgOpenBoxAck>(json);
                        OpenBoxAck(msg);
                    }
                    break;
                case GameProtocol.LEVELUP_DICE_ACK:
                    {
                        if (LevelUpDiceAck == null)
                            return false;

                        MsgLevelUpDiceAck msg = JsonConvert.DeserializeObject<MsgLevelUpDiceAck>(json);
                        LevelUpDiceAck(msg);
                    }
                    break;
                case GameProtocol.SEASON_INFO_ACK:
                    {
                        if (SeasonInfoAck == null)
                            return false;

                        MsgSeasonInfoAck msg = JsonConvert.DeserializeObject<MsgSeasonInfoAck>(json);
                        SeasonInfoAck(msg);
                    }
                    break;
                case GameProtocol.GET_RANK_ACK:
                    {
                        if (GetRankAck == null)
                            return false;

                        MsgGetRankAck msg = JsonConvert.DeserializeObject<MsgGetRankAck>(json);
                        GetRankAck(msg);
                    }
                    break;
                case GameProtocol.SEASON_PASS_INFO_ACK:
                    {
                        if (SeasonPassInfoAck == null)
                            return false;

                        MsgSeasonPassInfoAck msg = JsonConvert.DeserializeObject<MsgSeasonPassInfoAck>(json);
                        SeasonPassInfoAck(msg);
                    }
                    break;
                case GameProtocol.GET_SEASON_PASS_REWARD_ACK:
                    {
                        if (GetSeasonPassRewardAck == null)
                            return false;

                        MsgGetSeasonPassRewardAck msg = JsonConvert.DeserializeObject<MsgGetSeasonPassRewardAck>(json);
                        GetSeasonPassRewardAck(msg);
                    }
                    break;
                case GameProtocol.CLASS_REWARD_INFO_ACK:
                    {
                        if (ClassRewardInfoAck == null)
                            return false;

                        MsgClassRewardInfoAck msg = JsonConvert.DeserializeObject<MsgClassRewardInfoAck>(json);
                        ClassRewardInfoAck(msg);
                    }
                    break;
                case GameProtocol.GET_CLASS_REWARD_ACK:
                    {
                        if (GetClassRewardAck == null)
                            return false;

                        MsgGetClassRewardAck msg = JsonConvert.DeserializeObject<MsgGetClassRewardAck>(json);
                        GetClassRewardAck(msg);
                    }
                    break;
                case GameProtocol.QUEST_INFO_ACK:
                    {
                        if (QuestInfoAck == null)
                            return false;

                        MsgQuestInfoAck msg = JsonConvert.DeserializeObject<MsgQuestInfoAck>(json);
                        QuestInfoAck(msg);
                    }
                    break;
                case GameProtocol.QUEST_REWARD_ACK:
                    {
                        if (QuestRewardAck == null)
                            return false;

                        MsgQuestRewardAck msg = JsonConvert.DeserializeObject<MsgQuestRewardAck>(json);
                        QuestRewardAck(msg);
                    }
                    break;
            }

            return true;
        }
    }
}
