namespace ZombieCar.Gameplay.Enemies.States
{
    /// <summary>
    /// One state object serves every enemy (flyweight). All per-enemy data lives in
    /// <see cref="EnemyBlackboard"/>, so states keep no mutable fields.
    /// </summary>
    public interface IEnemyState
    {
        EnemyStateId Id { get; }

        void Enter(Enemy enemy);

        /// <summary>
        /// Updates the enemy and returns the state to be in next (return <see cref="Id"/> to stay).
        /// </summary>
        EnemyStateId Tick(Enemy enemy, float deltaTime);

        void Exit(Enemy enemy);
    }
}
