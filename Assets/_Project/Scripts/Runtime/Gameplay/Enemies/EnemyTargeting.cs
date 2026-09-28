using ZombieCar.Core.Combat;
using ZombieCar.Core.Lifecycle;

namespace ZombieCar.Gameplay.Enemies
{
    /// <summary>
    /// Exposes the player's car to the enemies only while gameplay runs,
    /// so enemies go back to idle after Win/Lose.
    /// </summary>
    public sealed class EnemyTargeting : IEnemyTargetProvider, IGameplaySystem
    {
        private readonly ITargetable _target;
        private bool _isHuntingAllowed;

        public EnemyTargeting(ITargetable target)
        {
            _target = target;
        }

        public void StartGameplay() => _isHuntingAllowed = true;

        public void StopGameplay() => _isHuntingAllowed = false;

        public bool TryGetTarget(out ITargetable target)
        {
            target = _target;
            return _isHuntingAllowed && _target.IsAlive;
        }
    }
}
