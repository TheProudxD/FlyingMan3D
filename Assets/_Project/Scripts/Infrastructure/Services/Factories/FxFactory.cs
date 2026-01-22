using _Project.Scripts.Infrastructure.Services.AssetManagement; 
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class FxFactory : IService
    {
        private readonly AssetProvider _assetProvider;

        public FxFactory(AssetProvider assetProvider) => _assetProvider = assetProvider;

        public async UniTask<GameObject> CreateEnemyRagdoll(Vector3 position, Quaternion rotation) =>
            await _assetProvider.CreateEnemyRagdoll(position, rotation);

        public async UniTask<GameObject> CreatePlayerRagdoll(Vector3 position, Quaternion rotation) =>
            await _assetProvider.CreatePlayerRagdoll(position, rotation);

        public async UniTask<GameObject> CreateSmoke(Vector3 position, Quaternion rotation) =>
            await _assetProvider.CreateSmoke(position, rotation);
    }
}