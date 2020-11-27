using System.Collections.Generic;

namespace Service.Net
{
    public delegate string HttpControllerDelegate(string json);

    public class HttpController
    {
        Dictionary<int, HttpControllerDelegate> _controllers;


        public HttpController()
        {
            _controllers = new Dictionary<int, HttpControllerDelegate>();
        }


        public bool AddController(int protocolId, HttpControllerDelegate callback)
        {
            if (_controllers.ContainsKey(protocolId) == true)
            {
                return false;
            }
            
            _controllers.Add(protocolId, callback);
            return true;
        }


        public bool AddControllers(Dictionary<int, HttpControllerDelegate> controllers)
        {
            foreach (var elem in controllers)
            {
                AddController(elem.Key, elem.Value);
            }
            return true;
        }


        public virtual string OnRecevice(int protocolId, string json) 
        {
            HttpControllerDelegate callback;
            if (_controllers.TryGetValue(protocolId, out callback) == false)
            {
                return string.Empty;
            }
            
            return callback(json);
        }
    }
}
