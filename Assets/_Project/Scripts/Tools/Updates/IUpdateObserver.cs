namespace _Project.Scripts.Tools.Updates
{
    public interface IUpdateObserver
    {
        void OnUpdate(float deltaTime);
        void OnFixedUpdate(float fixedDeltaTime);
        void OnLateUpdate(float deltaTime);
    }
}
