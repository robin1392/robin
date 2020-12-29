using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomWarsResource.Data
{
    public enum EVsmodeKey
    {
        None,
        StartCoolTime,
        WaveTime,
        GetStartSP,
        AddSP,
        GetStartDiceCost,
        DiceCostUp,
        TowerHp,
        GetDefenderTowerHp,
        DicePowerUpCost01,
        DicePowerUpCost02,
        DicePowerUpCost03,
        DicePowerUpCost04,
        DicePowerUpCost05,
        StartSuddenDeathWave,
        SuddenDeathAtkUp,
        SuddenDeathDefUp,
        GetSPPlusLevelupCost01,
        GetSPPlusLevelupCost02,
        GetSPPlusLevelupCost03,
        GetSPPlusLevelupCost04,
        GetSPPlusLevelupCost05,
        NormalWinReward_Gold,
        NormalWinReward_Key,
        PerfectReward_Gold,
        PerfectWinReward_Key,
        EndWave,
    }


    public class TDataVsmode : ITableData<int> 
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
