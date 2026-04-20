using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Level;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Infrastructure.Services.Scene;
using BhorGames.Mechanics;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class GameFactory : IService
    {
        private readonly LevelLifecycleService _levelLifecycle;
        private readonly LevelResourceService _levelResourceService;
        private readonly AssetProvider _assetProvider;
        private readonly CameraService _cameraService;

        private LevelSceneReferences _sceneRefs;
        private LevelSystem.Level _gameLevel;

        public GameFactory(LevelLifecycleService levelLifecycle, LevelResourceService levelResourceService, AssetProvider assetProvider, CameraService cameraService)
        {
            _levelLifecycle = levelLifecycle;
            _levelResourceService = levelResourceService;
            _assetProvider = assetProvider;
            _cameraService = cameraService;
        }

        public async UniTask Initialize() => await _levelLifecycle.Initialize();

        public void SetSceneRef(LevelSceneReferences sceneRefs)
        {
            _sceneRefs = sceneRefs;
        }

        public void ClearLevelHolder() => _levelLifecycle.ClearLevelHolder();

        public LevelSystem.Level CreateLevel() =>
            _gameLevel = _assetProvider.CreateLevel(_levelResourceService.Current.Value);

        public LevelSystem.Level GetCurrentLevel() => _gameLevel;

        public Finish GetFinish() => _levelLifecycle.GetFinish();

        public async UniTask<Finish> CreateFinish(Vector3 vector3, Quaternion identity) =>
            await _levelLifecycle.CreateFinish(vector3, identity);

        public async UniTask<RingHolder>
            GetRing(Vector3 calculateRingPosition, Spawner.Colors[] colorArray, int index) =>
            await _levelLifecycle.CreateRing(calculateRingPosition, colorArray, index);

        public async UniTask CreateBarrel(Vector3 randomPosition) =>
            await _levelLifecycle.CreateBarrel(randomPosition);

        public async UniTask CreateSlingshot(Vector3 position) =>
            await _levelLifecycle.CreateSlingshot(position);

        public Platform GetPlatform() => _sceneRefs.Platform;

        public Spawner GetSpawner() => _sceneRefs.Spawner;

        public Indicator GetIndicator() => _sceneRefs.Indicator;

        public void SetPlayerCamera() =>
            _cameraService.SetPlayerCamera(_levelLifecycle.GetMainPlayer());

        public void SetFinishCamera(float finishZPosition) =>
            _cameraService.SetFinishCamera(finishZPosition);

        public ObservableVariable<int> EnemiesCounter => _levelLifecycle.EnemiesCounter;

        public ObservableVariable<int> PlayersCounter => _levelLifecycle.PlayersCounter;
    }
}
