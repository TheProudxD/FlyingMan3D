using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Logger
{
    public class NoLogger : ILogger
    {
        public NoLogger(bool disableUnityLogs = false)
        {
            UnityEngine.Debug.unityLogger.logEnabled = !disableUnityLogs;
            UnityEngine.Debug.developerConsoleEnabled = !disableUnityLogs;
            UnityEngine.Debug.developerConsoleVisible = !disableUnityLogs;
        }

        public void Log(LogMessage message) { }

        public void Log(string message, Object context = null) { }
    }
}