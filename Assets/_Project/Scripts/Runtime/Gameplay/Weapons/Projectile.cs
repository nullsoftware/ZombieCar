using UnityEngine;
using ZombieCar.Core.Pooling;

namespace ZombieCar.Gameplay.Weapons
{
    /// <summary>
    /// Lightweight pooled bullet. <see cref="ProjectileSystem"/> moves all projectiles
    /// and checks their hits in one loop, so there is no per-bullet Update or Rigidbody.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Projectile : MonoBehaviour, IPoolable
    {
        [SerializeField] private TrailRenderer _trail;

        public Vector3 Velocity { get; private set; }
        public float TimeLeft { get; private set; }

        public void Launch(Vector3 velocity, float lifetime)
        {
            Velocity = velocity;
            TimeLeft = lifetime;
        }

        public void Advance(Vector3 step, float deltaTime)
        {
            transform.position += step;
            TimeLeft -= deltaTime;
        }

        public void OnSpawned()
        {
            if (_trail != null)
            {
                _trail.Clear();
            }
        }

        public void OnDespawned()
        {
            Velocity = Vector3.zero;
            TimeLeft = 0f;
        }
    }
}
