using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.FSM;
using _Project.Scripts.Infrastructure.FSM.States;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Resources;
using _Project.Scripts.SO;
using _Project.Scripts.Tools.Extensions;
using _Project.Scripts.UI.Buttons;
using LitMotion;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows
{
    public class LoseWindow : WindowBase
    {
        [Inject] private AnimationService _animationService;
        [Inject] private GameFactory _gameFactory;
        [Inject] private StateMachine _stateMachine;
        [Inject] private AdsService _adsService;
        [Inject] private MetricService _metricService;
        [Inject] private LevelResourceService _levelResourceService;

        [SerializeField] private Transform _popup;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _skipButton;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        private readonly float _fadeOutDuration = 0.75f;
        private readonly float _fadeInDuration = 0.2f;

        public override void Show()
        {
            base.Show();

            _animationService.FadeOut(_popup.gameObject, _fadeOutDuration, Ease.OutBounce);
            _restartButton.Add(RestartGame);
            _skipButton.Deactivate();
            /*
            if (_gameFactory.IsRestartLevel)
            {
                _skipButton.Activate();
                _skipButton.Add(ContinueGame);
            }
            else
            {
                _skipButton.Deactivate();
            }
            */

            /*_descriptionText.SetText(
                $"Получи дополнительные <color=#{_rewardMoneyColor}>{_moreTimeData.AdditionalTime.ToString()}</color> секунд");
            */

            _metricService.LevelLost(_levelResourceService.Current.Value);
        }

        private void ContinueGame()
        {
            //_adsService.PlayInterstitial();
            Hide();
            _metricService.GameContinuedForAd();
            _stateMachine.Enter<ContinueLevelState>();
        }

        private void RestartGame()
        {
            _restartButton.Remove(RestartGame);
            _adsService.PlayInterstitial();
            AudioService.PlayClickSound();
            _stateMachine.Enter<RestartLevelState>();
            Hide();
        }

        public override void Hide()
        {
            _skipButton.Deactivate();
            _restartButton.Remove(RestartGame);
            _skipButton.Remove(ContinueGame);
            _animationService.FadeIn(_popup.gameObject, _fadeInDuration, callback: () => base.Hide());
        }
    }
}