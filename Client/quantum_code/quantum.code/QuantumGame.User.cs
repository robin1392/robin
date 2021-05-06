using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quantum {
    partial class QuantumGame {
        
        public bool IsPlayableLocalPlayer(PlayerRef playerRef)
        {
            return GetLocalPlayers()[0] == playerRef;
        }
    }
}
