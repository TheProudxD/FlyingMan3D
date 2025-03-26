using System.Collections;

namespace _Project.Scripts.Tools.Coroutine
{
    public interface ICoroutineRunner
    {
        UnityEngine.Coroutine StartCoroutine(IEnumerator routine);
    }
}