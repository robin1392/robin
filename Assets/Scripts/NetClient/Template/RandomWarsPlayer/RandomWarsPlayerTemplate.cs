using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Player.RandomWarsPlayer.Common;

namespace Template.Player.RandomWarsPlayer
{
    public partial class RandomWarsPlayerTemplate : RandomWarsPlayerProtocol
    {
        public RandomWarsPlayerTemplate()
        {
            ReceiveEditNameAckCallback = OnEditNameController;
        }

    }
}
