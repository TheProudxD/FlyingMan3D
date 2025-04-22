using System.Collections;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class AudioLoader : IService
    {
        private readonly AudioService _audioService;
        private readonly string _audioPathInStreamingAssets = "Music/background.ogg";
        private readonly bool _playOnLoad = true;

        public AudioLoader(AudioService audioService) => _audioService = audioService;

        public IEnumerator Load()
        {
            string path = GetStreamingAssetsPath(_audioPathInStreamingAssets);

            using UnityWebRequest www =
                UnityWebRequestMultimedia.GetAudioClip(path, GetAudioType(_audioPathInStreamingAssets));

            yield return www.SendWebRequest();

            if (www.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Audio load error: " + www.error);
                yield break;
            }

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            clip.name = System.IO.Path.GetFileNameWithoutExtension(_audioPathInStreamingAssets);

            if (_playOnLoad)
            {
                _audioService.PlayMusic(clip);
            }
        }

        private string GetStreamingAssetsPath(string relativePath)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return Application.absoluteURL + "StreamingAssets/" + relativePath;
#else
            return System.IO.Path.Combine(Application.streamingAssetsPath, relativePath);
#endif
        }

        private AudioType GetAudioType(string path)
        {
            string extension = System.IO.Path.GetExtension(path).ToLower();

            return extension switch
            {
                ".mp3" => AudioType.MPEG,
                ".ogg" => AudioType.OGGVORBIS,
                ".wav" => AudioType.WAV,
                _ => AudioType.UNKNOWN
            };
        }
    }
}