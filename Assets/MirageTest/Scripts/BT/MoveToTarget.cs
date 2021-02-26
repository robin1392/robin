using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirageTest.Scripts;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("RandomWars")]
[Description("target을 향해 이동")]
public class MoveToTarget : ActionTask<ActorPathfinding>
{
    public BBParameter<Actor> target;

    protected override void OnExecute()
    {
        if (target.value == null)
        {
            EndAction(false);
            return;
        }
        
        agent.aIDestinationSetter.target = target.value.transform;
    }

    protected override void OnUpdate()
    {
        // if (target.value.IsAlive == false)
        // {
        //     EndAction(false);
        //     return;
        // }

        if (agent.aIPath.reachedDestination)
        {
            EndAction(true);
        }
    }

    protected override void OnStop()
    {
        agent.aIDestinationSetter.target = null;
    }
}