using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Tools.Other
{
    public class PoolMono<T> where T : MonoBehaviour
    {
        public bool AutoExpand { get; set; }
        public T Prefab { get; }
        public Transform Container { get; }

        protected List<T> Pool;

        public PoolMono(T prefab, int count)
        {
            Prefab = prefab;
            Container = null;
            CreatePool(Prefab, count, Container);
        }

        public PoolMono(T prefab, int count, Transform container)
        {
            Prefab = prefab;
            Container = container;
            CreatePool(Prefab, count, Container);
        }


        private void CreatePool(T prefab, int count, Transform container)
        {
            Pool = new List<T>();

            for (int i = 0; i < count; i++)
                CreateObject(prefab, container);
        }

        private T CreateObject(T prefab, Transform container, bool isActiveByDefault = false)
        {
            T createdObject = Object.Instantiate(prefab, container);
            createdObject.gameObject.SetActive(isActiveByDefault);
            Pool.Add(createdObject);
            return createdObject;
        }

        public bool HasFreeElement(out T element)
        {
            foreach (T mono in Pool.Where(mono => !mono.gameObject.activeInHierarchy))
            {
                mono.gameObject.SetActive(true);
                element = mono;
                return true;
            }

            element = null;
            return false;
        }

        public T GetFreeElement()
        {
            if (HasFreeElement(out T element))
                return element;

            if (AutoExpand)
                return CreateObject(Prefab, Container, true);

            throw new Exception(
                $"The pool of type {typeof(T).Name} is empty. Current elements number is: {Pool.Count}");
        }

        public T[] GetFreeElements(int count)
        {
            var freeElements = new List<T>();

            foreach (T mono in Pool.Where(mono => !mono.gameObject.activeInHierarchy))
            {
                freeElements.Add(mono);
                mono.gameObject.SetActive(true);
            }

            if (freeElements.Count >= count) return freeElements.ToArray();

            if (!AutoExpand)
                throw new Exception(
                    $"Pool of type {typeof(T).Name} doesn't have so much free elements. Only {freeElements.Count}/{count}");

            int difference = count - freeElements.Count;

            for (int i = 0; i < difference; i++)
            {
                T createdObject = CreateObject(Prefab, Container);
                createdObject.gameObject.SetActive(true);
                freeElements.Add(createdObject);
            }

            return freeElements.ToArray();
        }

        public T[] GetAllElements() => Pool.ToArray();

        public T[] GetAllActiveElements() => Pool.Where(element => element.gameObject.activeInHierarchy).ToArray();

        public int GetFreeElementsCount() => Pool.Count(mono => !mono.gameObject.activeInHierarchy);
    }
}