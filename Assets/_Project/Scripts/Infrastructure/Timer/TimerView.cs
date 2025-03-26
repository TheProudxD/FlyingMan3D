using _Project.Scripts.Gameplay;
using LitMotion;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI.Views
{
    public class TimerView : BaseView
    {
        [Inject] private Timer _timer;

        [SerializeField] private TextMeshProUGUI _minusTxt;

        private void OnEnable()
        {
            _timer.Time.Changed += OnTimerChanged;
            OnTimerChanged(_timer.Time.Value);
        }

        private void OnDisable() => _timer.Time.Changed -= OnTimerChanged;

        private void OnTimerChanged(int timeInSeconds)
        {
            int minutes = timeInSeconds / 60;
            int seconds = timeInSeconds % 60;
            Text.SetText($"{minutes:00}:{seconds:00}");
        }

        public void ShowMinusText() =>
            AnimationService.Color(_minusTxt, new Color(0.95f, 0f, 0f, 1f), new Color(0.95f, 0f, 0f, 0f), 1.5f,
                ease: Ease.OutBack);
    }
}