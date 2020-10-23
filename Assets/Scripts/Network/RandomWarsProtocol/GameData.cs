using System;
using System.Collections.Generic;
using System.IO;

namespace RandomWarsProtocol
{
    [Serializable]
    public class MsgUserInfo
    {
        // 유저 아이디
        public string UserId;
        // 유저명
        public string Name;
        // 트로피 수
        public int Trophy;
        // 보유 골드 수량
        public int Gold;
        // 보유 다이아몬드 수량
        public int Diamond;
        // 주사위 등급별 레벨업 횟수(타워hp에 추가)
        public int[] DiceLevelupCount;
        // 레벨
        public short Level;
        // 연승 횟수 (최대 15회)
        public byte WinStreak;


        public void Write(BinaryWriter bw)
        {
            bw.Write(UserId);
            bw.Write(Name);
            bw.Write(Trophy);
            bw.Write(Gold);
            bw.Write(Diamond);

            bw.Write(DiceLevelupCount.Length);
            byte[] bytes = new byte[DiceLevelupCount.Length * sizeof(int)];
            Buffer.BlockCopy(DiceLevelupCount, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);

            bw.Write(Level);
            bw.Write(WinStreak);
        }

        public static MsgUserInfo Read(BinaryReader br)
        {
            MsgUserInfo data = new MsgUserInfo();
            data.UserId = br.ReadString();
            data.Name = br.ReadString();
            data.Trophy = br.ReadInt32();
            data.Gold = br.ReadInt32();
            data.Diamond = br.ReadInt32();

            int length = br.ReadInt32();
            byte[] bytes = br.ReadBytes(length * sizeof(int));
            data.DiceLevelupCount = new int[length];
            for (var index = 0; index < length; index++)
            {
                data.DiceLevelupCount[index] = BitConverter.ToInt32(bytes, index * sizeof(int));
            }

            data.Level = br.ReadInt16();
            data.WinStreak = br.ReadByte();
            return data;
        }
    }


    [Serializable]
    public class MsgUserDeck
    {
        // 덱 인덱스
        public sbyte Index;
        // 덱 정보(주사위 아이디 배열)
        public int[] DeckInfo;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Index);

            bw.Write(DeckInfo.Length);
            byte[] bytes = new byte[DeckInfo.Length * sizeof(int)];
            Buffer.BlockCopy(DeckInfo, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);
        }

        public static MsgUserDeck Read(BinaryReader br)
        {
            MsgUserDeck data = new MsgUserDeck();
            data.Index = br.ReadSByte();

            int length = br.ReadInt32();
            byte[] bytes = br.ReadBytes(length * sizeof(int));
            data.DeckInfo = new int[length];
            for (var index = 0; index < length; index++)
            {
                data.DeckInfo[index] = BitConverter.ToInt32(bytes, index * sizeof(int));
            }
            return data;
        }
    }


    [Serializable]
    public class MsgUserDice
    {
        public int DiceId;
        public short Level;
        public short Count;

        public void Write(BinaryWriter bw)
        {
            bw.Write(DiceId);
            bw.Write(Level);
            bw.Write(Count);
        }

        public static MsgUserDice Read(BinaryReader br)
        {
            MsgUserDice data = new MsgUserDice();
            data.DiceId = br.ReadInt32();
            data.Level = br.ReadInt16();
            data.Count = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgPlayerBase
    {
        public ushort PlayerUId;
        public bool IsBottomPlayer;
        public string Name;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(IsBottomPlayer);
            bw.Write(Name);
        }

        public static MsgPlayerBase Read(BinaryReader br)
        {
            MsgPlayerBase data = new MsgPlayerBase();
            data.PlayerUId = br.ReadUInt16();
            data.IsBottomPlayer = br.ReadBoolean();
            data.Name = br.ReadString();
            return data;
        }
    }


    [Serializable]
    public class MsgPlayerInfo
    {
        // 플레이어 유니크 아이디(게임 세션별 유니크 생성)
        public ushort PlayerUId;
        public bool IsBottomPlayer;
        public string Name;
        public int CurrentSp;
        public int TowerHp;
        public short SpGrade;
        public short GetDiceCount;
        public int[] DiceIdArray;
        public short[] DiceLevelArray;

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

            bw.Write(DiceLevelArray.Length);
            bytes = new byte[DiceLevelArray.Length * sizeof(short)];
            Buffer.BlockCopy(DiceLevelArray, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);
        }

        public static MsgPlayerInfo Read(BinaryReader br)
        {
            MsgPlayerInfo data = new MsgPlayerInfo();
            data.PlayerUId = br.ReadUInt16();
            data.IsBottomPlayer = br.ReadBoolean();
            data.Name = br.ReadString();
            data.CurrentSp = br.ReadInt32();
            data.TowerHp = br.ReadInt32();
            data.SpGrade = br.ReadInt16();
            data.GetDiceCount = br.ReadInt16();

            int length = br.ReadInt32();
            byte[] bytes = br.ReadBytes(length * sizeof(int));
            data.DiceIdArray = new int[length];
            for (var index = 0; index < length; index++)
            {
                data.DiceIdArray[index] = BitConverter.ToInt32(bytes, index * sizeof(int));
            }

            length = br.ReadInt32();
            bytes = br.ReadBytes(length * sizeof(short));
            data.DiceLevelArray = new short[length];
            for (var index = 0; index < length; index++)
            {
                data.DiceLevelArray[index] = BitConverter.ToInt16(bytes, index * sizeof(short));
            }

            return data;
        }
    }


    [Serializable]
    public class MsgSyncMinionData
    {
        public ushort minionId;
        public int minionDataId;
        public int minionHp;
        public int minionMaxHp;
        public int minionPower;
        public int minionEffect;
        public short minionEffectUpgrade;
        public short minionEffectIngameUpgrade;
        public short minionDuration;
        public short minionCooltime;
        public MsgVector3 minionPos;

        public void Write(BinaryWriter bw)
        {
            bw.Write(minionId);
            bw.Write(minionDataId);
            bw.Write(minionHp);
            bw.Write(minionMaxHp);
            bw.Write(minionPower);
            bw.Write(minionEffect);
            bw.Write(minionEffectUpgrade);
            bw.Write(minionEffectIngameUpgrade);
            bw.Write(minionDuration);
            bw.Write(minionCooltime);
            minionPos.Write(bw);
        }

        public static MsgSyncMinionData Read(BinaryReader br)
        {
            MsgSyncMinionData data = new MsgSyncMinionData();
            data.minionId = br.ReadUInt16();
            data.minionDataId = br.ReadInt32();
            data.minionHp = br.ReadInt32();
            data.minionMaxHp = br.ReadInt32();
            data.minionPower = br.ReadInt32();
            data.minionEffect = br.ReadInt32();
            data.minionEffectUpgrade = br.ReadInt16();
            data.minionEffectIngameUpgrade = br.ReadInt16();
            data.minionDuration = br.ReadInt16();
            data.minionCooltime = br.ReadInt16();
            data.minionPos = MsgVector3.Read(br);
            return data;
        }
    }


    [Serializable]
    public class MsgGameDice
    {
        public int DiceId;
        public short SlotNum;
        public short Level;

        public void Write(BinaryWriter bw)
        {
            bw.Write(DiceId);
            bw.Write(SlotNum);
            bw.Write(Level);
        }

        public static MsgGameDice Read(BinaryReader br)
        {
            MsgGameDice data = new MsgGameDice();
            data.DiceId = br.ReadInt32();
            data.SlotNum = br.ReadInt16();
            data.Level = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgInGameUp
    {
        public int DiceId;
        public short Grade;

        public void Write(BinaryWriter bw)
        {
            bw.Write(DiceId);
            bw.Write(Grade);
        }

        public static MsgInGameUp Read(BinaryReader br)
        {
            MsgInGameUp data = new MsgInGameUp();
            data.DiceId = br.ReadInt32();
            data.Grade = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgVector3
    {
        public short X;
        public short Y;
        public short Z;

        public void Write(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
        }

        public static MsgVector3 Read(BinaryReader br)
        {
            MsgVector3 data = new MsgVector3();
            data.X = br.ReadInt16();
            data.Y = br.ReadInt16();
            data.Z = br.ReadInt16();
            return data;
        }
    }

    [Serializable]
    public class MsgQuaternion
    {
        public short X;
        public short Y;
        public short Z;
        public short W;

        public void Write(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
            bw.Write(W);
        }

        public static MsgQuaternion Read(BinaryReader br)
        {
            MsgQuaternion data = new MsgQuaternion();
            data.X = br.ReadInt16();
            data.Y = br.ReadInt16();
            data.Z = br.ReadInt16();
            data.W = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgMinionStatus
    {
        public MsgHitDamageMinionRelay[] arrHitDamageMinionRelay;
        public MsgDestroyMinionRelay[] arrDestroyMinionRelay;
        public MsgDestroyMagicRelay[] arrDestroyMagicRelay;
        public MsgFireballBombRelay[] arrFireballBombRelay;
        public MsgHealMinionRelay[] arrHealMinionRelay;
        public MsgMineBombRelay[] arrMineBombRelay;
        public MsgSturnMinionRelay[] arrSturnMinionRelay;
        public MsgRocketBombRelay[] arrRocketBombRelay;
        public MsgIceBombRelay[] arrIceBombRelay;
        public MsgFireManFireRelay[] arrFireManFireRelay;
        public MsgMinionCloackingRelay[] arrMinionCloackingRelay;
        public MsgMinionFlagOfWarRelay[] arrMinionFlagOfWarRelay;
        public MsgScarecrowRelay[] arrScarercrowRelay;
        public MsgLayzerTargetRelay[] arrLayzerTargetRelay;
        public MsgMinionInvincibilityRelay[] arrMinionInvincibilityRelay;
        public MsgFireBulletRelay[] arrFireBulletRelay;
        public MsgFireCannonBallRelay[] arrFireCannonBallRelay;
        public MsgSetMinionAnimationTriggerRelay[] arrMinionAnimationTriggerRelay;
        public MsgSetMagicTargetIdRelay[] arrMagicTargetIdRelay;
        public MsgSetMagicTargetRelay[] arrMagicTargetRelay;
        public MsgActivatePoolObjectRelay[] arrActivatePoolObjectRelay;
        public MsgSendMessageVoidRelay[] arrSendMessageVoidRelay;
        public MsgSendMessageParam1Relay[] arrSendMessageParam1Relay;
        public MsgSetMinionTargetRelay[] arrMinionTargetRelay;
        public MsgPushMinionRelay[] arrPushMinionRelay;

        public void Write(BinaryWriter bw)
        {
            int length = (arrHitDamageMinionRelay == null) ? 0 : arrHitDamageMinionRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrHitDamageMinionRelay[i].Write(bw);
            }

            length = (arrDestroyMinionRelay == null) ? 0 : arrDestroyMinionRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrDestroyMinionRelay[i].Write(bw);
            }

            length = (arrDestroyMagicRelay == null) ? 0 : arrDestroyMagicRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrDestroyMagicRelay[i].Write(bw);
            }

            length = (arrFireballBombRelay == null) ? 0 : arrFireballBombRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrFireballBombRelay[i].Write(bw);
            }

            length = (arrHealMinionRelay == null) ? 0 : arrHealMinionRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrHealMinionRelay[i].Write(bw);
            }

            length = (arrMineBombRelay == null) ? 0 : arrMineBombRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrMineBombRelay[i].Write(bw);
            }

            length = (arrSturnMinionRelay == null) ? 0 : arrSturnMinionRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrSturnMinionRelay[i].Write(bw);
            }

            length = (arrRocketBombRelay == null) ? 0 : arrRocketBombRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrRocketBombRelay[i].Write(bw);
            }

            length = (arrIceBombRelay == null) ? 0 : arrIceBombRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrIceBombRelay[i].Write(bw);
            }

            length = (arrFireManFireRelay == null) ? 0 : arrFireManFireRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrFireManFireRelay[i].Write(bw);
            }

            length = (arrMinionCloackingRelay == null) ? 0 : arrMinionCloackingRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrMinionCloackingRelay[i].Write(bw);
            }

            length = (arrMinionFlagOfWarRelay == null) ? 0 : arrMinionFlagOfWarRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrMinionFlagOfWarRelay[i].Write(bw);
            }

            length = (arrScarercrowRelay == null) ? 0 : arrScarercrowRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrScarercrowRelay[i].Write(bw);
            }

            length = (arrLayzerTargetRelay == null) ? 0 : arrLayzerTargetRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrLayzerTargetRelay[i].Write(bw);
            }

            length = (arrMinionInvincibilityRelay == null) ? 0 : arrMinionInvincibilityRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrMinionInvincibilityRelay[i].Write(bw);
            }

            length = (arrFireBulletRelay == null) ? 0 : arrFireBulletRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrFireBulletRelay[i].Write(bw);
            }

            length = (arrFireCannonBallRelay == null) ? 0 : arrFireCannonBallRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrFireCannonBallRelay[i].Write(bw);
            }

            length = (arrMinionAnimationTriggerRelay == null) ? 0 : arrMinionAnimationTriggerRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrMinionAnimationTriggerRelay[i].Write(bw);
            }

            length = (arrMagicTargetIdRelay == null) ? 0 : arrMagicTargetIdRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrMagicTargetIdRelay[i].Write(bw);
            }

            length = (arrMagicTargetRelay == null) ? 0 : arrMagicTargetRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrMagicTargetRelay[i].Write(bw);
            }

            length = (arrActivatePoolObjectRelay == null) ? 0 : arrActivatePoolObjectRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrActivatePoolObjectRelay[i].Write(bw);
            }

            length = (arrSendMessageVoidRelay == null) ? 0 : arrSendMessageVoidRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrSendMessageVoidRelay[i].Write(bw);
            }

            length = (arrSendMessageParam1Relay == null) ? 0 : arrSendMessageParam1Relay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrSendMessageParam1Relay[i].Write(bw);
            }

            length = (arrMinionTargetRelay == null) ? 0 : arrMinionTargetRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrMinionTargetRelay[i].Write(bw);
            }

            length = (arrPushMinionRelay == null) ? 0 : arrPushMinionRelay.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                arrPushMinionRelay[i].Write(bw);
            }
        }

        public static MsgMinionStatus Read(BinaryReader br)
        {
            MsgMinionStatus data = new MsgMinionStatus();

            int length = br.ReadInt32();
            if (length > 0)
            {
                data.arrHitDamageMinionRelay = new MsgHitDamageMinionRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrHitDamageMinionRelay[i] = MsgHitDamageMinionRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrDestroyMinionRelay = new MsgDestroyMinionRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrDestroyMinionRelay[i] = MsgDestroyMinionRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrDestroyMagicRelay = new MsgDestroyMagicRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrDestroyMagicRelay[i] = MsgDestroyMagicRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrFireballBombRelay = new MsgFireballBombRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrFireballBombRelay[i] = MsgFireballBombRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrHealMinionRelay = new MsgHealMinionRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrHealMinionRelay[i] = MsgHealMinionRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrMineBombRelay = new MsgMineBombRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrMineBombRelay[i] = MsgMineBombRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrSturnMinionRelay = new MsgSturnMinionRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrSturnMinionRelay[i] = MsgSturnMinionRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrRocketBombRelay = new MsgRocketBombRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrRocketBombRelay[i] = MsgRocketBombRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrIceBombRelay = new MsgIceBombRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrIceBombRelay[i] = MsgIceBombRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrFireManFireRelay = new MsgFireManFireRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrFireManFireRelay[i] = MsgFireManFireRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrMinionCloackingRelay = new MsgMinionCloackingRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrMinionCloackingRelay[i] = MsgMinionCloackingRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrMinionFlagOfWarRelay = new MsgMinionFlagOfWarRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrMinionFlagOfWarRelay[i] = MsgMinionFlagOfWarRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrScarercrowRelay = new MsgScarecrowRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrScarercrowRelay[i] = MsgScarecrowRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrLayzerTargetRelay = new MsgLayzerTargetRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrLayzerTargetRelay[i] = MsgLayzerTargetRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrMinionInvincibilityRelay = new MsgMinionInvincibilityRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrMinionInvincibilityRelay[i] = MsgMinionInvincibilityRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrFireBulletRelay = new MsgFireBulletRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrFireBulletRelay[i] = MsgFireBulletRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrFireCannonBallRelay = new MsgFireCannonBallRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrFireCannonBallRelay[i] = MsgFireCannonBallRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrMinionAnimationTriggerRelay = new MsgSetMinionAnimationTriggerRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrMinionAnimationTriggerRelay[i] = MsgSetMinionAnimationTriggerRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrMagicTargetIdRelay = new MsgSetMagicTargetIdRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrMagicTargetIdRelay[i] = MsgSetMagicTargetIdRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrMagicTargetRelay = new MsgSetMagicTargetRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrMagicTargetRelay[i] = MsgSetMagicTargetRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrActivatePoolObjectRelay = new MsgActivatePoolObjectRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrActivatePoolObjectRelay[i] = MsgActivatePoolObjectRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrSendMessageVoidRelay = new MsgSendMessageVoidRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrSendMessageVoidRelay[i] = MsgSendMessageVoidRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrSendMessageParam1Relay = new MsgSendMessageParam1Relay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrSendMessageParam1Relay[i] = MsgSendMessageParam1Relay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrMinionTargetRelay = new MsgSetMinionTargetRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrMinionTargetRelay[i] = MsgSetMinionTargetRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                data.arrPushMinionRelay = new MsgPushMinionRelay[length];
                for (int i = 0; i < length; i++)
                {
                    data.arrPushMinionRelay[i] = MsgPushMinionRelay.Read(br);
                }
            }

            return data;
        }
    }


    [Serializable]
    public class MsgHitDamageMinionRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public int Damage;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(Damage);
        }

        public static MsgHitDamageMinionRelay Read(BinaryReader br)
        {
            MsgHitDamageMinionRelay data = new MsgHitDamageMinionRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.Damage = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgDestroyMinionRelay
    {
        public ushort PlayerUId;
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgDestroyMinionRelay Read(BinaryReader br)
        {
            MsgDestroyMinionRelay data = new MsgDestroyMinionRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgHealMinionRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public int Heal;


        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(Heal);
        }

        public static MsgHealMinionRelay Read(BinaryReader br)
        {
            MsgHealMinionRelay data = new MsgHealMinionRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.Heal = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgPushMinionRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public MsgVector3 Dir;
        public short PushPower;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            Dir.Write(bw);
            bw.Write(PushPower);
        }

        public static MsgPushMinionRelay Read(BinaryReader br)
        {
            MsgPushMinionRelay data = new MsgPushMinionRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.Dir = MsgVector3.Read(br);
            data.PushPower = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgSetMinionAnimationTriggerRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public ushort TargetId;
        public byte Trigger;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(TargetId);
            bw.Write(Trigger);
        }

        public static MsgSetMinionAnimationTriggerRelay Read(BinaryReader br)
        {
            MsgSetMinionAnimationTriggerRelay data = new MsgSetMinionAnimationTriggerRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.TargetId = br.ReadUInt16();
            data.Trigger = br.ReadByte();
            return data;
        }
    }


    [Serializable]

    public class MsgFireArrowRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public MsgVector3 Dir;
        public int Damage;
        public short MoveSpeed;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            Dir.Write(bw);
            bw.Write(Damage);
            bw.Write(MoveSpeed);
        }

        public static MsgFireArrowRelay Read(BinaryReader br)
        {
            MsgFireArrowRelay data = new MsgFireArrowRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.Dir = MsgVector3.Read(br);
            data.Damage = br.ReadInt32();
            data.MoveSpeed = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgFireballBombRelay
    {
        public ushort PlayerUId;
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgFireballBombRelay Read(BinaryReader br)
        {
            MsgFireballBombRelay data = new MsgFireballBombRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgMineBombRelay
    {
        public ushort PlayerUId;
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgMineBombRelay Read(BinaryReader br)
        {
            MsgMineBombRelay data = new MsgMineBombRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgSetMagicTargetIdRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public ushort TargetId;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(TargetId);
        }

        public static MsgSetMagicTargetIdRelay Read(BinaryReader br)
        {
            MsgSetMagicTargetIdRelay data = new MsgSetMagicTargetIdRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.TargetId = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgSetMagicTargetRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public short X;
        public short Z;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(X);
            bw.Write(Z);
        }

        public static MsgSetMagicTargetRelay Read(BinaryReader br)
        {
            MsgSetMagicTargetRelay data = new MsgSetMagicTargetRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.X = br.ReadInt16();
            data.Z = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgSturnMinionRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public short SturnTime;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(SturnTime);
        }

        public static MsgSturnMinionRelay Read(BinaryReader br)
        {
            MsgSturnMinionRelay data = new MsgSturnMinionRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.SturnTime = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgRocketBombRelay
    {
        public ushort PlayerUId;
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgRocketBombRelay Read(BinaryReader br)
        {
            MsgRocketBombRelay data = new MsgRocketBombRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgIceBombRelay
    {
        public ushort PlayerUId;
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgIceBombRelay Read(BinaryReader br)
        {
            MsgIceBombRelay data = new MsgIceBombRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgDestroyMagicRelay
    {
        public ushort PlayerUId;
        public ushort BaseStatId;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(BaseStatId);
        }

        public static MsgDestroyMagicRelay Read(BinaryReader br)
        {
            MsgDestroyMagicRelay data = new MsgDestroyMagicRelay();
            data.PlayerUId = br.ReadUInt16();
            data.BaseStatId = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgFireCannonBallRelay
    {
        public ushort PlayerUId;
        public MsgVector3 ShootPos;
        public MsgVector3 TargetPos;
        public int Power;
        public short Range;
        public byte Type;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            ShootPos.Write(bw);
            TargetPos.Write(bw);
            bw.Write(Power);
            bw.Write(Range);
            bw.Write(Type);
        }

        public static MsgFireCannonBallRelay Read(BinaryReader br)
        {
            MsgFireCannonBallRelay data = new MsgFireCannonBallRelay();
            data.PlayerUId = br.ReadUInt16();
            data.ShootPos = MsgVector3.Read(br);
            data.TargetPos = MsgVector3.Read(br);
            data.Power = br.ReadInt32();
            data.Range = br.ReadInt16();
            data.Type = br.ReadByte();
            return data;
        }
    }


    [Serializable]
    public class MsgFireSpearRelay
    {
        public ushort PlayerUId;
        public MsgVector3 ShootPos;
        public ushort TargetId;
        public int Power;
        public short MoveSpeed;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            ShootPos.Write(bw);
            bw.Write(TargetId);
            bw.Write(Power);
            bw.Write(MoveSpeed);
        }

        public static MsgFireSpearRelay Read(BinaryReader br)
        {
            MsgFireSpearRelay data = new MsgFireSpearRelay();
            data.PlayerUId = br.ReadUInt16();
            data.ShootPos = MsgVector3.Read(br);
            data.TargetId = br.ReadUInt16();
            data.Power = br.ReadInt32();
            data.MoveSpeed = br.ReadInt16();
            return data;
        }

    }


    [Serializable]
    public class MsgFireManFireRelay
    {
        public ushort PlayerUId;
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgFireManFireRelay Read(BinaryReader br)
        {
            MsgFireManFireRelay data = new MsgFireManFireRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgActivatePoolObjectRelay
    {
        public int PoolName;
        public MsgVector3 HitPos;
        public MsgVector3 LocalScale;
        public MsgQuaternion Rotation;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PoolName);
            HitPos.Write(bw);
            LocalScale.Write(bw);
            Rotation.Write(bw);
        }

        public static MsgActivatePoolObjectRelay Read(BinaryReader br)
        {
            MsgActivatePoolObjectRelay data = new MsgActivatePoolObjectRelay();
            data.PoolName = br.ReadInt32();
            data.HitPos = MsgVector3.Read(br);
            data.LocalScale = MsgVector3.Read(br);
            data.Rotation = MsgQuaternion.Read(br);
            return data;
        }
    }


    [Serializable]
    public class MsgMinionCloackingRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public bool IsCloacking;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(IsCloacking);
        }

        public static MsgMinionCloackingRelay Read(BinaryReader br)
        {
            MsgMinionCloackingRelay data = new MsgMinionCloackingRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.IsCloacking = br.ReadBoolean();
            return data;
        }
    }


    [Serializable]
    public class MsgMinionFlagOfWarRelay
    {
        public ushort PlayerUId;
        public ushort BaseStatId;
        public short Effect;
        public bool IsFogOfWar;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(BaseStatId);
            bw.Write(Effect);
            bw.Write(IsFogOfWar);
        }

        public static MsgMinionFlagOfWarRelay Read(BinaryReader br)
        {
            MsgMinionFlagOfWarRelay data = new MsgMinionFlagOfWarRelay();
            data.PlayerUId = br.ReadUInt16();
            data.BaseStatId = br.ReadUInt16();
            data.Effect = br.ReadInt16();
            data.IsFogOfWar = br.ReadBoolean();
            return data;
        }
    }


    [Serializable]
    public class MsgSendMessageVoidRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public byte Message;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(Message);
        }

        public static MsgSendMessageVoidRelay Read(BinaryReader br)
        {
            MsgSendMessageVoidRelay data = new MsgSendMessageVoidRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.Message = br.ReadByte();
            return data;
        }
    }


    [Serializable]
    public class MsgSendMessageParam1Relay
    {
        public ushort PlayerUId;
        public ushort Id;
        public ushort TargetId;
        public byte Message;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(TargetId);
            bw.Write(Message);
        }

        public static MsgSendMessageParam1Relay Read(BinaryReader br)
        {
            MsgSendMessageParam1Relay data = new MsgSendMessageParam1Relay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.TargetId = br.ReadUInt16();
            data.Message = br.ReadByte();
            return data;
        }
    }


    [Serializable]
    public class MsgNecromancerBulletRelay
    {
        public ushort PlayerUId;
        public MsgVector3 ShootPos;
        public ushort TargetId;
        public int Power;
        public short BulletMoveSpeed;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            ShootPos.Write(bw);
            bw.Write(TargetId);
            bw.Write(Power);
            bw.Write(BulletMoveSpeed);
        }

        public static MsgNecromancerBulletRelay Read(BinaryReader br)
        {
            MsgNecromancerBulletRelay data = new MsgNecromancerBulletRelay();
            data.PlayerUId = br.ReadUInt16();
            data.ShootPos = MsgVector3.Read(br);
            data.TargetId = br.ReadUInt16();
            data.Power = br.ReadInt32();
            data.BulletMoveSpeed = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgSetMinionTargetRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public ushort TargetId;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(TargetId);
        }

        public static MsgSetMinionTargetRelay Read(BinaryReader br)
        {
            MsgSetMinionTargetRelay data = new MsgSetMinionTargetRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.TargetId = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgScarecrowRelay
    {
        public ushort PlayerUId;
        public ushort BaseStatId;
        public byte EyeLevel;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(BaseStatId);
            bw.Write(EyeLevel);
        }

        public static MsgScarecrowRelay Read(BinaryReader br)
        {
            MsgScarecrowRelay data = new MsgScarecrowRelay();
            data.PlayerUId = br.ReadUInt16();
            data.BaseStatId = br.ReadUInt16();
            data.EyeLevel = br.ReadByte();
            return data;
        }
    }


    [Serializable]
    public class MsgLayzerTargetRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public ushort[] TargetIdArray;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(TargetIdArray.Length);
            byte[] bytes = new byte[TargetIdArray.Length * sizeof(ushort)];
            Buffer.BlockCopy(TargetIdArray, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);
        }

        public static MsgLayzerTargetRelay Read(BinaryReader br)
        {
            MsgLayzerTargetRelay data = new MsgLayzerTargetRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();

            int length = br.ReadInt32();
            byte[] bytes = br.ReadBytes(length * sizeof(ushort));
            data.TargetIdArray = new ushort[length];
            for (var index = 0; index < length; index++)
            {
                data.TargetIdArray[index] = BitConverter.ToUInt16(bytes, index * sizeof(ushort));
            }

            return data;
        }
    }


    [Serializable]
    public class MsgMinionStatusRelay
    {
        public ushort PlayerUId;
        public byte PosIndex;
        public MsgVector3[] Pos;
        public int[] Hp;
        public MsgMinionStatus Relay;
        public int packetCount;
    }


    [Serializable]
    public class MsgFireBulletRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public MsgVector3 Dir;
        public int Damage;
        public short MoveSpeed;
        public byte Type;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            Dir.Write(bw);
            bw.Write(Damage);
            bw.Write(MoveSpeed);
            bw.Write(Type);
        }

        public static MsgFireBulletRelay Read(BinaryReader br)
        {
            MsgFireBulletRelay data = new MsgFireBulletRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.Dir = MsgVector3.Read(br);
            data.Damage = br.ReadInt32();
            data.MoveSpeed = br.ReadInt16();
            data.Type = br.ReadByte();
            return data;
        }
    }


    [Serializable]
    public class MsgMinionInvincibilityRelay
    {
        public ushort PlayerUId;
        public ushort Id;
        public short Time;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(Time);
        }

        public static MsgMinionInvincibilityRelay Read(BinaryReader br)
        {
            MsgMinionInvincibilityRelay data = new MsgMinionInvincibilityRelay();
            data.PlayerUId = br.ReadUInt16();
            data.Id = br.ReadUInt16();
            data.Time = br.ReadInt16();
            return data;
        }
    }
}
