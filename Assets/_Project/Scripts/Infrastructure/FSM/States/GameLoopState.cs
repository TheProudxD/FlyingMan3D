using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Audio;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.LevelSystem;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Tools.Coroutine;
using _Project.Scripts.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Infrastructure.FSM.States
{
    public class GameLoopState : IPayLoadState<IExitableState>
    {
        private static readonly int Win = Animator.StringToHash("Win");

        private readonly GameFactory _gameFactory;
        private readonly UIFactory _uiFactory;
        private readonly AudioService _audioService;
        private readonly StatisticsService _statisticsService;
        private readonly LoadingCurtain _loadingCurtain;
        private readonly LevelResourceService _levelResourceService;

        private StateMachine _stateMachine;
        private Hud _hud;
        private Level _level;

        public GameLoopState(GameFactory gameFactory,
            AudioService audioService, UIFactory uiFactory, StatisticsService statisticsService,
            LoadingCurtain loadingCurtain,
            LevelResourceService levelResourceService)
        {
            _gameFactory = gameFactory;
            _audioService = audioService;
            _uiFactory = uiFactory;
            _statisticsService = statisticsService;
            _loadingCurtain = loadingCurtain;
            _levelResourceService = levelResourceService;
        }

        public IExitableState FromState { get; set; }

        public void Enter(IExitableState fromState)
        {
            Resources.UnloadUnusedAssets();
            FromState = fromState;

            _gameFactory.EnemiesCounter.Changed += CheckEntitiesCount;
            _gameFactory.PlayersCounter.Changed += CheckEntitiesCount;

            //EnableHud();
            // _gameFactory.GetInputService().Enable();
        }

        private void CheckEntitiesCount(int _)
        {
            if (_gameFactory.Players.Count == 0 && _gameFactory.Enemies.Count >= 0)
            {
                foreach (Enemy item in _gameFactory.Enemies)
                {
                    item.Animator.SetBool(Win, true);
                    Object.Destroy(item.GetComponent<Rigidbody>());
                }

                _stateMachine.Enter<LoseLevelState>();
            }
            else if (_gameFactory.Enemies.Count == 0 && _gameFactory.Players.Count > 0)
            {
                foreach (PlayerController item in _gameFactory.Players)
                {
                    item.Animator.SetBool(Win, true);
                    Object.Destroy(item.GetComponent<Rigidbody>());
                }

                Coroutines.StartRoutine(WinCoroutine());
            }
        }

        public void Exit()
        {
            _gameFactory.EnemiesCounter.Changed -= CheckEntitiesCount;
            _gameFactory.PlayersCounter.Changed -= CheckEntitiesCount;
            // _gameFactory.GetInputService().Disable();
            //DisableHud();
        }

        private IEnumerator WinCoroutine()
        {
            ParticleSystem winParticle = _gameFactory.GetSpawner().WinParticle;
            winParticle.transform.position = Object.FindObjectOfType<Finish>().transform.position;
            winParticle.Play();
            yield return new WaitForSeconds(2);

            _stateMachine.Enter<WinLevelState>();
        }

        public void SetStateMachine(StateMachine value) => _stateMachine = value;

        private void EnableHud() => _hud.Show();

        private void DisableHud() => _hud.Hide();
    }
}