using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using ZombieCar.Cameras;
using ZombieCar.Core.Lifecycle;
using ZombieCar.Gameplay.Level;

namespace ZombieCar.GameFlow.States
{
    /// <summary>
    /// Starts every gameplay system and waits until the level is won or lost.
    /// </summary>
    public sealed class PlayingState : IGameState
    {
        private readonly IReadOnlyList<IGameplaySystem> _systems;
        private readonly ILevelOutcome _outcome;
        private readonly ICameraDirector _camera;

        public PlayingState(IReadOnlyList<IGameplaySystem> systems, ILevelOutcome outcome, ICameraDirector camera)
        {
            _systems = systems;
            _outcome = outcome;
            _camera = camera;
        }

        public GameStateId Id => GameStateId.Playing;

        public async UniTask<GameStateId> RunAsync(CancellationToken cancellationToken)
        {
            _camera.BlendToGameplay();
            SetSystemsRunning(true);

            LevelOutcome outcome;

            try
            {
                await UniTask.WaitUntil(() => _outcome.Evaluate() != LevelOutcome.InProgress,
                    cancellationToken: cancellationToken);
                outcome = _outcome.Evaluate();
            }
            finally
            {
                SetSystemsRunning(false);
            }

            return outcome == LevelOutcome.Won ? GameStateId.Won : GameStateId.Lost;
        }

        private void SetSystemsRunning(bool isRunning)
        {
            foreach (IGameplaySystem system in _systems)
            {
                if (isRunning)
                {
                    system.StartGameplay();
                }
                else
                {
                    system.StopGameplay();
                }
            }
        }
    }
}
