using System;

namespace _Project.Scripts.Tools.Other
{
    /// <summary>
    /// XOR encyption.
    /// NOTE: This is not a cryptographically secure method of encryption,
    /// use at own risk!
    /// </summary>
    public static class CryptoSymmetric
    {
        public static byte[] Encrypt(byte[] data, byte[] key) => XORArrayWithKey(data, EqualizeKey(data, key));

        public static byte[] Decrypt(byte[] data, byte[] key) => Encrypt(data, key);

        private static byte[] EqualizeKey(byte[] data, byte[] key)
        {
            //We need to repeat the key
            if (key.LongLength < data.LongLength)
            {
                long keyOriginalLength = key.LongLength;

                byte[] dest = new byte[data.Length];

                for (int i = 0; i < dest.Length; i++)
                {
                    dest[i] = key[i % keyOriginalLength];
                }

                return dest;
            }

            //We need to cut the key down
            if (key.LongLength > data.LongLength)
            {
                byte[] dest = new byte[data.LongLength];
                Array.Copy(key, 0, dest, 0, data.Length);
                return dest;
            }

            return key;
        }

        private static byte[] XORArrayWithKey(byte[] input, byte[] key)
        {
            byte[] XORd = new byte[input.LongLength];

            for (long i = 0; i < key.LongLength; i++)
            {
                XORd[i] = (byte)(input[i] ^ key[i]);
            }

            return XORd;
        }
    }
}