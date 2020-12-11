using System;
using System.Collections.Generic;

namespace Service.Net 
{
    public enum EInternalProtocol
    {
        // 신규 연결
        CONNECT_CLIENT,
        // 재연결
        RECONNECT_CLIENT,
        // 오프라인(소켓 닫힘)
        OFFLINE_CLIENT,
        // 접속해제(어플리케이션 요청)
        DISCONNECT_CLIENT,
        // 만료
        EXPIRED_CLIENT,
        // 일시 정지
        PAUSE_CLIENT,
        // 재개
        RESUME_CLIENT,
    } 


    public enum EPeerGroupType
    {
        ALL,
        OTHERS,
    }

    public class GameSessionConfig
    {
        public string Id { get; set;}
        public string ServerAddr { get; set;}
        public int Port { get; set;}
        public int MessageQueueCapacity { get; set;}
        public int MessageBufferSize { get; set;}
        public MessageController MsgController { get; set;}
    }


    public class GameSession
    {
        public string Id { get; private set; }
        public string ServerAddr { get; private set;}
        public int Port { get; private set;}

        protected List<Peer> _peers;


        public GameSession()
        {
            _peers = new List<Peer>();
        }


        public virtual void Init(GameSessionConfig config)
        {
            Id = config.Id;
            ServerAddr = config.ServerAddr;
            Port = config.Port;
            _peers.Clear();
        }


        public virtual void Update()
        {

        }

        public virtual Peer[] GetPeers(EPeerGroupType groupType, Peer peer) { return null; }


        public virtual void PushInternalMessage(object sender, EInternalProtocol protocolId, byte[] msg, int length)
        {

        }

        public virtual void PushExternalMessage(object sender, int protocolId, byte[] msg, int length)
        {

        }

        public virtual bool PushRelayMessage(object sender, int protocolId, byte[] msg, int length)
        {
            return false;
        }

        public virtual bool ProcessInternalMessage(Message msg)
        {
            return false;
        }

        public virtual bool ProcessExternalMessage(Message msg)
        {
            return false;
        }

        protected virtual void OnConnectClient(ClientSession clientSession) {}

        protected virtual void OnReconnectClient(ClientSession clientSession) {}

        protected virtual void OnOfflineClient(ClientSession clientSession) {}

        protected virtual void OnDisconnectClient(ClientSession clientSession) {}

        protected virtual void OnExpiredClient(ClientSession clientSession) {}

        protected virtual void OnPauseClient(ClientSession clientSession) {}

        protected virtual void OnResumeClient(ClientSession clientSession) {}

        protected virtual void OnTerminatedGameSession(string gameSessionId) {}
    }
}