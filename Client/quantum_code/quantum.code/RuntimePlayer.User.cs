using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum {
  partial class RuntimePlayer {
    public string Nickname;
    public int[] DeckDiceIds;
    public int[] DeckDiceOutGameLevels;
    public int GuardianId;
    
    partial void SerializeUserData(BitStream stream)
    {
        stream.Serialize(ref Nickname);
        stream.Serialize(ref DeckDiceIds);
        stream.Serialize(ref DeckDiceOutGameLevels);
        stream.Serialize(ref GuardianId);
    }
  }
}
