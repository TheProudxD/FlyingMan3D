using System.Collections.Generic;
using _Project.Scripts.Infrastructure.Services.Level;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class PlayerFactory : IService
    {
        private readonly LevelLifecycleService _levelLifecycle;

        public PlayerFactory(LevelLifecycleService levelLifecycle) => _levelLifecycle = levelLifecycle;

        public async UniTask<PlayerController> CreateMainPlayer() => await _levelLifecycle.CreateMainPlayer();

        public PlayerController GetNewPlayer() => _levelLifecycle.GetNewPlayer();

        public PlayerController GetMainPlayer() => _levelLifecycle.GetMainPlayer();

        public IReadOnlyList<PlayerController> GetAllPlayers() => _levelLifecycle.Players;

        public void DestroyPlayers() => _levelLifecycle.DestroyPlayers();

        public void RemovePlayer(PlayerController player) => _levelLifecycle.RemovePlayer(player);

        public void DestroyLastPlayer() => _levelLifecycle.DestroyLastPlayer();
    }
}