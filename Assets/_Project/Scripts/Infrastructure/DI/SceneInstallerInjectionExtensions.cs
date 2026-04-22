using System.Collections.Generic;
using _Project.Scripts.Tools;
using Reflex.Core;
using UnityEngine;

namespace _Project.Scripts.Infrastructure.DI
{
    public static class SceneInstallerInjectionExtensions
    {
        public static void InjectIfAssigned(this Container container, Object target)
        {
            if (target != null)
                container.Inject(target);
        }

        public static void InjectAll<T>(this Container container, IEnumerable<T> targets)
            where T : Object
        {
            foreach (T target in targets)
            {
                if (target != null)
                    container.Inject(target);
            }
        }
    }
}
