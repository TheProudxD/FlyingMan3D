using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.Services.Logger
{
    [Serializable]
    public class LogMessage
    {
        public LogType Type { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; }
        public Object Context { get; set; }

        public LogMessage(string message, Object context = null, LogType type = LogType.Log)
        {
            Message = message;
            Context = context;
            Type = type;
            Time = DateTime.Now;
        }
    }
}