using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table.Data
{
    public class TDataCoopModeMinion : ITableData<int>
    {
        public int id { get; set; }
        public string name { get; set; }
        public int minionId01 { get; set; }
        public int classLv01 { get; set; }
        public int diceLv01 { get; set; }
        public int minionId02 { get; set; }
        public int classLv02 { get; set; }
        public int diceLv02 { get; set; }
        public int minionId03 { get; set; }
        public int classLv03 { get; set; }
        public int diceLv03 { get; set; }
        public int minionId04 { get; set; }
        public int classLv04 { get; set; }
        public int diceLv04 { get; set; }
        public int minionId05 { get; set; }
        public int classLv05 { get; set; }
        public int diceLv05 { get; set; }
        public int minionId06 { get; set; }
        public int classLv06 { get; set; }
        public int diceLv06 { get; set; }
        public int minionId07 { get; set; }
        public int classLv07 { get; set; }
        public int diceLv07 { get; set; }
        public int minionId08 { get; set; }
        public int classLv08 { get; set; }
        public int diceLv08 { get; set; }
        public int minionId09 { get; set; }
        public int classLv09 { get; set; }
        public int diceLv09 { get; set; }
        public int minionId10 { get; set; }
        public int classLv10 { get; set; }
        public int diceLv10 { get; set; }
        public int minionId11 { get; set; }
        public int classLv11 { get; set; }
        public int diceLv11 { get; set; }
        public int minionId12 { get; set; }
        public int classLv12 { get; set; }
        public int diceLv12 { get; set; }
        public int minionId13 { get; set; }
        public int classLv13 { get; set; }
        public int diceLv13 { get; set; }
        public int minionId14 { get; set; }
        public int classLv14 { get; set; }
        public int diceLv14 { get; set; }
        public int minionId15 { get; set; }
        public int classLv15 { get; set; }
        public int diceLv15 { get; set; }



        public int PK()
        {
            return id;
        }


        public void Serialize(string[] cols)
        {
            id = int.Parse(cols[0]);
            name = cols[1];
            minionId01 = int.Parse(cols[2]);
            classLv01 = int.Parse(cols[3]);
            diceLv01 = int.Parse(cols[4]);
            minionId02 = int.Parse(cols[5]);
            classLv02 = int.Parse(cols[6]);
            diceLv02 = int.Parse(cols[7]);
            minionId03 = int.Parse(cols[8]);
            classLv03 = int.Parse(cols[9]);
            diceLv03 = int.Parse(cols[10]);
            minionId04 = int.Parse(cols[11]);
            classLv04 = int.Parse(cols[12]);
            diceLv04 = int.Parse(cols[13]);
            minionId05 = int.Parse(cols[14]);
            classLv05 = int.Parse(cols[15]);
            diceLv05 = int.Parse(cols[16]);
            minionId06 = int.Parse(cols[17]);
            classLv06 = int.Parse(cols[18]);
            diceLv06 = int.Parse(cols[19]);
            minionId07 = int.Parse(cols[20]);
            classLv07 = int.Parse(cols[21]);
            diceLv07 = int.Parse(cols[22]);
            minionId08 = int.Parse(cols[23]);
            classLv08 = int.Parse(cols[24]);
            diceLv08 = int.Parse(cols[25]);
            minionId09 = int.Parse(cols[26]);
            classLv09 = int.Parse(cols[27]);
            diceLv09 = int.Parse(cols[28]);
            minionId10 = int.Parse(cols[29]);
            classLv10 = int.Parse(cols[30]);
            diceLv10 = int.Parse(cols[31]);
            minionId11 = int.Parse(cols[32]);
            classLv11 = int.Parse(cols[33]);
            diceLv11 = int.Parse(cols[34]);
            minionId12 = int.Parse(cols[35]);
            classLv12 = int.Parse(cols[36]);
            diceLv12 = int.Parse(cols[37]);
            minionId13 = int.Parse(cols[38]);
            classLv13 = int.Parse(cols[39]);
            diceLv13 = int.Parse(cols[40]);
            minionId14 = int.Parse(cols[41]);
            classLv14 = int.Parse(cols[42]);
            diceLv14 = int.Parse(cols[43]);
            minionId15 = int.Parse(cols[44]);
            classLv15 = int.Parse(cols[45]);
            diceLv15 = int.Parse(cols[46]);
        }
    }
}
