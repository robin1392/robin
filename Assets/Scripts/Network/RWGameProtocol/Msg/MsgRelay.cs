﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace RWGameProtocol.Msg
{
    [Serializable]
    public struct MsgVector3
    {
        public int X;
        public int Y;
        public int Z;

        public void Write(BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
        }

        public static MsgVector3 Read(BinaryReader br)
        {
            MsgVector3 data = new MsgVector3();
            data.X = br.ReadInt32();
            data.Y = br.ReadInt32();
            data.Z = br.ReadInt32();
            return data;
        }
    }

    [Serializable]
    public struct MsgQuaternion
    {
        public int X;
        public int Y;
        public int Z;
        public int W;

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
            data.X = br.ReadInt32();
            data.Y = br.ReadInt32();
            data.Z = br.ReadInt32();
            data.W = br.ReadInt32();
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

        public void Read(BinaryReader br)
        {
            int length = br.ReadInt32();
            if (length > 0)
            {
                arrHitDamageMinionRelay = new MsgHitDamageMinionRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrHitDamageMinionRelay[i] = MsgHitDamageMinionRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrDestroyMinionRelay = new MsgDestroyMinionRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrDestroyMinionRelay[i] = MsgDestroyMinionRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrDestroyMagicRelay = new MsgDestroyMagicRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrDestroyMagicRelay[i] = MsgDestroyMagicRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrFireballBombRelay = new MsgFireballBombRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrFireballBombRelay[i] = MsgFireballBombRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrHealMinionRelay = new MsgHealMinionRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrHealMinionRelay[i] = MsgHealMinionRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrMineBombRelay = new MsgMineBombRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrMineBombRelay[i] = MsgMineBombRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrSturnMinionRelay = new MsgSturnMinionRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrSturnMinionRelay[i] = MsgSturnMinionRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrRocketBombRelay = new MsgRocketBombRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrRocketBombRelay[i] = MsgRocketBombRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrIceBombRelay = new MsgIceBombRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrIceBombRelay[i] = MsgIceBombRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrFireManFireRelay = new MsgFireManFireRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrFireManFireRelay[i] = MsgFireManFireRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrMinionCloackingRelay = new MsgMinionCloackingRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrMinionCloackingRelay[i] = MsgMinionCloackingRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrMinionFlagOfWarRelay = new MsgMinionFlagOfWarRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrMinionFlagOfWarRelay[i] = MsgMinionFlagOfWarRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrScarercrowRelay = new MsgScarecrowRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrScarercrowRelay[i] = MsgScarecrowRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrLayzerTargetRelay = new MsgLayzerTargetRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrLayzerTargetRelay[i] = MsgLayzerTargetRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrMinionInvincibilityRelay = new MsgMinionInvincibilityRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrMinionInvincibilityRelay[i] = MsgMinionInvincibilityRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrFireBulletRelay = new MsgFireBulletRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrFireBulletRelay[i] = MsgFireBulletRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrFireCannonBallRelay = new MsgFireCannonBallRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrFireCannonBallRelay[i] = MsgFireCannonBallRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrMinionAnimationTriggerRelay = new MsgSetMinionAnimationTriggerRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrMinionAnimationTriggerRelay[i] = MsgSetMinionAnimationTriggerRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrMagicTargetIdRelay = new MsgSetMagicTargetIdRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrMagicTargetIdRelay[i] = MsgSetMagicTargetIdRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrMagicTargetRelay = new MsgSetMagicTargetRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrMagicTargetRelay[i] = MsgSetMagicTargetRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrActivatePoolObjectRelay = new MsgActivatePoolObjectRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrActivatePoolObjectRelay[i] = MsgActivatePoolObjectRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrSendMessageVoidRelay = new MsgSendMessageVoidRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrSendMessageVoidRelay[i] = MsgSendMessageVoidRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrSendMessageParam1Relay = new MsgSendMessageParam1Relay[length];
                for (int i = 0; i < length; i++)
                {
                    arrSendMessageParam1Relay[i] = MsgSendMessageParam1Relay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrMinionTargetRelay = new MsgSetMinionTargetRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrMinionTargetRelay[i] = MsgSetMinionTargetRelay.Read(br);
                }
            }

            length = br.ReadInt32();
            if (length > 0)
            {
                arrPushMinionRelay = new MsgPushMinionRelay[length];
                for (int i = 0; i < length; i++)
                {
                    arrPushMinionRelay[i] = MsgPushMinionRelay.Read(br);
                }
            }
        }
    }


    [Serializable]
    public class MsgRemoveMinionRelay
    {
        public int PlayerUId;
        public int Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgRemoveMinionRelay Read(BinaryReader br)
        {
            MsgRemoveMinionRelay data = new MsgRemoveMinionRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgHitDamageMinionRelay
    {
        public int PlayerUId;
        public int Id;
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
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.Damage = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgDestroyMinionRelay
    {
        public int PlayerUId;
        public int Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgDestroyMinionRelay Read(BinaryReader br)
        {
            MsgDestroyMinionRelay data = new MsgDestroyMinionRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgHealMinionRelay
    {
        public int PlayerUId;
        public int Id;
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
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.Heal = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgPushMinionRelay
    {
        public int PlayerUId;
        public int Id;
        public MsgVector3 Dir;
        public int PushPower;

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
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.Dir = MsgVector3.Read(br);
            data.PushPower = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgSetMinionAnimationTriggerRelay
    {
        public int PlayerUId;
        public int Id;
        public int TargetId;
        public int Trigger;

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
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.TargetId = br.ReadInt32();
            data.Trigger = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgRemoveMagicRelay
    {
        public int PlayerUId;
        public int Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgRemoveMagicRelay Read(BinaryReader br)
        {
            MsgRemoveMagicRelay data = new MsgRemoveMagicRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    
    public class MsgFireArrowRelay
    {
        public int PlayerUId;
        public int Id;
        public MsgVector3 Dir;
        public int Damage;
        public int MoveSpeed;

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
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.Dir = MsgVector3.Read(br);
            data.Damage = br.ReadInt32();
            data.MoveSpeed = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgFireballBombRelay
    {
        public int PlayerUId;
        public int Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgFireballBombRelay Read(BinaryReader br)
        {
            MsgFireballBombRelay data = new MsgFireballBombRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgMineBombRelay
    {
        public int PlayerUId;
        public int Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgMineBombRelay Read(BinaryReader br)
        {
            MsgMineBombRelay data = new MsgMineBombRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgSetMagicTargetIdRelay
    {
        public int PlayerUId;
        public int Id;
        public int TargetId;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(TargetId);
        }

        public static MsgSetMagicTargetIdRelay Read(BinaryReader br)
        {
            MsgSetMagicTargetIdRelay data = new MsgSetMagicTargetIdRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.TargetId = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgSetMagicTargetRelay
    {
        public int PlayerUId;
        public int Id;
        public int X;
        public int Z;

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
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.X = br.ReadInt32();
            data.Z = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgSturnMinionRelay
    {
        public int PlayerUId;
        public int Id;
        public int SturnTime;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(SturnTime);
        }

        public static MsgSturnMinionRelay Read(BinaryReader br)
        {
            MsgSturnMinionRelay data = new MsgSturnMinionRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.SturnTime = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgRocketBombRelay
    {
        public int PlayerUId;
        public int Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgRocketBombRelay Read(BinaryReader br)
        {
            MsgRocketBombRelay data = new MsgRocketBombRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgIceBombRelay
    {
        public int PlayerUId;
        public int Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgIceBombRelay Read(BinaryReader br)
        {
            MsgIceBombRelay data = new MsgIceBombRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgDestroyMagicRelay
    {
        public int PlayerUId;
        public int BaseStatId;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(BaseStatId);
        }

        public static MsgDestroyMagicRelay Read(BinaryReader br)
        {
            MsgDestroyMagicRelay data = new MsgDestroyMagicRelay();
            data.PlayerUId = br.ReadInt32();
            data.BaseStatId = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgFireCannonBallRelay
    {
        public int PlayerUId;
        public MsgVector3 ShootPos;
        public MsgVector3 TargetPos;
        public int Power;
        public int Range;
        public int Type;

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
            data.PlayerUId = br.ReadInt32();
            data.ShootPos = MsgVector3.Read(br);
            data.TargetPos = MsgVector3.Read(br);
            data.Power = br.ReadInt32();
            data.Range = br.ReadInt32();
            data.Type = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgFireSpearRelay
    {
        public int PlayerUId;
        public MsgVector3 ShootPos;
        public int TargetId;
        public int Power;
        public int MoveSpeed;

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
            data.PlayerUId = br.ReadInt32();
            data.ShootPos = MsgVector3.Read(br);
            data.TargetId = br.ReadInt32();
            data.Power = br.ReadInt32();
            data.MoveSpeed = br.ReadInt32();
            return data;
        }

    }


    [Serializable]
    public class MsgFireManFireRelay
    {
        public int PlayerUId;
        public int Id;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
        }

        public static MsgFireManFireRelay Read(BinaryReader br)
        {
            MsgFireManFireRelay data = new MsgFireManFireRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgActivatePoolObjectRelay
    {
        public int PlayerUId;
        public int PoolName;
        public MsgVector3 HitPos;
        public MsgVector3 LocalScale;
        public MsgQuaternion Rotation;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(PoolName);
            HitPos.Write(bw);
            LocalScale.Write(bw);
            Rotation.Write(bw);
        }

        public static MsgActivatePoolObjectRelay Read(BinaryReader br)
        {
            MsgActivatePoolObjectRelay data = new MsgActivatePoolObjectRelay();
            data.PlayerUId = br.ReadInt32();
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
        public int PlayerUId;
        public int Id;
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
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.IsCloacking = br.ReadBoolean();
            return data;
        }
    }


    [Serializable]
    public class MsgMinionFlagOfWarRelay
    {
        public int PlayerUId;
        public int BaseStatId;
        public int Effect;
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
            data.PlayerUId = br.ReadInt32();
            data.BaseStatId = br.ReadInt32();
            data.Effect = br.ReadInt32();
            data.IsFogOfWar = br.ReadBoolean();
            return data;
        }
    }


    [Serializable]
    public class MsgSendMessageVoidRelay
    {
        public int PlayerUId;
        public int Id;
        public int Message;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(Message);
        }

        public static MsgSendMessageVoidRelay Read(BinaryReader br)
        {
            MsgSendMessageVoidRelay data = new MsgSendMessageVoidRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.Message = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgSendMessageParam1Relay
    {
        public int PlayerUId;
        public int Id;
        public int TargetId;
        public int Message;

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
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.TargetId = br.ReadInt32();
            data.Message = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgNecromancerBulletRelay
    {
        public int PlayerUId;
        public MsgVector3 ShootPos;
        public int TargetId;
        public int Power;
        public int BulletMoveSpeed;

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
            data.PlayerUId = br.ReadInt32();
            data.ShootPos = MsgVector3.Read(br);
            data.TargetId = br.ReadInt32();
            data.Power = br.ReadInt32();
            data.BulletMoveSpeed = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgSetMinionTargetRelay
    {
        public int PlayerUId;
        public int Id;
        public int TargetId;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(TargetId);
        }

        public static MsgSetMinionTargetRelay Read(BinaryReader br)
        {
            MsgSetMinionTargetRelay data = new MsgSetMinionTargetRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.TargetId = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgScarecrowRelay
    {
        public int PlayerUId;
        public int BaseStatId;
        public int EyeLevel;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(BaseStatId);
            bw.Write(EyeLevel);
        }

        public static MsgScarecrowRelay Read(BinaryReader br)
        {
            MsgScarecrowRelay data = new MsgScarecrowRelay();
            data.PlayerUId = br.ReadInt32();
            data.BaseStatId = br.ReadInt32();
            data.EyeLevel = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgLayzerTargetRelay
    {
        public int PlayerUId;
        public int Id;
        public int[] TargetIdArray;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(TargetIdArray.Length);
            byte[] bytes = new byte[TargetIdArray.Length * sizeof(int)];
            Buffer.BlockCopy(TargetIdArray, 0, bytes, 0, bytes.Length);
            bw.Write(bytes);
        }

        public static MsgLayzerTargetRelay Read(BinaryReader br)
        {
            MsgLayzerTargetRelay data = new MsgLayzerTargetRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();

            int length = br.ReadInt32();
            byte[] bytes = br.ReadBytes(length * sizeof(int));
            data.TargetIdArray = new int[length];
            for (var index = 0; index < length; index++)
            {
                data.TargetIdArray[index] = BitConverter.ToInt32(bytes, index * sizeof(int));
            }

            return data;
        }
    }


    [Serializable]
    public class MsgMinionStatusRelay
    {
        public int PlayerUId;
        public byte PosIndex;
        public MsgVector3[] Pos;
        public MsgMinionStatus Relay;
        public int packetCount;
    }


    [Serializable]
    public class MsgFireBulletRelay
    {
        public int PlayerUId;
        public int Id;
        public MsgVector3 Dir;
        public int Damage;
        public int MoveSpeed;
        public int Type;

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
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.Dir = MsgVector3.Read(br);
            data.Damage = br.ReadInt32();
            data.MoveSpeed = br.ReadInt32();
            data.Type = br.ReadInt32();
            return data;
        }
    }


    [Serializable]
    public class MsgMinionInvincibilityRelay
    {
        public int PlayerUId;
        public int Id;
        public int Time;

        public void Write(BinaryWriter bw)
        {
            bw.Write(PlayerUId);
            bw.Write(Id);
            bw.Write(Time);
        }

        public static MsgMinionInvincibilityRelay Read(BinaryReader br)
        {
            MsgMinionInvincibilityRelay data = new MsgMinionInvincibilityRelay();
            data.PlayerUId = br.ReadInt32();
            data.Id = br.ReadInt32();
            data.Time = br.ReadInt32();
            return data;
        }
    }
}
