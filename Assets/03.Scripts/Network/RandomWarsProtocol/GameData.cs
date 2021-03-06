using System;
using System.Collections.Generic;
using System.IO;

namespace RandomWarsProtocol
{
    [Serializable]
    public class MsgUserGoods
    {
        // 골드 수량
        public int Gold;
        // 다이아몬드 수량
        public int Diamond;
        // 열쇠 수량
        public int Key;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Gold);
            bw.Write(Diamond);
            bw.Write(Key);
        }

        public static MsgUserGoods Read(BinaryReader br)
        {
            MsgUserGoods data = new MsgUserGoods();
            data.Gold = br.ReadInt32();
            data.Diamond = br.ReadInt16();
            data.Key = br.ReadInt16();
            return data;
        }
    }



    [Serializable]
    public class MsgUserInfo
    {
        // 유저 아이디
        public string UserId;
        // 유저명
        public string Name;
        // 트로피 수
        public int Trophy;
        // 랭킹포인트
        public int RankingPoint;
        // 유저 재화
        public MsgUserGoods Goods;
        // 유저 클래스
        public short Class;
        // 연승 횟수 (최대 15회)
        public byte WinStreak;
        // 트로피 보상 획득 아이디 배열
        public int[] TrophyRewardIds;
        // Vip패스 구입 여부`
        public bool IsBuyVipPass;
        // 시즌 정보
        public MsgSeasonPassInfo SeasonInfo;
        public int WinCount;
        public int DefeatCount;
        public int HighTrophy;
        public bool EndTutorial;


        public void Write(BinaryWriter bw)
        {
            bw.Write(UserId);
            bw.Write(Name);
            bw.Write(Trophy);
            Goods.Write(bw);
            bw.Write(Class);
            bw.Write(WinStreak);

            bw.Write(TrophyRewardIds.Length);
            byte[] bytes = new byte[TrophyRewardIds.Length * sizeof(int)];
            Buffer.BlockCopy(TrophyRewardIds, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);
        }

        public static MsgUserInfo Read(BinaryReader br)
        {
            MsgUserInfo data = new MsgUserInfo();
            data.UserId = br.ReadString();
            data.Name = br.ReadString();
            data.Trophy = br.ReadInt32();
            data.Goods = MsgUserGoods.Read(br);
            data.Class = br.ReadInt16();
            data.WinStreak = br.ReadByte();

            int length = br.ReadInt32();
            byte[] bytes = br.ReadBytes(length * sizeof(int));
            data.TrophyRewardIds = new int[length];
            for (var index = 0; index < length; index++)
            {
                data.TrophyRewardIds[index] = BitConverter.ToInt32(bytes, index * sizeof(int));
            }
            return data;
        }
    }


    [Serializable]
    public class UserDeck
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

        public static UserDeck Read(BinaryReader br)
        {
            UserDeck data = new UserDeck();
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
    public class UserDice
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

        public static UserDice Read(BinaryReader br)
        {
            UserDice data = new UserDice();
            data.DiceId = br.ReadInt32();
            data.Level = br.ReadInt16();
            data.Count = br.ReadInt16();
            return data;
        }
    }


    //[Serializable]
    //public class ItemBaseInfo
    //{
    //    public int ItemId;
    //    public int Value;


    //    public void Write(BinaryWriter bw)
    //    {
    //        bw.Write(ItemId);
    //        bw.Write(Value);
    //    }

    //    public static ItemBaseInfo Read(BinaryReader br)
    //    {
    //        ItemBaseInfo data = new ItemBaseInfo();
    //        data.ItemId = br.ReadInt32();
    //        data.Value = br.ReadInt32();
    //        return data;
    //    }
    //}


    //[Serializable]
    //public class ItemBaseInfoMultiple
    //{
    //    public int ItemId;
    //    public ItemBaseInfo[] arrayReward;
    //}



    [Serializable]
    public class UserBox
    {
        public int BoxId;
        public int Count;

        public void Write(BinaryWriter bw)
        {
            bw.Write(BoxId);
            bw.Write(Count);
        }

        public static UserBox Read(BinaryReader br)
        {
            UserBox data = new UserBox();
            data.BoxId = br.ReadInt32();
            data.Count = br.ReadInt32();
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
        public bool IsMaster;
        public string Name;
        public int CurrentSp;
        public int TowerHp;
        public int Trophy;
        public short SpGrade;
        public short GetDiceCount;
        public int[] DiceIdArray;
        public short[] DiceLevelArray;


        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(IsBottomPlayer);
            bw.Write(IsMaster);
            bw.Write(Name);
            bw.Write(CurrentSp);
            bw.Write(TowerHp);
            bw.Write(Trophy);
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
            data.IsMaster = br.ReadBoolean();
            data.Name = br.ReadString();
            data.CurrentSp = br.ReadInt32();
            data.TowerHp = br.ReadInt32();
            data.Trophy = br.ReadInt32();
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
    public class MsgMonster
    {
        public ushort Id;
        public int DataId;
        public int Hp;
        public int Power;
        public int Effect;
        public short EffectCoolTime;
        public short Duration;
        public short MoveSpeed;
        public short AttackSpeed;


        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(DataId);
            bw.Write(Hp);
            bw.Write(Power);
            bw.Write(Effect);
            bw.Write(EffectCoolTime);
            bw.Write(Duration);
            bw.Write(MoveSpeed);
            bw.Write(AttackSpeed);
        }

        public static MsgMonster Read(BinaryReader br)
        {
            MsgMonster data = new MsgMonster();
            data.Id = br.ReadUInt16();
            data.DataId = br.ReadInt32();
            data.Hp = br.ReadInt32();
            data.Power = br.ReadInt32();
            data.Effect = br.ReadInt32();
            data.EffectCoolTime = br.ReadInt16();
            data.Duration = br.ReadInt16();
            data.MoveSpeed = br.ReadInt16();
            data.AttackSpeed = br.ReadInt16();
            return data;
        }
    }

    [Serializable]

    public class MsgDamage
    {
        public ushort Id;
        public int Damage;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(Damage);
        }

        public static MsgDamage Read(BinaryReader br)
        {
            MsgDamage data = new MsgDamage();
            data.Id = br.ReadUInt16();
            data.Damage = br.ReadInt32();
            return data;
        }
    }


    [Serializable]

    public class MsgDamageResult
    {
        public ushort Id;
        public int Hp;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(Hp);
        }

        public static MsgDamageResult Read(BinaryReader br)
        {
            MsgDamageResult data = new MsgDamageResult();
            data.Id = br.ReadUInt16();
            data.Hp = br.ReadInt32();
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
    public class MsgVector2
    {
        public short X;
        public short Y;

        public void Write(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
        }

        public static MsgVector2 Read(BinaryReader br)
        {
            MsgVector2 data = new MsgVector2();
            data.X = br.ReadInt16();
            data.Y = br.ReadInt16();
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
    public class MsgMinionStatusRelay
    {
        public ushort PlayerUId;
        public int packetCount;
        public MsgMinionInfo[] MinionInfo;
        public MsgMinionStatus Relay;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(packetCount);

            int length = (MinionInfo == null) ? 0 : MinionInfo.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                MinionInfo[i].Write(bw);
            }

            Relay.Write(bw);
        }

        public static MsgMinionStatusRelay Read(BinaryReader br)
        {
            MsgMinionStatusRelay data = new MsgMinionStatusRelay();
            data.PlayerUId = br.ReadUInt16();
            data.packetCount = br.ReadInt32();

            int length = br.ReadInt32();
            data.MinionInfo = new MsgMinionInfo[length];
            for (int i = 0; i < length; i++)
            {
                data.MinionInfo[i] = MsgMinionInfo.Read(br);
            }

            data.Relay = MsgMinionStatus.Read(br);
            return data;
        }
    }


    [Serializable]
    public class MsgMinionInfo
    {
        public ushort Id;
        public ushort CustomValue;
        public ushort DiceId;
        public byte DiceEyeLevel;
        public int Hp;
        public MsgVector2 Pos;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(CustomValue);
            bw.Write(DiceId);
            bw.Write(DiceEyeLevel);
            bw.Write(Hp);
            Pos.Write(bw);
        }

        public static MsgMinionInfo Read(BinaryReader br)
        {
            MsgMinionInfo data = new MsgMinionInfo();
            data.Id = br.ReadUInt16();
            data.CustomValue = br.ReadUInt16();
            data.DiceId = br.ReadUInt16();
            data.DiceEyeLevel = br.ReadByte();
            data.Hp = br.ReadInt32();
            data.Pos = MsgVector2.Read(br);
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
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
        }

        public static MsgDestroyMinionRelay Read(BinaryReader br)
        {
            MsgDestroyMinionRelay data = new MsgDestroyMinionRelay();
            data.Id = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgHealMinionRelay
    {
        public ushort Id;
        public int Heal;


        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(Heal);
        }

        public static MsgHealMinionRelay Read(BinaryReader br)
        {
            MsgHealMinionRelay data = new MsgHealMinionRelay();
            data.Id = br.ReadUInt16();
            data.Heal = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgPushMinionRelay
    {
        public ushort Id;
        public MsgVector3 Dir;
        public short PushPower;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            Dir.Write(bw);
            bw.Write(PushPower);
        }

        public static MsgPushMinionRelay Read(BinaryReader br)
        {
            MsgPushMinionRelay data = new MsgPushMinionRelay();
            data.Id = br.ReadUInt16();
            data.Dir = MsgVector3.Read(br);
            data.PushPower = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgSetMinionAnimationTriggerRelay
    {
        public ushort Id;
        public ushort TargetId;
        public byte Trigger;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(TargetId);
            bw.Write(Trigger);
        }

        public static MsgSetMinionAnimationTriggerRelay Read(BinaryReader br)
        {
            MsgSetMinionAnimationTriggerRelay data = new MsgSetMinionAnimationTriggerRelay();
            data.Id = br.ReadUInt16();
            data.TargetId = br.ReadUInt16();
            data.Trigger = br.ReadByte();
            return data;
        }
    }


    [Serializable]

    public class MsgFireArrowRelay
    {
        public ushort Id;
        public MsgVector3 Dir;
        public int Damage;
        public short MoveSpeed;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            Dir.Write(bw);
            bw.Write(Damage);
            bw.Write(MoveSpeed);
        }

        public static MsgFireArrowRelay Read(BinaryReader br)
        {
            MsgFireArrowRelay data = new MsgFireArrowRelay();
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
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
        }

        public static MsgFireballBombRelay Read(BinaryReader br)
        {
            MsgFireballBombRelay data = new MsgFireballBombRelay();
            data.Id = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgMineBombRelay
    {
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
        }

        public static MsgMineBombRelay Read(BinaryReader br)
        {
            MsgMineBombRelay data = new MsgMineBombRelay();
            data.Id = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgSetMagicTargetIdRelay
    {
        public ushort Id;
        public ushort TargetId;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(TargetId);
        }

        public static MsgSetMagicTargetIdRelay Read(BinaryReader br)
        {
            MsgSetMagicTargetIdRelay data = new MsgSetMagicTargetIdRelay();
            data.Id = br.ReadUInt16();
            data.TargetId = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgSetMagicTargetRelay
    {
        public ushort Id;
        public short X;
        public short Z;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(X);
            bw.Write(Z);
        }

        public static MsgSetMagicTargetRelay Read(BinaryReader br)
        {
            MsgSetMagicTargetRelay data = new MsgSetMagicTargetRelay();
            data.Id = br.ReadUInt16();
            data.X = br.ReadInt16();
            data.Z = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgSturnMinionRelay
    {
        public ushort Id;
        public short SturnTime;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(SturnTime);
        }

        public static MsgSturnMinionRelay Read(BinaryReader br)
        {
            MsgSturnMinionRelay data = new MsgSturnMinionRelay();
            data.Id = br.ReadUInt16();
            data.SturnTime = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgRocketBombRelay
    {
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
        }

        public static MsgRocketBombRelay Read(BinaryReader br)
        {
            MsgRocketBombRelay data = new MsgRocketBombRelay();
            data.Id = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgIceBombRelay
    {
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
        }

        public static MsgIceBombRelay Read(BinaryReader br)
        {
            MsgIceBombRelay data = new MsgIceBombRelay();
            data.Id = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgDestroyMagicRelay
    {
        public ushort BaseStatId;

        public void Write(BinaryWriter bw)
        {
            bw.Write(BaseStatId);
        }

        public static MsgDestroyMagicRelay Read(BinaryReader br)
        {
            MsgDestroyMagicRelay data = new MsgDestroyMagicRelay();
            data.BaseStatId = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgFireCannonBallRelay
    {
        public MsgVector3 ShootPos;
        public MsgVector3 TargetPos;
        public int Power;
        public short Range;
        public byte Type;

        public void Write(BinaryWriter bw)
        {
            ShootPos.Write(bw);
            TargetPos.Write(bw);
            bw.Write(Power);
            bw.Write(Range);
            bw.Write(Type);
        }

        public static MsgFireCannonBallRelay Read(BinaryReader br)
        {
            MsgFireCannonBallRelay data = new MsgFireCannonBallRelay();
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
        public MsgVector3 ShootPos;
        public ushort TargetId;
        public int Power;
        public short MoveSpeed;

        public void Write(BinaryWriter bw)
        {
            ShootPos.Write(bw);
            bw.Write(TargetId);
            bw.Write(Power);
            bw.Write(MoveSpeed);
        }

        public static MsgFireSpearRelay Read(BinaryReader br)
        {
            MsgFireSpearRelay data = new MsgFireSpearRelay();
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
        public ushort Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
        }

        public static MsgFireManFireRelay Read(BinaryReader br)
        {
            MsgFireManFireRelay data = new MsgFireManFireRelay();
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
        public ushort Id;
        public bool IsCloacking;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(IsCloacking);
        }

        public static MsgMinionCloackingRelay Read(BinaryReader br)
        {
            MsgMinionCloackingRelay data = new MsgMinionCloackingRelay();
            data.Id = br.ReadUInt16();
            data.IsCloacking = br.ReadBoolean();
            return data;
        }
    }


    [Serializable]
    public class MsgMinionFlagOfWarRelay
    {
        public ushort BaseStatId;
        public short Effect;
        public bool IsFogOfWar;

        public void Write(BinaryWriter bw)
        {
            bw.Write(BaseStatId);
            bw.Write(Effect);
            bw.Write(IsFogOfWar);
        }

        public static MsgMinionFlagOfWarRelay Read(BinaryReader br)
        {
            MsgMinionFlagOfWarRelay data = new MsgMinionFlagOfWarRelay();
            data.BaseStatId = br.ReadUInt16();
            data.Effect = br.ReadInt16();
            data.IsFogOfWar = br.ReadBoolean();
            return data;
        }
    }


    [Serializable]
    public class MsgSendMessageVoidRelay
    {
        public ushort Id;
        public byte Message;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(Message);
        }

        public static MsgSendMessageVoidRelay Read(BinaryReader br)
        {
            MsgSendMessageVoidRelay data = new MsgSendMessageVoidRelay();
            data.Id = br.ReadUInt16();
            data.Message = br.ReadByte();
            return data;
        }
    }


    [Serializable]
    public class MsgSendMessageParam1Relay
    {
        public ushort Id;
        public ushort TargetId;
        public byte Message;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(TargetId);
            bw.Write(Message);
        }

        public static MsgSendMessageParam1Relay Read(BinaryReader br)
        {
            MsgSendMessageParam1Relay data = new MsgSendMessageParam1Relay();
            data.Id = br.ReadUInt16();
            data.TargetId = br.ReadUInt16();
            data.Message = br.ReadByte();
            return data;
        }
    }


    [Serializable]
    public class MsgNecromancerBulletRelay
    {
        public MsgVector3 ShootPos;
        public ushort TargetId;
        public int Power;
        public short BulletMoveSpeed;

        public void Write(BinaryWriter bw)
        {
            ShootPos.Write(bw);
            bw.Write(TargetId);
            bw.Write(Power);
            bw.Write(BulletMoveSpeed);
        }

        public static MsgNecromancerBulletRelay Read(BinaryReader br)
        {
            MsgNecromancerBulletRelay data = new MsgNecromancerBulletRelay();
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
        public ushort Id;
        public ushort TargetId;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(TargetId);
        }

        public static MsgSetMinionTargetRelay Read(BinaryReader br)
        {
            MsgSetMinionTargetRelay data = new MsgSetMinionTargetRelay();
            data.Id = br.ReadUInt16();
            data.TargetId = br.ReadUInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgScarecrowRelay
    {
        public ushort BaseStatId;
        public byte EyeLevel;

        public void Write(BinaryWriter bw)
        {
            bw.Write(BaseStatId);
            bw.Write(EyeLevel);
        }

        public static MsgScarecrowRelay Read(BinaryReader br)
        {
            MsgScarecrowRelay data = new MsgScarecrowRelay();
            data.BaseStatId = br.ReadUInt16();
            data.EyeLevel = br.ReadByte();
            return data;
        }
    }


    [Serializable]
    public class MsgLayzerTargetRelay
    {
        public ushort Id;
        public ushort[] TargetIdArray;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(TargetIdArray.Length);
            byte[] bytes = new byte[TargetIdArray.Length * sizeof(ushort)];
            Buffer.BlockCopy(TargetIdArray, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);
        }

        public static MsgLayzerTargetRelay Read(BinaryReader br)
        {
            MsgLayzerTargetRelay data = new MsgLayzerTargetRelay();
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
    public class MsgFireBulletRelay
    {
        public ushort Id;
        public ushort targetId;
        public int Damage;
        public short MoveSpeed;
        public byte Type;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(targetId);
            bw.Write(Damage);
            bw.Write(MoveSpeed);
            bw.Write(Type);
        }

        public static MsgFireBulletRelay Read(BinaryReader br)
        {
            MsgFireBulletRelay data = new MsgFireBulletRelay();
            data.Id = br.ReadUInt16();
            data.targetId = br.ReadUInt16();
            data.Damage = br.ReadInt32();
            data.MoveSpeed = br.ReadInt16();
            data.Type = br.ReadByte();
            return data;
        }
    }


    [Serializable]
    public class MsgMinionInvincibilityRelay
    {
        public ushort Id;
        public short Time;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Id);
            bw.Write(Time);
        }

        public static MsgMinionInvincibilityRelay Read(BinaryReader br)
        {
            MsgMinionInvincibilityRelay data = new MsgMinionInvincibilityRelay();
            data.Id = br.ReadUInt16();
            data.Time = br.ReadInt16();
            return data;
        }
    }


    [Serializable]
    public class MsgSpawnInfo
    {
        public ushort PlayerUId;
        public MsgSpawnMinion[] SpawnMinion;


        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);

            int length = (SpawnMinion == null) ? 0 : SpawnMinion.Length;
            bw.Write(length);
            for (int i = 0; i < length; i++)
            {
                SpawnMinion[i].Write(bw);
            }
        }

        public static MsgSpawnInfo Read(BinaryReader br)
        {
            MsgSpawnInfo data = new MsgSpawnInfo();
            data.PlayerUId = br.ReadUInt16();

            int length = br.ReadInt32();
            data.SpawnMinion = new MsgSpawnMinion[length];
            for (int i = 0; i < length; i++)
            {
                data.SpawnMinion[i] = MsgSpawnMinion.Read(br);
            }

            return data;
        }
    }

    [Serializable]
    public class MsgSpawnMinion
    {
        public byte SlotIndex;
        public ushort DiceId;
        public short DiceLevel;
        public short DiceInGameUp;
        public ushort[] Id;


        public void Write(BinaryWriter bw)
        {
            bw.Write(SlotIndex);
            bw.Write(DiceId);
            bw.Write(DiceLevel);
            bw.Write(DiceInGameUp);

            bw.Write(Id.Length);
            byte[] bytes = new byte[Id.Length * sizeof(ushort)];
            Buffer.BlockCopy(Id, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);
        }

        public static MsgSpawnMinion Read(BinaryReader br)
        {
            MsgSpawnMinion data = new MsgSpawnMinion();
            data.SlotIndex = br.ReadByte();
            data.DiceId = br.ReadUInt16();
            data.DiceLevel = br.ReadInt16();
            data.DiceInGameUp = br.ReadInt16();

            int length = br.ReadInt32();
            byte[] bytes = br.ReadBytes(length * sizeof(ushort));
            data.Id = new ushort[length];
            for (var index = 0; index < length; index++)
            {
                data.Id[index] = BitConverter.ToUInt16(bytes, index * sizeof(ushort));
            }

            return data;
        }
    }


    [Serializable]
    public class MsgRankInfo
    {
        public int Ranking;
        public string Name;
        public short Class;
        public int Trophy;
        public int[] DeckInfo;
    }

    //[Serializable]
    //public class QuestData
    //{
    //    public int QuestId;
    //    public int Value;
    //    public int Status;

    //    public void Write(BinaryWriter bw)
    //    {
    //        bw.Write(QuestId);
    //        bw.Write(Value);
    //        bw.Write(Status);
    //    }

    //    public static QuestData Read(BinaryReader br)
    //    {
    //        QuestData data = new QuestData();
    //        data.QuestId = br.ReadInt32();
    //        data.Value = br.ReadInt32();
    //        data.Status = br.ReadInt32();
    //        return data;
    //    }
    //}


    //[Serializable]
    //public class QuestInfo
    //{
    //    // 초기화 남은 시간(초단위)
    //    public int RemainResetTime;
    //    // 퀘스트 데이터
    //    public QuestData[] QuestData;
    //    // 일일 보상 정보
    //    public QuestDayReward DayRewardInfo;
    //}


    [Serializable]
    public class QuestDayReward
    {
        // 보상 아이디
        public int DayRewardId;
        // 보상 획득 여부
        public bool[] DayRewardState;
        // 일일 보상 획득까지 남은 시간(초단위)
        public int DayRewardRemainTime;
    }


    [Serializable]
    public class MsgSeasonPassInfo
    {
        // 시즌 패스 아이디
        public int SeasonPassId;
        // 시즌 패스 구매 여부
        public bool BuySeasonPass;
        // 시즌 트로피
        public int SeasonTrophy;
        // 시즌 초기화 남은 시간(초단위)
        public int SeasonResetRemainTime;
        // 시즌 패스 보상 획득 아이디 배열
        public int[] SeasonPassRewardIds;
        // 시즌 패스 보상 단계
        public int SeasonPassRewardStep;
        // 시즌 초기화 필요여부
        public bool NeedSeasonReset;
        // 프리 시즌 여부
        public bool IsFreeSeason;
    }

}
