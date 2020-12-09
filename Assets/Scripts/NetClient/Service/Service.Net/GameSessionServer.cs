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


        public override void PushInternalMessage(ClientSession clientSession, EInternalProtocol protocolId, byte[] data, int length)
        {
            _messageQueue.Enqueue(clientSession, (int)protocolId, data, length);
        }


        public override void PushExternalMessage(ClientSession clientSession, int protocolId, byte[] data, int length)
        {
            _messageQueue.Enqueue(clientSession, protocolId, data, length);
        }


        public override bool PushRelayMessage(ClientSession clientSession, int protocolId, byte[] data, int length)
        {
            return false;
        }


        public override bool ProcessInternalMessage(Message msg)
        {
            switch ((EInternalProtocol)msg.ProtocolId)
            {
                case EInternalProtocol.CONNECT_CLIENT:
                {
                    _peers.Add(msg.ClientSession.Peer);

                    OnConnectClient(msg.ClientSession);
                    break;
                }
                case EInternalProtocol.RECONNECT_CLIENT:
                {
                    OnReconnectClient(msg.ClientSession);
                    break;  
                }
                case EInternalProtocol.OFFLINE_CLIENT:
                {
                    OnOfflineClient(msg.ClientSession);
                    break;  
                }
                case EInternalProtocol.DISCONNECT_CLIENT:
                {
                    OnDisconnectClient(msg.ClientSession);
                    break;  
                }
                case EInternalProtocol.EXPIRED_CLIENT:
                {
                    OnExpiredClient(msg.ClientSession);

                    // 모든 클라이언트 세션이 만료되면 게임 세션을 종료시킨다.
                    _peers.Remove(msg.ClientSession.Peer);
                    if (_peers.Count == 0)
                    {
                        OnTerminatedGameSession(Id);
                    }
                    break;  
                }
                case EInternalProtocol.PAUSE_CLIENT:
                {
                    OnPauseClient(msg.ClientSession);
                    break;  
                }
                case EInternalProtocol.RESUME_CLIENT:
                {
                    OnResumeClient(msg.ClientSession);
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

            return _msgController.OnRecevice(msg.ClientSession.Peer, msg.ProtocolId, msg.Data);
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