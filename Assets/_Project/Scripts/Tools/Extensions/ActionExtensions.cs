using System;
using System.Diagnostics;

namespace _Project.Scripts.Tools
{
    public static class ActionExtensions
    {
        /// <summary>
        /// Simply loops a given number of times
        /// </summary>
        /// <param name="cycles"></param>
        /// <param name="action"></param>
        public static void Repeat(this Action action, ulong cycles)
        {
            if (cycles == 0)
            {
                return;
            }

            for (ulong i = 0; i < cycles; i++)
            {
                action();
            }
        }

        /// <summary>
        /// Times how long the action took to complete and returns that time in milliseconds.
        /// </summary>
        public static long SpeedTest(this Action action)
        {
            Stopwatch watch = Stopwatch.StartNew();
            action();
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }
    }
}