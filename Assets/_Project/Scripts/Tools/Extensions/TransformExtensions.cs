using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Tools.Extensions
{
    public static class TransformExtensions
    {
        public static Transform FindDeep(this Transform obj, string name)
        {
            if (obj.name == name)
            {
                return obj;
            }

            int count = obj.childCount;

            for (int i = 0; i < count; ++i)
            {
                Transform posObj = obj.GetChild(i).FindDeep(name);

                if (posObj != null)
                {
                    return posObj;
                }
            }

            return null;
        }

        public static bool TryFind(this Transform obj, string name, out Transform foundObj)
        {
            foundObj = default;

            Transform foundedObject = obj.Find(name);

            foundObj = foundedObject != null
                ? foundedObject
                : throw new System.NullReferenceException("Can't find transform of object: " +
                                                          obj.name + " with name: " + name);

            return true;
        }

        public static bool TryFindDeep(this Transform obj, string name, out Transform foundObj)
        {
            foundObj = default;

            Transform foundedObject = obj.FindDeep(name);

            if (foundedObject == null)
                throw new System.NullReferenceException("Can't find transform of object: " +
                                                        obj.name + " with name: " + name);

            foundObj = foundedObject;
            return true;
        }

        public static List<T> GetAll<T>(this Transform obj)
        {
            var results = new List<T>();
            obj.GetComponentsInChildren(results);
            return results;
        }

        public static IEnumerable<Transform> Children(this Transform parent)
        {
            foreach (Transform child in parent)
            {
                yield return child;
            }
        }

        public static bool InRangeOf(this Transform source, Transform target, float maxDistance, float maxAngle = 360f)
        {
            Vector3 directionToTarget = (target.position - source.position).WithY(0);

            return directionToTarget.magnitude <= maxDistance &&
                   Vector3.Angle(source.forward, directionToTarget) <= maxAngle / 2;
        }

        public static void DestroyChildren(this Transform parent) =>
            parent.ForEveryChild(child => Object.Destroy(child.gameObject));

        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        public static void DestroyChildAt(this Transform transform, int i) =>
            Object.Destroy(transform.GetChild(i).gameObject);

        public static void ForEveryChild(this Transform parent, System.Action<Transform> action)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
                action(parent.GetChild(i));
        }

        public static void DisableChildren(this Transform parent) =>
            parent.ForEveryChild(child => child.gameObject.SetActive(false));

        public static void EnableChildren(this Transform parent) =>
            parent.ForEveryChild(child => child.gameObject.SetActive(true));

        public static void DestroyChildrenImmediate(this Transform parent) =>
            parent.ForEveryChild(child => Object.DestroyImmediate(child.gameObject));
    }
}