using System;

namespace ZombieCar.GameFlow
{
    public interface IGameStateEvents
    {
        event Action<GameStateId> OnStateChanged;

        GameStateId CurrentState { get; }
    }
}
