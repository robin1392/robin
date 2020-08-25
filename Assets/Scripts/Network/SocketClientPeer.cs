using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RWCoreNet;



/// <summary>
/// 게임 서버 클라이언트 peer
/// </summary>
public class SocketClientPeer : IPeer
{
    public ClientSession Session { get; private set; }

    public SocketClientPeer(ClientSession session)
    {
        Session = session;
        Session.SetPeer(this);
    }

    public void SendPacket(short protocolId, byte[] msg)
    {
        Session.Send(protocolId, msg);
    }

    public void OnRemoved()
    {

    }

    public void Disconnect()
    {
        Session.Disconnect();
    }

    public bool ReceivePacket(short protocolId, byte[] msg)
    {
        /// 미사용
        return true;
    }
    
}
