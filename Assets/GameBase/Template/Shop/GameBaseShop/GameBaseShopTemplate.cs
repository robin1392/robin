using System;
using Service.Template;
using Service.Net;
using Template.Shop.GameBaseShop.Common;

namespace Template.Shop.GameBaseShop
{
    public partial class GameBaseShopTemplate : GameBaseShopProtocol, ITemplate
    {
        public GameBaseShopTable Table { get; set; }


        public GameBaseShopTemplate()
        {
            // -----------------------------------------------------------------
            // Receive Delegate 연결
            // -----------------------------------------------------------------
            Table = new GameBaseShopTable();
        }


        public bool Init()
        {
            if (Table.Init("https://randomdice-wars-bucket.s3.ap-northeast-2.amazonaws.com/table/DEV") == false)
            {
                return false;
            }

            return true;
        }


        // -------------------------------------------------------------------
        // ITemplate 구현부
        // -------------------------------------------------------------------
        public void Update()
        {

        }


        public void Destroy()
        {

        }


        public bool PushItem(string pk)
        {
            return true;
        }


        public bool GetItem(string pk, string sk)
        {
            return true;
        }


        public bool ConnectNewPlayer(GamePlayer player)
        {
            return true;   
        }


        public bool DisconnectPlayer(GamePlayer player)
        {
            return true;
        }
    }
}
