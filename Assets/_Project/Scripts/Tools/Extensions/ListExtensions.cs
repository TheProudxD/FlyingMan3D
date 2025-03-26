using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Tools.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandom<T>(this IList<T> list, IList<T> itemsToExclude)
        {
            if (list.IsNullOrEmpty())
                return default(T);
            
            T val = list[RandomIndex(list)];

            while (itemsToExclude.Contains(val))
                val = list[RandomIndex(list)];

            return val;
        }
        
        public static int RandomIndex<T>(this IList<T> list) => Random.Range(0, list.Count);

        public static bool IsNullOrEmpty<T>(this IList<T> list) => list == null || list.Count == 0;

        public static bool IsEmpty<T>(this IList<T> list) => list.Count == 0;
        
        public static void Shuffle<T>(this IList<T> list)
        {
            for (var i = list.Count - 1; i > 1; i--)
            {
                var j = Random.Range(0, i + 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }

        public static void Off(this IList<GameObject> list)
        {
            foreach (var obj in list)
                obj.SetActive(false);
        }

        public static void On(this IList<GameObject> list)
        {
            foreach (var obj in list)
                obj.SetActive(true);
        }

        public static int GetEqualsCount<T>(this IList<T> list, T obj)
        {
            int index = 0;

            foreach (var item in list)
            {
                if (item.Equals(obj))
                    index++;
            }

            if (index > 0)
                return index;

            return -1;
        }

        public static string Print<T>(this IList<T> list, bool pretty = false)
        {
            string result = "";

            if (list == null)
            {
                result = "NULL";
            }
            else if (list.Count == 0)
            {
                result = "EMPTY";
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    result += $"{i}: {list[i].ToString()}";

                    if (i < list.Count - 1)
                    {
                        result += pretty ? "\n" : ", ";
                    }
                }
            }

            return result;
        }
/*
        public static void All<T>(this IList<T> list, Action<T> action)
        {
            foreach (var item in list)
                action(item);
        }
        */
    }
}