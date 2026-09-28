using UnityEngine;
using ZombieCar.Configs;
using ZombieCar.Effects;

namespace ZombieCar.Gameplay.Enemies.States
{
    /// <summary>
    /// Plays the death animation, keeps the corpse for a while, then asks to be returned to the pool.
    /// </summary>
    public sealed class EnemyDeadState : IEnemyState
    {
        private readonly EnemyConfig _config;
        private readonly IVfxService _vfx;

        public EnemyDeadState(EnemyConfig config, IVfxService vfx)
        {
            _config = config;
            _vfx = vfx;
        }

        public EnemyStateId Id => EnemyStateId.Dead;

        public void Enter(Enemy enemy)
        {
            enemy.SetHittable(false);
            enemy.Animation.PlayDeath();
            _vfx.Play(VfxType.EnemyDeath, enemy.Position + Vector3.up, enemy.transform.rotation);
        }

        public EnemyStateId Tick(Enemy enemy, float deltaTime)
        {
            if (enemy.Blackboard.StateTime >= _config.CorpseLifetime)
            {
                enemy.Blackboard.IsDespawnRequested = true;
            }

            return Id;
        }

        public void Exit(Enemy enemy)
        {
        }
    }
}
