using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace Service.Net
{
    public class GameSessionClient : GameSession
    {
        MessageQueue _messageQueue;

        public GameSessionClient()
        {
            _messageQueue = new MessageQueue();
        }


        public override void Init(GameSessionConfig config)
        {
            base.Init(config);

            _messageQueue.Init(config.MessageQueueCapacity, config.MessageBufferSize);
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
            if (_messageController == null)
            {
                return false;
            }

            ISender sender = msg.Sender as ISender;
            return _messageController.OnRecevice(sender, msg.ProtocolId, msg.Data, msg.Length);
        }
    }
}