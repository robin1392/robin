using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Service.Net
{
    public class GameSessionClient : GameSession
    {
        MessageQueue _messageQueue;
        MessageController _messageController;

        public GameSessionClient()
        {
            _messageQueue = new MessageQueue();
        }


        public override void Init(GameSessionConfig config)
        {
            base.Init(config);

            _messageQueue.Init(config.MessageQueueCapacity, config.MessageBufferSize);
            _messageController = config.MsgController;
        }


        public override void Update()
        {
            for (int i = 0; i < 100; i++)
            {
                Message msg = _messageQueue.Peek();
                if (msg == null)
                {
                    return;
                }


                if (ProcessInternalMessage(msg) == false)
                {
                    ProcessExternalMessage(msg);
                }

                _messageQueue.Dequeue();
            }
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
            if (_messageController == null)
            {
                return false;
            }

            return _messageController.OnRecevice(msg.ClientSession.Peer, msg.ProtocolId, msg.Data);
        }
    }
}