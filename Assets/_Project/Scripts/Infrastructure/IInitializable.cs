using System.Threading.Tasks;

namespace _Project.Scripts.Infrastructure
{
    public interface IInitializable
    {
        Task Initialize();
    }
}