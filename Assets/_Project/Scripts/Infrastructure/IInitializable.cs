using System.Collections;

namespace _Project.Scripts.Infrastructure
{
    public interface IInitializable
    {
        IEnumerator Initialize();
    }
}