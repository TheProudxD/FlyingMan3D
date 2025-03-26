using System;
using _Project.Scripts.Infrastructure.Observable;

namespace _Project.Scripts.Data
{
    public class PlayerProgress
    {
        public readonly ObservableVariable<int> HintsNumber;
        public readonly ObservableVariable<int> CurrentLevel;

        public PlayerProgress(int hintsNumber, int currentLevel)
        {
            if (hintsNumber < 0) throw new ArgumentOutOfRangeException(nameof(hintsNumber));
            if (currentLevel < 0) throw new ArgumentOutOfRangeException(nameof(currentLevel));

            HintsNumber = new ObservableVariable<int>(hintsNumber);
            CurrentLevel = new ObservableVariable<int>(currentLevel);
        }

        public override string ToString() => HintsNumber.Value + ", " + CurrentLevel.Value;
    }
}