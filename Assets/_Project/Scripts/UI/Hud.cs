using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Tools.Extensions;
using _Project.Scripts.UI.Buttons;
using _Project.Scripts.UI.Windows;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class Hud : MonoBehaviour, IUIContainer
    {
        [Inject] private GameFactory _gameFactory;
        [Inject] private AnimationService _animationService;

        [SerializeField] private PauseButton _pauseButton;
        [SerializeField] private TMP_Text _enemiesNumberText;
        [SerializeField] private TMP_Text _playersNumberText;

        [field: SerializeField] public GameObject TapToThrow { get; private set; }
        [field: SerializeField] public GameObject PowerupShop { get; private set; }

        public void Initialize()
        {
            Hide();
        }

        public void Show()
        {
            _pauseButton.Activate();
            _gameFactory.EnemiesCounter.ChangedWithOld += EnemiesCounterChanged;
            _gameFactory.PlayersCounter.ChangedWithOld += PlayersCounterChanged;
            _gameFactory.EnemiesCounter?.Invoke();
            _gameFactory.PlayersCounter?.Invoke();
        }

        private void EnemiesCounterChanged(int old, int @new)
        {
            _animationService.ResourceChanged(transform, old, @new, 0.5f,
                x => _enemiesNumberText.SetText(x.ToString()));
        } 
        
        private void PlayersCounterChanged(int old, int @new)
        {
            _animationService.ResourceChanged(transform, old, @new, 0.5f,
                x => _playersNumberText.SetText(x.ToString()));
        }

        public void Hide()
        {
            _pauseButton.Deactivate();
            _gameFactory.EnemiesCounter.ChangedWithOld -= EnemiesCounterChanged;
            _gameFactory.PlayersCounter.ChangedWithOld -= PlayersCounterChanged;
        }

        public void DeactivateStartText()
        {
            if (TapToThrow.activeInHierarchy)
                TapToThrow.SetActive(false);
            
            if (PowerupShop.activeInHierarchy)
                PowerupShop.SetActive(false);
        }
    }
}