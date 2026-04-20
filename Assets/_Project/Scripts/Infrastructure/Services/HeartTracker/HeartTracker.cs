using System;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Views;

namespace _Project.Scripts.Gameplay
{
    public class HeartTracker : IDisposable
    {
        private readonly int _heartsNumber = 3; // TODO: into config
        private readonly UIFactory _uiFactory;
        private readonly GameFactory _gameFactory;

        private int _heartCounter;
        private LivesTrackerView _view;

        public HeartTracker(UIFactory uiFactory, GameFactory gameFactory)
        {
            _uiFactory = uiFactory;
            _gameFactory = gameFactory;
            _heartCounter = _heartsNumber;
        }

        public event Action OnHeartsEnded;

        public void Initialize()
        {
            TryBindView();
            DisplayDefaultHearts();
        }

        private void DecreaseHeart()
        {
            if (_heartCounter <= 0)
                return;

            _heartCounter--;
            DisplayHearts();

            if (_heartCounter <= 0)
                OnHeartsEnded?.Invoke();
        }

        private void DisplayHearts()
        {
            if (TryBindView() == false)
                return;

            _view.DisplayHearts(_heartCounter);
        }

        private bool TryBindView()
        {
            if (_view != null)
                return true;

            Hud hud = _uiFactory.GetHUD();

            if (hud == null)
                return false;

            _view = hud.GetComponentInChildren<LivesTrackerView>(true);

            return _view != null;
        }

        public void DisplayDefaultHearts()
        {
            _heartCounter = _heartsNumber;
            DisplayHearts();
        }

        public void Dispose() { }
    }
}
