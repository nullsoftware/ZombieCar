using UnityEngine;
using ZombieCar.Gameplay.Weapons;

namespace ZombieCar.Configs
{
    [CreateAssetMenu(menuName = "Zombie Car/Projectile Config", fileName = "ProjectileConfig")]
    public sealed class ProjectileConfig : ScriptableObject
    {
        [Header("Pooling")]
        [SerializeField] private Projectile _prefab;
        [SerializeField, Min(0)] private int _prewarmCount = 24;

        [Header("Ballistics")]
        [SerializeField, Min(0.1f)] private float _speed = 45f;
        [SerializeField, Min(0.05f)] private float _lifetime = 1.2f;
        [Tooltip("Radius of the sphere cast used for hit detection.")]
        [SerializeField, Min(0f)] private float _radius = 0.2f;
        [SerializeField] private LayerMask _hitMask = ~0;

        [Header("Damage")]
        [SerializeField, Min(0f)] private float _damage = 25f;

        public Projectile Prefab => _prefab;
        public int PrewarmCount => _prewarmCount;
        public float Speed => _speed;
        public float Lifetime => _lifetime;
        public float Radius => _radius;
        public LayerMask HitMask => _hitMask;
        public float Damage => _damage;
    }
}
