using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using ZombieCar.GameFlow;

namespace ZombieCar.Tests
{
    public sealed class GameStateMachineTests
    {
        private sealed class StubState : IGameState
        {
            private readonly Func<CancellationToken, GameStateId> _run;

            public StubState(GameStateId id, Func<CancellationToken, GameStateId> run)
            {
                Id = id;
                _run = run;
            }

            public GameStateId Id { get; }

            public UniTask<GameStateId> RunAsync(CancellationToken cancellationToken) =>
                UniTask.FromResult(_run(cancellationToken));
        }

        [Test]
        public void RunAsync_FollowsStatesUntilCancelled()
        {
            using var cancellation = new CancellationTokenSource();
            var visited = new List<GameStateId>();
            int readyVisits = 0;

            var machine = new GameStateMachine(new IGameState[]
            {
                new StubState(GameStateId.Ready, token =>
                {
                    if (++readyVisits == 2)
                    {
                        cancellation.Cancel();
                        token.ThrowIfCancellationRequested();
                    }

                    return GameStateId.Playing;
                }),
                new StubState(GameStateId.Playing, _ => GameStateId.Lost),
                new StubState(GameStateId.Lost, _ => GameStateId.Ready),
                new StubState(GameStateId.Won, _ => GameStateId.Ready),
            });
            machine.OnStateChanged += visited.Add;

            UniTask run = machine.RunAsync(GameStateId.Ready, cancellation.Token);

            Assert.That(run.Status, Is.EqualTo(UniTaskStatus.Canceled));
            Assert.That(visited, Is.EqualTo(new[]
            {
                GameStateId.Ready, GameStateId.Playing, GameStateId.Lost, GameStateId.Ready,
            }));
            Assert.That(machine.CurrentState, Is.EqualTo(GameStateId.Ready));
        }

        [Test]
        public void Constructor_RejectsDuplicateStates()
        {
            Assert.Throws<InvalidOperationException>(() => new GameStateMachine(new IGameState[]
            {
                new StubState(GameStateId.Ready, _ => GameStateId.Ready),
                new StubState(GameStateId.Ready, _ => GameStateId.Ready),
            }));
        }
    }
}
