using System;
using _Project.Scripts.Gameplay;
using Reflex.Attributes;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.Services.LevelSystem
{
    [CreateAssetMenu(menuName = "Create Level", fileName = "Level", order = 0)]
    public class Level : ScriptableObject
    {
        [Inject] private Timer _timer;

        [field: SerializeField] public int LevelTimer { get; private set; }
        [field: SerializeField] public UpperRing[] Rings { get; private set; }
        [field: SerializeField] public int EnemyCount { get; private set; }
        [field: SerializeField] public float TimeDif { get; private set; }
        [field: SerializeField] public float MaxLaunchSpeed { get; private set; }
        [field: SerializeField] public float StopDistance { get; private set; }
        [field: SerializeField] public float EnemyStopDistance { get; private set; }
        [field: SerializeField] public float EnemyMoveSpeed { get; private set; }

        public Score Score { get; private set; } = new();

        public event Action LevelStarted;
        public event Action<LevelResultType> LevelFinished;

        public void Initialize(int starsNumber)
        {
            Subscribe();
            LevelStarted?.Invoke();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _timer.TimeOver += LevelLost;
        }

        private void Unsubscribe()
        {
            _timer.TimeOver -= LevelLost;
        }

        private void LevelLost(Timer.TimerOverResultType timerOverResultType)
        {
            switch (timerOverResultType)
            {
                case Timer.TimerOverResultType.OutOfTime:
                    LevelFinished?.Invoke(LevelResultType.Lose);
                    break;
                case Timer.TimerOverResultType.Force:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(timerOverResultType), timerOverResultType, null);
            }
        }
    }
}