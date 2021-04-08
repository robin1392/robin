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
                    _peers.Add(peer);
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

                    // 모든 클라이언트 세션이 만료되면 게임 세션을 종료시킨다.
                    _peers.Remove(peer);
                    if (_peers.Count == 0)
                    {
                        OnTerminatedGameSession(GameSessionId);
                    }
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


        public bool OnProcessMessage(Message msg) 
        { 
            if (ProcessInternalMessage(msg) == false)
            {
                ProcessExternalMessage(msg);
            }

            return true; 
        }
        
        public Peer[] GetPeers(EBroadCastType type, ClientSession session)
        {
            List<Peer> peerList = new List<Peer>();
            foreach (var peer in _peers)
            {
                // sender 자신을 제외한다.
                if (type == EBroadCastType.OTHERS && peer == session)
                {
                    continue;
                }

                // sender 자신이 아니면 제외한다.
                if (type == EBroadCastType.TO_MYSELF && peer != session)
                {
                    continue;
                }

                peerList.Add(peer);
            }

            return peerList.ToArray();
        }

    }
}