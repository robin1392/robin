using System;
using System.Collections.Generic;

namespace Service.Net
{
    public class GameSessionServer : GameSession
    {
        MessageQueueThread _messageQueue;

        MessageController _msgController;


        public GameSessionServer()
        {
            _messageQueue = new MessageQueueThread();
        }


        public override void Init(GameSessionConfig config)
        {
            base.Init(config);

            _messageQueue.Init(config.MessageQueueCapacity, config.MessageBufferSize, OnProcessMessage);
            _msgController = config.MsgController;
        }


        public override void PushInternalMessage(object sender, EInternalProtocol protocolId, byte[] msg, int length)
        {
            _messageQueue.Enqueue(sender, (int)protocolId, msg, length);
        }


        public override void PushExternalMessage(object sender, int protocolId, byte[] msg, int length)
        {
            _messageQueue.Enqueue(sender, protocolId, msg, length);
        }


        public override bool PushRelayMessage(object sender, int protocolId, byte[] msg, int length)
        {
            return false;
        }


        public override bool ProcessInternalMessage(Message msg)
        {
            ClientSession clientSession = msg.Sender as ClientSession;
            switch ((EInternalProtocol)msg.ProtocolId)
            {
                case EInternalProtocol.CONNECT_CLIENT:
                {
                    OnConnectClient(clientSession);
                    break;
                }
                case EInternalProtocol.RECONNECT_CLIENT:
                {
                    OnReconnectClient(clientSession);
                    break;  
                }
                case EInternalProtocol.OFFLINE_CLIENT:
                {
                    OnOfflineClient(clientSession);
                    break;  
                }
                case EInternalProtocol.DISCONNECT_CLIENT:
                {
                    OnDisconnectClient(clientSession);
                    break;  
                }
                case EInternalProtocol.EXPIRED_CLIENT:
                {
                    OnExpiredClient(clientSession);

                    // 모든 클라이언트 세션이 만료되면 게임 세션을 종료시킨다.
                    _peers.Remove(clientSession.Peer);
                    if (_peers.Count == 0)
                    {
                        OnTerminatedGameSession(Id);
                    }
                    break;  
                }
                case EInternalProtocol.PAUSE_CLIENT:
                {
                    OnPauseClient(clientSession);
                    break;  
                }
                case EInternalProtocol.RESUME_CLIENT:
                {
                    OnResumeClient(clientSession);
                    break;  
                }
                default:
                {
                    return false;             
                }
                    
            }

            return true;
        }


        public override bool ProcessExternalMessage(Message msg)
        {
            if (_msgController == null)
            {
                return false;
            }

            return _msgController.OnRecevice(msg.Sender as ISender, msg.ProtocolId, msg.Data, msg.Length);
        }


        public bool OnProcessMessage(Message msg) 
        { 
            if (ProcessInternalMessage(msg) == false)
            {
                ProcessExternalMessage(msg);
            }

            return true; 
        }
    }
}