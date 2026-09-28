using ZombieCar.Core.Combat;

namespace ZombieCar.Gameplay.Enemies
{
    public interface IEnemyTargetProvider
    {
        /// <summary>
        /// Returns <c>false</c> when there is nothing to hunt (gameplay stopped or target dead).
        /// </summary>
        bool TryGetTarget(out ITargetable target);
    }
}
