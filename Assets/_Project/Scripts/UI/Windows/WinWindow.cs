using System.Collections;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure;
using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Config;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.Infrastructure.Services.Review;
using _Project.Scripts.Tools.Extensions;
using _Project.Scripts.UI;
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
        [Inject] private LevelResourceService _levelResourceService;
        [Inject] private MoneyResourceService _moneyResourceService;
        [Inject] private GameFactory _gameFactory;

        [SerializeField] private Transform _popup;
        [SerializeField] private ParticleSystem _confetti;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _moneyRewardText;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private ReplayGameButton _replayLevelButton;
        [SerializeField] private WheelMultiplierButton _multiplierButton;
        [SerializeField] private CoinRewardAnimation _coinRewardAnimation;
        [SerializeField] private Image _emojiImage;

        private readonly CompositeMotionHandle _compositeMotionHandle = new();
        private WinWindowAnimationsConfig _animationsConfig;
        private WaitForSecondsRealtime _showRestartButtonCoroutine;
        private WinImagesConfig _winImagesConfig;
        private int _rewardAmount;
        private Coroutine _showNextLevelButtonRoutine;

        protected override void OnAwake()
        {
            base.OnAwake();
            _animationsConfig = _configService.Get<WinWindowAnimationsConfig>();
            _winImagesConfig = _configService.Get<WinImagesConfig>();
            _showRestartButtonCoroutine = new WaitForSecondsRealtime(_animationsConfig.TimeBeforeShowRestartButton);
        }

        public override void Show()
        {
            base.Show();

            _animationService.FadeOut(_popup.gameObject, _animationsConfig.ShowDuration, Ease.OutBounce);

            Time.timeScale = 0;

            _emojiImage.sprite = _winImagesConfig.Sprites.RandomElement();

            _rewardAmount = _gameFactory.GetCurrentLevel().MoneyReward;
            _multiplierButton.Activate();
            _multiplierButton.MakeInteractive();
            _multiplierButton.SetInitReward(_rewardAmount);
            _multiplierButton.Add(MultiplyMoney);
            UpdateMoneyRewardText();

            _nextLevelButton.Add(PlayMoneyFX);
            _replayLevelButton.Add(Restart);
            _replayLevelButton.Activate();
            SubscribeRewardFx();

            _showNextLevelButtonRoutine = StartCoroutine(ShowNextLevelButtonCoroutine());

            _moneyResourceService.Add(this, _rewardAmount);
            _metricService.LevelPassed(_levelResourceService.Current.Value);
            UpdateTitleText();
            AnimateEmoji();
        }

        private void AnimateEmoji() =>
            _animationService.RotateZ(_emojiImage.transform, 10f, 1.05f, ease: Ease.Linear, loopType: LoopType.Yoyo);

        private void PlayMoneyFX()
        {
            _nextLevelButton.Remove(PlayMoneyFX);
            _nextLevelButton.Deactivate();
            _multiplierButton.Deactivate();

            if (_coinRewardAnimation == null)
            {
                LoadNextLevel();
                return;
            }

            _coinRewardAnimation.CountCoins();
        }

        private void SubscribeRewardFx()
        {
            if (_coinRewardAnimation == null)
                return;

            _coinRewardAnimation.OnAnimationFinished -= LoadNextLevel;
            _coinRewardAnimation.OnAnimationFinished += LoadNextLevel;
        }

        private void UnsubscribeRewardFx()
        {
            if (_coinRewardAnimation == null)
                return;

            _coinRewardAnimation.OnAnimationFinished -= LoadNextLevel;
        }

        private void UpdateTitleText()
        {
            //_titleText.SetText($"Уровень {_levelResourceService.ObservableValue} пройден!");
        }

        private void MultiplyMoney() => _adsService.PlayRewardedVideo("multiplyMoney", () =>
        {
            _multiplierButton.Deactivate();
            _multiplierButton.Remove(MultiplyMoney);
            // StopCoroutine(_showContinueButtonCoroutine);
            // _metricService.ScoreMultiplierUsed();
            _moneyResourceService.Spend(this, _rewardAmount);
            _rewardAmount *= _multiplierButton.Multiplier;
            _moneyResourceService.Add(this, _rewardAmount);
            UpdateMoneyRewardText();
            ShowNextLevelButton();
        });

        private void UpdateMoneyRewardText() => _moneyRewardText.SetText($"+ {_rewardAmount}");

        private void ShowNextLevelButton()
        {
            _nextLevelButton.Activate();

            _animationService.ShakingScale(_multiplierButton.transform, _animationsConfig.FromScale,
                _animationsConfig.ToScale, _animationsConfig.Duration, -1, Ease.OutSine);
        }

        private IEnumerator ShowNextLevelButtonCoroutine()
        {
            _nextLevelButton.Deactivate();
            yield return _showRestartButtonCoroutine;

            ShowNextLevelButton();
        }

        private void LoadNextLevel()
        {
            if (IsHiding)
                return;

            _nextLevelButton.Deactivate();
            AudioService.PlayClickSound();
            _adsService.PlayInterstitial();
            _stateMachine.Enter<LoadLevelState>();
            Hide();
        }

        private void Restart()
        {
            _replayLevelButton.Remove(Restart);
            Hide();
        }

        public override void Hide()
        {
            if (!BeginHide())
                return;

            _nextLevelButton.Remove(PlayMoneyFX);
            _replayLevelButton.Remove(Restart);
            _multiplierButton.Remove(MultiplyMoney);
            UnsubscribeRewardFx();

            if (_showNextLevelButtonRoutine != null)
            {
                StopCoroutine(_showNextLevelButtonRoutine);
                _showNextLevelButtonRoutine = null;
            }

            _animationService.FadeIn(_popup.gameObject, _animationsConfig.HideDuration, callback: () =>
            {
                Time.timeScale = 1;
                CloseImmediately();
            });
        }
    }
}
