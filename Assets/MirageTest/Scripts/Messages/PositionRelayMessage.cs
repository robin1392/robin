using System;
using Mirage;
using RandomWarsProtocol;
using UnityEngine;

namespace MirageTest.Scripts.Messages
{
    public class PositionRelayMessage
    {
        public uint netId;
        public short positionX;
        public short positionY;
    }
    
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
    
    public class MatchDataMessage
    {
        public MatchPlayer Player1;
        public MatchPlayer Player2;
    }
}