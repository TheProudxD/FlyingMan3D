using _Project.Scripts.Infrastructure.Observable;

namespace _Project.Scripts.Data
{
    public class PowerupProgress
    {
        public int health = 1;
        public int healthProgress = 1;
        public float movingSpeed = 3.5f;
        public int movingSpeedProgress = 1;
        public float flyingControl = 65;
        public int flyingControlProgress = 1;

        public PowerupProgress(int health, int healthProgress, float movingSpeed, int movingSpeedProgress,
            float flyingControl, int flyingControlProgress)
        {
            this.health = health;
            this.healthProgress = healthProgress;
            this.movingSpeed = movingSpeed;
            this.movingSpeedProgress = movingSpeedProgress;
            this.flyingControl = flyingControl;
            this.flyingControlProgress = flyingControlProgress;
        }
    }

    public class PlayerProgress
    {
        public readonly ObservableVariable<int> CurrentLevel;
        public readonly ObservableVariable<int> MoneyNumber;
        public readonly ObservableVariable<int> RichestLevel;

        public PlayerProgress(int currentLevel, int moneyNumber, int richestLevel)
        {
            CurrentLevel = new ObservableVariable<int>(currentLevel);
            MoneyNumber = new ObservableVariable<int>(moneyNumber);
            RichestLevel = new ObservableVariable<int>(richestLevel);
        }

        public override string ToString() => CurrentLevel.Value + ", " + MoneyNumber.Value;
    }
}