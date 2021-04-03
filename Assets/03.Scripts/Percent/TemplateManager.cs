using System;
using Service.Net;
using Service.Core;
using Template.Shop.GameBaseShop.Common;
using Template.Account.GameBaseAccount.Common;
using Template.User.RandomwarsUser.Common;
using Template.Quest.GameBaseQuest.Common;
using Template.Character.RandomwarsDice.Common;
using Template.Item.RandomwarsItem.Common;
using Template.Season.GameBaseSeason.Common;
using Template.MailBox.GameBaseMailBox.Common;
using Template.Match.RandomwarsMatch.Common;
using Percent.Platform;


namespace Percent
{
    public class TemplateManager : ManagerSingleton<TemplateManager>
    {
        private GameSessionClient _clientSession;

        public GameBaseAccountProtocol Account { get; private set; }
        public RandomwarsUserProtocol User { get; private set; }
        public GameBaseQuestProtocol Quest { get; private set; }
        public RandomwarsDiceProtocol Dice { get; private set; }
        public RandomwarsItemProtocol Item { get; private set; }
        public GameBaseShopProtocol Shop { get; private set; }
        public GameBaseSeasonProtocol Season { get; private set; }
        public GameBaseMailBoxProtocol MailBox { get; private set; }
        public RandomwarsMatchProtocol Match { get; private set; }


        private void Start()
        {
            Logger.Init(new GameBaseLogger());

            // HTTP 클라이언트 생성
            HttpClient httpClient = new HttpClient(_clientSession);
            string serverAddr = NetworkManager.Get().serverAddr;

            // 클라이언트 세션 생성
            _clientSession = new GameSessionClient();
            _clientSession.Init(new GameSessionConfig 
            {
                MessageBufferSize = 10240,
                MessageQueueCapacity = 100,
            });

            // 템플릿 프로토콜 세션에 등록
            Account = new GameBaseAccountProtocol(httpClient, serverAddr);
            _clientSession.AddProtocol(Account);

            User = new RandomwarsUserProtocol();
            _clientSession.AddProtocol(User);

            Quest = new GameBaseQuestProtocol();
            _clientSession.AddProtocol(Quest);

            Shop = new GameBaseShopProtocol();
            _clientSession.AddProtocol(Shop);

            Dice = new RandomwarsDiceProtocol();
            _clientSession.AddProtocol(Dice);

            Item = new RandomwarsItemProtocol();
            _clientSession.AddProtocol(Item);

            Season = new GameBaseSeasonProtocol();
            _clientSession.AddProtocol(Season);

            MailBox = new GameBaseMailBoxProtocol();
            _clientSession.AddProtocol(MailBox);

            Match = new RandomwarsMatchProtocol();
            _clientSession.AddProtocol(Match);
        }


        public void Destroy()
        {
        }


        private void Update()
        {
            if (_clientSession != null)
            {
                _clientSession.Update();
            }
        }
    }
}
