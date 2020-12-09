using System;
using Service.Template;
using Service.Template.Table;
using Template.Stage.RandomWarsMatch.Table;

namespace Template.Stage.RandomWarsMatch
{
    public class RandomWarsMatchTable
    {
        public TableData<int, TDataVsMode> VsMode { get; private set; }
  
        
        public RandomWarsMatchTable()
        {
            VsMode = new TableData<int, TDataVsMode>();
        }


        public bool Init(string path)
        {
            VsMode.Init(new TableLoaderRemoteCSV<int, TDataVsMode>(), path + "/Vsmode.csv");
            return true;
        }
    }
}