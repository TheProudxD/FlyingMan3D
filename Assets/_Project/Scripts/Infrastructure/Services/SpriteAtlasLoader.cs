using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace _Project.Scripts.Infrastructure.Services
{
    public class SpriteAtlasLoader : IService, System.IDisposable
    {
        private AsyncOperationHandle<SpriteAtlas>? _atlasHandle;
        private SpriteAtlas _loadedAtlas;
        private string _loadedAtlasKey;

        public bool IsLoaded => _loadedAtlas != null;
        public string LoadedAtlasKey => _loadedAtlasKey;

        public async UniTask<SpriteAtlas> Load(string atlasKey, bool forceReload = false)
        {
            if (string.IsNullOrWhiteSpace(atlasKey))
            {
                Debug.LogError("[SpriteAtlasLoader] Atlas key is null or empty.");
                return null;
            }

            if (!forceReload && _loadedAtlas != null && _loadedAtlasKey == atlasKey)
                return _loadedAtlas;

            Release();

            try
            {
                AsyncOperationHandle<SpriteAtlas> handle = Addressables.LoadAssetAsync<SpriteAtlas>(atlasKey);
                _atlasHandle = handle;

                SpriteAtlas atlas = await handle.Task.AsUniTask();

                if (atlas == null)
                {
                    Debug.LogError($"[SpriteAtlasLoader] Failed to load atlas by key '{atlasKey}'.");
                    Release();
                    return null;
                }

                _loadedAtlas = atlas;
                _loadedAtlasKey = atlasKey;
                return _loadedAtlas;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[SpriteAtlasLoader] Failed to load atlas '{atlasKey}': {exception.Message}");
                Release();
                return null;
            }
        }

        public bool TryGetSprite(string spriteName, out Sprite sprite)
        {
            sprite = null;

            if (_loadedAtlas == null)
            {
                Debug.LogWarning("[SpriteAtlasLoader] SpriteAtlas is not loaded yet.");
                return false;
            }

            sprite = _loadedAtlas.GetSprite(spriteName);

            if (sprite == null)
            {
                Debug.LogWarning(
                    $"[SpriteAtlasLoader] Sprite '{spriteName}' was not found in atlas '{_loadedAtlasKey}'.");
                return false;
            }

            return true;
        }

        public Sprite GetSprite(string spriteName)
        {
            TryGetSprite(spriteName, out Sprite sprite);
            return sprite;
        }

        public void Release()
        {
            if (_atlasHandle.HasValue && _atlasHandle.Value.IsValid())
                Addressables.Release(_atlasHandle.Value);

            _atlasHandle = null;
            _loadedAtlas = null;
            _loadedAtlasKey = null;
        }

        public void Dispose() => Release();
    }
}
