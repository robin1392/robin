using Service.Template.Table;

namespace Template.Item.RandomWarsDice.Table
{
    public class TDataDiceUpgrade : ITableData<int>
    {
        public int id { get; set; }
        public string name { get; set; }
        public int diceGrade { get; set; }
        public int diceLv { get; set; }
        public int needCard { get; set; }
        public int needGold { get; set; }
        public int getTowerHp { get; set; }


        public int PK()
        {
            return id;
        }


        public void Serialize(string[] cols)
        {
            id = int.Parse(cols[0]);
            name = cols[1];
            diceGrade = int.Parse(cols[2]);
            diceLv = int.Parse(cols[3]);
            needCard = int.Parse(cols[4]);
            needGold = int.Parse(cols[5]);
            getTowerHp = int.Parse(cols[6]);
        }
    }
}