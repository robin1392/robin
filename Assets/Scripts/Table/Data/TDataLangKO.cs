using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table.Data
{
    public class TDataLangKO : ITableData<string>
    {
        public string stringKey { get; set; }
        public string textDesc { get; set; }

        public string PK()
        {
            return stringKey;
        }


        public void Serialize(string[] cols)
        {
            stringKey = cols[0];
            textDesc = cols[1];
        }
    }
}
