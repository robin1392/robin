using System;
using System.IO;
using Service.Template.Common;

namespace Template.Item.RandomWarsDice.Common
{
    [Serializable]
    public class MsgOpenBoxReward
    {
        public REWARD_TYPE RewardType;
        public int Id;
        public int Value;


        public void Write(BinaryWriter bw)
        {
            bw.Write((byte)RewardType);
            bw.Write(Id);
            bw.Write(Value);
        }

        public static MsgOpenBoxReward Read(BinaryReader br)
        {
            MsgOpenBoxReward data = new MsgOpenBoxReward();
            data.RewardType = (REWARD_TYPE)br.ReadByte();
            data.Id = br.ReadInt32();
            data.Value = br.ReadInt32();
            return data;
        }
    }
}