﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
    PT_HITMINION,
    PT_HITDAMAGE,
    PT_DESTROYMINION,
    PT_HEALMINION,
    PT_LEVELUPDICE,
    
    // -- unit pt define
    PT_FIREBALLBOMB,
    PT_ICEBALLBOMB,
    PT_MINIONATTACKSPEEDFACTOR,
    PT_STURNMINION,
    PT_SETMAGICTARGET,
    PT_MINEBOMB,
    PT_FIRECANNONBALL,
    PT_FIREARROW,
    PT_MINIONANITRIGGER,
    PT_FIREMANFIRE,
    PT_FIRESPEAR,
    PT_SPAWNSKELETON,
    PT_TELEPORTMINION,
    PT_MAX,
}