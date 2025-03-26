using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace _Project.Scripts.Tools.Extensions
{
    public static class GenericsExtensions
    {
        public static bool IsNull<T>(this T instance) => instance == null;

        public static bool NotNull<T>(this T instance) => instance != null;

        public static bool IsNull<T>(this T instance, Action<T> actionIfNull)
            where T : class
        {
            if (instance != null)
                return false;

            actionIfNull?.Invoke(instance);

            return true;
        }

        public static bool NotNull<T>(this T instance, Action<T> actionIfNotNull)
            where T : class
        {
            if (instance == null) return false;

            actionIfNotNull?.Invoke(instance);

            return true;
        }
        
        public static void Write<T>(this T message) => Console.WriteLine(message);

        public static void DebugLog<T>(this T message) => Debug.Log(message);

        public static T DeepCopy<T>(this T self)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("Type must be iserializable");
            }

            if (ReferenceEquals(self, null))
                return default(T);

            var formatter = new BinaryFormatter();
            using var stream = new MemoryStream();
            formatter.Serialize(stream, self);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }

        public static T With<T>(this T self, Action<T> set)
        {
            set?.Invoke(self);
            return self;
        }

        public static T With<T>(this T self, Action<T> apply, Func<bool> when)
        {
            if (when())
                apply?.Invoke(self);

            return self;
        }

        public static T With<T>(this T self, Action<T> set, bool when)
        {
            if (when)
                set?.Invoke(self);

            return self;
        }
        public static string ToElementsString<TSource>(
            this IEnumerable<TSource> enumerable,
            string splitter = ", ")
        {
            string str = "";

            using (var enumerator = enumerable.GetEnumerator())
            {
                bool putFirst = false;

                while (enumerator.MoveNext())
                {
                    str +=
                        (putFirst ? splitter : "") + //If I already put the first index, add in the splitter
                        enumerator.Current.ToString();

                    putFirst = true;
                }
            }

            return str;
        }
        
        public static string ToElementsString<TSource1, TSource2>(
            this Dictionary<TSource1, TSource2> dict,
            string keyValueSeparator = " => ",
            string elementSeparator = "\n")
        {
            string returnString = "";

            var dictEnumerator = dict.GetEnumerator();

            while (dictEnumerator.MoveNext())
            {
                returnString +=
                    elementSeparator +
                    dictEnumerator.Current.Key.ToString() +
                    keyValueSeparator +
                    dictEnumerator.Current.Value.ToString();
            }

            return returnString.Remove(0, elementSeparator.Length);
        }
    }
}