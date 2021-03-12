using System;
using System.Collections.Generic;
using System.IO;
using Service.Core;

namespace Template.User.RandomwarsUser.Common
{
    public class MsgUserInfo
    {
        public string UserId;
        public string UserName = string.Empty;
        public short Class;
        public byte WinStreak;
        public int Gold;
        public int Diamond;
        public int Key;
        public int Trophy;
        public int[] TrophyRewardIds;
        public bool IsBuyVipPass;
        public int WinCount;
        public int DefeatCount;
        public int HighTrophy;
        public bool EndTutorial;
    }


    // 유저 아이템 정보
    public class UserItemInfo
    {
        public List<ItemBaseInfo> listBox;
        public List<ItemBaseInfo> listPass;
        public List<ItemBaseInfo> listEmoticon;
        public List<int> listEmoticonSlot;
    }
}