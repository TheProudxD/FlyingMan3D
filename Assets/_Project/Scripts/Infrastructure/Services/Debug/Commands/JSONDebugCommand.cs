using System;
using System.IO;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Debug.Commands
{
    public class JSONDebugCommand : DebugCommand
    {
        public override string ID => "to_json_data";
        public override string Description => "Save something into json";
        public override string Format => "to_json_data";
        
        public override void Invoke()
        {
            try
            {
                string path = $@"Assets/Debug/{ID + DateTime.Now.ToShortTimeString()}.json";
                string assetsDebug = "Assets/Debug/";

                using (var writer = new StreamWriter(path))
                {
                    if (!Directory.Exists(assetsDebug))
                        Directory.CreateDirectory(assetsDebug);

                    var data = new DebugJSONData<int> { Object = 123 };

                    string jsonData = JsonUtility.ToJson(data);

                    UnityEngine.Debug.Log(jsonData);

                    writer.Write(jsonData);
                }

                UnityEngine.Debug.Log($"Saved '{ID}' into json");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }
    }
}