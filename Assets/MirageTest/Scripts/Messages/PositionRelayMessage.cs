using System;
using Mirage;
using RandomWarsProtocol;

namespace MirageTest.Scripts.Messages
{
    public class PositionRelayMessage
    {
        public uint netId;
        public sbyte positionX;
        public sbyte positionY;
    }
}