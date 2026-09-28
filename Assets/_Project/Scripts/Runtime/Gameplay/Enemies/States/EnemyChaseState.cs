using UnityEngine;
using ZombieCar.Configs;
using ZombieCar.Core.Combat;
using ZombieCar.Core.Extensions;

namespace ZombieCar.Gameplay.Enemies.States
{
    /// <summary>
    /// Runs to the closest point of the car's body until it is in attack range.
    /// </summary>
    public sealed class EnemyChaseState : IEnemyState
    {
        private readonly EnemyConfig _config;
        private readonly IEnemyTargetProvider _targets;

        public EnemyChaseState(EnemyConfig config, IEnemyTargetProvider targets)
        {
            _config = config;
            _targets = targets;
        }

        public EnemyStateId Id => EnemyStateId.Chase;

        public void Enter(Enemy enemy) => enemy.Animation.PlayRun();

        public EnemyStateId Tick(Enemy enemy, float deltaTime)
        {
            if (!_targets.TryGetTarget(out ITargetable target))
            {
                return EnemyStateId.Idle;
            }

            Vector3 contactPoint = target.GetClosestPoint(enemy.Position);
            float distance = enemy.Position.FlatDistanceTo(contactPoint);

            if (distance <= _config.AttackRange)
            {
                return EnemyStateId.Attack;
            }

            if (distance > _config.LoseTargetDistance)
            {
                return EnemyStateId.Idle;
            }

            enemy.MoveTowards(contactPoint, _config.MoveSpeed, _config.TurnSpeed, deltaTime);
            return Id;
        }

        public void Exit(Enemy enemy)
        {
        }
    }
}
