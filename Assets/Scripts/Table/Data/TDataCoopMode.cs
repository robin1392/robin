using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table.Data
{
    public enum ECoopModeKey
    {
        coopmodePlayCntMax = 1,
        coopmodeADCnt,
        serachuserTropy,
        serachuserTropyMaxCnt,
        MaxWave,
        coopTowerHp,
        bossID1st,
        bossID2nd,
        bossID3rd,
        bossID4th,
        bossID5th,
        callGuardianTowerHp_1st,
        callGuardianTowerHp_2nd,
        get1stBoss_Wave,
        get2ndBoss_Wave,
        get3rdBoss_Wave,
        get4thBoss_Wave,
        get5thBoss_Wave,
        eggtobossCoolTime,
        basicrewardWave,
        basicrewardID,
        basicrewardValue,
        bossRewardID01,
        bossRewardID02,
        bossRewardID03,
        bossRewardID04,
        bossRewardID05,
    }

    public class TDataCoopMode : ITableData<int>
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
