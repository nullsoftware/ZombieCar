using System;
using System.Collections.Generic;

namespace ZombieCar.Gameplay.Enemies.States
{
    /// <summary>
    /// Runs the shared enemy states against each enemy's blackboard.
    /// </summary>
    public sealed class EnemyStateMachine
    {
        private readonly IEnemyState[] _states;

        public EnemyStateMachine(IReadOnlyList<IEnemyState> states)
        {
            _states = new IEnemyState[Enum.GetValues(typeof(EnemyStateId)).Length];

            foreach (IEnemyState state in states)
            {
                _states[(int)state.Id] = state;
            }

            for (int i = 0; i < _states.Length; i++)
            {
                if (_states[i] == null)
                {
                    throw new InvalidOperationException($"No enemy state registered for {(EnemyStateId)i}.");
                }
            }
        }

        public void Start(Enemy enemy, EnemyStateId initialState)
        {
            EnemyBlackboard blackboard = enemy.Blackboard;
            blackboard.State = initialState;
            blackboard.StateTime = 0f;
            _states[(int)initialState].Enter(enemy);
        }

        public void Tick(Enemy enemy, float deltaTime)
        {
            EnemyBlackboard blackboard = enemy.Blackboard;
            blackboard.StateTime += deltaTime;

            // "Any state" transition: a killed enemy goes to Dead no matter what it was doing.
            EnemyStateId next = !enemy.IsAlive && blackboard.State != EnemyStateId.Dead
                ? EnemyStateId.Dead
                : _states[(int)blackboard.State].Tick(enemy, deltaTime);

            if (next != blackboard.State)
            {
                ChangeState(enemy, next);
            }
        }

        private void ChangeState(Enemy enemy, EnemyStateId next)
        {
            EnemyBlackboard blackboard = enemy.Blackboard;
            _states[(int)blackboard.State].Exit(enemy);
            blackboard.State = next;
            blackboard.StateTime = 0f;
            _states[(int)next].Enter(enemy);
        }
    }
}
