using System;
using System.Collections.Generic;

namespace _Project.Scripts.Infrastructure.Observable
{
    public class ObservableVariable<T> : IReadonlyObservableVariable<T>
    {
        public event Action<T> Changed;
        public event Action<T, T> ChangedWithOld;

        private readonly IEqualityComparer<T> _equalityComparer;
        private T _current;
        private T _old;

        public T Value
        {
            get => _current;
            set
            {
                _old = _current;
                _current = value;

                if (_equalityComparer?.Equals(_old, _current) == false)
                    Invoke();
            }
        }

        public ObservableVariable() : this(default(T)) { }

        public ObservableVariable(T value) : this(value, EqualityComparer<T>.Default) { }

        public ObservableVariable(T value, IEqualityComparer<T> equalityComparer)
        {
            Value = value;
            _equalityComparer = equalityComparer;
        }

        public override string ToString() => _current.ToString();

        public void Invoke()
        {
            ChangedWithOld?.Invoke(_old, _current);
            Changed?.Invoke(_current);
        }

        // public ObservableVariable<TT> Clone<TT>() => (ObservableVariable<TT>)MemberwiseClone();
    }
}