using System;
using System.Collections;
using _Project.Scripts.Infrastructure.Observable;
using _Project.Scripts.Infrastructure.Services;
using _Project.Scripts.Tools.Coroutine;
using UnityEngine;

namespace _Project.Scripts.Gameplay
{
    public class Timer : IService
    {
        public enum TimerOverResultType
        {
            OutOfTime,
            Force
        }

        private readonly ObservableVariable<int> _time;
        private int _initialTime;
        private Coroutine _coroutine;
        
        public IReadonlyObservableVariable<int> Time => _time;

        public event Action<TimerOverResultType> TimeOver;

        public Timer(int time = 0) => _time = new ObservableVariable<int>(time);

        public void Start(int time)
        {
            _initialTime = time;

            if (_coroutine != null)
            {
                Coroutines.StopRoutine(_coroutine);
            }

            _coroutine = Coroutines.StartRoutine(DecreaseTime(time));
        }

        public void Stop()
        {
            Coroutines.StopRoutine(_coroutine);
            _time.Value = 0;
            TimeOver?.Invoke(TimerOverResultType.Force);
        }

        public void Reset() => _time.Value = _initialTime;

        public void Restart()
        {
            if (_coroutine == null)
                return;

            Coroutines.StopRoutine(_coroutine);
            Reset();
            _coroutine = Coroutines.StartRoutine(DecreaseTime(_time.Value));
        }

        public void Pause() => Coroutines.StopRoutine(_coroutine);

        public void Unpause() => _coroutine = Coroutines.StartRoutine(DecreaseTime(_time.Value));

        private IEnumerator DecreaseTime(int time)
        {
            _time.Value = time;

            if (_time.Value <= 0)
            {
                TimeOver?.Invoke(TimerOverResultType.OutOfTime);
                yield break;
            }

            while (_time.Value > 0)
            {
                _time.Value -= 1;
                yield return new WaitForSeconds(1);
            }

            TimeOver?.Invoke(TimerOverResultType.OutOfTime);
        }

        public void Decrease(int value)
        {
            _time.Value -= value;
            _time.Value = Mathf.Max(_time.Value, 0);
        }
    }
}