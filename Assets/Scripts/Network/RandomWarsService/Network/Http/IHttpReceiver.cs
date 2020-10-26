using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RandomWarsService.Network.Http
{
    public interface IHttpReceiver
    {
        Task<string> ProcessRequest(int protocolId, string json);
        bool ProcessResponse(int protocolId, string json);
    }
}
