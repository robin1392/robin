using System;
using System.IO;

namespace Template.Account.RandomWarsAccount.Common
{
    [Serializable]
    public class MsgAccount
    {
        public string PlatformId;
        public MsgPlayer PlayerInfo;
        public MsgDeck[] DeckInfo;
        public MsgDice[] DiceInfo;
        public MsgBox[] BoxInfo;


        public void Write(BinaryWriter bw)
        {
            bw.Write(PlatformId);
            PlayerInfo.Write(bw);

            int length = (DeckInfo == null) ? 0 : DeckInfo.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                DeckInfo[i].Write(bw);
            }

            length = (DiceInfo == null) ? 0 : DiceInfo.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                DiceInfo[i].Write(bw);
            }

            length = (BoxInfo == null) ? 0 : BoxInfo.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                BoxInfo[i].Write(bw);
            }
        }

        public static MsgAccount Read(BinaryReader br)
        {
            MsgAccount data = new MsgAccount();
            data.PlatformId = br.ReadString();
            data.PlayerInfo = MsgPlayer.Read(br);

            int length = br.ReadInt32();
            data.DeckInfo = new MsgDeck[length];
            for (int i = 0; i < length; i++)
            {
                data.DeckInfo[i] = MsgDeck.Read(br);
            }

            length = br.ReadInt32();
            data.DiceInfo = new MsgDice[length];
            for (int i = 0; i < length; i++)
            {
                data.DiceInfo[i] = MsgDice.Read(br);
            }

            length = br.ReadInt32();
            data.BoxInfo = new MsgBox[length];
            for (int i = 0; i < length; i++)
            {
                data.BoxInfo[i] = MsgBox.Read(br);
            }

            return data;
        }

    }

    [Serializable]
    public class MsgPlayer
    {
        public string PlayerGuid;
        public string Name;
        public int Trophy;
        public int Gold;
        public int Diamond;
        public int Key;
        public short Class;
        public byte WinStreak;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerGuid);
            bw.Write(Name);
            bw.Write(Trophy);
            bw.Write(Gold);
            bw.Write(Diamond);
            bw.Write(Key);
            bw.Write(Class);
            bw.Write(WinStreak);
        }

        public static MsgPlayer Read(BinaryReader br)
        {
            return new MsgPlayer
            {
                PlayerGuid = br.ReadString(),
                Name = br.ReadString(),
                Trophy = br.ReadInt32(),
                Gold = br.ReadInt32(),
                Diamond = br.ReadInt32(),
                Key = br.ReadInt32(),
                Class = br.ReadInt16(),
                WinStreak = br.ReadByte(),
            };
        }
    }


    public class MsgDeck
    {
        public int Index;
        public int[] DiceIds;


        public void Write(BinaryWriter bw)
        {
            bw.Write(Index);

            int length = (DiceIds == null) ? 0 : DiceIds.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                bw.Write(DiceIds[i]);
            }
        }

        public static MsgDeck Read(BinaryReader br)
        {
            MsgDeck data = new MsgDeck();
            data.Index = br.ReadInt32();

            int length = br.ReadInt32();
            data.DiceIds = new int[length];
            for (int i = 0; i < length; i++)
            {
                data.DiceIds[i] = br.ReadInt32();
            }

            return data;
        }
    }


    public class MsgBox
    {
        public int Id;
        public int Count;


        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(Count);
        }

        public static MsgBox Read(BinaryReader br)
        {
            return new MsgBox
            {
                Id = br.ReadInt32(),
                Count = br.ReadInt32()
            };
        }
    }


    public class MsgDice
    {
        public int Id;
        public int Level;
        public int Count;


        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(Level);
            bw.Write(Count);
        }

        public static MsgDice Read(BinaryReader br)
        {
            return new MsgDice
            {
                Id = br.ReadInt32(),
                Level = br.ReadInt32(),
                Count = br.ReadInt32()
            };
        }
    }
}