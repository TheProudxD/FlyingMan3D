using System;
using System.Collections;
using System.Threading.Tasks;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.Infrastructure.Services.Localization;
using _Project.Scripts.Infrastructure.Services.Localization.SO;
using _Project.Scripts.Tools.Camera;
using _Project.Scripts.Tools.Extensions;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityUtils;
using Object = UnityEngine.Object;
using _Project.Scripts.Gameplay;

namespace _Project.Scripts.Infrastructure.Services.AssetManagement
{
    public class AssetProvider : IService, IDisposable, ITaskInitializable
    {
        private readonly ProjectObjectInjector _projectObjectInjector;
        private ConfigService _configService;

        public AssetProvider(ProjectObjectInjector projectObjectInjector) =>
            _projectObjectInjector = projectObjectInjector;

        public UniTask Initialize()
        {
            if (_configService == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                UnityEngine.Debug.LogError("[AssetProvider] ConfigService is not assigned.");
#endif
            }

            return UniTask.CompletedTask;
        }

        public void SetConfigService(ConfigService configService) => _configService = configService;

        public UniTask<AudioServiceView> CreateAudioServiceView() =>
            Instantiate<AudioServiceView>(AssetPath.AUDIO_SERVICE_VIEW);

        public UniTask<UIRoot> CreateUIRoot() =>
            Instantiate<UIRoot>(AssetPath.UI_ROOT_PATH);

        public UniTask<LoadingCurtain> CreateLoadingCurtain() =>
            Instantiate<LoadingCurtain>(AssetPath.LOADING_CURTAIN);

        public async UniTask<CameraSetup> CreateCameraSetup() =>
            await Instantiate<CameraSetup>(AssetPath.CAMERA_SETUP, overrideTransform: false);

        public async UniTask<UIContainer> Instantiate(WindowId windowId, Transform uiRoot)
        {
            if (_configService == null)
            {
                UnityEngine.Debug.LogError("[AssetProvider] ConfigService is not assigned for window instantiation.");
                return null;
            }

            WindowConfig windowConfig = _configService.ForWindow(windowId);
            if (windowConfig == null || windowConfig.Prefab == null)
            {
                UnityEngine.Debug.LogError($"[AssetProvider] Missing window config or prefab for {windowId}.");
                return null;
            }

            AsyncOperationHandle<GameObject> asyncOperationHandle = windowConfig.Prefab.LoadAssetAsync<GameObject>();
            Task<GameObject> task = asyncOperationHandle.Task;
            await task;

            if (task.Result == null)
            {
                UnityEngine.Debug.LogError($"[AssetProvider] Failed to load prefab for window {windowId}.");
                return null;
            }

            var component = Instantiate<UIContainer>(task.Result, parent: uiRoot);
            if (component == null)
            {
                UnityEngine.Debug.LogError($"[AssetProvider] Loaded window prefab for {windowId} has no UIContainer.");
                return null;
            }

            component.gameObject.SetActive(true);
            return component;
        }

        public UniTask<ConfigContainer> GetConfigContainer() =>
            Load<ConfigContainer>(AssetPath.CONFIG_CONTAINER);

        public UniTask<GraphicsPreset> GetMobileGraphicsPreset() =>
            Load<GraphicsPreset>(AssetPath.MOBILE_GRAPHICS_PRESET);

        public UniTask<GraphicsPreset> GetDesktopGraphicsPreset() =>
            Load<GraphicsPreset>(AssetPath.DESKTOP_GRAPHICS_PRESET);

        public async UniTask<TextCollection> GetLocal(LocaleConfig localeConfig)
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

        public UniTask<WindowStaticData> GetWindowStaticData() => Load<WindowStaticData>(AssetPath.WINDOW_CONFIG_PATH);

        public UniTask<Enemy> CreateSimpleEnemy(Vector3 position, Quaternion rotation) =>
            Instantiate<Enemy>(AssetPath.ENEMY, position, rotation);

        public UniTask<BigEnemy> CreateBigEnemy(Vector3 position, Quaternion rotation) =>
            Instantiate<BigEnemy>(AssetPath.BIG_ENEMY, position, rotation);

        public UniTask<LargeEnemy> CreateLargeEnemy(Vector3 position, Quaternion rotation)
            => Instantiate<LargeEnemy>(AssetPath.LARGE_ENEMY, position, rotation);

        public UniTask<Finish> CreateFinish(Vector3 position, Quaternion rotation) =>
            Instantiate<Finish>(AssetPath.FINISH, position, rotation);

        public async UniTask<RingHolder> CreateRing(Vector3 position, Spawner.Colors[] colors, int level)
        {
            var ringPrefab = await Instantiate<RingHolder>(AssetPath.RING, position);
            if (ringPrefab == null)
                return null;

            if (ringPrefab.Renderers != null)
                ringPrefab.Renderers.sharedMaterial.color = colors[level].RingColor;

            if (ringPrefab.TransRenderers != null)
                ringPrefab.TransRenderers.sharedMaterial.color = colors[level].RingTransColor;

            return ringPrefab;
        }

        public UniTask<GameObject> CreateSmoke(Vector3 position, Quaternion rotation) =>
            Instantiate(AssetPath.SMOKE, position, rotation);

        public async UniTask<GameObject> CreateEnemyRagdoll(Vector3 position, Quaternion rotation)
        {
            GameObject enemy = await Instantiate(AssetPath.ENEMY_RAGDOLL, position, rotation);
            enemy.layer = 8;
            return enemy;
        }

        public async UniTask<GameObject> CreatePlayerRagdoll(Vector3 position, Quaternion rotation)
        {
            GameObject player = await Instantiate(AssetPath.PLAYER_RAGDOLL, position, rotation);
            player.layer = 8;
            return player;
        }

        public PlayerController CreatePlayer(GameObject root, Vector3 position, Quaternion rotation) =>
            Instantiate<PlayerController>(root, position, rotation);

        public UniTask<GameObject> CreateSlingshot(Vector3 position) =>
            Instantiate(AssetPath.SLINGSHOT, position);

        public UniTask<PlayerController> CreatePlayer(Vector3 position) =>
            Instantiate<PlayerController>(AssetPath.PLAYER, position);

        public LevelSystem.Level CreateLevel(int levelId)
        {
            var levelContainer = _configService.Get<LevelContainer>();
            return levelContainer[levelId];
        }

        public UniTask<ExplosionBarrel> CreateBarrel(Vector3 transformPosition) =>
            Instantiate<ExplosionBarrel>(AssetPath.BARREL, transformPosition);

        public void GetRingByType(
            RingData ringData,
            GameObject currentChildGo,
            PlayerFactory playerFactory,
            AudioService audioService)
        {
            if (ringData == null || currentChildGo == null)
                return;

            RingBase ring = ringData.RingType switch
            {
                RingType.Additive => currentChildGo.AddComponent<AdditiveRing>(),
                RingType.Multiplier => currentChildGo.AddComponent<MultiplierRing>(),
                RingType.Reducer => currentChildGo.AddComponent<ReducerRing>(),
                RingType.Divider => currentChildGo.AddComponent<DividerRing>(),
                _ => null
            };

            InitializeRing(ring, ringData, playerFactory, audioService);
        }

        private void InitializeRing(
            RingBase ring,
            RingData ringData,
            PlayerFactory playerFactory,
            AudioService audioService)
        {
            if (ring == null)
                return;

            ring.Effect = ringData.Effect;
            ring.Speed = ringData.Speed;
            ring.MovementAxis = ringData.MovementAxis;
            ring.Construct(playerFactory, audioService);
        }

        private GameObject Instantiate(GameObject prefab, Vector3 position = default, Quaternion rotation = default,
            Transform parent = null, bool isActivateGameObject = true)
        {
            if (prefab == null)
            {
                UnityEngine.Debug.LogError("[AssetProvider] Tried to instantiate null prefab.");
                return null;
            }

            prefab.SetActive(false);
            GameObject gameObject = Object.Instantiate(prefab, position, rotation, parent);
            _projectObjectInjector.Inject(gameObject);
            gameObject.SetActive(isActivateGameObject);
            prefab.SetActive(isActivateGameObject);
            return gameObject;
        }

        private async UniTask<GameObject> Instantiate(string path, Vector3 position = default,
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
            if (prefab == null)
            {
                UnityEngine.Debug.LogError($"[AssetProvider] Tried to instantiate null prefab for component {typeof(T).Name}.");
                return null;
            }

            prefab.SetActive(false);
            GameObject gameObject = Object.Instantiate(prefab, position, rotation, parent);
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                UnityEngine.Debug.LogError($"[AssetProvider] Prefab {prefab.name} has no component {typeof(T).Name}.");
                Object.Destroy(gameObject);
                prefab.SetActive(isActivateGameObject);
                return null;
            }

            _projectObjectInjector.Inject(component);
            component.enabled = componentEnabled;
            gameObject.SetActive(isActivateGameObject);
            prefab.SetActive(isActivateGameObject);
            return component;
        }

        private async UniTask<T> Instantiate<T>(string path, Vector3 position = default, Quaternion rotation = default,
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

        private UniTask<T> Load<T>(string path) where T : Object
        {
            // return UnityEngine.Resources.LoadAsync<T>(path).AsTask<T>();
            var asset = Addressables.LoadAssetAsync<T>(path);
            return asset.Task.AsUniTask();
        }

        public void Dispose() { }
    }
}
