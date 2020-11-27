using System.Collections.Generic;

namespace Service.Net
{
    public class BaseProtocol
    {
        public Dictionary<int, ControllerDelegate> MessageControllers { get; protected set; }

        public Dictionary<int, HttpControllerDelegate> HttpMessageControllers { get; protected set; }
    }
}