using UnityEngine;

namespace ZombieCar.Gameplay.Weapons
{
    /// <summary>
    /// Visual rig of the turret: a yaw pivot and the muzzle projectiles leave from.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class TurretView : MonoBehaviour
    {
        [SerializeField] private Transform _yawPivot;
        [SerializeField] private Transform _muzzle;

        public Transform Muzzle => _muzzle;

        public void SetYaw(float angle) => _yawPivot.localRotation = Quaternion.Euler(0f, angle, 0f);
    }
}
