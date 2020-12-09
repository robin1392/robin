using System.Collections.Generic;

namespace Service.Net
{
    public delegate bool ControllerDelegate(Peer peer, byte[] msg);

    public class MessageController
    {
        Dictionary<int, ControllerDelegate> _controllers;


        public MessageController()
        {
            _controllers = new Dictionary<int, ControllerDelegate>();
        }


        public bool AddController(int protocolId, ControllerDelegate callback)
        {
            if (_controllers.ContainsKey(protocolId) == true)
            {
                return false;
            }
            
            _controllers.Add(protocolId, callback);
            return true;
        }


        public bool AddControllers(Dictionary<int, ControllerDelegate> controllers)
        {
            foreach (var elem in controllers)
            {
                AddController(elem.Key, elem.Value);
            }
            return true;
        }


        public virtual bool OnRecevice(Peer peer, int protocolId, byte[] msg) 
        {
            ControllerDelegate controllerCallback;
            if (_controllers.TryGetValue(protocolId, out controllerCallback) == false)
            {
                return false;
            }
            
            return controllerCallback(peer, msg);
        }
    }
}