using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using RWCoreNetwork;
using RWGameProtocol;
using RWGameProtocol.Msg;



public class SocketSendEvent
{

    private GamePacketSender _sender; 
    public SocketSendEvent(GamePacketSender sender)
    {
        this._sender = sender;
    }


    public void SendPacket(GameProtocol protocol , IPeer peer , params object[] param)
    {
        switch (protocol)
        {
            case GameProtocol.JOIN_GAME_REQ:
            {
                _sender.JoinGameReq(peer , (string)param[0], 1);
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
                float damage = (float)param[0] * Global.g_networkBaseValue;
                _sender.HitDamageReq(peer , (int)damage);
                break;
            }
            
            #region relay

            case GameProtocol.REMOVE_MINION_RELAY:
            {
                _sender.RemoveMinionRelay(peer , (int)param[0] , (int)param[1]);
                break;
            }
            case GameProtocol.DESTROY_MINION_RELAY:
            {
                _sender.DestroyMinionRelay(peer , (int)param[0] , (int)param[1]);
                break;
            }
            case GameProtocol.REMOVE_MAGIC_RELAY:
            {
                _sender.RemoveMagicRelay(peer , (int)param[0] , (int)param[1]);
                break;
            }
            case GameProtocol.HIT_DAMAGE_MINION_RELAY:
            {
                float damage = (float)param[2] * Global.g_networkBaseValue;
                float delay = (float)param[3] * Global.g_networkBaseValue;
                _sender.HitDamageMinionRelay(peer , (int)param[0] , (int)param[1] , (int)damage , (int)delay);
                break;
            }
            case GameProtocol.HEAL_MINION_RELAY:
            {
                float serverHeal = (float)param[2] * Global.g_networkBaseValue;
                _sender.HealMinionRelay(peer , (int)param[0] , (int)param[1] ,(int)serverHeal);
                break;
            }
            case GameProtocol.PUSH_MINION_RELAY:
            {
                //PushMinionRelay(IPeer peer, int playerUId, int id, int x, int y, int z, int pushPower)
                break;
            }
            case GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY:
            {
                _sender.SetMinionAnimationTriggerRelay(peer, (int) param[0], (int) param[1], (string) param[2]);
                break;
            }
            case GameProtocol.FIRE_ARROW_RELAY:
            {
                _sender.FireArrowRelay(peer , (int)param[0] , (int)param[1] ,(int)param[2] ,(int)param[3] ,(int)param[4] ,(int)param[5] ,(int)param[6] );
                break;
            }
            case GameProtocol.FIRE_BALL_BOMB_RELAY:
            {
                _sender.FireballBombRelay(peer ,(int)param[0] , (int)param[1]);
                break;
            }
            case GameProtocol.MINE_BOMB_RELAY:
            {
                _sender.MineBombRelay(peer , (int)param[0] , (int)param[1]);
                break;
            }
            case GameProtocol.SET_MAGIC_TARGET_ID_RELAY:
            {
                _sender.SetMagicTargetIdRelay(peer , (int)param[0] , (int)param[1] , (int)param[2]);
                break;
            }
            case GameProtocol.SET_MAGIC_TARGET_POS_RELAY:
            {
                _sender.SetMagicTargetRelay(peer , (int)param[0] , (int)param[1] , (int)param[2], (int)param[3]);
                break;
            }
            
            case GameProtocol.STURN_MINION_RELAY:
            {
                //SturnMinionRelay(IPeer peer, int playerUId, int id, int sturnTime)
                _sender.SturnMinionRelay(peer , (int)param[0] , (int)param[1] , (int)param[2]);
                break;
            }
            case GameProtocol.DESTROY_MAGIC_RELAY:
            {
                //MsgDestroyMagic(IPeer peer, int playerUId, int baseStatId)
                _sender.MsgDestroyMagic(peer ,(int)param[0] , (int)param[1]  );
                break;
            }
            case GameProtocol.ROCKET_BOMB_RELAY:
            {
                //RocketBombRelay(IPeer peer, int playerUId, int id)
                _sender.RocketBombRelay(peer  , (int)param[0] , (int)param[1] );
                break;
            }
            case GameProtocol.ICE_BOMB_RELAY:
            {
                //IceBombRelay(IPeer peer, int playerUId, int id)
                _sender.IceBombRelay(peer  , (int)param[0] ,(int)param[1]);
                break;
            }
            case GameProtocol.FIRE_CANNON_BALL_RELAY:
            {
                //MsgFireCannonBall(IPeer peer, int playerUId, MsgVector3 shootPos, MsgVector3 targetPos, int power, int range)

                Vector3 startPos = (Vector3) param[1];
                Vector3 targetPos = (Vector3) param[2];

                MsgVector3 chStPos = new MsgVector3();
                MsgVector3 chTgPos = new MsgVector3();
                
                chStPos.X = (int)(startPos.x * Global.g_networkBaseValue);
                chStPos.Y = (int)(startPos.y * Global.g_networkBaseValue);
                chStPos.Z = (int)(startPos.z * Global.g_networkBaseValue);
                
                chTgPos.X = (int)(startPos.x * Global.g_networkBaseValue);
                chTgPos.Y = (int)(startPos.y * Global.g_networkBaseValue);
                chTgPos.Z = (int)(startPos.z * Global.g_networkBaseValue);
                
                _sender.MsgFireCannonBall(peer ,(int)param[0] ,chStPos , chTgPos , (int)param[3] , (int)param[4]);
                break;
            }
            case GameProtocol.FIRE_SPEAR_RELAY:
            {
                //FireSpearRelay(IPeer peer, int playerUId, MsgVector3 shootPos, int targetId, int power)
                break;
            }
            case GameProtocol.FIRE_MAN_FIRE_RELAY:
            {
                //FireManFireRelay(IPeer peer, int playerUId, int id)
                break;
            }
            case GameProtocol.ACTIVATE_POOL_OBJECT_RELAY:
            {
                //ActivatePoolObjectRelay(IPeer peer, int playerUId, string poolName, MsgVector3 hitPos, MsgVector3 localScale, MsgQuaternion rotation)
                break;
            }
            case GameProtocol.MINION_CLOACKING_RELAY:
            {
                //MinionCloackingRelay(IPeer peer, int playerUId, int id, bool isCloacking)
                break;
            }
            case GameProtocol.MINION_FOG_OF_WAR_RELAY:
            {
                //MinionFogOfWarRelay(IPeer peer, int playerUId, int baseStatId, int effect, bool isFogOfWar)
                break;
            }
            case GameProtocol.SEND_MESSAGE_VOID_RELAY:
            {
                //SendMessageVoidRelay(IPeer peer, int playerUId, int id, string message)
                break;
            }
            case GameProtocol.SEND_MESSAGE_PARAM1_RELAY:
            {
                //SendMessageParam1Relay(IPeer peer, int playerUId, int id, int targetId, string message)
                break;
            }
            case GameProtocol.NECROMANCER_BULLET_RELAY:
            {
                //NecromancerBulletRelay(IPeer peer, int playerUId, MsgVector3 shootPos, int targetId, int power, int bulletMoveSpeed)
                break;
            }
            case GameProtocol.SET_MINION_TARGET_RELAY:
            {
                //SetMinionTargetRelay(IPeer peer, int playerUId, int id, int targetId)
                break;
            }
            case GameProtocol.MINION_STATUS_RELAY:
            {
                break;
            }
            
            #endregion
            
        }
    }
    
    
    
    
    
    
}
