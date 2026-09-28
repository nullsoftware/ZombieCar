using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ZombieCar.Cameras;
using ZombieCar.Controls;
using ZombieCar.Core.Lifecycle;

namespace ZombieCar.GameFlow.States
{
    /// <summary>
    /// Initial state: resets the level, frames the stationary car and waits for the first tap.
    /// </summary>
    public sealed class ReadyState : IGameState
    {
        private readonly IReadOnlyList<IResettable> _resettables;
        private readonly ICameraDirector _camera;
        private readonly ITapInput _tapInput;

        public ReadyState(IReadOnlyList<IResettable> resettables, ICameraDirector camera, ITapInput tapInput)
        {
            _resettables = resettables;
            _camera = camera;
            _tapInput = tapInput;
        }

        public GameStateId Id => GameStateId.Ready;

        public async UniTask<GameStateId> RunAsync(CancellationToken cancellationToken)
        {
            foreach (IResettable resettable in _resettables)
            {
                resettable.ResetState();
            }

            _camera.CutToIntro();

            await _tapInput.WaitForTapAsync(cancellationToken);
            return GameStateId.Playing;
        }
    }
}
