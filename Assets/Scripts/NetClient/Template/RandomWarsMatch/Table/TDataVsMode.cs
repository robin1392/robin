using Service.Template.Table;

namespace Template.Stage.RandomWarsMatch.Table
{
    public class TDataVsMode : ITableData<int>
    {
        public int id { get; set; }
        public string name { get; set; }
        public int value { get; set; }



        public int PK()
        {
            return id;
        }


        public void Serialize(string[] cols)
        {
            id = int.Parse(cols[0]);
            name = cols[1];
            value = int.Parse(cols[2]);
        }
    }
}