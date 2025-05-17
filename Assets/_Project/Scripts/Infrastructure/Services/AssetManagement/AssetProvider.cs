using System;
using System.Collections;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.Infrastructure.Services.Localization;
using _Project.Scripts.Infrastructure.Services.Localization.SO;
using _Project.Scripts.Tools;
using _Project.Scripts.Tools.Camera;
using _Project.Scripts.Tools.Extensions;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows;
using Reflex.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityUtils;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.Services.AssetManagement
{
    public class AssetProvider : IService, IDisposable, IInitializable
    {
        private readonly Container _container;
        private ConfigService _configService;

        public AssetProvider(Container container) => _container = container;

        public Task Initialize()
        {
            _configService = _container.Resolve<ConfigService>();
            return Task.CompletedTask;
        }

        public Task<AudioServiceView> CreateAudioServiceView() =>
            Instantiate<AudioServiceView>(AssetPath.AUDIO_SERVICE_VIEW);

        public Task<UIRoot> CreateUIRoot() =>
            Instantiate<UIRoot>(AssetPath.UI_ROOT_PATH);

        public Task<LoadingCurtain> CreateLoadingCurtain() =>
            Instantiate<LoadingCurtain>(AssetPath.LOADING_CURTAIN);

        public async Task<CameraSetup> CreateCameraSetup() =>
            await Instantiate<CameraSetup>(AssetPath.CAMERA_SETUP, overrideTransform: false);

        public async Task<UIContainer> Instantiate(WindowId windowId, Transform uiRoot)
        {
            WindowConfig windowConfig = _configService.ForWindow(windowId);
            // windowConfig.Prefab.gameObject.SetActive(false);
            AsyncOperationHandle<GameObject> asyncOperationHandle = windowConfig.Prefab.LoadAssetAsync<GameObject>();
            Task<GameObject> task = asyncOperationHandle.Task;
            await task;
            
            var component = Instantiate<UIContainer>(task.Result, parent: uiRoot);

            component.gameObject.SetActive(true);
            // windowConfig.Prefab.gameObject.SetActive(true);
            return component;
        }

        public Task<ConfigContainer> GetConfigContainer() =>
            Load<ConfigContainer>(AssetPath.CONFIG_CONTAINER);

        public Task<GraphicsPreset> GetMobileGraphicsPreset() =>
            Load<GraphicsPreset>(AssetPath.MOBILE_GRAPHICS_PRESET);

        public Task<GraphicsPreset> GetDesktopGraphicsPreset() =>
            Load<GraphicsPreset>(AssetPath.DESKTOP_GRAPHICS_PRESET);

        public async Task<TextCollection> GetLocal(LocaleConfig localeConfig)
        {
            string path = localeConfig.Code.ToString();
            var textCollection = await Load<TextCollection>(path);

            if (textCollection == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                UnityEngine.Debug.LogError("No collection found at path " + path);
#endif
            }

            return textCollection;
        }

        public Task<WindowStaticData> GetWindowStaticData() => Load<WindowStaticData>(AssetPath.WINDOW_CONFIG_PATH);

        public Task<Enemy> CreateSimpleEnemy(Vector3 position, Quaternion rotation) =>
            Instantiate<Enemy>(AssetPath.ENEMY, position, rotation);

        public Task<BigEnemy> CreateBigEnemy(Vector3 position, Quaternion rotation) =>
            Instantiate<BigEnemy>(AssetPath.BIG_ENEMY, position, rotation);

        public Task<LargeEnemy> CreateLargeEnemy(Vector3 position, Quaternion rotation)
            => Instantiate<LargeEnemy>(AssetPath.LARGE_ENEMY, position, rotation);

        public Task<Finish> GetFinish(Vector3 position, Quaternion rotation) =>
            Instantiate<Finish>(AssetPath.FINISH, position, rotation);

        public async Task<RingHolder> CreateRing(Vector3 position, Spawner.Colors[] colors, int level)
        {
            var ringPrefab = await Instantiate<RingHolder>(AssetPath.RING, position);

            ringPrefab.Renderers.sharedMaterial.color = colors[level].RingColor;
            ringPrefab.TransRenderers.sharedMaterial.color = colors[level].RingTransColor;

            return ringPrefab;
        }

        public Task<GameObject> CreateSmoke(Vector3 position, Quaternion rotation) =>
            Instantiate(AssetPath.SMOKE, position, rotation);

        public async Task<GameObject> CreateEnemyRagdoll(Vector3 position, Quaternion rotation)
        {
            GameObject enemy = await Instantiate(AssetPath.ENEMY_RAGDOLL, position, rotation);
            enemy.layer = 8;
            return enemy;
        }

        public async Task<GameObject> CreatePlayerRagdoll(Vector3 position, Quaternion rotation)
        {
            GameObject player = await Instantiate(AssetPath.PLAYER_RAGDOLL, position, rotation);
            player.layer = 8;
            return player;
        }

        public PlayerController CreatePlayer(GameObject root, Vector3 position, Quaternion rotation) =>
            Instantiate<PlayerController>(root, position, rotation);

        public Task<GameObject> CreateSlingshot(Vector3 position) =>
            Instantiate(AssetPath.SLINGSHOT, position);

        public Task<PlayerController> CreatePlayer(Vector3 position) =>
            Instantiate<PlayerController>(AssetPath.PLAYER, position);

        public Level CreateLevel(int levelId)
        {
            var levelContainer = _configService.Get<LevelContainer>();
            return levelContainer[levelId];
        }

        public void GetRingByType(RingData ringData, GameObject currentChildGo)
        {
            RingBase ring = ringData.RingType switch
            {
                RingType.Additive => currentChildGo.AddComponent<AdditiveRing>(),
                RingType.Multiplier => currentChildGo.AddComponent<MultiplierRing>(),
                RingType.Reducer => currentChildGo.AddComponent<ReducerRing>(),
                RingType.Divider => currentChildGo.AddComponent<DividerRing>(),
                _ => null
            };

            InitializeRing(ring, ringData);
        }

        private void InitializeRing(RingBase ring, RingData ringData)
        {
            ring.Effect = ringData.Effect;
            ring.Speed = ringData.Speed;
            ring.MovementAxis = ringData.MovementAxis;
            _container.Inject(ring);
        }

        public Task<ExplosionBarrel> CreateBarrel(Vector3 transformPosition) =>
            Instantiate<ExplosionBarrel>(AssetPath.BARREL, transformPosition);

        private GameObject Instantiate(GameObject prefab, Vector3 position = default, Quaternion rotation = default,
            Transform parent = null, bool isActivateGameObject = true)
        {
            prefab.SetActive(false);
            GameObject gameObject = Object.Instantiate(prefab, position, rotation, parent);
            _container.Inject(gameObject);
            gameObject.SetActive(isActivateGameObject);
            prefab.SetActive(isActivateGameObject);
            return gameObject;
        }

        private async Task<GameObject> Instantiate(string path, Vector3 position = default,
            Quaternion rotation = default,
            Transform parent = null, bool overrideTransform = true, bool isActivateGameObject = true)
        {
            var prefab = await Load<GameObject>(path);

            if (overrideTransform == false)
            {
                position = prefab.transform.position;
                rotation = prefab.transform.rotation;
            }

            return Instantiate(prefab, position, rotation, parent, isActivateGameObject);
        }

        public T Instantiate<T>(GameObject prefab, Vector3 position = default, Quaternion rotation = default,
            Transform parent = null, bool isActivateGameObject = true, bool componentEnabled = true)
            where T : MonoBehaviour
        {
            prefab.SetActive(false);
            GameObject gameObject = Object.Instantiate(prefab, position, rotation, parent);
            T component = gameObject.GetComponent<T>();
            _container.Inject(component);
            component.enabled = componentEnabled;
            gameObject.SetActive(isActivateGameObject);
            prefab.SetActive(isActivateGameObject);
            return component;
        }

        private async Task<T> Instantiate<T>(string path, Vector3 position = default, Quaternion rotation = default,
            Transform parent = null, bool overrideTransform = true, bool isActivateGameObject = true,
            bool componentEnabled = true)
            where T : MonoBehaviour
        {
            var prefab = await Load<GameObject>(path);

            if (overrideTransform == false)
            {
                position = prefab.transform.position;
                rotation = prefab.transform.rotation;
            }

            return Instantiate<T>(prefab, position, rotation, parent, isActivateGameObject, componentEnabled);
        }

        private Task<T> Load<T>(string path) where T : Object
        {
            // return UnityEngine.Resources.LoadAsync<T>(path).AsTask<T>();
            var asset = Addressables.LoadAssetAsync<T>(path);
            return asset.Task;
        }

        private IEnumerator LoadAsync<T>(string path) where T : Object
        {
            yield return UnityEngine.Resources.LoadAsync<T>(path);
        }

        public void Dispose() { }
    }
}