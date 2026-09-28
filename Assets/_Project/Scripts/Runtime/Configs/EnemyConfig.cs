using UnityEngine;
using ZombieCar.Gameplay.Enemies;

namespace ZombieCar.Configs
{
    [CreateAssetMenu(menuName = "Zombie Car/Enemy Config", fileName = "EnemyConfig")]
    public sealed class EnemyConfig : ScriptableObject
    {
        [Header("Pooling")]
        [SerializeField] private Enemy _prefab;
        [SerializeField, Min(0)] private int _prewarmCount = 48;

        [Header("Health")]
        [SerializeField, Min(1f)] private float _maxHealth = 50f;

        [Header("Movement")]
        [SerializeField, Min(0f)] private float _moveSpeed = 4.5f;
        [SerializeField, Min(0f)] private float _turnSpeed = 540f;

        [Header("Perception")]
        [Tooltip("The enemy wakes up and starts chasing when the car gets this close.")]
        [SerializeField, Min(0f)] private float _detectionRadius = 20f;
        [Tooltip("The enemy gives up the chase when the car is this far away.")]
        [SerializeField, Min(0f)] private float _loseTargetDistance = 40f;

        [Header("Attack")]
        [Tooltip("Distance to the car's surface at which the enemy starts attacking.")]
        [SerializeField, Min(0f)] private float _attackRange = 1.1f;
        [SerializeField, Min(0f)] private float _attackDamage = 6f;
        [SerializeField, Min(0.05f)] private float _attackCooldown = 1f;
        [Tooltip("Delay between the start of the attack animation and the moment damage is dealt.")]
        [SerializeField, Min(0f)] private float _attackHitDelay = 0.35f;

        [Header("Death")]
        [Tooltip("How long the corpse stays (death animation) before returning to the pool.")]
        [SerializeField, Min(0f)] private float _corpseLifetime = 2.5f;

        public Enemy Prefab => _prefab;
        public int PrewarmCount => _prewarmCount;
        public float MaxHealth => _maxHealth;
        public float MoveSpeed => _moveSpeed;
        public float TurnSpeed => _turnSpeed;
        public float DetectionRadius => _detectionRadius;
        public float LoseTargetDistance => _loseTargetDistance;
        public float AttackRange => _attackRange;
        public float AttackDamage => _attackDamage;
        public float AttackCooldown => _attackCooldown;
        public float AttackHitDelay => _attackHitDelay;
        public float CorpseLifetime => _corpseLifetime;

        private void OnValidate()
        {
            _attackHitDelay = Mathf.Min(_attackHitDelay, _attackCooldown);
            _loseTargetDistance = Mathf.Max(_loseTargetDistance, _detectionRadius);
        }
    }
}
