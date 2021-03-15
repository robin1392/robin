using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MirageTest.Scripts.SyncAction
{
    public static class ActionLookup
    {
        private static Dictionary<int, SyncActionWithTarget> _syncActionWithTargets;
        private static Dictionary<int, SyncActionWithoutTarget> _syncActionWithoutTargets;
        
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
            
            Dictionary<int, SyncActionWithoutTarget> syncActionWithoutTargets = new Dictionary<int, SyncActionWithoutTarget>();
            foreach (var type in 
                Assembly.GetAssembly(typeof(SyncActionWithoutTarget)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(SyncActionWithoutTarget))))
            {
                var instance = (SyncActionWithoutTarget) Activator.CreateInstance(type);
                syncActionWithoutTargets.Add(type.GetHashCode(), instance);
            }

            _syncActionWithoutTargets = syncActionWithoutTargets;
        }
        
        public static SyncActionWithTarget GetActionWithTarget(int hash)
        {
            return _syncActionWithTargets[hash];
        }
        
        public static SyncActionWithoutTarget GetActionWithoutTarget(int hash)
        {
            return _syncActionWithoutTargets[hash];
        }
    }
}