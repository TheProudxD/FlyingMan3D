using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace _Project.Scripts.Infrastructure
{
    public interface IInitializable
    {
        void Initialize();
    }
    
    public interface ITaskInitializable
    {
        UniTask Initialize();
    }
}