using System;
using System.IO;
using Service.Template.Common;

namespace Template.Player.RandomWarsPlayer.Common
{

    [Serializable]
    public class OpenBoxReward
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

        public static OpenBoxReward Read(BinaryReader br)
        {
            OpenBoxReward data = new OpenBoxReward();
            data.RewardType = (REWARD_TYPE)br.ReadByte();
            data.Id = br.ReadInt32();
            data.Value = br.ReadInt32();
            return data;
        }
    }



}