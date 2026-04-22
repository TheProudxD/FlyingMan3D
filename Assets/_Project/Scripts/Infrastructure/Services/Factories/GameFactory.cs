using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Camera;
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
        private readonly LevelEntityRegistry _levelEntityRegistry;
        private readonly LevelRuntimeObjectFactory _levelRuntimeObjectFactory;
        private readonly LevelPlayerLifecycleService _levelPlayerLifecycleService;
        private readonly LevelResourceService _levelResourceService;
        private readonly AssetProvider _assetProvider;
        private readonly CameraService _cameraService;
        private LevelSceneReferences _sceneRefs;

        private LevelSystem.Level _gameLevel;

        public GameFactory(
            LevelEntityRegistry levelEntityRegistry,
            LevelRuntimeObjectFactory levelRuntimeObjectFactory,
            LevelPlayerLifecycleService levelPlayerLifecycleService,
            LevelResourceService levelResourceService,
            AssetProvider assetProvider,
            CameraService cameraService)
        {
            _levelEntityRegistry = levelEntityRegistry;
            _levelRuntimeObjectFactory = levelRuntimeObjectFactory;
            _levelPlayerLifecycleService = levelPlayerLifecycleService;
            _levelResourceService = levelResourceService;
            _assetProvider = assetProvider;
            _cameraService = cameraService;
        }

        public async UniTask Initialize() => await _levelEntityRegistry.Initialize();

        public void SetSceneRef(LevelSceneReferences sceneRefs) => _sceneRefs = sceneRefs;

        public void ClearLevelHolder()
        {
            _levelEntityRegistry.ClearLevel();
            _levelRuntimeObjectFactory.ResetState();
        }

        public LevelSystem.Level CreateLevel() =>
            _gameLevel = _assetProvider.CreateLevel(_levelResourceService.Current.Value);

        public LevelSystem.Level GetCurrentLevel() => _gameLevel;

        public Finish GetFinish() => _levelRuntimeObjectFactory.GetFinish();

        public async UniTask<Finish> CreateFinish(Vector3 vector3, Quaternion identity) =>
            await _levelRuntimeObjectFactory.CreateFinish(vector3, identity);

        public async UniTask<RingHolder>
            GetRing(Vector3 calculateRingPosition, Spawner.Colors[] colorArray, int index) =>
            await _levelRuntimeObjectFactory.CreateRing(calculateRingPosition, colorArray, index);

        public async UniTask CreateBarrel(Vector3 randomPosition) =>
            await _levelRuntimeObjectFactory.CreateBarrel(randomPosition);

        public async UniTask CreateSlingshot(Vector3 position) =>
            await _levelRuntimeObjectFactory.CreateSlingshot(position);

        public Platform GetPlatform() => _sceneRefs.Platform;

        public Spawner GetSpawner() => _sceneRefs.Spawner;

        public Indicator GetIndicator() => _sceneRefs.Indicator;

        public void SetPlayerCamera() =>
            _cameraService.SetPlayerCamera(_levelPlayerLifecycleService.GetMainPlayer());

        public void SetFinishCamera(float finishZPosition) =>
            _cameraService.SetFinishCamera(finishZPosition);

        public ObservableVariable<int> EnemiesCounter => _levelEntityRegistry.EnemiesCounter;

        public ObservableVariable<int> PlayersCounter => _levelEntityRegistry.PlayersCounter;
    }
}
