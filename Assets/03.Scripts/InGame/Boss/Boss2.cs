﻿using System.Collections;
using System.Collections.Generic;
using ED;
using Microsoft.Win32.SafeHandles;
using RandomWarsProtocol;
using UnityEngine;

public class Boss2 : Minion
{
    private float _skillCastedTime;
    private bool _isSkillCasting;
    private float _localAttackSpeed = 1f;

    public override void Initialize()
    {
        base.Initialize();
        _skillCastedTime = -effectCooltime;
        //KZSee:
        // attackSpeed = 1f;
        // effectCooltime = 1f;
        Skill();
    }

    public void Skill()
    {
        StartCoroutine(SkillCoroutine());
    }

    IEnumerator SkillCoroutine()
    {
        float originAttackSpeed = attackSpeed;
        float animationSpeed = 1f;
        int loopCount = 1;
        
        while (true)
        {
            yield return new WaitForSeconds(effectCooltime);

            loopCount++;
            _localAttackSpeed = Mathf.Clamp(1f + 0.05f * loopCount, 1f, 5f);
            //KZSee:
            // attackSpeed = 1f / _localAttackSpeed;
            animator.SetFloat("AttackSpeed", _localAttackSpeed);
        }
    }
}
