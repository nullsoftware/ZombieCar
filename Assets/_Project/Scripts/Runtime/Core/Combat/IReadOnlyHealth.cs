using System;

namespace ZombieCar.Core.Combat
{
    /// <summary>
    /// Read-only view of a <see cref="Health"/> model for presenters and feedback systems.
    /// </summary>
    public interface IReadOnlyHealth
    {
        event Action<float> OnDamaged;
        event Action OnChanged;
        event Action OnDied;

        float Current { get; }
        float Max { get; }
        float Normalized { get; }
        bool IsAlive { get; }
    }
}
