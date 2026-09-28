using System.Threading;
using Cysharp.Threading.Tasks;

namespace ZombieCar.GameFlow
{
    /// <summary>
    /// An async step of the game loop. It runs until its exit condition is met,
    /// cleans up after itself, and returns the next state. States never reference each other.
    /// </summary>
    public interface IGameState
    {
        GameStateId Id { get; }

        UniTask<GameStateId> RunAsync(CancellationToken cancellationToken);
    }
}
