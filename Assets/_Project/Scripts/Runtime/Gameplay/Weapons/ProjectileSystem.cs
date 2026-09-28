using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using ZombieCar.Configs;
using ZombieCar.Core.Combat;
using ZombieCar.Core.Lifecycle;
using ZombieCar.Core.Pooling;
using ZombieCar.Effects;

namespace ZombieCar.Gameplay.Weapons
{
    /// <summary>
    /// Spawns projectiles from a pool, moves them and resolves hits with sphere casts,
    /// so fast bullets cannot pass through thin targets.
    /// </summary>
    public sealed class ProjectileSystem : IProjectileLauncher, ITickable, IResettable, IDisposable
    {
        private readonly ProjectileConfig _config;
        private readonly IVfxService _vfx;
        private readonly ComponentPool<Projectile> _pool;
        private readonly List<Projectile> _active;

        public ProjectileSystem(ProjectileConfig config, IVfxService vfx)
        {
            _config = config;
            _vfx = vfx;
            _pool = new ComponentPool<Projectile>(config.Prefab, config.PrewarmCount);
            _active = new List<Projectile>(Math.Max(config.PrewarmCount, 16));
        }

        public void Launch(Vector3 origin, Vector3 direction)
        {
            Projectile projectile = _pool.Get(origin, Quaternion.LookRotation(direction));
            projectile.Launch(direction * _config.Speed, _config.Lifetime);
            _active.Add(projectile);
        }

        public void Tick()
        {
            float deltaTime = Time.deltaTime;

            if (deltaTime <= 0f)
            {
                return;
            }

            // Iterate backwards: despawning swaps the last element into the current slot.
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                Projectile projectile = _active[i];
                Vector3 step = projectile.Velocity * deltaTime;
                float distance = step.magnitude;
                Vector3 direction = step / distance;

                if (Physics.SphereCast(projectile.transform.position, _config.Radius, direction, out RaycastHit hit,
                        distance, _config.HitMask, QueryTriggerInteraction.Ignore))
                {
                    HandleHit(hit, direction);
                    DespawnAt(i);
                    continue;
                }

                projectile.Advance(step, deltaTime);

                if (projectile.TimeLeft <= 0f)
                {
                    DespawnAt(i);
                }
            }
        }

        public void ResetState()
        {
            _pool.ReleaseAll();
            _active.Clear();
        }

        public void Dispose() => _pool.Dispose();

        private void HandleHit(RaycastHit hit, Vector3 direction)
        {
            Vector3 normal = hit.normal.sqrMagnitude > 0f ? hit.normal : -direction;
            Quaternion rotation = Quaternion.LookRotation(normal);
            IDamageable damageable = hit.collider.GetComponentInParent<IDamageable>();

            if (damageable is { IsAlive: true })
            {
                damageable.TakeDamage(_config.Damage);
                _vfx.Play(VfxType.EnemyHit, hit.point, rotation);
            }
            else
            {
                _vfx.Play(VfxType.ProjectileImpact, hit.point, rotation);
            }
        }

        private void DespawnAt(int index)
        {
            Projectile projectile = _active[index];
            int lastIndex = _active.Count - 1;
            _active[index] = _active[lastIndex];
            _active.RemoveAt(lastIndex);
            _pool.Release(projectile);
        }
    }
}
