using System;
using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Factories;
using _Project.Scripts.Infrastructure.Services.Resources;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Gameplay
{
    public class HintService : IService
    {
        private readonly GameFactory _gameFactory;
        private readonly HintResourceService _hintResourceService;
        private readonly MetricService _metricService;

        private Action _hintUsed;
        private bool _inHintMode;

        public bool InHintMode => _inHintMode;

        public HintService(GameFactory gameFactory, HintResourceService hintResourceService,
            MetricService metricService)
        {
            _gameFactory = gameFactory;
            _hintResourceService = hintResourceService;
            _metricService = metricService;
        }

        public IReadonlyObservableVariable<int> HintsNumber => _hintResourceService.ObservableValue;

        public bool UseHint(Action hintUsed = null)
        {
            _hintUsed = hintUsed;

            if (InHintMode)
                return false;

            if (_hintResourceService.IsOutOfHints())
                return false;

            _inHintMode = true;
            _hintResourceService.Spend(this, 1);

            int findHintIndex = FindHintIndex();

            if (findHintIndex != -1)
            {
                var hintObj = Object.FindObjectOfType<HintStar>();
                hintObj.Enable();
            }

            return true;
        }

        private int FindHintIndex()
        {
            int hintIndex = -1;

            /*SpotItem[] spotItems = _gameFactory.GetCurrentLevel().PairItemTrue;

            for (int i = 0; i < spotItems.Length; i++)
            {
                if (!spotItems[i].isSpotted)
                {
                    hintIndex = i;
                    break;
                }
            }*/

            return hintIndex;
        }

        public void DisableHintMode()
        {
            if (_inHintMode == false)
                return;

            _metricService.HintUsed();
            Object.FindObjectOfType<HintStar>().Disable();
            _hintUsed?.Invoke();
            _inHintMode = false;
        }
    }
}