using System;
using System.IO;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public class MsgPlayerBase
    {
        public int PlayerUId;
        public bool IsBottomPlayer;
        public string Name;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(IsBottomPlayer);
            bw.Write(Name);
        }

        public void Read(BinaryReader br)
        {
            PlayerUId = br.ReadInt32();
            IsBottomPlayer = br.ReadBoolean();
            Name = br.ReadString();
        }
    }


    [Serializable]
    public class MsgPlayerInfo
    {
        // �÷��̾� ����ũ ���̵�(���� ���Ǻ� ����ũ ����)
        public int PlayerUId;
        public bool IsBottomPlayer;
        public string Name;
        public int CurrentSp;
        public int TowerHp;
        public short SpGrade;
        public short GetDiceCount;
        public int[] DiceIdArray;
        public short[] DiceUpgradeArray;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(IsBottomPlayer);
            bw.Write(Name);
            bw.Write(CurrentSp);
            bw.Write(TowerHp);
            bw.Write(SpGrade);
            bw.Write(GetDiceCount);

            bw.Write(DiceIdArray.Length);
            byte[] bytes = new byte[DiceIdArray.Length * sizeof(int)];
            Buffer.BlockCopy(DiceIdArray, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);

            bw.Write(DiceUpgradeArray.Length);
            bytes = new byte[DiceUpgradeArray.Length * sizeof(short)];
            Buffer.BlockCopy(DiceUpgradeArray, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);
        }

        public void Read(BinaryReader br)
        {
            PlayerUId = br.ReadInt32();
            IsBottomPlayer = br.ReadBoolean();
            Name = br.ReadString();
            CurrentSp = br.ReadInt32();
            TowerHp = br.ReadInt32();
            SpGrade = br.ReadInt16();
            GetDiceCount = br.ReadInt16();

            int length = br.ReadInt32();
            byte[] bytes = br.ReadBytes(length * sizeof(int));

            DiceIdArray = new int[length];
            for (var index = 0; index < length; index++)
            {
                DiceIdArray[index] = BitConverter.ToInt32(bytes, index * sizeof(int));
            }

            length = br.ReadInt32();
            bytes = br.ReadBytes(length * sizeof(short));
            DiceUpgradeArray = new short[length];
            for (var index = 0; index < length; index++)
            {
                DiceUpgradeArray[index] = BitConverter.ToInt16(bytes, index * sizeof(short));
            }
        }
    }


    [Serializable]
    public class MsgJoinGameReq
    {
        public string PlayerSessionId;

        // ��� �� �ε���
        public sbyte DeckIndex;
    }


    [Serializable]
    public class MsgJoinGameAck
    {
        // ���� �ڵ�
        public short ErrorCode;

        // �÷��̾� ����.
        public MsgPlayerInfo PlayerInfo;
    }


    [Serializable]
    public class MsgJoinGameNotify
    {
        // �ٸ� �÷��̾� ����
        public MsgPlayerInfo OtherPlayerInfo;
    }
}