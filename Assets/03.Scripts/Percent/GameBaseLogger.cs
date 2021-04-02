using UnityEngine;
using Service.Core;

namespace Percent
{
    public class GameBaseLogger : ILog
    {
        public void Info(string text)
        {
            UnityEngine.Debug.Log(text);
        }

        public void Fatal(string text)
        {
            UnityEngine.Debug.LogError(text);
        }

        public void Error(string text)
        {
            UnityEngine.Debug.LogError(text);
        }

        public void Warn(string text)
        {
            UnityEngine.Debug.Log(text);
        }

        public void Debug(string text)
        {
            UnityEngine.Debug.Log(text);
        }
    }
}