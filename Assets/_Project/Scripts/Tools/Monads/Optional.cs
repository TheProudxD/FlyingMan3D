using System;
using System.Collections.Generic;

namespace _Project.Scripts.Tools.Monads
{
    public struct Optional<T>
    {
        public static readonly Optional<T> NoValue = new();

        private readonly bool _hasValue;
        private readonly T _value;

        public Optional(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public T Value => _hasValue ? _value : throw new InvalidOperationException("No value");
    
        public bool HasValue => _hasValue;

        public T GetValueOrDefault() => _value;

        public T GetValueOrDefault(T defaultValue) => _hasValue ? _value : defaultValue;

        public TResult Match<TResult>(Func<T, TResult> onValue, Func<TResult> onNoValue) => _hasValue ? onValue(_value) : onNoValue();

        public Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> bind) => _hasValue ? bind(_value) : Optional<TResult>.NoValue;

        public Optional<TResult> Select<TResult>(Func<T, TResult> map) => _hasValue ? new Optional<TResult>(map(_value)) : Optional<TResult>.NoValue;

        public static Optional<TResult> Combine<T1, T2, TResult>(Optional<T1> first, Optional<T2> second,
            Func<T1, T2, TResult> combiner)
        {
            if (first.HasValue && second.HasValue)
            {
                return new Optional<TResult>(combiner(first.Value, second.Value));
            }

            return Optional<TResult>.NoValue;
        }

        public static Optional<T> Some(T value) => new(value);

        public static Optional<T> None() => NoValue;

        public override bool Equals(object obj) => obj is Optional<T> other && Equals(other);

        public bool Equals(Optional<T> other) =>
            !_hasValue ? !other._hasValue : EqualityComparer<T>.Default.Equals(_value, other._value);

        public override int GetHashCode() =>
            (_hasValue.GetHashCode() * 397) ^ EqualityComparer<T>.Default.GetHashCode(_value);

        public override string ToString() => _hasValue ? $"Some({_value})" : "None";

        public static implicit operator Optional<T>(T value) => new(value);

        public static implicit operator bool(Optional<T> value) => value._hasValue;

        public static explicit operator T(Optional<T> value) => value.Value;
    }
}