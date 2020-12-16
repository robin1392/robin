using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Account.RandomWarsAccount.Common;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Template.Account.RandomWarsAccount
{
    public class RandomWarsAccountTemplate : RandomWarsAccountProtocol
    {
        public RandomWarsAccountTemplate()
        {
            ReceiveLoginAccountAckCallback = OnReceiveLoginAccountAck;
        }


        bool OnReceiveLoginAccountAck(ERandomWarsAccountErrorCode errorCode, MsgAccount accountInfo)
        {
            if (errorCode == ERandomWarsAccountErrorCode.NOT_FOUND_ACCOUNT)
            {
                ObscuredPrefs.SetString("UserKey", string.Empty);
                ObscuredPrefs.Save();

                UI_Start.Get().SetTextStatus(string.Empty);
                UI_Start.Get().btn_GuestAccount.gameObject.SetActive(true);
                UI_Start.Get().btn_GuestAccount.onClick.AddListener(() =>
                {
                    UI_Start.Get().btn_GuestAccount.gameObject.SetActive(false);
                    UI_Start.Get().SetTextStatus(Global.g_startStatusUserData);

                    //AuthUserReq(string.Empty);
                });
                return true;
            }

            UserInfoManager.Get().SetUserInfo(accountInfo.PlayerInfo);
            UserInfoManager.Get().SetDeck(accountInfo.DeckInfo);
            UserInfoManager.Get().SetDice(accountInfo.DiceInfo);
            UserInfoManager.Get().SetBox(accountInfo.BoxInfo);

            GameStateManager.Get().UserAuthOK();

            UnityUtil.Print("RECV AUTH => PlayerGuid", accountInfo.PlayerInfo.PlayerGuid, "green");
            return true;
        }
    }
}
