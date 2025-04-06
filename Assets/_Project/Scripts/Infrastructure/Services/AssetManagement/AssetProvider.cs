using System;
using System.Collections;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.Infrastructure.Services.Localization;
using _Project.Scripts.Infrastructure.Services.Localization.SO;
using _Project.Scripts.Tools;
using _Project.Scripts.Tools.Camera;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Windows;
using Reflex.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.Services.AssetManagement
{
    public class AssetProvider : IService, IDisposable, IInitializable
    {
        private readonly Container _container;
        private ConfigService _configService;

        public AssetProvider(Container container) => _container = container;

        public IEnumerator Initialize()
        {
            _configService = _container.Resolve<ConfigService>();
            yield break;
        }

        public Hud CreateHUD() => Instantiate<Hud>(UIAssetsPath.HUD);

        public AudioServiceView CreateAudioServiceView() => Instantiate<AudioServiceView>(AssetPath.AUDIO_SERVICE_VIEW);

        public Transform CreateUIRoot() => Instantiate<UIRoot>(UIAssetsPath.UI_ROOT_PATH).transform;

        public LoadingCurtain CreateLoadingCurtain() => Instantiate<LoadingCurtain>(UIAssetsPath.LOADING_CURTAIN);

        public CameraSetup CreateCameraSetup() =>
            Instantiate<CameraSetup>(UIAssetsPath.CameraSetup, overrideTransform: false);

        public WindowBase Instantiate(WindowId windowId, Transform uiRoot)
        {
            WindowConfig windowConfig = _configService.ForWindow(windowId);
            windowConfig.Prefab.gameObject.SetActive(false);
            WindowBase component = Object.Instantiate(windowConfig.Prefab, uiRoot);
            _container.Inject(component);
            component.gameObject.SetActive(true);
            windowConfig.Prefab.gameObject.SetActive(true);
            return component;
        }

        public ConfigContainer GetConfigContainer() => Load<ConfigContainer>(AssetPath.CONFIG_CONTAINER);

        public GraphicsPreset GetMobileGraphicsPreset() =>
            Load<GraphicsPreset>(AssetPath.MOBILE_GRAPHICS_PRESET);

        public GraphicsPreset GetDesktopGraphicsPreset() =>
            Load<GraphicsPreset>(AssetPath.DESKTOP_GRAPHICS_PRESET);

        public TextCollection GetLocal(LocaleConfig localeConfig)
        {
            string path = string.Format(AssetPath.LOCALE_RESOURCES_PATH_FORMAT, localeConfig.Code);
            var textCollection = Load<TextCollection>(path);

            if (textCollection == null)
            {
                textCollection = Load<TextCollection>(path);
            }

            if (textCollection == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                UnityEngine.Debug.LogError("No collection found at path " + path);
#endif
            }

            return textCollection;
        }

        public WindowStaticData GetWindowStaticData() => Load<WindowStaticData>(AssetPath.WINDOW_CONFIG_PATH);

        public Enemy GetEnemy(Vector3 position, Quaternion rotation) =>
            Instantiate<Enemy>(AssetPath.ENEMY, position, rotation);

        public Finish GetFinish(Vector3 position, Quaternion rotation) =>
            Instantiate<Finish>(AssetPath.FINISH, position, rotation);

        public RingHolder CreateRing(Vector3 position, Spawner.Colors[] colors, int level)
        {
            var ringPrefab = Instantiate<RingHolder>(AssetPath.RING, position);

            ringPrefab.LeftRenderer.sharedMaterial.color = colors[level].RingColor;
            ringPrefab.LeftTransRenderer.sharedMaterial.color = colors[level].RingTransColor;
            ringPrefab.RightRenderer.sharedMaterial.color = colors[level].RingColor;
            ringPrefab.RightTransRenderer.sharedMaterial.color = colors[level].RingTransColor;

            return ringPrefab;
        }

        public GameObject CreateSmoke(Vector3 position, Quaternion rotation) =>
            Instantiate(AssetPath.SMOKE, position, rotation);

        public GameObject CreateEnemyRagdoll(Vector3 position, Quaternion rotation)
        {
            GameObject enemy = Instantiate(AssetPath.ENEMY_RAGDOLL, position, rotation);
            enemy.layer = 8;
            return enemy;
        }

        public GameObject CreatePlayerRagdoll(Vector3 position, Quaternion rotation)
        {
            GameObject player = Instantiate(AssetPath.PLAYER_RAGDOLL, position, rotation);
            player.layer = 8;
            return player;
        }

        public PlayerController CreatePlayer(GameObject root, Vector3 position, Quaternion rotation) =>
            Instantiate<PlayerController>(root, position, rotation);

        public GameObject CreateSlingshot(Vector3 position) =>
            Instantiate(AssetPath.SLINGSHOT, position);

        public PlayerController CreatePlayer(Vector3 position) =>
            Instantiate<PlayerController>(AssetPath.PLAYER, position);

        public Level CreateLevel(int levelId)
        {
            var levelContainer = _configService.Get<LevelContainer>();
            return levelContainer[levelId];
        }

        public void GetRingByType(RingData ringData, GameObject currentChildGo)
        {
            switch (ringData.RingType)
            {
                case RingType.Additive:
                    var a = currentChildGo.AddComponent<AdditiveRing>();
                    a.Effect = ringData.Effect;
                    a.Text.SetText("+" + ringData.Effect);
                    _container.Inject(a);
                    break;
                case RingType.Multiplier:
                    var m = currentChildGo.AddComponent<MultiplierRing>();
                    m.Effect = ringData.Effect;
                    m.Text.SetText("x" + ringData.Effect);
                    _container.Inject(m);
                    break;
                case RingType.Reducer:
                    var r = currentChildGo.AddComponent<ReducerRing>();
                    r.Effect = ringData.Effect;
                    r.Text.SetText("-" + ringData.Effect);
                    _container.Inject(r);
                    break;
                case RingType.Divider:
                    var d = currentChildGo.AddComponent<DividerRing>();
                    d.Effect = ringData.Effect;
                    d.Text.SetText("/" + ringData.Effect);
                    _container.Inject(d);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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

        private GameObject Instantiate(string path, Vector3 position = default, Quaternion rotation = default,
            Transform parent = null, bool overrideTransform = true, bool isActivateGameObject = true)
        {
            var prefab = Load<GameObject>(path);

            if (overrideTransform == false)
            {
                position = prefab.transform.position;
                rotation = prefab.transform.rotation;
            }

            return Instantiate(prefab, position, rotation, parent, isActivateGameObject);
        }

        private T Instantiate<T>(GameObject prefab, Vector3 position = default, Quaternion rotation = default,
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

        private T Instantiate<T>(string path, Vector3 position = default, Quaternion rotation = default,
            Transform parent = null, bool overrideTransform = true, bool isActivateGameObject = true,
            bool componentEnabled = true)
            where T : MonoBehaviour
        {
            var prefab = Load<GameObject>(path);

            if (overrideTransform == false)
            {
                position = prefab.transform.position;
                rotation = prefab.transform.rotation;
            }

            return Instantiate<T>(prefab, position, rotation, parent, isActivateGameObject, componentEnabled);
        }

        private T Load<T>(string path) where T : Object =>
            //AssetRef.AssetsLoader.LoadAssetSync<T>(AssetRef.None);
            UnityEngine.Resources.Load<T>(path);

        private IEnumerator LoadAsync<T>(string path) where T : Object
        {
            yield return UnityEngine.Resources.LoadAsync<T>(path);
        }

        public void Dispose() { }
    }
}