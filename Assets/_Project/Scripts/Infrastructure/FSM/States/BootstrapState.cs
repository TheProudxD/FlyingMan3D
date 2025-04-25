using System.Collections;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Localization;
using _Project.Scripts.Tools.Coroutine;
using _Project.Scripts.UI;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class BootstrapState : IState, IInitializable
    {
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly MetricService _metricService;
        private readonly LocalizationService _localizationService;
        private readonly DeviceSpecificGraphics _graphicsService;
        private readonly ConfigService _configService;
        private readonly AssetProvider _assetProvider;
        private readonly AudioService _audioService;
        private readonly GameFactory _gameFactory;
        // private readonly AudioLoader _audioLoader;

        private StateMachine _stateMachine;

        public BootstrapState(SceneLoader sceneLoader, LoadingCurtain loadingCurtain,
            MetricService metricService, LocalizationService localizationService,
            DeviceSpecificGraphics graphicsService, ConfigService configService, AssetProvider assetProvider,
            AudioService audioService, GameFactory gameFactory)
        {
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _metricService = metricService;
            _localizationService = localizationService;
            _graphicsService = graphicsService;
            _configService = configService;
            _assetProvider = assetProvider;
            _audioService = audioService;
            _gameFactory = gameFactory;
        }

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        public void Enter() => Coroutines.StartRoutine(Initialize());

        public IEnumerator Initialize()
        {
            _loadingCurtain.Show();
            Cursor.lockState = CursorLockMode.Confined;
            yield return _assetProvider.Initialize();
            yield return _configService.Initialize();
            yield return _audioService.Initialize();

            // Coroutines.StartRoutine(_audioLoader.Load());

            _localizationService.DefineLanguage();
            _graphicsService.DefineGraphicsSettings();
            _metricService.GameLoaded();
            LoadGame();
        }

        public void Exit() { }

        private void LoadGame() => _sceneLoader.Load(SceneNames.MAIN_SCENE, OnEnterLoadLevel);

        private void OnEnterLoadLevel() => _stateMachine.Enter<LoadGameState>();
    }
}