using Mirage;

namespace MirageTest.Scripts.Messages
{
    [NetworkMessage]
    public class AddPlayerProxyMessage
    {
        public string userId;
    }
}