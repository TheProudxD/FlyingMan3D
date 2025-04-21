using System;

namespace YG
{
    public partial class SavesYG
    {
        public int level = 1;
        public int richestLevel = -1;
        public int money = 250;

        public int health = 1;
        public int healthProgress = 1;
        [NonSerialized] public int healthDelta = 1;

        public float movingSpeed = 3.5f;
        public int movingSpeedProgress = 1;
        [NonSerialized] public float movingSpeedDelta = 1.5f;

        public float flyingControl = 65;
        public int flyingControlProgress = 1;
        [NonSerialized] public float flyingControlDelta = 3f;
    }
}