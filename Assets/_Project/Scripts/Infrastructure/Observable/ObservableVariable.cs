using System;

namespace _Project.Scripts.Infrastructure.Observable
{
    public class ObservableVariable<T> : IReadonlyObservableVariable<T>
    {
        public event Action<T> Changed;
        public event Action<T, T> ChangedWithOld;

        private T _current;
        private T _old;

        public T Value
        {
            get => _current;
            set
            {
                _old = _current;
                _current = value;
                Invoke();
            }
        }

        public ObservableVariable(T value = default) => Value = value;

        public override string ToString() => _current.ToString();

        public void Invoke()
        {
            ChangedWithOld?.Invoke(_old, _current);
            Changed?.Invoke(_current);
        }

        public ObservableVariable<T> Clone<T>() => (ObservableVariable<T>)MemberwiseClone();
    }
}