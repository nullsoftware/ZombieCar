using ZombieCar.Configs;
using ZombieCar.Core.Combat;
using ZombieCar.Core.Extensions;

namespace ZombieCar.Gameplay.Enemies.States
{
    /// <summary>
    /// Stands still until the car enters the detection radius.
    /// </summary>
    public sealed class EnemyIdleState : IEnemyState
    {
        private readonly EnemyConfig _config;
        private readonly IEnemyTargetProvider _targets;

        public EnemyIdleState(EnemyConfig config, IEnemyTargetProvider targets)
        {
            _config = config;
            _targets = targets;
        }

        public EnemyStateId Id => EnemyStateId.Idle;

        public void Enter(Enemy enemy) => enemy.Animation.PlayIdle();

        public EnemyStateId Tick(Enemy enemy, float deltaTime)
        {
            if (!_targets.TryGetTarget(out ITargetable target))
            {
                return Id;
            }

            float radius = _config.DetectionRadius;
            return enemy.Position.FlatSqrDistanceTo(target.Position) <= radius * radius
                ? EnemyStateId.Chase
                : Id;
        }

        public void Exit(Enemy enemy)
        {
        }
    }
}
