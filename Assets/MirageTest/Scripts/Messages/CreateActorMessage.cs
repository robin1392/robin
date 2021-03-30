using UnityEngine;

namespace MirageTest.Scripts.Messages
{
    public class CreateActorMessage
    {
        public int diceId;
        public byte ownerTag;
        public byte team;
        public byte inGameLevel;
        public byte outGameLevel;
        public Vector3[] positions;
        public float delay;
    }
}