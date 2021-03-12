using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MirageTest.Scripts.SyncAction
{
    public static class ActionLookup
    {
        private static Dictionary<int, SyncActionWithTarget> _syncActionWithTargets;
        
        static ActionLookup()
        {
            Dictionary<int, SyncActionWithTarget> syncActionWithTargets = new Dictionary<int, SyncActionWithTarget>();
            foreach (var type in 
                Assembly.GetAssembly(typeof(SyncActionWithTarget)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(SyncActionWithTarget))))
            {
                var instance = (SyncActionWithTarget) Activator.CreateInstance(type);
                syncActionWithTargets.Add(type.GetHashCode(), instance);
            }

            _syncActionWithTargets = syncActionWithTargets;
        }
        
        public static SyncActionWithTarget GetActionWithTarget(int hash)
        {
            return _syncActionWithTargets[hash];
        }
    }
}