using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MirageTest.Scripts;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

[Category("RandomWars")]
[Description("사정거리에 들어 올때까지 target을 향해 이동")]
public class MoveToTargetUntilInAttackRange : ActionTask<ActorProxy>
{
    public BBParameter<Actor> target;

    protected override void OnExecute()
    {
        if (target.value == null)
        {
            EndAction(false);
            return;
        }
        
        agent.actorPathfinding.aIPath.canMove = true;
        agent.actorPathfinding.aIDestinationSetter.target = target.value.transform;

        Update();
    }

    protected override void OnUpdate()
    {
        Update();
    }

    void Update()
    {
        if (target.value.IsAlive == false)
        {
            Stop();
            EndAction(false);
            return;
        }
        
        if (agent.actor.GetDistanceSqrMagnitudeWith(target.value) < 1)
        {
            Stop();
            EndAction(true);
        }
    }

    void Stop()
    {
        agent.actorPathfinding.aIPath.canMove = false;
        agent.actorPathfinding.aIDestinationSetter.target = null;
    }

    protected override void OnStop()
    {
        Stop();
    }
}