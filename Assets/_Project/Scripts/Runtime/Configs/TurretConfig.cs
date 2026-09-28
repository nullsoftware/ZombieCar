using UnityEngine;

namespace ZombieCar.Configs
{
    [CreateAssetMenu(menuName = "Zombie Car/Turret Config", fileName = "TurretConfig")]
    public sealed class TurretConfig : ScriptableObject
    {
        [Header("Aiming")]
        [Tooltip("Degrees the turret turns for a drag across the full screen width.")]
        [SerializeField, Min(0f)] private float _aimSensitivity = 240f;
        [Tooltip("Max yaw to either side of the car's forward direction.")]
        [SerializeField, Range(0f, 180f)] private float _maxYawAngle = 80f;
        [SerializeField, Min(0f)] private float _turnSpeed = 720f;

        [Header("Shooting")]
        [Tooltip("Shots per second.")]
        [SerializeField, Min(0.1f)] private float _fireRate = 6f;

        public float AimSensitivity => _aimSensitivity;
        public float MaxYawAngle => _maxYawAngle;
        public float TurnSpeed => _turnSpeed;
        public float FireInterval => 1f / _fireRate;
    }
}
