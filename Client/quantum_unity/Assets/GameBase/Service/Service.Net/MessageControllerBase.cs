using System.Collections.Generic;

namespace Service.Net
{
    public class MessageControllerBase
    {
        public Dictionary<int, ControllerDelegate> MessageControllers { get; protected set; }
    }
}