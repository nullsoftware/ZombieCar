namespace ZombieCar.Core.Lifecycle
{
    /// <summary>
    /// Implemented by systems that only run during the gameplay phase
    /// (driving, shooting, enemy hunting). The gameplay state switches them on and off.
    /// </summary>
    public interface IGameplaySystem
    {
        void StartGameplay();

        void StopGameplay();
    }
}
