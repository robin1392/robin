using System;
using Service.Net;
using Template.Shop.GameBaseShop;
using Template.Shop.GameBaseShop.Common;

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



        public GameBaseClientSession()
        {
            Player = new GameBasePlayer();

            ShopTemplate = new GameBaseShopTemplate();
        }


        public void Init(GameBaseClientConfig config)
        {
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
                "https://vj7nnp92xd.execute-api.ap-northeast-2.amazonaws.com/prod", 
                this);



            //ShopTemplate.Init();
            //_messageController.AddControllers(ShopTemplate.MessageControllers);
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
