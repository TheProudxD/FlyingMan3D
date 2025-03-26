using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Tools.LINQGameObject
{
    // API Frontend

    public static partial class GameObjectExtensions
    {
        // Traverse Game Objects, based on Axis(Parent, Child, Children, Ancestors/Descendants, BeforeSelf/BeforeAfter)

        /// <summary>Gets the parent GameObject of this GameObject. If this GameObject has no parent, returns null.</summary>
        public static GameObject Parent(this GameObject origin)
        {
            if (origin == null) return null;

            var parentTransform = origin.transform.parent;
            if (parentTransform == null) return null;

            return parentTransform.gameObject;
        }

        /// <summary>Gets the first child GameObject with the specified name. If there is no GameObject with the speficided name, returns null.</summary>
        public static GameObject Child(this GameObject origin, string name)
        {
            if (origin == null) return null;

            var child = origin.transform.Find(name); // transform.find can get inactive object
            if (child == null) return null;
            return child.gameObject;
        }

        /// <summary>Returns a collection of the child GameObjects.</summary>
        public static ChildrenEnumerable Children(this GameObject origin)
        {
            return new ChildrenEnumerable(origin, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the child GameObjects.</summary>
        public static ChildrenEnumerable ChildrenAndSelf(this GameObject origin)
        {
            return new ChildrenEnumerable(origin, true);
        }

        /// <summary>Returns a collection of the ancestor GameObjects of this GameObject.</summary>
        public static AncestorsEnumerable Ancestors(this GameObject origin)
        {
            return new AncestorsEnumerable(origin, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this element, and the ancestors of this GameObject.</summary>
        public static AncestorsEnumerable AncestorsAndSelf(this GameObject origin)
        {
            return new AncestorsEnumerable(origin, true);
        }

        /// <summary>Returns a collection of the descendant GameObjects.</summary>
        public static DescendantsEnumerable Descendants(this GameObject origin, Func<Transform, bool> descendIntoChildren = null)
        {
            return new DescendantsEnumerable(origin, false, descendIntoChildren);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and all descendant GameObjects of this GameObject.</summary>
        public static DescendantsEnumerable DescendantsAndSelf(this GameObject origin, Func<Transform, bool> descendIntoChildren = null)
        {
            return new DescendantsEnumerable(origin, true, descendIntoChildren);
        }

        /// <summary>Returns a collection of the sibling GameObjects before this GameObject.</summary>
        public static BeforeSelfEnumerable BeforeSelf(this GameObject origin)
        {
            return new BeforeSelfEnumerable(origin, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects before this GameObject.</summary>
        public static BeforeSelfEnumerable BeforeSelfAndSelf(this GameObject origin)
        {
            return new BeforeSelfEnumerable(origin, true);
        }

        /// <summary>Returns a collection of the sibling GameObjects after this GameObject.</summary>
        public static AfterSelfEnumerable AfterSelf(this GameObject origin)
        {
            return new AfterSelfEnumerable(origin, false);
        }

        /// <summary>Returns a collection of GameObjects that contain this GameObject, and the sibling GameObjects after this GameObject.</summary>
        public static AfterSelfEnumerable AfterSelfAndSelf(this GameObject origin)
        {
            return new AfterSelfEnumerable(origin, true);
        }

        // Implements hand struct enumerator.

        public struct ChildrenEnumerable : IEnumerable<GameObject>
        {
            private readonly GameObject _origin;
            private readonly bool _withSelf;

            public ChildrenEnumerable(GameObject origin, bool withSelf)
            {
                _origin = origin;
                _withSelf = withSelf;
            }

            /// <summary>Returns a collection of specified component in the source collection.</summary>
            public OfComponentEnumerable<T> OfComponent<T>()
                where T : Component
            {
                return new OfComponentEnumerable<T>(ref this);
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            /// <param name="detachParent">set to parent = null.</param>
            public void Destroy(bool useDestroyImmediate = false, bool detachParent = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Destroy(useDestroyImmediate, false);
                }
                if (detachParent)
                {
                    _origin.transform.DetachChildren();
                    if (_withSelf)
                    {
#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)
                        _origin.transform.SetParent(null);
#else
                        _origin.transform.parent = null;
#endif
                    }
                }
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (predicate(item))
                    {
                        item.Destroy(useDestroyImmediate, false);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                // check GameObject is destroyed only on GetEnumerator timing
                return (_origin == null)
                    ? new Enumerator(null, _withSelf, false)
                    : new Enumerator(_origin.transform, _withSelf, true);
            }

            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            private int GetChildrenSize()
            {
                return _origin.transform.childCount + (_withSelf ? 1 : 0);
            }

            public void ForEach(Action<GameObject> action)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    action(e.Current);
                }
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(ref GameObject[] array)
            {
                var index = 0;

                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? GetChildrenSize() : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? GetChildrenSize() : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? GetChildrenSize() : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? GetChildrenSize() : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    var state = let(item);

                    if (!filter(state)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? GetChildrenSize() : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(state);
                }

                return index;
            }

            public GameObject[] ToArray()
            {
                var array = new GameObject[GetChildrenSize()];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject[] ToArray(Func<GameObject, bool> filter)
            {
                var array = new GameObject[GetChildrenSize()];
                var len = ToArrayNonAlloc(filter, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, T> selector)
            {
                var array = new T[GetChildrenSize()];
                var len = ToArrayNonAlloc<T>(selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
            {
                var array = new T[GetChildrenSize()];
                var len = ToArrayNonAlloc(filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
            {
                var array = new T[GetChildrenSize()];
                var len = ToArrayNonAlloc(let, filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject First()
            {
                var e = GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }
                else
                {
                    throw new InvalidOperationException("sequence is empty.");
                }
            }

            public GameObject FirstOrDefault()
            {
                var e = GetEnumerator();
                return (e.MoveNext())
                    ? e.Current
                    : null;
            }

            #endregion

            public struct Enumerator : IEnumerator<GameObject>
            {
                private readonly int _childCount; // childCount is fixed when GetEnumerator is called.

                private readonly Transform _originTransform;
                private readonly bool _canRun;

                private bool _withSelf;
                private int _currentIndex;
                private GameObject _current;

                internal Enumerator(Transform originTransform, bool withSelf, bool canRun)
                {
                    _originTransform = originTransform;
                    _withSelf = withSelf;
                    _childCount = canRun ? originTransform.childCount : 0;
                    _currentIndex = -1;
                    _canRun = canRun;
                    _current = null;
                }

                public bool MoveNext()
                {
                    if (!_canRun) return false;

                    if (_withSelf)
                    {
                        _current = _originTransform.gameObject;
                        _withSelf = false;
                        return true;
                    }

                    _currentIndex++;
                    if (_currentIndex < _childCount)
                    {
                        var child = _originTransform.GetChild(_currentIndex);
                        _current = child.gameObject;
                        return true;
                    }

                    return false;
                }

                public GameObject Current { get { return _current; } }
                object IEnumerator.Current { get { return _current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }

            public struct OfComponentEnumerable<T> : IEnumerable<T>
                where T : Component
            {
                private ChildrenEnumerable _parent;

                public OfComponentEnumerable(ref ChildrenEnumerable parent)
                {
                    _parent = parent;
                }

                public OfComponentEnumerator<T> GetEnumerator()
                {
                    return new OfComponentEnumerator<T>(ref _parent);
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #region LINQ

                public void ForEach(Action<T> action)
                {
                    var e = GetEnumerator();
                    while (e.MoveNext())
                    {
                        action(e.Current);
                    }
                }

                public T First()
                {
                    var e = GetEnumerator();
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                    else
                    {
                        throw new InvalidOperationException("sequence is empty.");
                    }
                }

                public T FirstOrDefault()
                {
                    var e = GetEnumerator();
                    return (e.MoveNext())
                        ? e.Current
                        : null;
                }

                public T[] ToArray()
                {
                    var array = new T[_parent.GetChildrenSize()];
                    var len = ToArrayNonAlloc(ref array);
                    if (array.Length != len)
                    {
                        Array.Resize(ref array, len);
                    }
                    return array;
                }

                /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
                public int ToArrayNonAlloc(ref T[] array)
                {
                    var index = 0;
                    var e = GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (array.Length == index)
                        {
                            var newSize = (index == 0) ? _parent.GetChildrenSize() : index * 2;
                            Array.Resize(ref array, newSize);
                        }
                        array[index++] = e.Current;
                    }

                    return index;
                }

                #endregion
            }

            public struct OfComponentEnumerator<T> : IEnumerator<T>
                where T : Component
            {
                private Enumerator _enumerator; // enumerator is mutable
                private T _current;

#if UNITY_EDITOR
                private static List<T> _componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                public OfComponentEnumerator(ref ChildrenEnumerable parent)
                {
                    _enumerator = parent.GetEnumerator();
                    _current = default(T);
                }

                public bool MoveNext()
                {
                    while (_enumerator.MoveNext())
                    {
#if UNITY_EDITOR
                        _enumerator.Current.GetComponents<T>(_componentCache);
                        if (_componentCache.Count != 0)
                        {
                            _current = _componentCache[0];
                            _componentCache.Clear();
                            return true;
                        }
#else
                        
                        var component = _enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            _current = component;
                            return true;
                        }
#endif
                    }

                    return false;
                }

                public T Current { get { return _current; } }
                object IEnumerator.Current { get { return _current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }
        }

        public struct AncestorsEnumerable : IEnumerable<GameObject>
        {
            private readonly GameObject _origin;
            private readonly bool _withSelf;

            public AncestorsEnumerable(GameObject origin, bool withSelf)
            {
                _origin = origin;
                _withSelf = withSelf;
            }

            /// <summary>Returns a collection of specified component in the source collection.</summary>
            public OfComponentEnumerable<T> OfComponent<T>()
                where T : Component
            {
                return new OfComponentEnumerable<T>(ref this);
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Destroy(useDestroyImmediate, false);
                }
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (predicate(item))
                    {
                        item.Destroy(useDestroyImmediate, false);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                // check GameObject is destroyed only on GetEnumerator timing
                return (_origin == null)
                    ? new Enumerator(null, null, _withSelf, false)
                    : new Enumerator(_origin, _origin.transform, _withSelf, true);
            }

            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            public void ForEach(Action<GameObject> action)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    action(e.Current);
                }
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(ref GameObject[] array)
            {
                var index = 0;

                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    var state = let(item);

                    if (!filter(state)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(state);
                }

                return index;
            }

            public GameObject[] ToArray()
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject[] ToArray(Func<GameObject, bool> filter)
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(filter, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc<T>(selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(let, filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject First()
            {
                var e = GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }
                else
                {
                    throw new InvalidOperationException("sequence is empty.");
                }
            }

            public GameObject FirstOrDefault()
            {
                var e = GetEnumerator();
                return (e.MoveNext())
                    ? e.Current
                    : null;
            }

            #endregion

            public struct Enumerator : IEnumerator<GameObject>
            {
                private readonly bool _canRun;

                private GameObject _current;
                private Transform _currentTransform;
                private bool _withSelf;

                internal Enumerator(GameObject origin, Transform originTransform, bool withSelf, bool canRun)
                {
                    _current = origin;
                    _currentTransform = originTransform;
                    _withSelf = withSelf;
                    _canRun = canRun;
                }

                public bool MoveNext()
                {
                    if (!_canRun) return false;

                    if (_withSelf)
                    {
                        // withSelf, use origin and originTransform
                        _withSelf = false;
                        return true;
                    }

                    var parentTransform = _currentTransform.parent;
                    if (parentTransform != null)
                    {
                        _current = parentTransform.gameObject;
                        _currentTransform = parentTransform;
                        return true;
                    }

                    return false;
                }

                public GameObject Current { get { return _current; } }
                object IEnumerator.Current { get { return _current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }

            public struct OfComponentEnumerable<T> : IEnumerable<T>
                where T : Component
            {
                private AncestorsEnumerable _parent;

                public OfComponentEnumerable(ref AncestorsEnumerable parent)
                {
                    _parent = parent;
                }

                public OfComponentEnumerator<T> GetEnumerator()
                {
                    return new OfComponentEnumerator<T>(ref _parent);
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #region LINQ

                public void ForEach(Action<T> action)
                {
                    var e = GetEnumerator();
                    while (e.MoveNext())
                    {
                        action(e.Current);
                    }
                }

                public T First()
                {
                    var e = GetEnumerator();
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                    else
                    {
                        throw new InvalidOperationException("sequence is empty.");
                    }
                }

                public T FirstOrDefault()
                {
                    var e = GetEnumerator();
                    return (e.MoveNext())
                        ? e.Current
                        : null;
                }

                public T[] ToArray()
                {
                    var array = new T[4];
                    var len = ToArrayNonAlloc(ref array);
                    if (array.Length != len)
                    {
                        Array.Resize(ref array, len);
                    }
                    return array;
                }

                /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
                public int ToArrayNonAlloc(ref T[] array)
                {
                    var index = 0;
                    var e = GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (array.Length == index)
                        {
                            var newSize = (index == 0) ? 4 : index * 2;
                            Array.Resize(ref array, newSize);
                        }
                        array[index++] = e.Current;
                    }

                    return index;
                }

                #endregion
            }

            public struct OfComponentEnumerator<T> : IEnumerator<T>
                where T : Component
            {
                private Enumerator _enumerator; // enumerator is mutable
                private T _current;

#if UNITY_EDITOR
                private static List<T> _componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                public OfComponentEnumerator(ref AncestorsEnumerable parent)
                {
                    _enumerator = parent.GetEnumerator();
                    _current = default(T);
                }

                public bool MoveNext()
                {
                    while (_enumerator.MoveNext())
                    {
#if UNITY_EDITOR
                        _enumerator.Current.GetComponents<T>(_componentCache);
                        if (_componentCache.Count != 0)
                        {
                            _current = _componentCache[0];
                            _componentCache.Clear();
                            return true;
                        }
#else
                        
                        var component = _enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            _current = component;
                            return true;
                        }
#endif
                    }

                    return false;
                }

                public T Current { get { return _current; } }
                object IEnumerator.Current { get { return _current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }
        }

        public struct DescendantsEnumerable : IEnumerable<GameObject>
        {
            private static readonly Func<Transform, bool> AlwaysTrue = _ => true;

            private readonly GameObject _origin;
            private readonly bool _withSelf;
            private readonly Func<Transform, bool> _descendIntoChildren;

            public DescendantsEnumerable(GameObject origin, bool withSelf, Func<Transform, bool> descendIntoChildren)
            {
                _origin = origin;
                _withSelf = withSelf;
                _descendIntoChildren = descendIntoChildren ?? AlwaysTrue;
            }

            /// <summary>Returns a collection of specified component in the source collection.</summary>
            public OfComponentEnumerable<T> OfComponent<T>()
                where T : Component
            {
                return new OfComponentEnumerable<T>(ref this);
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Destroy(useDestroyImmediate, false);
                }
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (predicate(item))
                    {
                        item.Destroy(useDestroyImmediate, false);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                // check GameObject is destroyed only on GetEnumerator timing
                if (_origin == null)
                {
                    return new Enumerator(null, _withSelf, false, null, _descendIntoChildren);
                }

                InternalUnsafeRefStack refStack;
                if (InternalUnsafeRefStack.RefStackPool.Count != 0)
                {
                    refStack = InternalUnsafeRefStack.RefStackPool.Dequeue();
                    refStack.Reset();
                }
                else
                {
                    refStack = new InternalUnsafeRefStack(6);
                }

                return new Enumerator(_origin.transform, _withSelf, true, refStack, _descendIntoChildren);
            }

            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            private void ResizeArray<T>(ref int index, ref T[] array)
            {
                if (array.Length == index)
                {
                    var newSize = (index == 0) ? 4 : index * 2;
                    Array.Resize(ref array, newSize);
                }
            }

            private void DescendantsCore(ref Transform transform, ref Action<GameObject> action)
            {
                if (!_descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    action(child.gameObject);
                    DescendantsCore(ref child, ref action);
                }
            }

            private void DescendantsCore(ref Transform transform, ref int index, ref GameObject[] array)
            {
                if (!_descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    ResizeArray(ref index, ref array);
                    array[index++] = child.gameObject;
                    DescendantsCore(ref child, ref index, ref array);
                }
            }

            private void DescendantsCore(ref Func<GameObject, bool> filter, ref Transform transform, ref int index, ref GameObject[] array)
            {
                if (!_descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    var childGameObject = child.gameObject;
                    if (filter(childGameObject))
                    {
                        ResizeArray(ref index, ref array);
                        array[index++] = childGameObject;
                    }
                    DescendantsCore(ref filter, ref child, ref index, ref array);
                }
            }

            private void DescendantsCore<T>(ref Func<GameObject, T> selector, ref Transform transform, ref int index, ref T[] array)
            {
                if (!_descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    ResizeArray(ref index, ref array);
                    array[index++] = selector(child.gameObject);
                    DescendantsCore(ref selector, ref child, ref index, ref array);
                }
            }

            private void DescendantsCore<T>(ref Func<GameObject, bool> filter, ref Func<GameObject, T> selector, ref Transform transform, ref int index, ref T[] array)
            {
                if (!_descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    var childGameObject = child.gameObject;
                    if (filter(childGameObject))
                    {
                        ResizeArray(ref index, ref array);
                        array[index++] = selector(childGameObject);
                    }
                    DescendantsCore(ref filter, ref selector, ref child, ref index, ref array);
                }
            }

            private void DescendantsCore<TState, T>(ref Func<GameObject, TState> let, ref Func<TState, bool> filter, ref Func<TState, T> selector, ref Transform transform, ref int index, ref T[] array)
            {
                if (!_descendIntoChildren(transform)) return;

                var childCount = transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    var child = transform.GetChild(i);
                    var state = let(child.gameObject);
                    if (filter(state))
                    {
                        ResizeArray(ref index, ref array);
                        array[index++] = selector(state);
                    }
                    DescendantsCore(ref let, ref filter, ref selector, ref child, ref index, ref array);
                }
            }

            /// <summary>Use internal iterator for performance optimization.</summary>
            /// <param name="action"></param>
            public void ForEach(Action<GameObject> action)
            {
                if (_withSelf)
                {
                    action(_origin);
                }
                var originTransform = _origin.transform;
                DescendantsCore(ref originTransform, ref action);
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(ref GameObject[] array)
            {
                var index = 0;
                if (_withSelf)
                {
                    ResizeArray(ref index, ref array);
                    array[index++] = _origin;
                }

                var originTransform = _origin.transform;
                DescendantsCore(ref originTransform, ref index, ref array);

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
            {
                var index = 0;
                if (_withSelf && filter(_origin))
                {
                    ResizeArray(ref index, ref array);
                    array[index++] = _origin;
                }
                var originTransform = _origin.transform;
                DescendantsCore(ref filter, ref originTransform, ref index, ref array);

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                if (_withSelf)
                {
                    ResizeArray(ref index, ref array);
                    array[index++] = selector(_origin);
                }
                var originTransform = _origin.transform;
                DescendantsCore(ref selector, ref originTransform, ref index, ref array);

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                if (_withSelf && filter(_origin))
                {
                    ResizeArray(ref index, ref array);
                    array[index++] = selector(_origin);
                }
                var originTransform = _origin.transform;
                DescendantsCore(ref filter, ref selector, ref originTransform, ref index, ref array);

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
            {
                var index = 0;
                if (_withSelf)
                {
                    var state = let(_origin);
                    if (filter(state))
                    {
                        ResizeArray(ref index, ref array);
                        array[index++] = selector(state);
                    }
                }

                var originTransform = _origin.transform;
                DescendantsCore(ref let, ref filter, ref selector, ref originTransform, ref index, ref array);

                return index;
            }

            public GameObject[] ToArray()
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject[] ToArray(Func<GameObject, bool> filter)
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(filter, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc<T>(selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(let, filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject First()
            {
                var e = GetEnumerator();
                try
                {
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                    else
                    {
                        throw new InvalidOperationException("sequence is empty.");
                    }
                }
                finally
                {
                    e.Dispose();
                }
            }

            public GameObject FirstOrDefault()
            {
                var e = GetEnumerator();
                try
                {
                    return (e.MoveNext())
                        ? e.Current
                        : null;
                }
                finally
                {
                    e.Dispose();
                }
            }

            #endregion

            internal class InternalUnsafeRefStack
            {
                public static Queue<InternalUnsafeRefStack> RefStackPool = new Queue<InternalUnsafeRefStack>();

                public int Size = 0;
                public Enumerator[] Array; // Pop = this.array[--size];

                public InternalUnsafeRefStack(int initialStackDepth)
                {
                    Array = new Enumerator[initialStackDepth];
                }

                public void Push(ref Enumerator e)
                {
                    if (Size == Array.Length)
                    {
                        System.Array.Resize(ref Array, Array.Length * 2);
                    }
                    Array[Size++] = e;
                }

                public void Reset()
                {
                    Size = 0;
                }
            }

            public struct Enumerator : IEnumerator<GameObject>
            {
                private readonly int _childCount; // childCount is fixed when GetEnumerator is called.

                private readonly Transform _originTransform;
                private bool _canRun;

                private bool _withSelf;
                private int _currentIndex;
                private GameObject _current;
                private InternalUnsafeRefStack _sharedStack;
                private Func<Transform, bool> _descendIntoChildren;

                internal Enumerator(Transform originTransform, bool withSelf, bool canRun, InternalUnsafeRefStack sharedStack, Func<Transform, bool> descendIntoChildren)
                {
                    _originTransform = originTransform;
                    _withSelf = withSelf;
                    _childCount = canRun ? originTransform.childCount : 0;
                    _currentIndex = -1;
                    _canRun = canRun;
                    _current = null;
                    _sharedStack = sharedStack;
                    _descendIntoChildren = descendIntoChildren;
                }

                public bool MoveNext()
                {
                    if (!_canRun) return false;

                    while (_sharedStack.Size != 0)
                    {
                        if (_sharedStack.Array[_sharedStack.Size - 1].MoveNextCore(true, out _current))
                        {
                            return true;
                        }
                    }

                    if (!_withSelf && !_descendIntoChildren(_originTransform))
                    {
                        // reuse
                        _canRun = false;
                        InternalUnsafeRefStack.RefStackPool.Enqueue(_sharedStack);
                        return false;
                    }

                    if (MoveNextCore(false, out _current))
                    {
                        return true;
                    }
                    else
                    {
                        // reuse
                        _canRun = false;
                        InternalUnsafeRefStack.RefStackPool.Enqueue(_sharedStack);
                        return false;
                    }
                }

                private bool MoveNextCore(bool peek, out GameObject current)
                {
                    if (_withSelf)
                    {
                        current = _originTransform.gameObject;
                        _withSelf = false;
                        return true;
                    }

                    ++_currentIndex;
                    if (_currentIndex < _childCount)
                    {
                        var item = _originTransform.GetChild(_currentIndex);
                        if (_descendIntoChildren(item))
                        {
                            var childEnumerator = new Enumerator(item, true, true, _sharedStack, _descendIntoChildren);
                            _sharedStack.Push(ref childEnumerator);
                            return _sharedStack.Array[_sharedStack.Size - 1].MoveNextCore(true, out current);
                        }
                        else
                        {
                            current = item.gameObject;
                            return true;
                        }
                    }

                    if (peek)
                    {
                        _sharedStack.Size--; // Pop
                    }

                    current = null;
                    return false;
                }

                public GameObject Current { get { return _current; } }
                object IEnumerator.Current { get { return _current; } }

                public void Dispose()
                {
                    if (_canRun)
                    {
                        _canRun = false;
                        InternalUnsafeRefStack.RefStackPool.Enqueue(_sharedStack);
                    }
                }

                public void Reset() { throw new NotSupportedException(); }
            }

            public struct OfComponentEnumerable<T> : IEnumerable<T>
                where T : Component
            {
                private DescendantsEnumerable _parent;

                public OfComponentEnumerable(ref DescendantsEnumerable parent)
                {
                    _parent = parent;
                }

                public OfComponentEnumerator<T> GetEnumerator()
                {
                    return new OfComponentEnumerator<T>(ref _parent);
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #region LINQ

                public T First()
                {
                    var e = GetEnumerator();
                    try
                    {
                        if (e.MoveNext())
                        {
                            return e.Current;
                        }
                        else
                        {
                            throw new InvalidOperationException("sequence is empty.");
                        }
                    }
                    finally
                    {
                        e.Dispose();
                    }
                }

                public T FirstOrDefault()
                {
                    var e = GetEnumerator();
                    try
                    {
                        return (e.MoveNext())
                            ? e.Current
                            : null;
                    }
                    finally
                    {
                        e.Dispose();
                    }
                }

                /// <summary>Use internal iterator for performance optimization.</summary>
                public void ForEach(Action<T> action)
                {
                    if (_parent._withSelf)
                    {
                        T component = default(T);
#if UNITY_EDITOR
                        _parent._origin.GetComponents<T>(_componentCache);
                        if (_componentCache.Count != 0)
                        {
                            component = _componentCache[0];
                            _componentCache.Clear();
                        }
#else
                        component = _parent._origin.GetComponent<T>();
#endif

                        if (component != null)
                        {
                            action(component);
                        }
                    }

                    var originTransform = _parent._origin.transform;
                    OfComponentDescendantsCore(ref originTransform, ref action);
                }


                public T[] ToArray()
                {
                    var array = new T[4];
                    var len = ToArrayNonAlloc(ref array);
                    if (array.Length != len)
                    {
                        Array.Resize(ref array, len);
                    }
                    return array;
                }

#if UNITY_EDITOR
                private static List<T> _componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                private void OfComponentDescendantsCore(ref Transform transform, ref Action<T> action)
                {
                    if (!_parent._descendIntoChildren(transform)) return;

                    var childCount = transform.childCount;
                    for (int i = 0; i < childCount; i++)
                    {
                        var child = transform.GetChild(i);

                        T component = default(T);
#if UNITY_EDITOR
                        child.GetComponents<T>(_componentCache);
                        if (_componentCache.Count != 0)
                        {
                            component = _componentCache[0];
                            _componentCache.Clear();
                        }

                        component = child.GetComponent<T>();
#endif

                        if (component != null)
                        {
                            action(component);
                        }
                        OfComponentDescendantsCore(ref child, ref action);
                    }
                }

                private void OfComponentDescendantsCore(ref Transform transform, ref int index, ref T[] array)
                {
                    if (!_parent._descendIntoChildren(transform)) return;

                    var childCount = transform.childCount;
                    for (int i = 0; i < childCount; i++)
                    {
                        var child = transform.GetChild(i);
                        T component = default(T);
#if UNITY_EDITOR
                        child.GetComponents<T>(_componentCache);
                        if (_componentCache.Count != 0)
                        {
                            component = _componentCache[0];
                            _componentCache.Clear();
                        }
#else
                        component = child.GetComponent<T>();
#endif

                        if (component != null)
                        {
                            if (array.Length == index)
                            {
                                var newSize = (index == 0) ? 4 : index * 2;
                                Array.Resize(ref array, newSize);
                            }

                            array[index++] = component;
                        }
                        OfComponentDescendantsCore(ref child, ref index, ref array);
                    }
                }

                /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
                public int ToArrayNonAlloc(ref T[] array)
                {
                    var index = 0;
                    if (_parent._withSelf)
                    {
                        T component = default(T);
#if UNITY_EDITOR
                        _parent._origin.GetComponents<T>(_componentCache);
                        if (_componentCache.Count != 0)
                        {
                            component = _componentCache[0];
                            _componentCache.Clear();
                        }
#else
                        component = _parent._origin.GetComponent<T>();
#endif

                        if (component != null)
                        {
                            if (array.Length == index)
                            {
                                var newSize = (index == 0) ? 4 : index * 2;
                                Array.Resize(ref array, newSize);
                            }

                            array[index++] = component;
                        }
                    }

                    var originTransform = _parent._origin.transform;
                    OfComponentDescendantsCore(ref originTransform, ref index, ref array);

                    return index;
                }

                #endregion
            }

            public struct OfComponentEnumerator<T> : IEnumerator<T>
                where T : Component
            {
                private Enumerator _enumerator; // enumerator is mutable
                private T _current;

#if UNITY_EDITOR
                private static List<T> _componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                public OfComponentEnumerator(ref DescendantsEnumerable parent)
                {
                    _enumerator = parent.GetEnumerator();
                    _current = default(T);
                }

                public bool MoveNext()
                {
                    while (_enumerator.MoveNext())
                    {
#if UNITY_EDITOR
                        _enumerator.Current.GetComponents<T>(_componentCache);
                        if (_componentCache.Count != 0)
                        {
                            _current = _componentCache[0];
                            _componentCache.Clear();
                            return true;
                        }
#else
                        
                        var component = _enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            _current = component;
                            return true;
                        }
#endif
                    }

                    return false;
                }

                public T Current { get { return _current; } }
                object IEnumerator.Current { get { return _current; } }

                public void Dispose()
                {
                    _enumerator.Dispose();
                }

                public void Reset() { throw new NotSupportedException(); }
            }
        }

        public struct BeforeSelfEnumerable : IEnumerable<GameObject>
        {
            private readonly GameObject _origin;
            private readonly bool _withSelf;

            public BeforeSelfEnumerable(GameObject origin, bool withSelf)
            {
                _origin = origin;
                _withSelf = withSelf;
            }

            /// <summary>Returns a collection of specified component in the source collection.</summary>
            public OfComponentEnumerable<T> OfComponent<T>()
                where T : Component
            {
                return new OfComponentEnumerable<T>(ref this);
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Destroy(useDestroyImmediate, false);
                }
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (predicate(item))
                    {
                        item.Destroy(useDestroyImmediate, false);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                // check GameObject is destroyed only on GetEnumerator timing
                return (_origin == null)
                    ? new Enumerator(null, _withSelf, false)
                    : new Enumerator(_origin.transform, _withSelf, true);
            }

            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            public void ForEach(Action<GameObject> action)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    action(e.Current);
                }
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(ref GameObject[] array)
            {
                var index = 0;

                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    var state = let(item);

                    if (!filter(state)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(state);
                }

                return index;
            }

            public GameObject[] ToArray()
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject[] ToArray(Func<GameObject, bool> filter)
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(filter, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc<T>(selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(let, filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject First()
            {
                var e = GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }
                else
                {
                    throw new InvalidOperationException("sequence is empty.");
                }
            }

            public GameObject FirstOrDefault()
            {
                var e = GetEnumerator();
                return (e.MoveNext())
                    ? e.Current
                    : null;
            }

            #endregion

            public struct Enumerator : IEnumerator<GameObject>
            {
                private readonly int _childCount; // childCount is fixed when GetEnumerator is called.
                private readonly Transform _originTransform;
                private bool _canRun;

                private bool _withSelf;
                private int _currentIndex;
                private GameObject _current;
                private Transform _parent;

                internal Enumerator(Transform originTransform, bool withSelf, bool canRun)
                {
                    _originTransform = originTransform;
                    _withSelf = withSelf;
                    _currentIndex = -1;
                    _canRun = canRun;
                    _current = null;
                    _parent = originTransform.parent;
                    _childCount = (_parent != null) ? _parent.childCount : 0;
                }

                public bool MoveNext()
                {
                    if (!_canRun) return false;

                    if (_parent == null) goto RETURN_SELF;

                    _currentIndex++;
                    if (_currentIndex < _childCount)
                    {
                        var item = _parent.GetChild(_currentIndex);

                        if (item == _originTransform)
                        {
                            goto RETURN_SELF;
                        }

                        _current = item.gameObject;
                        return true;
                    }

                    RETURN_SELF:
                    if (_withSelf)
                    {
                        _current = _originTransform.gameObject;
                        _withSelf = false;
                        _canRun = false; // reached self, run complete.
                        return true;
                    }

                    return false;
                }

                public GameObject Current { get { return _current; } }
                object IEnumerator.Current { get { return _current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }

            public struct OfComponentEnumerable<T> : IEnumerable<T>
                where T : Component
            {
                private BeforeSelfEnumerable _parent;

                public OfComponentEnumerable(ref BeforeSelfEnumerable parent)
                {
                    _parent = parent;
                }

                public OfComponentEnumerator<T> GetEnumerator()
                {
                    return new OfComponentEnumerator<T>(ref _parent);
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #region LINQ

                public void ForEach(Action<T> action)
                {
                    var e = GetEnumerator();
                    while (e.MoveNext())
                    {
                        action(e.Current);
                    }
                }

                public T First()
                {
                    var e = GetEnumerator();
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                    else
                    {
                        throw new InvalidOperationException("sequence is empty.");
                    }
                }

                public T FirstOrDefault()
                {
                    var e = GetEnumerator();
                    return (e.MoveNext())
                        ? e.Current
                        : null;
                }

                public T[] ToArray()
                {
                    var array = new T[4];
                    var len = ToArrayNonAlloc(ref array);
                    if (array.Length != len)
                    {
                        Array.Resize(ref array, len);
                    }
                    return array;
                }

                /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
                public int ToArrayNonAlloc(ref T[] array)
                {
                    var index = 0;
                    var e = GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (array.Length == index)
                        {
                            var newSize = (index == 0) ? 4 : index * 2;
                            Array.Resize(ref array, newSize);
                        }
                        array[index++] = e.Current;
                    }

                    return index;
                }

                #endregion
            }

            public struct OfComponentEnumerator<T> : IEnumerator<T>
                where T : Component
            {
                private Enumerator _enumerator; // enumerator is mutable
                private T _current;

#if UNITY_EDITOR
                private static List<T> _componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                public OfComponentEnumerator(ref BeforeSelfEnumerable parent)
                {
                    _enumerator = parent.GetEnumerator();
                    _current = default(T);
                }

                public bool MoveNext()
                {
                    while (_enumerator.MoveNext())
                    {
#if UNITY_EDITOR
                        _enumerator.Current.GetComponents<T>(_componentCache);
                        if (_componentCache.Count != 0)
                        {
                            _current = _componentCache[0];
                            _componentCache.Clear();
                            return true;
                        }
#else
                        
                        var component = _enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            _current = component;
                            return true;
                        }
#endif
                    }

                    return false;
                }

                public T Current { get { return _current; } }
                object IEnumerator.Current { get { return _current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }
        }

        public struct AfterSelfEnumerable : IEnumerable<GameObject>
        {
            private readonly GameObject _origin;
            private readonly bool _withSelf;

            public AfterSelfEnumerable(GameObject origin, bool withSelf)
            {
                _origin = origin;
                _withSelf = withSelf;
            }

            /// <summary>Returns a collection of specified component in the source collection.</summary>
            public OfComponentEnumerable<T> OfComponent<T>()
                where T : Component
            {
                return new OfComponentEnumerable<T>(ref this);
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    e.Current.Destroy(useDestroyImmediate, false);
                }
            }

            /// <summary>Destroy every GameObject in the source collection safety(check null).</summary>
            /// <param name="useDestroyImmediate">If in EditMode, should be true or pass !Application.isPlaying.</param>
            public void Destroy(Func<GameObject, bool> predicate, bool useDestroyImmediate = false)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (predicate(item))
                    {
                        item.Destroy(useDestroyImmediate, false);
                    }
                }
            }

            public Enumerator GetEnumerator()
            {
                // check GameObject is destroyed only on GetEnumerator timing
                return (_origin == null)
                    ? new Enumerator(null, _withSelf, false)
                    : new Enumerator(_origin.transform, _withSelf, true);
            }

            IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region LINQ

            public void ForEach(Action<GameObject> action)
            {
                var e = GetEnumerator();
                while (e.MoveNext())
                {
                    action(e.Current);
                }
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(ref GameObject[] array)
            {
                var index = 0;

                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc(Func<GameObject, bool> filter, ref GameObject[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = item;
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    if (!filter(item)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(item);
                }

                return index;
            }

            /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
            public int ToArrayNonAlloc<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector, ref T[] array)
            {
                var index = 0;
                var e = GetEnumerator(); // does not need to call Dispose.
                while (e.MoveNext())
                {
                    var item = e.Current;
                    var state = let(item);

                    if (!filter(state)) continue;

                    if (array.Length == index)
                    {
                        var newSize = (index == 0) ? 4 : index * 2;
                        Array.Resize(ref array, newSize);
                    }
                    array[index++] = selector(state);
                }

                return index;
            }

            public GameObject[] ToArray()
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject[] ToArray(Func<GameObject, bool> filter)
            {
                var array = new GameObject[4];
                var len = ToArrayNonAlloc(filter, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc<T>(selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<T>(Func<GameObject, bool> filter, Func<GameObject, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public T[] ToArray<TState, T>(Func<GameObject, TState> let, Func<TState, bool> filter, Func<TState, T> selector)
            {
                var array = new T[4];
                var len = ToArrayNonAlloc(let, filter, selector, ref array);
                if (array.Length != len)
                {
                    Array.Resize(ref array, len);
                }
                return array;
            }

            public GameObject First()
            {
                var e = GetEnumerator();
                if (e.MoveNext())
                {
                    return e.Current;
                }
                else
                {
                    throw new InvalidOperationException("sequence is empty.");
                }
            }

            public GameObject FirstOrDefault()
            {
                var e = GetEnumerator();
                return (e.MoveNext())
                    ? e.Current
                    : null;
            }

            #endregion

            public struct Enumerator : IEnumerator<GameObject>
            {
                private readonly int _childCount; // childCount is fixed when GetEnumerator is called.
                private readonly Transform _originTransform;
                private readonly bool _canRun;

                private bool _withSelf;
                private int _currentIndex;
                private GameObject _current;
                private Transform _parent;

                internal Enumerator(Transform originTransform, bool withSelf, bool canRun)
                {
                    _originTransform = originTransform;
                    _withSelf = withSelf;
                    _currentIndex = (originTransform != null) ? originTransform.GetSiblingIndex() + 1 : 0;
                    _canRun = canRun;
                    _current = null;
                    _parent = originTransform.parent;
                    _childCount = (_parent != null) ? _parent.childCount : 0;
                }

                public bool MoveNext()
                {
                    if (!_canRun) return false;

                    if (_withSelf)
                    {
                        _current = _originTransform.gameObject;
                        _withSelf = false;
                        return true;
                    }

                    if (_currentIndex < _childCount)
                    {
                        _current = _parent.GetChild(_currentIndex).gameObject;
                        _currentIndex++;
                        return true;
                    }

                    return false;
                }

                public GameObject Current { get { return _current; } }
                object IEnumerator.Current { get { return _current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }

            public struct OfComponentEnumerable<T> : IEnumerable<T>
                where T : Component
            {
                private AfterSelfEnumerable _parent;

                public OfComponentEnumerable(ref AfterSelfEnumerable parent)
                {
                    _parent = parent;
                }

                public OfComponentEnumerator<T> GetEnumerator()
                {
                    return new OfComponentEnumerator<T>(ref _parent);
                }

                IEnumerator<T> IEnumerable<T>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #region LINQ

                public void ForEach(Action<T> action)
                {
                    var e = GetEnumerator();
                    while (e.MoveNext())
                    {
                        action(e.Current);
                    }
                }

                public T First()
                {
                    var e = GetEnumerator();
                    if (e.MoveNext())
                    {
                        return e.Current;
                    }
                    else
                    {
                        throw new InvalidOperationException("sequence is empty.");
                    }
                }

                public T FirstOrDefault()
                {
                    var e = GetEnumerator();
                    return (e.MoveNext())
                        ? e.Current
                        : null;
                }

                public T[] ToArray()
                {
                    var array = new T[4];
                    var len = ToArrayNonAlloc(ref array);
                    if (array.Length != len)
                    {
                        Array.Resize(ref array, len);
                    }
                    return array;
                }

                /// <summary>Store element into the buffer, return number is size. array is automaticaly expanded.</summary>
                public int ToArrayNonAlloc(ref T[] array)
                {
                    var index = 0;
                    var e = GetEnumerator();
                    while (e.MoveNext())
                    {
                        if (array.Length == index)
                        {
                            var newSize = (index == 0) ? 4 : index * 2;
                            Array.Resize(ref array, newSize);
                        }
                        array[index++] = e.Current;
                    }

                    return index;
                }

                #endregion
            }

            public struct OfComponentEnumerator<T> : IEnumerator<T>
                where T : Component
            {
                private Enumerator _enumerator; // enumerator is mutable
                private T _current;

#if UNITY_EDITOR
                private static List<T> _componentCache = new List<T>(); // for no allocate on UNITY_EDITOR
#endif

                public OfComponentEnumerator(ref AfterSelfEnumerable parent)
                {
                    _enumerator = parent.GetEnumerator();
                    _current = default(T);
                }

                public bool MoveNext()
                {
                    while (_enumerator.MoveNext())
                    {
#if UNITY_EDITOR
                        _enumerator.Current.GetComponents<T>(_componentCache);
                        if (_componentCache.Count != 0)
                        {
                            _current = _componentCache[0];
                            _componentCache.Clear();
                            return true;
                        }
#else
                        
                        var component = _enumerator.Current.GetComponent<T>();
                        if (component != null)
                        {
                            _current = component;
                            return true;
                        }
#endif
                    }

                    return false;
                }

                public T Current { get { return _current; } }
                object IEnumerator.Current { get { return _current; } }
                public void Dispose() { }
                public void Reset() { throw new NotSupportedException(); }
            }
        }
    }
}
