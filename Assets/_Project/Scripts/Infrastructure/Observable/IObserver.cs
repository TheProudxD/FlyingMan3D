using System;

namespace _Project.Scripts.Infrastructure.Observable
{
    public interface IObserver<in T> : IDisposable
    {
        void AddObservable(IObservable<T> observable);
    }
}