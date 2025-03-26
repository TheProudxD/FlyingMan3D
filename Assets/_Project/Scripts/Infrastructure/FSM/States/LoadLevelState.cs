using System.Collections;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.PersistentProgress;
using _Project.Scripts.Tools.Coroutine;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Views;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class LoadLevelState : IPayLoadState<string>, IInitializable
    {
        private readonly SceneLoader _sceneLoader;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly SaveLoadService _saveLoadService;
        private readonly UIFactory _uiFactory;
        private readonly HeartTracker _heartTracker;
        private readonly AssetProvider _assetProvider;
        private readonly GameFactory _gameFactory;

        private StateMachine _stateMachine;
        private string _sceneName;

        public LoadLevelState(SceneLoader sceneLoader, LoadingCurtain loadingCurtain,
            SaveLoadService saveLoadService, UIFactory uiFactory, HeartTracker heartTracker,
            AssetProvider assetProvider, GameFactory gameFactory)
        {
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _saveLoadService = saveLoadService;
            _uiFactory = uiFactory;
            _heartTracker = heartTracker;
            _assetProvider = assetProvider;
            _gameFactory = gameFactory;
        }

        public void Enter(string sceneName)
        {
            _loadingCurtain.Show();

            _sceneName = sceneName;

            Coroutines.StartRoutine(Initialize());
        }

        public IEnumerator Initialize()
        {
            yield return _uiFactory.Initialize();
            yield return _gameFactory.Initialize();

            _uiFactory.GetHUD().Initialize();
            _heartTracker.Initialize();

            foreach (ScoreBaseView view in Object.FindObjectsByType<ScoreBaseView>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
            {
                view.Initialize();
            }
            
            _sceneLoader.Load(_sceneName, OnLoaded);
        }

        private void OnLoaded()
        {
            _saveLoadService.InformAll();

            _stateMachine.Enter<GameLoopState, GameEnterState>(GameEnterState.LoadNext);
        }

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        public void Exit()
        {
            //_loadingCurtain.Hide();
        }
    }
}