using System;
using System.Collections.Generic;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services.AssetManagement;
using _Project.Scripts.Infrastructure.Services.Scene;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Level
{
    public class LevelPlayerLifecycleService : IService
    {
        private readonly AssetProvider _assetProvider;
        private readonly LevelEntityRegistry _entityRegistry;
        private readonly PlayerStateCopyService _playerStateCopyService;
        private LevelSceneReferences _sceneRefs;

        public LevelPlayerLifecycleService(
            AssetProvider assetProvider,
            LevelEntityRegistry entityRegistry,
            PlayerStateCopyService playerStateCopyService)
        {
            _assetProvider = assetProvider;
            _entityRegistry = entityRegistry;
            _playerStateCopyService = playerStateCopyService;
        }

        public void SetSceneRef(LevelSceneReferences sceneRefs) => _sceneRefs = sceneRefs;

        public async UniTask<PlayerController> CreateMainPlayer()
        {
            var startPosition = new Vector3(0, 1.75f, -1);
            PlayerController mainPlayer = await _assetProvider.CreatePlayer(startPosition);
            if (mainPlayer == null)
                throw new InvalidOperationException("Failed to create main player instance.");

            Rigidbody hips = mainPlayer.SelfHips;
            Rigidbody capsule = _sceneRefs?.Slingshot?.Capsule;

            if (hips == null)
            {
                UnityEngine.Object.Destroy(mainPlayer.gameObject);
                throw new InvalidOperationException(
                    "Main player has no hips Rigidbody. Check Player prefab bindings.");
            }

            if (capsule == null)
            {
                UnityEngine.Object.Destroy(mainPlayer.gameObject);
                throw new InvalidOperationException(
                    "Slingshot capsule is not initialized. Check scene refs and slingshot setup.");
            }

            var fixedJoint = hips.gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = capsule;
            mainPlayer.SetInitial(capsule.transform);
            AddPlayer(mainPlayer);

            _entityRegistry.TrackLevelObject(mainPlayer.gameObject);
            return mainPlayer;
        }

        public PlayerController GetNewPlayer()
        {
            PlayerController mainPlayer = GetMainPlayer();

            if (mainPlayer == null)
                return null;

            float spawnGap = UnityEngine.Random.Range(2f, 5f);
            Vector3 randomPos = UnityEngine.Random.onUnitSphere * spawnGap;

            GameObject root = mainPlayer.gameObject;
            PlayerController player =
                _assetProvider.CreatePlayer(root, root.transform.position + randomPos, root.transform.rotation);

            _playerStateCopyService.CopyPlayerState(mainPlayer, player);
            AddPlayer(player);
            return player;
        }

        public void AddPlayer(PlayerController player) => _entityRegistry.AddPlayer(player);

        public void RemovePlayer(PlayerController player) => _entityRegistry.RemovePlayer(player);

        public void DestroyPlayers() => _entityRegistry.DestroyPlayers();

        public PlayerController GetMainPlayer() => _entityRegistry.GetMainPlayer();

        public IReadOnlyList<PlayerController> GetAllPlayers() => _entityRegistry.Players;

        public void DestroyLastPlayer() => _entityRegistry.DestroyLastPlayer();
    }
}
