using System;
using System.Collections.Generic;

namespace Template.MailBox.GameBaseMailBox.Common
{
    // 우편 정보
    [Serializable]
    public class MailInfo
    {
        // 아이디 (식별값)
        public string mailId;
        // 제목
        public Dictionary<string, string> mailTitles;
        // 내용
        public Dictionary<string, string> mailContents;
        // 발송자
        public string mailFrom;
        // 상품 목록
        public MailItemInfo[] mailItems;
        // 만료까지 남은 시간
        public int expireRemainTime;
    }

    // 우편 상품 정보
    [Serializable]
    public class MailItemInfo
    {
        public int itemType;
        public int itemId;
        public int itemAmount;
    }
}
