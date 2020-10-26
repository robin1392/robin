using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using RandomWarsService.Network.NetService;
using RandomWarsProtocol;
using RandomWarsProtocol.Serializer;



public class SocketSendEvent
{

    private PacketSender _sender; 
    public SocketSendEvent(PacketSender sender)
    {
        this._sender = sender;
    }



    public void SendPacket(GameProtocol protocol , Peer peer , params object[] param)
    {
        switch (protocol)
        {
            case GameProtocol.JOIN_GAME_REQ:
            {
                _sender.JoinGameReq(peer , (string)param[0], (sbyte)param[1]);
                break;
            }
            case GameProtocol.LEAVE_GAME_REQ:
            {
                _sender.LeaveGameReq(peer);
                break;
            }
            case GameProtocol.READY_GAME_REQ:
            {
                _sender.ReadyGameReq(peer);
                break;
            }
            case GameProtocol.GET_DICE_REQ:
            {
                _sender.GetDiceReq(peer);
                break;
            }
            case GameProtocol.LEVEL_UP_DICE_REQ:
            {
                _sender.LevelUpDiceReq(peer , (short)param[0] , (short)param[1]);
                break;
            }
            case GameProtocol.UPGRADE_SP_REQ:
            {
                _sender.UpgradeSpReq(peer);
                break;
            }
            case GameProtocol.INGAME_UP_DICE_REQ:
            {
                _sender.InGameUpDiceReq(peer, (int) param[0]);
                break;
            }
            case GameProtocol.HIT_DAMAGE_REQ:
            {
                _sender.HitDamageReq(peer, Convert.ToUInt16(param[0]), (int)param[1]);
                break;
            }
            
            #region relay

            case GameProtocol.DESTROY_MINION_RELAY:
            {
                _sender.DestroyMinionRelay(peer , (ushort)param[0] , (ushort)param[1]);
                break;
            }
            case GameProtocol.DESTROY_MAGIC_RELAY:
            {
                //MsgDestroyMagic(IPeer peer, int playerUId, int baseStatId)
                _sender.MsgDestroyMagic(peer ,(ushort)param[0] , (ushort)param[1]  );
                break;
            }
            case GameProtocol.HIT_DAMAGE_MINION_RELAY:
            {
                int damage = ConvertNetMsg.MsgFloatToInt( (float)param[2] );
                _sender.HitDamageMinionRelay(peer , (ushort)param[0] , (ushort)param[1] , damage);
                break;
            }
            
            
            
            
            case GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY:
            {
                _sender.SetMinionAnimationTriggerRelay(peer, (ushort) param[0], (ushort) param[1], (ushort)param[3] , (byte) param[2] );
                break;
            }
            case GameProtocol.SET_MAGIC_TARGET_ID_RELAY:
            {
                _sender.SetMagicTargetIdRelay(peer , (ushort)param[0] , (ushort)param[1] , (ushort)param[2]);
                break;
            }
            case GameProtocol.SET_MAGIC_TARGET_POS_RELAY:
            {
                _sender.SetMagicTargetRelay(peer , (ushort)param[0] , (ushort)param[1] , (short)param[2], (short)param[3]);
                break;
            }
            
            
            
            case GameProtocol.HEAL_MINION_RELAY:
            {
                int serverHeal = ConvertNetMsg.MsgFloatToInt((float)param[2] );
                _sender.HealMinionRelay(peer , (ushort)param[0] , (ushort)param[1] , serverHeal);
                break;
            }
            case GameProtocol.FIRE_BALL_BOMB_RELAY:
            {
                _sender.FireballBombRelay(peer ,(ushort)param[0] , (ushort)param[1]);
                break;
            }
            case GameProtocol.MINE_BOMB_RELAY:
            {
                _sender.MineBombRelay(peer , (ushort)param[0] , (ushort)param[1]);
                break;
            }
            case GameProtocol.STURN_MINION_RELAY:
            {
                //SturnMinionRelay(IPeer peer, int playerUId, int id, int sturnTime)
                _sender.SturnMinionRelay(peer , (ushort)param[0] , (ushort)param[1] , (short)param[2]);
                break;
            }
            case GameProtocol.ROCKET_BOMB_RELAY:
            {
                //RocketBombRelay(IPeer peer, int playerUId, int id)
                _sender.RocketBombRelay(peer  , (ushort)param[0] , (ushort)param[1] );
                break;
            }
            case GameProtocol.ICE_BOMB_RELAY:
            {
                //IceBombRelay(IPeer peer, int playerUId, int id)
                _sender.IceBombRelay(peer  , (ushort)param[0] ,(ushort)param[1]);
                break;
            }
            case GameProtocol.FIRE_MAN_FIRE_RELAY:
            {
                //FireManFireRelay(IPeer peer, int playerUId, int id)
                _sender.FireManFireRelay(peer, (ushort)param[0] , (ushort)param[1]);
                break;
            }
            case GameProtocol.MINION_CLOACKING_RELAY:
            {
                //MinionCloackingRelay(IPeer peer, int playerUId, int id, bool isCloacking)
                _sender.MinionCloackingRelay(peer, (ushort) param[0], (ushort) param[1] ,(bool) param[2]);
                break;
            }
            case GameProtocol.MINION_FLAG_OF_WAR_RELAY:
            {
                //MinionFogOfWarRelay(IPeer peer, int playerUId, int baseStatId, int effect, bool isFogOfWar)
                _sender.MinionFogOfWarRelay(peer , (ushort)param[0], (ushort)param[1] , (short)param[2] , (bool)param[3]);
                break;
            }
            case GameProtocol.SCARECROW_RELAY:
            {
                //ScarecrowRelay(IPeer peer, int playerUId, int baseStatId, int eyeLevel)
                _sender.ScarecrowRelay(peer , (ushort)param[0] , (ushort)param[1] , (byte)param[2]);
                break;
            }
            case GameProtocol.LAYZER_TARGET_RELAY:
            {
                //LayzerTargetRelay(IPeer peer, int playerUId, int id, int[] targetId)
                _sender.LayzerTargetRelay(peer , (ushort)param[0] , (ushort)param[1] , (ushort[]) param[2]);
                
                break;
            }
            case GameProtocol.MINION_INVINCIBILITY_RELAY:
            {
                //MinionInvincibilityRelay(Peer peer, int playerUId, int id, int time)
                _sender.MinionInvincibilityRelay(peer , (ushort)param[0] , (ushort)param[1] , (short)param[2]);
                break;
            }
            
            
            //
            case GameProtocol.FIRE_BULLET_RELAY:
            {
                //FireBulletRelay(Peer peer, int playerUId, int id, int x, int y, int z, int damage, int moveSpeedk, int type)
                _sender.FireBulletRelay(peer , (ushort)param[0] , (ushort)param[1] , new MsgVector3 { X = (short)param[2], Y = (short)param[3], Z = (short)param[4] } ,(int)param[5] ,(short)param[6] , (byte)param[7] );
                break;
            }
            case GameProtocol.FIRE_ARROW_RELAY:
            {
                _sender.FireArrowRelay(peer , (ushort)param[0] , (ushort)param[1] , new MsgVector3 { X = (short)param[2], Y = (short)param[3], Z = (short)param[4] }, (int)param[5] ,(short)param[6] );
                break;
            }
            case GameProtocol.FIRE_SPEAR_RELAY:
            {
                //FireSpearRelay(IPeer peer, int playerUId, MsgVector3 shootPos, int targetId, int power)
                
                MsgVector3 chStPos = ConvertNetMsg.VectorToMsg((Vector3) param[1]);
                
                _sender.FireSpearRelay(peer , (ushort)param[0] , chStPos , (ushort)param[2] , (int)param[3] , (short)param[4]);
                break;
            }
            case GameProtocol.NECROMANCER_BULLET_RELAY:
            {
                //NecromancerBulletRelay(IPeer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int bulletMoveSpeed)
                
                MsgVector3 chStPos = ConvertNetMsg.VectorToMsg((Vector3) param[1]);
                
                _sender.NecromancerBulletRelay(peer , (ushort)param[0] , chStPos , (ushort)param[2] , (int)param[3] , (short)param[4]);
                
                break;
            }
            case GameProtocol.FIRE_CANNON_BALL_RELAY:
            {
                //MsgFireCannonBall(IPeer peer, int playerUId, MsgVector3 shootPos, MsgVector3 targetPos, int power, int range)
                
                MsgVector3 chStPos = ConvertNetMsg.VectorToMsg((Vector3) param[1]);
                MsgVector3 chTgPos = ConvertNetMsg.VectorToMsg((Vector3) param[2]);
                
                _sender.MsgFireCannonBall(peer ,(ushort)param[0] ,chStPos , chTgPos , (int)param[3] , (short)param[4] , (byte)param[5]);
                break;
            }
            
            
            
            
            case GameProtocol.ACTIVATE_POOL_OBJECT_RELAY:
            {
                //ActivatePoolObjectRelay(IPeer peer, int playerUId, string poolName, MsgVector3 hitPos, MsgVector3 localScale, MsgQuaternion rotation)
                
                MsgVector3 chStPos = ConvertNetMsg.VectorToMsg((Vector3) param[1]);
                MsgQuaternion chRot = ConvertNetMsg.QuaternionToMsg((Quaternion) param[2]);
                MsgVector3 chScale = ConvertNetMsg.VectorToMsg((Vector3) param[3]);
                
                _sender.ActivatePoolObjectRelay(peer , (int)param[0] , chStPos , chScale, chRot);
                
                break;
            }
            case GameProtocol.SEND_MESSAGE_VOID_RELAY:
            {
                //SendMessageVoidRelay(IPeer peer, int playerUId, int id, string message)
                _sender.SendMessageVoidRelay(peer , (ushort)param[0] , (ushort)param[1] , (byte)param[2]);
                break;
            }
            case GameProtocol.SEND_MESSAGE_PARAM1_RELAY:
            {
                //SendMessageParam1Relay(IPeer peer, int playerUId, int id, int targetId, string message)
                _sender.SendMessageParam1Relay(peer , (ushort)param[0] , (ushort)param[1] , (ushort)param[3] , (byte)param[2]);
                break;
            }
            case GameProtocol.SET_MINION_TARGET_RELAY:
            {
                //SetMinionTargetRelay(IPeer peer, int playerUId, int id, int targetId)
                
                _sender.SetMinionTargetRelay(peer , (ushort)param[0] , (ushort)param[1] , (ushort)param[2]);
                break;
            }
            case GameProtocol.PUSH_MINION_RELAY:
            {
                //PushMinionRelay(IPeer peer, int playerUId, int id, int x, int y, int z, int pushPower)
                
                _sender.PushMinionRelay(peer , (ushort)param[0] , (ushort)param[1] , new MsgVector3 { X = (short)param[2], Y = (short)param[3], Z = (short)param[4] }, (short)param[5] );
                break;
            }
            
            
            
            
            case GameProtocol.MINION_STATUS_RELAY:
            {
                    //MinionStatusRelay(IPeer peer, int playerUId, byte posIndex, MsgVector3[] pos)
                    _sender.MinionStatusRelay(peer , Convert.ToUInt16(param[0]), (byte)param[1] , (MsgVector2[])param[2], (int[])param[3], (MsgMinionStatus)param[4], (int)param[5]);
                break;
            }
            
            
            
            
            // reconnect , pause , etc...
            case GameProtocol.START_SYNC_GAME_REQ:
            {
                _sender.StartSyncGameReq(peer , (int)param[0] , (int)param[1], (MsgSyncMinionData[])param[2] , (int)param[3] , (int)param[4], (MsgSyncMinionData[])param[5]);
                break;
            }
            case GameProtocol.END_SYNC_GAME_REQ:
            {
                _sender.EndSyncGameReq(peer);
                break;
            }
            
            case GameProtocol.RECONNECT_GAME_REQ:
            {
                _sender.ReconnectGameReq(peer);
                break;
            }
            case GameProtocol.READY_SYNC_GAME_REQ:
            {
                _sender.ReadySyncGameReq(peer);
                break;
            }
            
            #endregion
            
        }
    }
    
    
    
    
    
    
}
