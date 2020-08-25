using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RWCoreNet;
using RWCoreNet.NetPacket;
using RWSocketProtocol.Protocol;


public class SocketRecvPacket : IPacketProcessor
{
    public bool DoWork(IPeer session, short protocolId, byte[] data)
    {
        switch ((GameProtocol) protocolId)
        {
            case GameProtocol.MSG_BEGIN:
            {
                break;
            }
        }

        return true;
    }
}
