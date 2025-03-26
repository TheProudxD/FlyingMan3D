namespace _Project.Scripts.Infrastructure.Observable
{
    public interface IReadonlyObservableVariable<out T> : IObservable<T>
    {
        public T Value { get; }
    }
}