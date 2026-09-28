using System;
using System.Collections.Generic;
using UnityEngine;
using ZombieCar.Configs;
using ZombieCar.Core.Lifecycle;
using ZombieCar.Core.Pooling;
using ZombieCar.Gameplay.Enemies.States;

namespace ZombieCar.Gameplay.Enemies.Spawning
{
    /// <summary>
    /// Owns the enemy pool and the list of live enemies. On every level reset it
    /// clears the previous population and places a fresh one in the Idle state.
    /// </summary>
    public sealed class EnemySpawner : IResettable, IDisposable
    {
        private readonly EnemyConfig _config;
        private readonly IEnemySpawnLayout _layout;
        private readonly EnemyStateMachine _stateMachine;
        private readonly ComponentPool<Enemy> _pool;
        private readonly List<Enemy> _active;
        private readonly List<Pose> _spawnPoses;

        public EnemySpawner(EnemyConfig config, IEnemySpawnLayout layout, EnemyStateMachine stateMachine)
        {
            _config = config;
            _layout = layout;
            _stateMachine = stateMachine;
            _pool = new ComponentPool<Enemy>(config.Prefab, config.PrewarmCount);
            _active = new List<Enemy>(config.PrewarmCount);
            _spawnPoses = new List<Pose>(config.PrewarmCount);
        }

        public IReadOnlyList<Enemy> ActiveEnemies => _active;

        public void ResetState()
        {
            DespawnAll();
            _layout.Build(_spawnPoses);

            foreach (Pose pose in _spawnPoses)
            {
                Spawn(pose);
            }
        }

        public void Despawn(Enemy enemy)
        {
            int index = _active.IndexOf(enemy);

            if (index < 0)
            {
                return;
            }

            // Swap-remove keeps it O(1) apart from the lookup; order does not matter.
            int lastIndex = _active.Count - 1;
            _active[index] = _active[lastIndex];
            _active.RemoveAt(lastIndex);
            _pool.Release(enemy);
        }

        public void Dispose() => _pool.Dispose();

        private void Spawn(Pose pose)
        {
            Enemy enemy = _pool.Get(pose.position, pose.rotation);
            enemy.Initialize(_config.MaxHealth);
            _stateMachine.Start(enemy, EnemyStateId.Idle);
            _active.Add(enemy);
        }

        private void DespawnAll()
        {
            _pool.ReleaseAll();
            _active.Clear();
        }
    }
}
