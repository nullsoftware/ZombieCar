using UnityEngine;
using ZombieCar.Core.Combat;
using ZombieCar.Core.Extensions;
using ZombieCar.Core.Pooling;
using ZombieCar.Gameplay.Enemies.Animation;

namespace ZombieCar.Gameplay.Enemies
{
    /// <summary>
    /// Pooled enemy: health, hit collider, movement and animation.
    /// AI decisions are made by the shared states in <see cref="States.EnemyStateMachine"/>.
    /// Animation: the enemy uses the <see cref="EnemyAnimator"/> components in its hierarchy. A real animator
    /// (Mecanim with clips) wins over a fallback placeholder (procedural); the unused ones are disabled.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Enemy : MonoBehaviour, IDamageable, IPoolable
    {
        [SerializeField] private Collider _hitCollider;

        private readonly Health _health = new(1f);

        public EnemyBlackboard Blackboard { get; } = new();
        public IEnemyAnimator Animation { get; private set; } = NullEnemyAnimator.Instance;
        public IReadOnlyHealth Health => _health;
        public bool IsAlive => _health.IsAlive;
        public Vector3 Position => transform.position;

        private void Awake()
        {
            EnemyAnimator[] animators = GetComponentsInChildren<EnemyAnimator>(true);
            EnemyAnimator selected = SelectAnimator(animators);

            foreach (EnemyAnimator animator in animators)
            {
                animator.enabled = animator == selected;
            }

            if (selected != null)
            {
                Animation = selected;
            }
        }

        public void Initialize(float maxHealth)
        {
            _health.Restore(maxHealth);
            Blackboard.Reset();
            SetHittable(true);
            Animation.ResetPose();
        }

        public void TakeDamage(float amount)
        {
            if (!_health.IsAlive)
            {
                return;
            }

            _health.TakeDamage(amount);

            // Death is handled by the Dead state; here we only react to a hit we survived.
            if (_health.IsAlive)
            {
                Animation.PlayHit();
            }
        }

        public void SetHittable(bool isHittable)
        {
            if (_hitCollider != null)
            {
                _hitCollider.enabled = isHittable;
            }
        }

        public void MoveTowards(Vector3 point, float speed, float turnSpeed, float deltaTime)
        {
            Vector3 position = transform.position;
            Vector3 offset = (point - position).Flat();
            float distance = offset.magnitude;

            if (distance < 0.001f)
            {
                return;
            }

            Vector3 direction = offset / distance;
            RotateTowards(direction, turnSpeed, deltaTime);
            transform.position = position + direction * Mathf.Min(speed * deltaTime, distance);
        }

        public void LookAt(Vector3 point, float turnSpeed, float deltaTime)
        {
            Vector3 direction = (point - transform.position).Flat();

            if (direction.sqrMagnitude > 0.0001f)
            {
                RotateTowards(direction, turnSpeed, deltaTime);
            }
        }

        public void OnSpawned()
        {
        }

        public void OnDespawned()
        {
            Blackboard.Reset();
        }

        private void RotateTowards(Vector3 direction, float turnSpeed, float deltaTime)
        {
            Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * deltaTime);
        }

        private static EnemyAnimator SelectAnimator(EnemyAnimator[] animators)
        {
            EnemyAnimator fallback = null;

            foreach (EnemyAnimator animator in animators)
            {
                if (!animator.IsAvailable)
                {
                    continue;
                }

                if (!animator.IsFallback)
                {
                    return animator;
                }

                if (fallback == null)
                {
                    fallback = animator;
                }
            }

            return fallback;
        }

        private void Reset() => _hitCollider = GetComponent<Collider>();
    }
}
