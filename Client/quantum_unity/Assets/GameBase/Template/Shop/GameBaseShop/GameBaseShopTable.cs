using Service.Template.Table;
using Template.Shop.GameBaseShop.Table;

namespace Template.Shop.GameBaseShop
{
    public class GameBaseShopTable
    {
        public TableData<int, TDataShopInfo> ShopInfo { get; private set; }
        public TableData<int, TDataEventShopList> EventShopList { get; private set; }
        public TableData<int, TDataOnedayShopList> OnedayShopList { get; private set; }


        public GameBaseShopTable()
        {
            ShopInfo = new TableData<int, TDataShopInfo>();
            EventShopList = new TableData<int, TDataEventShopList>();
            OnedayShopList = new TableData<int, TDataOnedayShopList>();
        }


        public bool Init(string path)
        {
            ShopInfo.Init(new TableLoaderRemoteCSV<int, TDataShopInfo>(), path + "/ShopInfo.csv");
            EventShopList.Init(new TableLoaderRemoteCSV<int, TDataEventShopList>(), path + "/EventShopList.csv");
            OnedayShopList.Init(new TableLoaderRemoteCSV<int, TDataOnedayShopList>(), path + "/OnedayShopList.csv");

            return true;
        }
    }
}



