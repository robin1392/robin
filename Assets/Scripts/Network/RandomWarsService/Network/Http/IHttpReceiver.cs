using System;
using System.Collections.Generic;
using System.Text;

namespace RandomWarsService.Network.Http
{
    public interface IHttpReceiver
    {
        bool Process(int protocolId, string json);
    }
}
