using System;
using System.IO;
using System.Text;

namespace _Project.Scripts.Infrastructure.Services.Logger
{
    public class FileAppender
    {
        private readonly object _lockObject = new();

        public string FileName { get; }

        public FileAppender(string fileName) => FileName = fileName;

        public bool Append(string content)
        {
            try
            {
                lock (_lockObject)
                {
                    using FileStream fs = File.Open(FileName, FileMode.Append, FileAccess.Write, FileShare.Read);

                    byte[] bytes = Encoding.UTF8.GetBytes(content);
                    fs.Write(bytes, 0, bytes.Length);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
