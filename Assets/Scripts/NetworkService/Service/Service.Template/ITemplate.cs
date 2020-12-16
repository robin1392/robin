using System.Collections.Generic;
using Service.Net;

namespace Service.Template
{
    public interface ITemplate
    {
        void Update();
        void Destroy();
        bool PushItem(string pk);
        bool GetItem(string pk, string sk);
        bool ConnectPeer(Peer peer);
        bool DisconnectPeer(Peer peer);
    }
}