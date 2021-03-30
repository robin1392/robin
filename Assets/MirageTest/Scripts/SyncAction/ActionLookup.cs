using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MirageTest.Scripts.SyncAction
{
    public static class ActionLookup
    {
        private static Dictionary<string, SyncActionWithTarget> _syncActionWithTargets;
        private static Dictionary<string, SyncActionWithoutTarget> _syncActionWithoutTargets;
        
        static ActionLookup()
        {
            Dictionary<string, SyncActionWithTarget> syncActionWithTargets = new Dictionary<string, SyncActionWithTarget>();
            foreach (var type in 
                Assembly.GetAssembly(typeof(SyncActionWithTarget)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(SyncActionWithTarget))))
            {
                var instance = (SyncActionWithTarget) Activator.CreateInstance(type);
                syncActionWithTargets.Add(type.Name, instance);
            }

            _syncActionWithTargets = syncActionWithTargets;
            
            Dictionary<string, SyncActionWithoutTarget> syncActionWithoutTargets = new Dictionary<string, SyncActionWithoutTarget>();
            foreach (var type in 
                Assembly.GetAssembly(typeof(SyncActionWithoutTarget)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(SyncActionWithoutTarget))))
            {
                var instance = (SyncActionWithoutTarget) Activator.CreateInstance(type);
                syncActionWithoutTargets.Add(type.Name, instance);
            }

            _syncActionWithoutTargets = syncActionWithoutTargets;
        }
        
        public static SyncActionWithTarget GetActionWithTarget(string hash)
        {
            return _syncActionWithTargets[hash];
        }
        
        public static SyncActionWithoutTarget GetActionWithoutTarget(string hash)
        {
            return _syncActionWithoutTargets[hash];
        }
    }
}