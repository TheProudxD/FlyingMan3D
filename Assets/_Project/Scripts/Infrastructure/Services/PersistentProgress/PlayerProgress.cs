using _Project.Scripts.Infrastructure.Observable;

namespace _Project.Scripts.Data
{
    public class PowerupProgress
    {
        public int health { get; private set; } = 1;
        public int healthProgress { get; private set; } = 1;
        public float movingSpeed { get; private set; } = 3.5f;
        public int movingSpeedProgress { get; private set; } = 1;
        public float flyingControl { get; private set; } = 65;
        public int flyingControlProgress { get; private set; } = 1;

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

        public void SetHealthProgress(int value) => healthProgress = value;
        public void SetMovingSpeedProgress(int value) => movingSpeedProgress = value;
        public void SetFlyingControlProgress(int value) => flyingControlProgress = value;

        public void AddHealth(int delta) => health += delta;
        public void AddMovingSpeed(float delta) => movingSpeed += delta;
        public void AddFlyingControl(float delta) => flyingControl += delta;
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