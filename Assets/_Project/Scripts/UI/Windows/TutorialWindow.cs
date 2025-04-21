using System;
using System.Collections;
using System.Linq;
using _Project.Scripts.Gameplay;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Tools.Extensions;
using LitMotion;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.UI.Windows
{
    public class TutorialWindow : WindowBase
    {
        [Inject] private GameFactory _gameFactory;
        [Inject] private UIFactory _uiFactory;
        [Inject] private MetricService _metricService;
        [Inject] private AnimationService _animationService;

        [SerializeField] private RectTransform _cursor;
        [SerializeField] private Image _slider;
        [SerializeField] private Image _background;
        [SerializeField] private TextMeshProUGUI _holdAndDragText;
        [SerializeField] private TextMeshProUGUI _tapToThrowText;

        private const int TUTORIAL_DURATION = 5;
        private readonly CompositeMotionHandle _compositeMotionHandle = new();
        private readonly WaitForSecondsRealtime _tutorialDurationWaitForSeconds = new(TUTORIAL_DURATION);
        private Coroutine _closeTutorialCoroutine;

        public override void Show()
        {
            base.Show();

            _background.Deactivate();
            _slider.Deactivate();
            _holdAndDragText.Deactivate();
            _tapToThrowText.Deactivate();
            _cursor.Deactivate();
            AnimateHandShootingCursor();
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (_closeTutorialCoroutine != null)
                    StopCoroutine(_closeTutorialCoroutine);

                Hide();
            }
        }

        private void AnimateHandShootingCursor()
        {
            Hud hud = _uiFactory.GetHUD();
            hud.Hide();

            _cursor.Activate();
            _tapToThrowText.Activate();
            _cursor.anchoredPosition = Vector3.zero;

            _animationService.ShakingScale(_cursor, 1f, 1.4f, 0.5f, loops: 15, callback: Hide,
                compositeMotionHandle: _compositeMotionHandle);
        }

        public void AnimateHandMovementCursor()
        {
            _background.Activate();
            _cursor.Activate();
            _slider.Activate();
            _holdAndDragText.Activate();
            _tapToThrowText.Deactivate();

            _compositeMotionHandle.Cancel();

            _closeTutorialCoroutine = StartCoroutine(CloseTutorial());

            Time.timeScale = 0;
            Rect rect = _slider.rectTransform.rect;
            float y = _slider.rectTransform.anchoredPosition.y - 70;
            int offset = 60;
            Vector2 leftScreen = new Vector3(rect.xMin + offset, y, 0);
            Vector2 rightScreen = new Vector3(rect.xMax + offset, y, 0);

            _animationService.AnimatePath(_cursor, leftScreen, rightScreen, 1,
                compositeMotionHandle: _compositeMotionHandle, loopType: LoopType.Yoyo);
        }

        public override void Hide()
        {
            base.Hide();
            Hud hud = _uiFactory.GetHUD();
            hud.Show();
            Time.timeScale = 1;

            _holdAndDragText.Deactivate();
            _tapToThrowText.Deactivate();
            _compositeMotionHandle.Cancel();

            if (_closeTutorialCoroutine != null)
                StopCoroutine(_closeTutorialCoroutine);

            _metricService.TutorialPassed();
        }

        private IEnumerator CloseTutorial()
        {
            Hud hud = _uiFactory.GetHUD();
            hud.Hide();
            yield return _tutorialDurationWaitForSeconds;

            Hide();
        }
    }
}