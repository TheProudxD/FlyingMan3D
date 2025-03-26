using System;
using System.Security.Cryptography;

namespace _Project.Scripts.Tools.Other
{
    public static class CryptoRand
    {
        private static RNGCryptoServiceProvider rng = new();

        /// <summary>
        /// Gets an array of random bytes.
        /// </summary>
        /// <returns>The byte array.</returns>
        /// <param name="byteCount">Length of byte array.</param>
        public static byte[] GetBytes(int byteCount)
        {
            byte[] randBytes = new byte[byteCount];

            rng.GetBytes(randBytes);

            return randBytes;
        }

        public static byte[] GetBytes(byte[] byteArray)
        {
            rng.GetBytes(byteArray);
            return byteArray;
        }

        /// <summary>
        /// Returns a random double from 0-1
        /// </summary>
        public static double Range()
        {
            byte[] bytes = new byte[8];
            rng.GetBytes(bytes);
            //Testing new way to generate random numbers
            //double d = Math.Abs(bytes.DecDouble()) % 1.0;
            ulong ul = BitConverter.ToUInt64(bytes, 0) / (1 << 11);
            double d = ul / (double)(1UL << 53);
            return d;
        }

        public static double Range(double min, double max) => (max - min) * Range() + min;

        public static float Range(float min, float max) => (max - min) * (float)Range() + min;

        /// <summary>
        /// Picks a random element from the parameters passed.
        /// </summary>
        /// <param name="array">Array.</param>
        /// <typeparam name="T">The type parameter.</typeparam>
        public static T Pick<T>(params T[] array)
            => array[(int)(array.Length * Range())];

        /// <summary>
        /// Has an n probability of returning a true
        /// </summary>
        /// <returns></returns>
        public static bool Chance(double n) => Range(0, 1) <= n;
    }
}