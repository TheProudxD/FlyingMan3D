using System.Collections.Generic;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services.Level;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure.Services.Factories
{
    public class PlayerFactory : IService
    {
        private readonly LevelPlayerLifecycleService _playerLifecycleService;

        public PlayerFactory(LevelPlayerLifecycleService playerLifecycleService) =>
            _playerLifecycleService = playerLifecycleService;

        public async UniTask<PlayerController> CreateMainPlayer() => await _playerLifecycleService.CreateMainPlayer();

        public PlayerController GetNewPlayer() => _playerLifecycleService.GetNewPlayer();

        public PlayerController GetMainPlayer() => _playerLifecycleService.GetMainPlayer();

        public IReadOnlyList<PlayerController> GetAllPlayers() => _playerLifecycleService.GetAllPlayers();

        public void DestroyPlayers() => _playerLifecycleService.DestroyPlayers();

        public void RemovePlayer(PlayerController player) => _playerLifecycleService.RemovePlayer(player);

        public void DestroyLastPlayer() => _playerLifecycleService.DestroyLastPlayer();
    }
}
