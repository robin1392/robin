
namespace Service.Template
{
    public interface ITemplate
    {
        void Update();
        void Destroy();
        bool PushItem(string pk);
        bool GetItem(string pk, string sk);
        bool ConnectNewPlayer(GamePlayer player);
        bool DisconnectPlayer(GamePlayer player);
    }
}