using System;
using Service.Template;
using Service.Template.Table;
using Template.Item.RandomWarsDice.Table;

namespace Template.Item.RandomWarsDice
{
    public class RandomWarsDiceTable
    {
        public TableData<int, TDataDiceInfo> DiceInfo { get; private set; }
        public TableData<int, TDataDiceUpgrade> DiceUpgrade { get; private set; }
        public TableData<int, TDataDiceLevelInfo> DiceLevelInfo { get; private set; }
        public TableData<int, TDataBoxList> BoxList { get; private set; }
        public TableData<int, TDataBoxProductInfo> BoxProductInfo { get; private set; }


        public RandomWarsDiceTable()
        {
            DiceInfo = new TableData<int, TDataDiceInfo>();
            DiceUpgrade = new TableData<int, TDataDiceUpgrade>();
            DiceLevelInfo = new TableData<int, TDataDiceLevelInfo>();
            BoxList = new TableData<int, TDataBoxList>();
            BoxProductInfo = new TableData<int, TDataBoxProductInfo>();
     }


        public bool Init(string path)
        {
            DiceInfo.Init(new TableLoaderRemoteCSV<int, TDataDiceInfo>(), path + "/DiceInfo.csv");
            DiceUpgrade.Init(new TableLoaderRemoteCSV<int, TDataDiceUpgrade>(), path + "/DiceUpgrade.csv");
            DiceLevelInfo.Init(new TableLoaderRemoteCSV<int, TDataDiceLevelInfo>(), path + "/DiceLevelInfo.csv");
            BoxList.Init(new TableLoaderRemoteCSV<int, TDataBoxList>(), path + "/BoxList.csv");
            BoxProductInfo.Init(new TableLoaderRemoteCSV<int, TDataBoxProductInfo>(), path + "/BoxProductInfo.csv");

            return true;
        }
    }
}