namespace ZombieCar.Core.Lifecycle
{
    /// <summary>
    /// Implemented by systems that must return to their initial state when the level restarts.
    /// </summary>
    public interface IResettable
    {
        void ResetState();
    }
}
