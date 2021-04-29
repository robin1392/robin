using Photon.Deterministic;
using System;

namespace Quantum
{
    partial class RuntimeConfig
    {
        public int Mode;

        partial void SerializeUserData(BitStream stream)
        {
            stream.Serialize(ref Mode);
        }
    }
}