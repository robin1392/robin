using System;

namespace Table.Data
{
    public class TDataBoxList : ITableData<int>
    {
        public int id { get; set; }
        public string name { get; set; }
        public string boxIcon { get; set; }
        public int buyboxType { get; set; }
        public int buyboxValue { get; set; }
        public bool freeOpen { get; set; }
        public int openKeyValue { get; set; }
        public int[] productId { get; set; }
        public int buyLimitCnt { get; set; }
        public bool isUse { get; set; }


        public int PK()
        {
            return id;
        }


        public void Serialize(string[] cols)
        {
            id = int.Parse(cols[0]);
            name = cols[1];
            boxIcon = cols[2];
            buyboxType = int.Parse(cols[3]);
            buyboxValue = int.Parse(cols[4]);
            freeOpen = bool.Parse(cols[5]);
            openKeyValue = int.Parse(cols[6]);
            productId = Array.ConvertAll(cols[7].Split('|'), s => int.Parse(s));
            buyLimitCnt = int.Parse(cols[8]);
            isUse = bool.Parse(cols[9]);
        }
    }
}