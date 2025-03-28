using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using Reflex.Core;
using Reflex.Injectors;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.Tools
{
    public static class Utils
    {
        public static WaitForSeconds GetWaitForSeconds(float seconds) => WaitFor.Seconds(seconds);

#if UNITY_EDITOR
        public static void ClearConsole()
        {
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
        }
#endif

        public static string BuildPath(string key) => string.Concat(Application.persistentDataPath, "/", key);

        public static void Inject<T>(this Container container, T monoBehaviour)
        {
            // if (monoBehaviour == null)
            //    return;

            if (monoBehaviour is not MonoBehaviour monoBehaviourComponent)
                return;

            GameObjectInjector.InjectRecursive(monoBehaviourComponent.gameObject, container);
            AttributeInjector.Inject(monoBehaviour, container);

            if (monoBehaviourComponent.gameObject == null)
                return;


            monoBehaviourComponent.gameObject.SetActive(true);
        }

        public static string GiveAllFields<T>(this T obj)
        {
            return typeof(T)
                .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Aggregate("", (current, field) => current + field.Name + ": " + field.GetValue(obj) + " ");
        }

        public static T GetObstacle<T>(IEnumerable<RaycastHit> raycastHits)
            where T : MonoBehaviour
        {
            foreach (RaycastHit raycast in raycastHits.Where(raycast => raycast.collider != null))
            {
                if (raycast.collider.gameObject.TryGetComponent(out T t))
                {
                    return t;
                }
            }

            return null;
        }

        public static bool IsPointerOverUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return true;

            var pe = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pe, hits);
            return hits.Count > 0;
        }

        public static float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }

        public static Vector3 ApplyRotationToVector(Vector3 vec, Vector3 vecRotation) =>
            ApplyRotationToVector(vec, GetAngleFromVectorFloat(vecRotation));

        public static Vector3 ApplyRotationToVector(Vector3 vec, float angle) =>
            Quaternion.Euler(0, 0, angle) * vec;

        public static void IfConditions(bool condition1, bool condition2, Action only1, Action only2,
            Action both = null, Action neither = null
        )
        {
            if (condition1 && !condition2 && only1 != null)
                only1();

            if (!condition1 && condition2 && only2 != null)
                only2();

            if (condition1 && condition2 && both != null)
                both();

            if (!condition1 && !condition2 && neither != null)
                neither();
        }

        public static IEnumerator LerpFunction(float duration, Action<float> action)
        {
            float time = 0f;

            while (time < duration)
            {
                action?.Invoke(time / duration);

                time += Time.deltaTime;
                yield return null;
            }
        }

        public static IEnumerator LerpFunction(float duration, List<Action<float>> action)
        {
            var time = 0f;

            while (time < duration)
            {
                action.ForEach(a => a?.Invoke(time / duration));

                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}