using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using ZombieCar.Gameplay.Enemies.Spawning;
using ZombieCar.Gameplay.Enemies.States;

namespace ZombieCar.Gameplay.Enemies
{
    /// <summary>
    /// Ticks every live enemy from one loop (no per-enemy Update) and returns
    /// finished corpses to the pool.
    /// </summary>
    public sealed class EnemyAiSystem : ITickable
    {
        private readonly EnemySpawner _spawner;
        private readonly EnemyStateMachine _stateMachine;

        public EnemyAiSystem(EnemySpawner spawner, EnemyStateMachine stateMachine)
        {
            _spawner = spawner;
            _stateMachine = stateMachine;
        }

        public void Tick()
        {
            float deltaTime = Time.deltaTime;
            IReadOnlyList<Enemy> enemies = _spawner.ActiveEnemies;

            // Iterate backwards: despawning swaps the last enemy into the current slot.
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = enemies[i];
                _stateMachine.Tick(enemy, deltaTime);

                if (enemy.Blackboard.IsDespawnRequested)
                {
                    _spawner.Despawn(enemy);
                }
            }
        }
    }
}
