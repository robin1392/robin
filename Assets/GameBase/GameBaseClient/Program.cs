using System;
using System.Threading;
using Percent.GameBaseClient;
using Template.Shop.GameBaseShop.Table;
using Template.Shop.GameBaseShop.Common;

 

namespace GameBaseClient
{
    class Program
    {
        static GameBaseClientSession session;

        static void Main(string[] args)
        {
            session = new GameBaseClientSession();
            session.Init(new GameBaseClientConfig());
            
            // api 요청
            string playerGuid = "testguid1234354";
            session.ShopTemplate.ShopInfoReq(session.HttpClient, playerGuid, OnShopInfoAck);


            while(true)
            {
                session.Update();
                Thread.Sleep(1000);
            }
        }


        static bool OnShopInfoAck(GameBaseShopErrorCode errorCode, ShopInfo[] arrayShopInfo)
        {
            Console.WriteLine("OnShopInfoAck.");

            foreach (var shopInfo in arrayShopInfo)
            {
                TDataShopInfo dataShopInfo;
                if (session.ShopTemplate.Table.ShopInfo.GetData(shopInfo.shopId, out dataShopInfo) == false)
                {
                    Console.WriteLine("shop name: " + dataShopInfo.name);
                }
            }
            
            return true;
        }
    }
}
