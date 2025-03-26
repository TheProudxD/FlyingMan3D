using System;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Infrastructure.Services.Config;
using LitMotion;
using Reflex.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.UI
{
    public class CoinRewardAnimation : MonoBehaviour
    {
        [Inject] private ConfigService _configService;
        [Inject] private AnimationService _animationService;

        [SerializeField] private GameObject _coinPrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private Transform _targetPos;
        [SerializeField] private int _coinsAmount;
        [SerializeField] private float _radius;

        private int _finishedCounter;
        private CoinRewardAnimationConfig _animationConfig;

        public event Action OnAnimationFinished;

        private void Awake()
        {
            _animationConfig = _configService.Get<CoinRewardAnimationConfig>();

            for (int i = 0; i < _coinsAmount; i++)
            {
                Vector3 randomPosition = _container.position + (Vector3)Random.insideUnitCircle * _radius;
                Instantiate(_coinPrefab, randomPosition, Quaternion.Euler(new Vector3(90, 0, 0)), _container);
            }
        }

        public void CountCoins()
        {
            float delay = _animationConfig.StartDelay;
            _finishedCounter = 0;

            for (int i = 0; i < _coinsAmount; i++)
            {
                Transform coin = _container.GetChild(i);
                Vector3 initialPosition = coin.position;

                _animationService.Scale(coin, Vector3.zero, Vector3.one, _animationConfig.ScaleUpDuration,
                    ease: Ease.OutBack, delay: delay);

                _animationService.Move(coin, coin.position, _targetPos.position, _animationConfig.MoveDuration,
                    ease: Ease.InBack, delay: delay + 0.5f);

                _animationService.Rotate(coin, coin.localRotation.eulerAngles, Vector3.zero,
                    _animationConfig.RotationDuration,
                    ease: Ease.InBack, delay: delay + 0.5f);

                _animationService.Scale(coin, Vector3.one, Vector3.zero, _animationConfig.ScaleDownDuration,
                    ease: Ease.OutBack, delay: delay + 1.5f, callback: () =>
                    {
                        coin.transform.position = initialPosition;
                        Count();
                    });

/*
                LMotion.Create(Vector3.zero, Vector3.one, _animationConfig.ScaleUpDuration)
                    .WithDelay(delay)
                    .WithEase(Ease.OutBack)
                    .Bind(v => coin.transform.localScale = v);

                LMotion.Create(coin.position, _targetPos.position, _animationConfig.MoveDuration)
                    .WithDelay(delay + 0.5f)
                    .WithEase(Ease.InBack)
                    .Bind(x => coin.position = x);
                    
                LMotion.Create(coin.localRotation.eulerAngles, Vector3.zero, _animationConfig.RotationDuration)
                    .WithDelay(delay + 0.5f)
                    .WithEase(Ease.InBack)
                    .Bind(v => coin.localRotation = Quaternion.Euler(v));

                LMotion.Create(Vector3.one, Vector3.zero, _animationConfig.ScaleDownDuration)
                    .WithDelay(delay + 1.5f)
                    .WithEase(Ease.OutBack)
                    .WithOnComplete(() =>
                    {
                        coin.transform.position = initialPosition;
                        Count();
                    })
                    .Bind(v => coin.transform.localScale = v);
*/
                delay += _animationConfig.DelayStep;
            }
        }

        private void Count()
        {
            _finishedCounter++;

            if (_finishedCounter == _coinsAmount)
                OnAnimationFinished?.Invoke();
        }
    }
}