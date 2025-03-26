using System.IO;
using _Project.Scripts.Infrastructure.DI;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Logger
{
    public class FileLogger : ILogger
    {
        private const string FOLDER = "Logs";

        private readonly FileWriter _fileWriter;

        public FileLogger(bool usingAppLogs = false)
        {
            string folderPath = $"{Application.persistentDataPath}/{FOLDER}";

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            _fileWriter = new FileWriter(folderPath);

            if (usingAppLogs)
            {
                Application.logMessageReceivedThreaded += OnLogMessageReceived;
            }
        }

        ~FileLogger()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
            _fileWriter?.Dispose();
        }

        public void Log(LogMessage message) => _fileWriter.Write(message);

        public void Log(string message, Object context = null) => Log(new LogMessage(message, context));

        private void OnLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                _fileWriter.Write(new LogMessage(condition, type: type));
                _fileWriter.Write(new LogMessage(stacktrace, type: type));
            }
            else
            {
                _fileWriter.Write(new LogMessage(condition, type: type));
            }
        }
    }
}