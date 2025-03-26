using _Project.Scripts.Infrastructure.DI;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Logger
{
    public class UnityAndFileLogger : ILogger
    {
        private readonly ILogger _unityLogger;
        private readonly ILogger _fileLogger;

        public UnityAndFileLogger(ILogger unityLogger, ILogger fileLogger)
        {
            _unityLogger = unityLogger;
            _fileLogger = fileLogger;
        }

        public void Log(LogMessage message)
        {
            _unityLogger.Log(message);
            _fileLogger.Log(message);
        }

        public void Log(string message, Object context) => Log(new LogMessage(message, context));
    }
}