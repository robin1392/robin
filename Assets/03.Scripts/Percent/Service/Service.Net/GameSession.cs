using System;
using System.Collections.Generic;
using Service.Core;

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


    public class GameSessionConfig
    {
        public string Id { get; set;}
        public int MessageQueueCapacity { get; set;}
        public int MessageBufferSize { get; set;}
    }


    public class GameSession
    {
        public string GameSessionId { get; private set; }
        MessageController _messageController;


        public GameSession()
        {
            _messageController = new MessageController();
        }


        public virtual void Init(GameSessionConfig config)
        {
            GameSessionId = config.Id;
        }


        public virtual void Update()
        {

        }


        public void AddControllers(Dictionary<int, ControllerDelegate> controllers)
        {
            _messageController.AddControllers(controllers);
        }


        public virtual void PushInternalMessage(ClientSession session, EInternalProtocol protocolId, byte[] msg, int length)
        {

        }

        public virtual void PushExternalMessage(ClientSession session, int protocolId, byte[] msg, int length)
        {

        }

        public virtual bool PushRelayMessage(ClientSession session, int protocolId, byte[] msg, int length)
        {
            return false;
        }

        public virtual bool ProcessInternalMessage(Message msg)
        {
            return false;
        }

        public virtual bool ProcessExternalMessage(Message msg)
        {
            if (_messageController == null)
            {
                return false;
            }

            _messageController.OnRecevice(msg.Session, msg.ProtocolId, msg.Data, msg.Length);
            return true;
        }

        protected virtual void OnConnectClient(UserToken userToken) {}

        protected virtual void OnReconnectClient(UserToken userToken) {}

        protected virtual void OnOfflineClient(UserToken userToken) {}

        protected virtual void OnDisconnectClient(UserToken userToken) {}

        protected virtual void OnExpiredClient(UserToken userToken) {}

        protected virtual void OnPauseClient(UserToken userToken) {}

        protected virtual void OnResumeClient(UserToken userToken) {}

        protected virtual void OnTerminatedGameSession(string gameSessionId) {}
    }
}