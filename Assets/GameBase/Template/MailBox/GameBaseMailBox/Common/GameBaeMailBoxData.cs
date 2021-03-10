using System;
using System.Collections.Generic;
using Service.Core;

namespace Template.MailBox.GameBaseMailBox.Common
{
    // 우편 정보
    [Serializable]
    public class MailInfo
    {
        // 아이디 (식별값)
        public string mailId;
        // 테이블 아이디
        public int mailTableId;
        // 상품 목록
        public ItemBaseInfo[] mailItems;
        // 만료까지 남은 시간
        public int expireRemainTime;
        // 추가 텍스트 목록
        public List<string> listText;
    }
}
