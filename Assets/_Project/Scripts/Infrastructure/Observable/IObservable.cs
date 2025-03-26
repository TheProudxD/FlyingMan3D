using System;

namespace _Project.Scripts.Infrastructure.Observable
{
    public interface IObservable<out T>
    {
        event Action<T> Changed;

        event Action<T, T> ChangedWithOld;
    }
}