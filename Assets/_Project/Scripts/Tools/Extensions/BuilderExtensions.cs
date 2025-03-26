using System;

namespace _Project.Scripts.Tools.Extensions
{
    public static class BuilderExtensions
    {
        public static T With<T>(this T self, Action<T> set)
        {
            set.Invoke(self);
            return self;
        }

        public static T With<T>(this T self, Action<T> apply, Func<bool> when)
        {
            if (when())
            {
                apply(self);
            }

            return self;
        }

        public static T With<T>(this T self, Action<T> apply, bool when)
        {
            if (when)
            {
                apply(self);
            }

            return self;
        }
    }
}
