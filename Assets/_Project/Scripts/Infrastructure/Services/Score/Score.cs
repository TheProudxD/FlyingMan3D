using _Project.Scripts.Infrastructure.Observable;

namespace _Project.Scripts.Infrastructure.Services.LevelSystem
{
    public class Score
    {
        public const int DEFAULT_VALUE = 0;

        private readonly ObservableVariable<int> _value = new(DEFAULT_VALUE);

        public IReadonlyObservableVariable<int> Value => _value;

        public virtual void Add(int value)
        {
            _value.Value += value;
        }

        public void Reset()
        {
            _value.Value = DEFAULT_VALUE;
        }
    }
}