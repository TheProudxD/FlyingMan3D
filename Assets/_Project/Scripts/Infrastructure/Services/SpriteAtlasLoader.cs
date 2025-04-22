using System.Collections;
using _Project.Scripts.Infrastructure.Services;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class SpriteAtlasLoader : IService
    {
        private readonly string _atlasPathInStreamingAssets = "Atlases/Game.spriteatlas";

        private SpriteAtlas _loadedAtlas;

/*
        public IEnumerator Load()
        {

            string path = GetStreamingAssetsPath(_atlasPathInStreamingAssets);

            using UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(path);

            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load AssetBundle: {webRequest.error}");
                yield break;
            }

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);

            if (bundle == null)
            {
                Debug.LogError("Failed to extract AssetBundle");
                yield break;
            }

            var atlasRequest =
                bundle.LoadAssetAsync<SpriteAtlas>(
                    System.IO.Path.GetFileNameWithoutExtension(_atlasPathInStreamingAssets));

            yield return atlasRequest;

            _loadedAtlas = atlasRequest.asset as SpriteAtlas;

            if (_loadedAtlas == null)
            {
                Debug.LogError("Failed to load SpriteAtlas from AssetBundle");
            }
            else
            {
                Debug.Log("SpriteAtlas loaded successfully!");
                // Здесь можно использовать атлас
            }

            bundle.Unload(false);
        }
*/
        public Sprite GetSprite(string spriteName)
        {
            if (_loadedAtlas != null)
                return _loadedAtlas.GetSprite(spriteName);

            Debug.LogWarning("SpriteAtlas not loaded yet");
            return null;
        }

        // public IEnumerator Load()
        // {
        //     string path = GetStreamingAssetsPath(_atlasPathInStreamingAssets);
        //
        //     using UnityWebRequest webRequest = UnityWebRequest.Get(path);
        //
        //     yield return webRequest?.SendWebRequest();
        //
        //     if (webRequest == null)
        //         yield break;
        //
        //     if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        //     {
        //         Debug.LogError("Error loading sprite atlas: " + webRequest.error);
        //         yield break;
        //     }
        //
        //     AssetBundleCreateRequest bundleCreateRequest =
        //         AssetBundle.LoadFromMemoryAsync(webRequest.downloadHandler.data);
        //
        //     yield return bundleCreateRequest;
        //
        //     AssetBundle bundle = bundleCreateRequest.assetBundle;
        //
        //     if (bundle == null)
        //     {
        //         Debug.LogError("Failed to load AssetBundle");
        //         yield break;
        //     }
        //
        //     var atlasRequest =
        //         bundle.LoadAssetAsync<SpriteAtlas>(
        //             System.IO.Path.GetFileNameWithoutExtension(_atlasPathInStreamingAssets));
        //
        //     yield return atlasRequest;
        //
        //     SpriteAtlas loadedAtlas = atlasRequest.asset as SpriteAtlas;
        //
        //     if (loadedAtlas == null)
        //     {
        //         Debug.LogError("Failed to load SpriteAtlas from AssetBundle");
        //     }
        //     // else
        //     // {
        //     //     // Здесь вы можете использовать загруженный атлас
        //     //     Debug.Log("Sprite atlas loaded successfully!");
        //     //
        //     //     // Пример: получить спрайт из атласа
        //     //     Sprite sprite = loadedAtlas.GetSprite("SpriteName");
        //     //
        //     //     if (sprite != null)
        //     //     {
        //     //         // Применить спрайт к объекту
        //     //         GetComponent<SpriteRenderer>().sprite = sprite;
        //     //     }
        //     // }
        //
        //     // Выгружаем бандл
        //     bundle.Unload(false);
        // }

        private string GetStreamingAssetsPath(string relativePath)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return Application.absoluteURL + "StreamingAssets/" + relativePath;
#else
            return System.IO.Path.Combine(Application.streamingAssetsPath, relativePath);
#endif
        }
    }
}