using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ED;
using MirageTest.Scripts;

public class Guardian_02 : Minion
{
    private float _skillCastedTime;

    public override void Initialize()
    {
        base.Initialize();
        
        _skillCastedTime = -effectCooltime;
    }
    
    public override IEnumerator Attack()
    {
        if (_spawnedTime >= _skillCastedTime + effectCooltime)
        {
            _skillCastedTime = _spawnedTime;
            yield return SkillCoroutine();
        }

        yield return base.Attack();
    }
    
    IEnumerator SkillCoroutine()
    {
        ActorProxy.PlayAnimationWithRelay(AnimationHash.Skill, target);
        
        yield return new WaitForSeconds(0.8f);
        
        ActorProxy.AddBuff(BuffInfos.Invincibility, 2f);
        
        yield return new WaitForSeconds(0.2f);
    }
}
