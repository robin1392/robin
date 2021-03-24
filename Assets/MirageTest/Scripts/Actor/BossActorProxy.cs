using Mirage;
using RandomWarsResource.Data;

namespace MirageTest.Scripts
{
    public class BossActorProxy : ActorProxy
    {
        [SyncVar]
        
        private TDataCoopModeBossInfo _bossInfo;

        public TDataCoopModeBossInfo bossInfo
        {
            get
            {
                if (_bossInfo == null)
                {
                    TableManager.Get().CoopModeBossInfo.GetData(dataId, out _bossInfo);
                }

                return _bossInfo;
            }
        }
        
        protected override void OnSpawnActor()
        {
            
        }
    }
}