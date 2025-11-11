using System.Threading.Tasks;

namespace _Project.Scripts.Infrastructure
{
    public interface IInitializable
    {
        void Initialize();
    }
    
    public interface ITaskInitializable
    {
        Task Initialize();
    }
}