using UnityEngine;

namespace ZombieCar.Core.Extensions
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Projects the vector onto the XZ ground plane.
        /// </summary>
        public static Vector3 Flat(this Vector3 vector) => new(vector.x, 0f, vector.z);

        public static float FlatDistanceTo(this Vector3 from, Vector3 to) => (to - from).Flat().magnitude;

        public static float FlatSqrDistanceTo(this Vector3 from, Vector3 to) => (to - from).Flat().sqrMagnitude;
    }
}
