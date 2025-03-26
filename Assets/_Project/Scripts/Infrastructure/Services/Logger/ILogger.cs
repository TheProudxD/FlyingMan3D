using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Logger
{
    public interface ILogger
    {
        void Log(LogMessage message);

        void Log(string message, Object context = null);
    }
}