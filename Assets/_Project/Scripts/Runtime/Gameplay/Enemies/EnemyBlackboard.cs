using ZombieCar.Gameplay.Enemies.States;

namespace ZombieCar.Gameplay.Enemies
{
    /// <summary>
    /// Per-enemy AI data. Enemy states are shared (flyweight), so everything
    /// that belongs to one enemy is kept here.
    /// </summary>
    public sealed class EnemyBlackboard
    {
        public EnemyStateId State { get; set; }
        public float StateTime { get; set; }
        public float AttackTimer { get; set; }
        public bool AttackHitDelivered { get; set; }
        public bool IsDespawnRequested { get; set; }

        public void Reset()
        {
            State = EnemyStateId.Idle;
            StateTime = 0f;
            AttackTimer = 0f;
            AttackHitDelivered = false;
            IsDespawnRequested = false;
        }
    }
}
