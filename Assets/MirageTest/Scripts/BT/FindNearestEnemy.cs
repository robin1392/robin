using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirageTest.Scripts;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("RandomWars")]
[Description("가장 가까운 적을 블랙보드 target에 설정, 타겟을 찾으면 True")]
public class FindNearestEnemy : ActionTask<Actor>
{
    public BBParameter<Actor> target;
    protected override string info => $"{target} = {agentInfo}.FindNearestEnemy()";

    protected override void OnExecute()
    {
        Actor nearestActor = null;
        var nearestDistanceSqr = float.MaxValue; 
            
        var cols = Physics.OverlapSphere(agent.transform.position, 999);
        var actors = cols.Select(col => col.GetComponentInParent<Actor>()).Where(actor => actor != null);
        var nearestEnemy = agent.GetNearestEnemy(actors);
        target.value = nearestEnemy;
        EndAction(target != null);
    }
}
