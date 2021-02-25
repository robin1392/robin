using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;
using UnityEngine.Serialization;

namespace MirageTest.Scripts
{
    public class ActorPathfinding : MonoBehaviour
    {
        public Seeker seeker;
        public RVOController rvoController;
        public AIPath aIPath;
        public AIDestinationSetter aIDestinationSetter;
        
        public void EnableAIPathfinding(bool b)
        {
            seeker.enabled = b;
            if (b == false)
            {
                rvoController.layer = 0;
                rvoController.collidesWith = 0;
            }
            rvoController.enabled = b;
            aIPath.enabled = b;
            aIDestinationSetter.enabled = b;
        }

        public void SetTarget(Transform target)
        {
            aIDestinationSetter.target = target;
        }
    }
}