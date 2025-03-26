using System.Collections;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Infrastructure.Services.Review;
using _Project.Scripts.Tools.Extensions;
using _Project.Scripts.UI.Buttons;
using LitMotion;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows
{
    public class WinWindow : WindowBase
    {
        [Inject] private AnimationService _animationService;
        [Inject] private StateMachine _stateMachine;
        [Inject] private AdsService _adsService;
        [Inject] private MetricService _metricService;
        [Inject] private ConfigService _configService;
        [Inject] private UIFactory _uiFactory;
        [Inject] private ReviewShowService _reviewShowService;
        [Inject] private HeartTracker _heartTracker;
        [Inject] private LevelResourceService _levelResourceService;

        [SerializeField] private Transform _popup;
        [SerializeField] private ParticleSystem _confetti;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private ReplayGameButton _replayLevelButton;
        [SerializeField] private SendReviewButton _sendReviewButton;
        [SerializeField] private MoreGamesButton _moreGamesButton;

        private WinWindowAnimationsConfig _animationsConfig;
        private WaitForSecondsRealtime _showRestartButtonCoroutine;

        protected override void OnAwake()
        {
            base.OnAwake();
            _animationsConfig = _configService.Get<WinWindowAnimationsConfig>();
            _showRestartButtonCoroutine = new WaitForSecondsRealtime(_animationsConfig.TimeBeforeShowRestartButton);
        }

        public override void Show()
        {
            base.Show();

            Time.timeScale = 0;

            //_uiFactory.GetHUD().Hide();

            _animationService.FadeOut(_popup.gameObject, _animationsConfig.ShowDuration, Ease.OutBounce);

            _nextLevelButton.Add(LoadNextLevel);
            _nextLevelButton.Activate();

            _replayLevelButton.Activate();
            //StartCoroutine(ShowRestartButtonCoroutine());
            _replayLevelButton.Add(Restart);

            _metricService.LevelPassed();
            UpdateTitleText();

            TryShowSendReviewButton();

            //_coinRewardAnimation.OnAnimationFinished += LoadNextLevel;
        }

        private void UpdateTitleText()
        {
            //_titleText.SetText($"Уровень {_levelResourceService.ObservableValue} пройден!");
        }

        private IEnumerator ShowRestartButtonCoroutine()
        {
            yield return _showRestartButtonCoroutine;

            ShowRestartButton();
        }

        private void LoadNextLevel()
        {
            _nextLevelButton.Deactivate();
            AudioService.PlayClickSound();
            _adsService.PlayInterstitial();
            _stateMachine.Enter<GameLoopState, GameEnterState>(GameEnterState.LoadNext);
            Hide();
        }

        private void ShowRestartButton()
        {
            _replayLevelButton.Activate();

            /*_animationService.ShakingScale(_multiplierButton.transform, _animationsConfig.FromScale,
                _animationsConfig.ToScale, _animationsConfig.Duration, -1, Ease.OutSine,
                compositeMotionHandle: _compositeMotionHandle);*/
        }

        private void TryShowSendReviewButton()
        {
            if (_reviewShowService.CanShow())
            {
                _sendReviewButton.Activate();
                _moreGamesButton.Deactivate();
            }
            else
            {
                _moreGamesButton.Activate();
                _sendReviewButton.Deactivate();
            }
        }

        private void Restart()
        {
            _replayLevelButton.Remove(Restart);
            Hide();
        }

        public override void Hide()
        {
            _nextLevelButton.Remove(LoadNextLevel);
            _replayLevelButton.Remove(Restart);
            //_coinRewardAnimation.OnAnimationFinished -= LoadNextLevel;
            //_sendReviewButton.Deactivate();
            //_moreGamesButton.Deactivate();

            _animationService.FadeIn(_popup.gameObject, _animationsConfig.HideDuration, callback: () =>
            {
                Time.timeScale = 1;
                base.Hide();
            });
        }
    }
}