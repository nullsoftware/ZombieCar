using UnityEngine;

namespace ZombieCar.Configs
{
    [CreateAssetMenu(menuName = "Zombie Car/Car Config", fileName = "CarConfig")]
    public sealed class CarConfig : ScriptableObject
    {
        [Header("Health")]
        [SerializeField, Min(1f)] private float _maxHealth = 100f;

        [Header("Movement")]
        [SerializeField, Min(0f)] private float _maxSpeed = 7f;
        [SerializeField, Min(0.01f)] private float _acceleration = 4f;
        [SerializeField, Min(0.01f)] private float _braking = 12f;

        [Header("Feedback")]
        [SerializeField, Min(0f)] private float _hitShakeForce = 0.35f;
        [SerializeField, Min(0f)] private float _destroyedShakeForce = 1.5f;

        public float MaxHealth => _maxHealth;
        public float MaxSpeed => _maxSpeed;
        public float Acceleration => _acceleration;
        public float Braking => _braking;
        public float HitShakeForce => _hitShakeForce;
        public float DestroyedShakeForce => _destroyedShakeForce;
    }
}
