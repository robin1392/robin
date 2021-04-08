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
    public class GameBase : Singleton<GameBase>
    {
        private GameSessionClient _gameSession;

        public string ServerAddr;


        public GameBaseAccountProtocol Account { get; private set; }
        public RandomwarsUserProtocol User { get; private set; }
        public GameBaseQuestProtocol Quest { get; private set; }
        public RandomwarsDiceProtocol Dice { get; private set; }
        public RandomwarsItemProtocol Item { get; private set; }
        public GameBaseShopProtocol Shop { get; private set; }
        public GameBaseSeasonProtocol Season { get; private set; }
        public GameBaseMailBoxProtocol MailBox { get; private set; }
        public RandomwarsMatchProtocol Match { get; private set; }
        


        void Start()
        {
            Service.Core.Logger.Init(new GameBaseLogger());

            // 게임 세션 생성
            _gameSession = new GameSessionClient();
            _gameSession.Init(new GameSessionConfig 
            {
                MessageBufferSize = 10240,
                MessageQueueCapacity = 100,
            });


            // HTTP 클라이언트 생성
            HttpClient httpClient = new HttpClient();

            // 템플릿 프로토콜 객체 생성
            Account = new GameBaseAccountProtocol(httpClient, serverAddr);
            Season = new GameBaseSeasonProtocol(httpClient, ServerAddr);
            User = new RandomwarsUserProtocol();
            Quest = new GameBaseQuestProtocol();
            Shop = new GameBaseShopProtocol();
            Dice = new RandomwarsDiceProtocol();
            Item = new RandomwarsItemProtocol();
            MailBox = new GameBaseMailBoxProtocol();
            Match = new RandomwarsMatchProtocol();

            // 게임 세션에 템플릿 컨트롤러 추가
            _gameSession.AddControllers(Account.MessageControllers);
            _gameSession.AddControllers(Season.MessageControllers);
            _gameSession.AddControllers(User.MessageControllers);
            _gameSession.AddControllers(Quest.MessageControllers);
            _gameSession.AddControllers(Shop.MessageControllers);
            _gameSession.AddControllers(Dice.MessageControllers);
            _gameSession.AddControllers(Item.MessageControllers);
            _gameSession.AddControllers(MailBox.MessageControllers);
            _gameSession.AddControllers(Match.MessageControllers);

            httpClient.Init(_gameSession);
        }


        void Update()
        {
            if (_gameSession != null)
            {
                _gameSession.Update();
            }
        }
    }

}


// namespace Percent
// {
    

//     // public class TemplateManager : ManagerSingleton<TemplateManager>
//     // {
//     //     public GameBaseAccountProtocol Account { get; private set; }
//     //     public RandomwarsUserProtocol User { get; private set; }
//     //     public GameBaseQuestProtocol Quest { get; private set; }
//     //     public RandomwarsDiceProtocol Dice { get; private set; }
//     //     public RandomwarsItemProtocol Item { get; private set; }
//     //     public GameBaseShopProtocol Shop { get; private set; }
//     //     public GameBaseSeasonProtocol Season { get; private set; }
//     //     public GameBaseMailBoxProtocol MailBox { get; private set; }
//     //     public RandomwarsMatchProtocol Match { get; private set; }


//     //     private void Start()
//     //     {
//     //         Logger.Init(new GameBaseLogger());

//     //         // 클라이언트 세션 생성
//     //         _clientSession = new GameSessionClient();
//     //         _clientSession.Init(new GameSessionConfig 
//     //         {
//     //             MessageBufferSize = 10240,
//     //             MessageQueueCapacity = 100,
//     //         });


//     //         // HTTP 클라이언트 생성
//     //         HttpClient httpClient = new HttpClient();
//     //         string serverAddr = NetworkManager.Get().serverAddr;


//     //         // 템플릿 프로토콜 세션에 등록
//     //         Account = new GameBaseAccountProtocol(httpClient, serverAddr);
//     //         _clientSession.AddControllers(Account.MessageControllers);

//     //         Gamebase.Season = new GameBaseSeasonProtocol(httpClient, serverAddr);
//     //         _clientSession.AddControllers(Gamebase.Season.MessageControllers);

//     //         // User = new RandomwarsUserProtocol();
//     //         // _clientSession.AddProtocol(User);

//     //         // Quest = new GameBaseQuestProtocol();
//     //         // _clientSession.AddProtocol(Quest);

//     //         // Shop = new GameBaseShopProtocol();
//     //         // _clientSession.AddProtocol(Shop);

//     //         // Dice = new RandomwarsDiceProtocol();
//     //         // _clientSession.AddProtocol(Dice);

//     //         // Item = new RandomwarsItemProtocol();
//     //         // _clientSession.AddProtocol(Item);

//     //         // Season = new GameBaseSeasonProtocol();
//     //         // _clientSession.AddProtocol(Season);

//     //         // MailBox = new GameBaseMailBoxProtocol();
//     //         // _clientSession.AddProtocol(MailBox);

//     //         // Match = new RandomwarsMatchProtocol();
//     //         // _clientSession.AddProtocol(Match);


//     //         httpClient.Init(_clientSession);
//     //     }


//     //     public void Destroy()
//     //     {
//     //     }


//     //     private void Update()
//     //     {
//     //         if (_clientSession != null)
//     //         {
//     //             _clientSession.Update();
//     //         }
//     //     }
//     // }
// }
