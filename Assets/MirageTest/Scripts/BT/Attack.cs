using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirageTest.Scripts;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("RandomWars")]
[Description("target을 공격")]
public class Attack : ActionTask<Actor>
{
    public BBParameter<Actor> target;
    private float elapsed = 0;

    protected override void OnExecute()
    {
        agent.animator.SetBool( Animator.StringToHash("Attack"), true);
        agent.transform.LookAt(target.value.transform);
    }

    protected override void OnUpdate()
    {
        if (elapsedTime > 1)
        {
            target.value.Damage(1);
            EndAction(true);
        }
    }

    protected override void OnStop()
    {
        agent.animator.SetBool( Animator.StringToHash("Attack"), false);
    }
}