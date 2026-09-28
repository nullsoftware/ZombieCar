using UnityEngine;
using ZombieCar.Configs;
using ZombieCar.Core.Combat;
using ZombieCar.Core.Extensions;
using ZombieCar.Effects;

namespace ZombieCar.Gameplay.Enemies.States
{
    /// <summary>
    /// Swings at the car on a cooldown. Damage lands partway into the swing
    /// (in sync with the attack animation), and only if the car is still in reach.
    /// </summary>
    public sealed class EnemyAttackState : IEnemyState
    {
        // Leaving range needs a bit more distance than entering it, so the enemy does not flicker between states.
        private const float LeaveRangeMultiplier = 1.5f;

        private readonly EnemyConfig _config;
        private readonly IEnemyTargetProvider _targets;
        private readonly IVfxService _vfx;

        public EnemyAttackState(EnemyConfig config, IEnemyTargetProvider targets, IVfxService vfx)
        {
            _config = config;
            _targets = targets;
            _vfx = vfx;
        }

        public EnemyStateId Id => EnemyStateId.Attack;

        public void Enter(Enemy enemy) => BeginSwing(enemy);

        public EnemyStateId Tick(Enemy enemy, float deltaTime)
        {
            if (!_targets.TryGetTarget(out ITargetable target))
            {
                return EnemyStateId.Idle;
            }

            Vector3 contactPoint = target.GetClosestPoint(enemy.Position);

            if (enemy.Position.FlatDistanceTo(contactPoint) > _config.AttackRange * LeaveRangeMultiplier)
            {
                return EnemyStateId.Chase;
            }

            enemy.LookAt(contactPoint, _config.TurnSpeed, deltaTime);

            EnemyBlackboard blackboard = enemy.Blackboard;
            blackboard.AttackTimer += deltaTime;

            if (!blackboard.AttackHitDelivered && blackboard.AttackTimer >= _config.AttackHitDelay)
            {
                blackboard.AttackHitDelivered = true;
                target.TakeDamage(_config.AttackDamage);
                PlayHitEffect(enemy, contactPoint);
            }

            if (blackboard.AttackTimer >= _config.AttackCooldown)
            {
                BeginSwing(enemy);
            }

            return Id;
        }

        public void Exit(Enemy enemy)
        {
        }

        private static void BeginSwing(Enemy enemy)
        {
            enemy.Blackboard.AttackTimer = 0f;
            enemy.Blackboard.AttackHitDelivered = false;
            enemy.Animation.PlayAttack();
        }

        private void PlayHitEffect(Enemy enemy, Vector3 contactPoint)
        {
            // The effect faces away from the car surface, towards the attacker.
            Vector3 normal = (enemy.Position - contactPoint).Flat();
            Quaternion rotation = normal.sqrMagnitude > 0.0001f
                ? Quaternion.LookRotation(normal)
                : Quaternion.LookRotation(-enemy.transform.forward);

            _vfx.Play(VfxType.CarHit, contactPoint, rotation);
        }
    }
}
