using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ZombieCar.GameFlow
{
    /// <summary>
    /// Runs game states one after another until cancelled. Each state returns the id of the next one.
    /// </summary>
    public sealed class GameStateMachine : IGameStateEvents
    {
        private readonly Dictionary<GameStateId, IGameState> _states = new();

        public event Action<GameStateId> OnStateChanged;

        public GameStateMachine(IReadOnlyList<IGameState> states)
        {
            foreach (IGameState state in states)
            {
                if (!_states.TryAdd(state.Id, state))
                {
                    throw new InvalidOperationException($"Game state {state.Id} is registered more than once.");
                }
            }
        }

        public GameStateId CurrentState { get; private set; }

        public async UniTask RunAsync(GameStateId initialState, CancellationToken cancellationToken)
        {
            GameStateId next = initialState;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (!_states.TryGetValue(next, out IGameState state))
                {
                    throw new InvalidOperationException($"Game state {next} is not registered.");
                }

                CurrentState = next;
                OnStateChanged?.Invoke(next);
                next = await state.RunAsync(cancellationToken);
            }
        }
    }
}
