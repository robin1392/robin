using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PLAY_TYPE
{
    BATTLE,
    CO_OP,
}

/// <summary>
/// 포톤 통신 타입 정의
/// </summary>
public enum E_PTDefine
{
    PT_NONE,
    PT_READY,
    PT_CHANGELAYER,
    PT_DEACTIVEWAIT,
    PT_ADDSP,
    PT_SETDECK,
    PT_SPAWNMINION,
    PT_SPAWN,
    PT_ENDGAME,
    PT_REMOVEMINION,
    PT_GETDICE,
    PT_REMOVEMAGIC,
    PT_HITMINIONANDMAGIC,
    PT_HITDAMAGE,
    PT_DESTROYMINION,
    PT_DESTROYMAGIC,
    PT_HEALMINION,
    PT_LEVELUPDICE,
    PT_NICKNAME,
    
    // -- unit pt define
    PT_FIREBULLET,
    PT_FIREBALLBOMB,
    PT_ICEBALLBOMB,
    PT_ROCKETBOMB,
    PT_MINIONATTACKSPEEDFACTOR,
    PT_STURNMINION,
    PT_SETMAGICTARGET,
    PT_MINEBOMB,
    PT_FIRECANNONBALL,
    PT_MINIONANITRIGGER,
    PT_FIREMANFIRE,
    PT_SPAWNSKELETON,
    PT_TELEPORTMINION,
    PT_LAYZERTARGET,
    PT_MINIONINVINCIBILITY,
    PT_SCARECROW,
    PT_ACTIVATEPOOLOBJECT,
    PT_MINIONCLOACKING,
    PT_MINIONFOGOFWAR,
    PT_SENDMESSAGEVOID,
    PT_SENDMESSAGEPARAM1,
    PT_SETMINIONTARGET,
    PT_PUSHMINION,
    PT_MAX,
}

public enum E_MaterialType
{
    BOTTOM,
    TOP,
    HALFTRANSPARENT,
    TRANSPARENT,
}

public enum E_CannonType
{
    DEFAULT,
    BOMBER,
}

public enum E_BulletType
{
    ARROW,
    SPEAR,
    NECROMANCER,
    MAGICIAN,
    ARBITER,
    BABYDRAGON,
    VALLISTA_SPEAR,
}

public enum E_AniTrigger
{
    Idle,
    Attack,
    Skill,
    Attack1,
    Attack2,
    AttackReady,
}

public enum E_ActionSendMessage
{
    LookAndAniTrigger,
    DashMessage,
    Aiming,
    StopAiming,
    JumpTarget,
    Skill,
    FireBullet,
}

public enum E_PoolName
{
    None,
    Arrow,
    Spear,
    Effect_Death,
    Effect_SpawnLine,
    Effect_Poison,
    Effect_Bomb,
    Effect_Lightning,
    Effect_Sturn,
    Effect_ShotHit,
    Effect_ArrowHit,
    Effect_Robot_Summon,
    Effect_Heal,
    Effect_Cloaking,
    Effect_Shield,
    Babydragon_Bullet,
    Bomber_Bullet,
    CannonBall,
    Magician_Bullet,
    Necromancer_Bullet,
    Scarecrow,
    Effect_Dash,
    Effect_Dust,
    Effect_Saint,
    Effect_Stone,
    Effect_Support,
    particle_wizard_skill_book,
}