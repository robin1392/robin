using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RandomWarsProtocol;


public interface ISyncData
{
    byte[] Serialize();
    void Deserialize(byte[] buffer);
}


class NetClientSyncData : ISyncData
{
    public ushort PlayerUId;
    public int packetCount;
    public MsgMinionInfo[] MinionInfo;
    public MsgMinionStatus Relay;


    public byte[] Serialize()
    {
        return null;
    }


    public void Deserialize(byte[] buffer)
    {

    }
}
