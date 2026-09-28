using UnityEngine;

namespace ZombieCar.Core.Combat
{
    /// <summary>
    /// A damageable entity that AI can chase and attack.
    /// </summary>
    public interface ITargetable : IDamageable
    {
        Vector3 Position { get; }

        /// <summary>
        /// Closest point on the target's surface to <paramref name="point"/>.
        /// Used for melee range checks against large targets such as the car.
        /// </summary>
        Vector3 GetClosestPoint(Vector3 point);
    }
}
