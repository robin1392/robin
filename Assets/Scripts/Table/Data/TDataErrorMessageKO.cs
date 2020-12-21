using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table.Data
{
    public class TDataErrorMessageKO : ITableData<int>
    {
        public string stringKey { get; set; }
        public int id { get; set; }
        public string textDesc { get; set; }


        public int PK()
        {
            return id;
        }


        public void Serialize(string[] cols)
        {
            stringKey = cols[0];
            id = int.Parse(cols[1]);
            textDesc = cols[2];
        }
    }
}
