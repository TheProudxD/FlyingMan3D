using System.Linq;
using UnityEngine;

namespace _Project.Scripts.Tools.Extensions
{
    public static class GameObjectExtensions
    {
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        public static void DestroyChildren(this GameObject gameObject) => gameObject.transform.DestroyChildren();

        public static void HideInHierarchy(this GameObject gameObject) =>
            gameObject.hideFlags = HideFlags.HideInHierarchy;

        public static void DestroyChildrenImmediate(this GameObject gameObject)
        {
            gameObject.transform.DestroyChildrenImmediate();
        }

        public static void EnableChildren(this GameObject gameObject)
        {
            gameObject.transform.EnableChildren();
        }

        public static void DisableChildren(this GameObject gameObject)
        {
            gameObject.transform.DisableChildren();
        }

        public static void ResetTransformation(this GameObject gameObject)
        {
            gameObject.transform.Reset();
        }

        public static string Path(this GameObject gameObject) =>
            "/" + string.Join("/",
                gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray());
        
        public static string PathFull(this GameObject gameObject) => gameObject.Path() + "/" + gameObject.name;
        
        public static void SetLayersRecursively(this GameObject gameObject, int layer) {
            gameObject.layer = layer;
            gameObject.transform.ForEveryChild(child => child.gameObject.SetLayersRecursively(layer));
        }

        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();

            return component;
        }

        public static void AskLayer(this GameObject obj, int lvl)
        {
            obj.gameObject.layer = lvl;
            if (obj.transform.childCount <= 0) return;

            foreach (Transform d in obj.transform)
            {
                AskLayer(d.gameObject, lvl);
            }
        }

        public static T GetSafeComponent<T>(this GameObject obj) where T : MonoBehaviour
        {
            T component = obj.GetComponent<T>();

            if (component == null)
            {
                Debug.LogError("Expected to find component of type "
                               + typeof(T) + " but found none", obj);
            }

            return component;
        }


        // public static void RendererSetActive(Transform renderer)
        // {
        //     var component = renderer.gameObject.GetComponent<Renderer>();
        //     if (component)
        //         component.enabled = _isVisible;
        // }

        public static void AskColor(this GameObject obj, Color color)
        {
            if (obj.TryGetComponent(out Renderer renderer))
            {
                foreach (var curMaterial in renderer.materials)
                {
                    curMaterial.color = color;
                }
            }

            if (obj.transform.childCount <= 0) return;

            foreach (Transform d in obj.transform)
            {
                AskColor(d.gameObject, color);
            }
        }

        public static void DisableRigidBody(this GameObject obj)
        {
            var rigidbodies = obj.GetComponentsInChildren<Rigidbody>();

            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = true;
            }
        }

        public static void EnableRigidBody(this GameObject obj, float force)
        {
            EnableRigidBody(obj);
            obj.GetComponent<Rigidbody>().AddForce(obj.transform.forward * force);
        }

        public static void EnableRigidBody(this GameObject obj)
        {
            var rigidbodies = obj.GetComponentsInChildren<Rigidbody>();

            foreach (var rb in rigidbodies)
            {
                rb.isKinematic = false;
            }
        }

        public static void ConstraintsRigidBody(this GameObject self, RigidbodyConstraints rigidbodyConstraints)
        {
            var rigidbodies = self.GetComponentsInChildren<Rigidbody>();

            foreach (var rb in rigidbodies)
            {
                rb.constraints = rigidbodyConstraints;
            }
        }

        public static bool TryGetComponent<T>(this GameObject obj, out T component)
        {
            component = default;

            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                var newComponent = obj.GetComponent<T>();

                if (newComponent as Component == null) return false;

                component = newComponent;
            }
            else
            {
                throw new System.Exception("Searching type os not a component");
            }

            return true;
        }

        public static bool TryGetComponentInChildren<T>(this GameObject obj, out T component)
        {
            component = default;

            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                T newComponent = obj.GetComponentInChildren<T>();

                if (newComponent as Component == null) return false;

                component = newComponent;
            }
            else
            {
                throw new System.Exception("Searching type os not a component");
            }

            return true;
        }

        public static bool HasComponent<T>(this GameObject obj)
        {
            bool hasComponent = false;

            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                T component = obj.GetComponent<T>();
                hasComponent = component is not null;
            }
            else
            {
                throw new System.Exception("Searching type os not a component");
            }

            return hasComponent;
        }

        public static bool HasComponentInChildren<T>(this GameObject obj)
        {
            bool hasComponent = false;

            if (typeof(T).IsSubclassOf(typeof(Component)))
            {
                T component = obj.GetComponentInChildren<T>();
                hasComponent = component is not null;
            }
            else
            {
                throw new System.Exception("Searching type os not a component");
            }

            return hasComponent;
        }
    }
}