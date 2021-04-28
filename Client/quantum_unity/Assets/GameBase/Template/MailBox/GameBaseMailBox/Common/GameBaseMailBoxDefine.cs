using System;
namespace Template.MailBox.GameBaseMailBox.Common
{
    public enum GameBaseMailBoxErrorCode
    {
        Success = 0,
        Fatal,

        ErrorNotFoundMailBoxInfo,
        ErrorNotFoundMailInfo,
        ErrorMailBoxLimitExceeded,
    }
}
