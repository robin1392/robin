using System;
using System.Collections.Generic;

namespace Service.Net
{
    
    public enum EBroadCastType
    {
        ALL,
        OTHERS,
        TO_MYSELF,
    }


    public class GameSessionServer : GameSession
    {
        MessageQueueThread _messageQueue;

        List<Peer> _peers;


        public GameSessionServer()
        {
            _messageQueue = new MessageQueueThread();
            _peers = new List<Peer>();
        }


        public override void Init(GameSessionConfig config)
        {
            base.Init(config);

            _messageQueue.Init(config.MessageQueueCapacity, config.MessageBufferSize, OnProcessMessage);
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
                    _peers.Add(clientSession.Peer);
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
                        OnTerminatedGameSession(GameSessionId);
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
            if (_messageController == null)
            {
                return false;
            }

            return _messageController.OnRecevice(msg.Sender as ISender, msg.ProtocolId, msg.Data, msg.Length);
        }


        public bool OnProcessMessage(Message msg) 
        { 
            if (ProcessInternalMessage(msg) == false)
            {
                ProcessExternalMessage(msg);
            }

            return true; 
        }


        
        public Peer[] GetPeers(EBroadCastType type, ISender sender)
        {
            List<Peer> peerList = new List<Peer>();
            foreach (var peer in _peers)
            {
                // sender 자신을 제외한다.
                if (type == EBroadCastType.OTHERS && peer == sender)
                {
                    continue;
                }

                // sender 자신이 아니면 제외한다.
                if (type == EBroadCastType.TO_MYSELF && peer != sender)
                {
                    continue;
                }

                peerList.Add(peer);
            }

            return peerList.ToArray();
        }

    }
}