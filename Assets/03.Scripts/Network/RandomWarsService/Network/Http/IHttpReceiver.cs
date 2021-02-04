using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RandomWarsService.Network.Http
{
    public interface IHttpReceiver
    {
        Task<string> ProcessAsync(int protocolId, string json);
        bool Process(int protocolId, string json);
    }
}
