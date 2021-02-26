using UnityEngine;

namespace MirageTest.Scripts
{
    public class ActorProxyTransformParentSetter : NetworkBehaviourParentSetter
    {
        protected override string ParentName => "Actors";
    }
}