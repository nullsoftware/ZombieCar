using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ZombieCar.Configs;
using ZombieCar.Controls;

namespace ZombieCar.GameFlow.States
{
    public abstract class GameOverState : IGameState
    {
        private readonly ITapInput _tapInput;
        private readonly GameFlowConfig _config;

        protected GameOverState(ITapInput tapInput, GameFlowConfig config)
        {
            _tapInput = tapInput;
            _config = config;
        }

        public abstract GameStateId Id { get; }

        public async UniTask<GameStateId> RunAsync(CancellationToken cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_config.RestartInputDelay), cancellationToken: cancellationToken);
            await _tapInput.WaitForTapAsync(cancellationToken);
            return GameStateId.Ready;
        }
    }
}
