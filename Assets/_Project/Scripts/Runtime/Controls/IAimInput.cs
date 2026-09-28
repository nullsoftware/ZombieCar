using UnityEngine;

namespace ZombieCar.Controls
{
    public interface IAimInput
    {
        bool IsAiming { get; }

        /// <summary>
        /// Pointer drag delta for this frame, normalized by screen width
        /// (1 = a drag across the whole screen). Zero when not aiming.
        /// </summary>
        Vector2 AimDelta { get; }
    }
}
