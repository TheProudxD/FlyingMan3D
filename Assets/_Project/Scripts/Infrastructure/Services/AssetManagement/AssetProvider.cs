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

        public Level CreateLevel(int levelId)
        {
            var levelContainer = _configService.Get<LevelContainer>();
            Level levelPrefab = levelContainer[levelId];
            return Instantiate<Level>(levelPrefab.gameObject, new Vector3(0, -250, 0));
        }
    }
}