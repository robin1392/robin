using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirageTest.Scripts;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("RandomWars")]
[Description("공격 범위에 대상이 있으면 True")]
public class IsTargetInAttackRange : ActionTask<Actor>
{
    public BBParameter<Actor> target;

    protected override void OnExecute()
    {
        if (target.value == null)
        {
            EndAction(false);
            return;
        }
        
        EndAction(agent.GetDistanceSqrMagnitudeWith(target.value) < 1);
    }
}