using System;
using Service.Net;
using Service.Core;
using Template.Shop.GameBaseShop;
using Template.Account.GameBaseAccount.Common;
using Template.User.RandomwarsUser.Common;
using Template.Quest.RandomwarsQuest.Common;
using Template.Character.RandomwarsDice.Common;
using Template.Item.RandomwarsBox.Common;
using Template.Season.RandomwarsSeason.Common;


namespace Percent.GameBaseClient
{
    public class GameBaseClientSession : GameSessionClient
    {
        // 소켓 클라이언트 객체
        public NetServiceClient SocketClient { get; set; }
        // HTTP 클라이언트 객체
        public HttpClient HttpClient { get; set; }
        // 플레이어 객체
        public GameBasePlayer Player { get; set; }


        // ------------------------------------------------------------
        // 템플릿
        public GameBaseShopTemplate ShopTemplate { get; set; }
        public GameBaseAccountProtocol AccountTemplate { get; set; }
        public RandomwarsUserProtocol UserTemplate { get; set; }
        public RandomwarsQuestProtocol QuestTemplate { get; set; }
        public RandomwarsDiceProtocol DiceTemplate { get; set; }
        public RandomwarsBoxProtocol BoxTemplate { get; set; }
        public RandomwarsSeasonProtocol SeasonTemplate { get; set; }


        public GameBaseClientSession()
        {
            Player = new GameBasePlayer();
            ShopTemplate = new GameBaseShopTemplate();
            AccountTemplate = new GameBaseAccountProtocol();
            UserTemplate = new RandomwarsUserProtocol();
            QuestTemplate = new RandomwarsQuestProtocol();
            DiceTemplate = new RandomwarsDiceProtocol();
            BoxTemplate = new RandomwarsBoxProtocol();
            SeasonTemplate = new RandomwarsSeasonProtocol();
        }


        public void Init(GameBaseClientConfig config)
        {
            Logger.Init(config.Logger);

            base.Init(new GameSessionConfig 
            {
                MessageBufferSize = config.BufferSize,
                MessageQueueCapacity = config.MessageQueueCapacity,
            });

            // 소켓 클라이언트 생성
            SocketClient = new NetServiceClient();
            SocketClient.Init(new NetServiceConfig
            {
                MaxConnectionNum = config.MaxConnectionNum,
                BufferSize = config.BufferSize,
                KeepAliveTime = config.KeepAliveTime,
                KeepAliveInterval = config.KeepAliveInterval,
            });
            SocketClient.SetGameSession(this);


            // HTTP 클라이언트 생성
            HttpClient = new HttpClient(
                "https://er12bk2rue.execute-api.ap-northeast-2.amazonaws.com/test", 
                this);


            ShopTemplate.Init();

            _messageController.AddControllers(ShopTemplate.MessageControllers);
            _messageController.AddControllers(AccountTemplate.MessageControllers);
            _messageController.AddControllers(UserTemplate.MessageControllers);
            _messageController.AddControllers(QuestTemplate.MessageControllers);
            _messageController.AddControllers(DiceTemplate.MessageControllers);
            _messageController.AddControllers(BoxTemplate.MessageControllers);
            _messageController.AddControllers(SeasonTemplate.MessageControllers);
        }


        public void Destroy()
        {
        }


        public override void Update()
        {
            base.Update();

            if (SocketClient != null)
            {
                SocketClient.Update();
            }
        }


        protected override void OnConnectClient(ClientSession clientSession)
        {
            Player.SetClientSession(clientSession);
        }

        protected override void OnReconnectClient(ClientSession clientSession)
        {
        }

        protected override void OnOfflineClient(ClientSession clientSession)
        {
        }

        protected override void OnDisconnectClient(ClientSession clientSession)
        {
        }
    }
}
