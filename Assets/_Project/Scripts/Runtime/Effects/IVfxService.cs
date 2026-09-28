using UnityEngine;

namespace ZombieCar.Effects
{
    public interface IVfxService
    {
        /// <summary>
        /// Plays a pooled one-shot effect. Does nothing if no prefab is set for the type.
        /// </summary>
        void Play(VfxType type, Vector3 position, Quaternion rotation);
    }
}
