using System;
using _Project.Scripts.Infrastructure.Observable;

namespace _Project.Scripts.Data
{
    public class PlayerProgress
    {
        public readonly ObservableVariable<int> CurrentLevel;
        public readonly ObservableVariable<int> MoneyNumber;

        public PlayerProgress(int currentLevel, int moneyNumber)
        {
            CurrentLevel = new ObservableVariable<int>(currentLevel);
            MoneyNumber = new ObservableVariable<int>(moneyNumber);
        }

        public override string ToString() => CurrentLevel.Value + ", " + MoneyNumber.Value;
    }
}