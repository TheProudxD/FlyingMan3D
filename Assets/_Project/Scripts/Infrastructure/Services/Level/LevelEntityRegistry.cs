using System.Collections.Generic;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Observable;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.Services.Level
{
    public class LevelEntityRegistry : IService, ITaskInitializable
    {
        private readonly List<GameObject> _levelObjects = new();
        private readonly List<PlayerController> _players = new();
        private readonly List<EnemyBase> _enemies = new();

        public IReadOnlyList<PlayerController> Players => _players;
        public IReadOnlyList<EnemyBase> Enemies => _enemies;
        public ObservableVariable<int> EnemiesCounter { get; private set; }
        public ObservableVariable<int> PlayersCounter { get; private set; }

        public UniTask Initialize()
        {
            EnemiesCounter ??= new ObservableVariable<int>(_enemies.Count);
            PlayersCounter ??= new ObservableVariable<int>(_players.Count);

            EnemiesCounter.Value = _enemies.Count;
            PlayersCounter.Value = _players.Count;
            return UniTask.CompletedTask;
        }

        public void TrackLevelObject(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            _levelObjects.Add(gameObject);
        }

        public void AddPlayer(PlayerController player)
        {
            if (player == null)
                return;

            _players.Add(player);
            PlayersCounter.Value = _players.Count;
        }

        public void AddEnemy(EnemyBase enemy)
        {
            if (enemy == null)
                return;

            _enemies.Add(enemy);
            EnemiesCounter.Value = _enemies.Count;
        }

        public void RemovePlayer(PlayerController player, bool destroyGameObject = true)
        {
            if (player == null)
                return;

            _players.Remove(player);
            PlayersCounter.Value = _players.Count;

            if (destroyGameObject)
                Object.Destroy(player.gameObject);
        }

        public void RemoveEnemy(EnemyBase enemy)
        {
            if (enemy == null)
                return;

            _enemies.Remove(enemy);
            EnemiesCounter.Value = _enemies.Count;
        }

        public void DestroyPlayers()
        {
            foreach (PlayerController player in _players)
            {
                if (player == null)
                    continue;

                Object.Destroy(player.gameObject);
            }

            _players.Clear();
            PlayersCounter.Value = _players.Count;
        }

        public PlayerController GetMainPlayer() => _players.Count > 0 ? _players[0] : null;

        public void DestroyLastPlayer()
        {
            int lastPlayerIndex = _players.Count - 1;

            if (lastPlayerIndex < 0)
                return;

            PlayerController player = _players[lastPlayerIndex];
            RemovePlayer(player);
        }

        public void ClearLevel()
        {
            foreach (GameObject gameObject in _levelObjects)
            {
                if (gameObject != null)
                    Object.Destroy(gameObject);
            }

            foreach (PlayerController player in _players)
            {
                if (player != null)
                    Object.Destroy(player.gameObject);
            }

            foreach (EnemyBase enemy in _enemies)
            {
                if (enemy != null)
                    Object.Destroy(enemy.gameObject);
            }

            _levelObjects.Clear();
            _players.Clear();
            _enemies.Clear();

            EnemiesCounter.Value = _enemies.Count;
            PlayersCounter.Value = _players.Count;
        }
    }
}
