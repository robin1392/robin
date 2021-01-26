using System.Collections.Generic;
using Service.Net;

namespace Service.Template
{
    public class GamePlayer : Peer
    {
        // 플레이어 식별 아이디(DB primarykey)
        public string PlayerGuid { get; set; }

        public List<IGameRule> GameRules { get; set; }
    }
}