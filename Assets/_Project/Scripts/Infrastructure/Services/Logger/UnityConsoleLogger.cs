using System;
using _Project.Scripts.Infrastructure.DI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.Services.Logger
{
    public class UnityConsoleLogger : ILogger
    {
        public void Log(LogMessage message)
        {
            switch (message.Type)
            {
                case LogType.Error:
                    UnityEngine.Debug.LogError(message.Message, message.Context);
                    break;
                case LogType.Assert:
                    UnityEngine.Debug.LogAssertion(message.Message, message.Context);
                    break;
                case LogType.Warning:
                    UnityEngine.Debug.LogWarning(message.Message, message.Context);
                    break;
                case LogType.Log:
                    UnityEngine.Debug.Log(message.Message, message.Context);
                    break;
                case LogType.Exception:
                    UnityEngine.Debug.LogException(new Exception(message.Message));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Log(string message, Object context) => Log(new LogMessage(message, context));
    }
}