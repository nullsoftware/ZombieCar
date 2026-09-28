using System.Threading;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace ZombieCar.GameFlow
{
    /// <summary>
    /// Starts the game loop when the scope starts and stops it when the scope is destroyed.
    /// </summary>
    public sealed class GameLoop : IAsyncStartable
    {
        private readonly GameStateMachine _stateMachine;

        public GameLoop(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            // REMARK: cancellation means the scope was destroyed (scene unload / exit play mode), so it is not an error.
            await _stateMachine.RunAsync(GameStateId.Ready, cancellation).SuppressCancellationThrow();
        }
    }
}
