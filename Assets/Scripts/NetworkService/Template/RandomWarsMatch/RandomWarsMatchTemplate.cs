using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Stage.RandomWarsMatch.Common;


namespace Template.Stage.RandomWarsMatch
{
    public partial class RandomWarsMatchTemplate : RandomWarsMatchProtocol
    {
        public RandomWarsMatchTemplate()
        {
            ReceiveRequestMatchAckCallback = OnRequestMatchController;
            ReceiveStatusMatchAckCallback = OnStatusMatchController;
            ReceiveCancelMatchAckCallback = OnCancelMatchController;
            JoinMatchAckCallback = OnJoinMatchController;
        }
    }
}
