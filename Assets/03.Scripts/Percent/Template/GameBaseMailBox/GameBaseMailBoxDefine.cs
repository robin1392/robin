using System;
namespace Template.MailBox.GameBaseMailBox.Common
{
    public enum EGameBaseMailBoxErrorCode
    {
        Success = 0,
        Fatal,

        ErrorNotFoundMailBoxInfo,
        ErrorNotFoundMailInfo,
        ErrorMailBoxLimitExceeded,
    }
}
