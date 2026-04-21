using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Project.Scripts.Infrastructure.Services.Audio
{
    public class AudioClipLoader : IService, IDisposable
    {
        private readonly Dictionary<string, AsyncOperationHandle<AudioClip>> _handles = new();
        private readonly Dictionary<string, AudioClip> _loadedClips = new();

        public bool TryGetLoaded(string clipKey, out AudioClip clip)
        {
            clip = null;

            if (string.IsNullOrWhiteSpace(clipKey))
                return false;

            return _loadedClips.TryGetValue(clipKey, out clip) && clip != null;
        }

        public UniTask<AudioClip> LoadSound(string clipKey, bool forceReload = false) =>
            Load(clipKey, forceReload);

        public UniTask<AudioClip> LoadMusic(string clipKey, bool forceReload = false) =>
            Load(clipKey, forceReload);

        public async UniTask<AudioClip> Load(string clipKey, bool forceReload = false)
        {
            if (string.IsNullOrWhiteSpace(clipKey))
            {
                Debug.LogError("[AudioClipLoader] Clip key is null or empty.");
                return null;
            }

            if (!forceReload && TryGetLoaded(clipKey, out AudioClip cachedClip))
                return cachedClip;

            if (forceReload)
                Release(clipKey);

            try
            {
                AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(clipKey);
                _handles[clipKey] = handle;

                AudioClip clip = await handle.Task.AsUniTask();

                if (clip == null)
                {
                    Debug.LogError($"[AudioClipLoader] Failed to load clip by key '{clipKey}'.");
                    Release(clipKey);
                    return null;
                }

                _loadedClips[clipKey] = clip;
                return clip;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[AudioClipLoader] Failed to load clip '{clipKey}': {exception.Message}");
                Release(clipKey);
                return null;
            }
        }

        public void Release(string clipKey)
        {
            if (string.IsNullOrWhiteSpace(clipKey))
                return;

            if (_handles.TryGetValue(clipKey, out AsyncOperationHandle<AudioClip> handle) && handle.IsValid())
                Addressables.Release(handle);

            _handles.Remove(clipKey);
            _loadedClips.Remove(clipKey);
        }

        public void ReleaseAll()
        {
            foreach ((string clipKey, AsyncOperationHandle<AudioClip> handle) in _handles)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            _handles.Clear();
            _loadedClips.Clear();
        }

        public void Dispose() => ReleaseAll();
    }
}
