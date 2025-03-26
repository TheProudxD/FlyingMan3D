using System;

namespace _Project.Scripts.Data
{
    [Serializable]
    public class PlayerData
    {
        public int MoneyAmount;

        // TODO: R9
        public Action Changed;

        public PlayerData(int moneyAmount)
        {
            if (moneyAmount < 0) throw new ArgumentOutOfRangeException(nameof(moneyAmount));

            MoneyAmount = moneyAmount;
        }
        
        public void Add(int amount)
        {
            MoneyAmount += amount;
            Changed?.Invoke();
        }
    }
}