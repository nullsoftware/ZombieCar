namespace ZombieCar.Core.Pooling
{
    /// <summary>
    /// Optional callbacks for pooled components. Use them to reset per-use state
    /// (trails, particles, animation) instead of relying on OnEnable/OnDisable.
    /// </summary>
    public interface IPoolable
    {
        void OnSpawned();

        void OnDespawned();
    }
}
