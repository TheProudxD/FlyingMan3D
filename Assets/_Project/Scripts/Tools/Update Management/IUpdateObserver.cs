namespace _Project.Scripts.Tools.Update_Management
{
    public interface IUpdateObserver
    {
        void OnUpdate(float deltaTime);
        void OnFixedUpdate(float fixedDeltaTime);
        void OnLateUpdate(float deltaTime);
    }
}