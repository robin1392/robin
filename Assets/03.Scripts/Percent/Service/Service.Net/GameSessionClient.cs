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


        public override void PushInternalMessage(ClientSession session, EInternalProtocol protocolId, byte[] msg, int length)
        {
            _messageQueue.Enqueue(session, (int)protocolId, msg, length);
        }


        public override void PushExternalMessage(ClientSession session, int protocolId, byte[] msg, int length)
        {
            _messageQueue.Enqueue(session, protocolId, msg, length);
        }


        public override bool PushRelayMessage(ClientSession session, int protocolId, byte[] msg, int length)
        {
            return false;
        }


        public override bool ProcessInternalMessage(Message msg)
        {
            var peer = msg.Session as Peer;
            switch ((EInternalProtocol)msg.ProtocolId)
            {
                case EInternalProtocol.CONNECT_CLIENT:
                {
                    OnConnectClient(peer.UserToken);
                    break;
                }
                case EInternalProtocol.RECONNECT_CLIENT:
                {
                    OnReconnectClient(peer.UserToken);
                    break;  
                }
                case EInternalProtocol.OFFLINE_CLIENT:
                {
                    OnOfflineClient(peer.UserToken);
                    break;  
                }
                case EInternalProtocol.DISCONNECT_CLIENT:
                {
                    OnDisconnectClient(peer.UserToken);
                    break;  
                }
                case EInternalProtocol.EXPIRED_CLIENT:
                {
                    OnExpiredClient(peer.UserToken);
                    break;  
                }
                case EInternalProtocol.PAUSE_CLIENT:
                {
                    OnPauseClient(peer.UserToken);
                    break;  
                }
                case EInternalProtocol.RESUME_CLIENT:
                {
                    OnResumeClient(peer.UserToken);
                    break;  
                }
                default:
                {
                    return false;             
                }
            }

            return true;
        }
    }
}