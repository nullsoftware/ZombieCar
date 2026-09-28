using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using VContainer;
using ZombieCar.Bootstrap;
using ZombieCar.GameFlow;
using ZombieCar.Gameplay.Level;
using ZombieCar.Gameplay.Vehicles;
using Object = UnityEngine.Object;

namespace ZombieCar.Tests
{
    public sealed class GameLoopSmokeTests : InputTestFixture
    {
        private const string GameScenePath = "Assets/_Project/Scenes/Game.unity";
        private const float TimeScale = 6f;
        private const float RoundTimeout = 30f;

        private Mouse _mouse;

        public override void Setup()
        {
            base.Setup();
            _mouse = InputSystem.AddDevice<Mouse>();
        }

        public override void TearDown()
        {
            Time.timeScale = 1f;
            base.TearDown();
        }

        [UnityTest]
        public IEnumerator FullRound_StartsOnTap_EndsAndRestarts()
        {
            if (SceneUtility.GetBuildIndexByScenePath(GameScenePath) < 0)
            {
                Assert.Ignore("Game scene is not generated yet (Tools/Zombie Car/Build Game Scene).");
            }

            yield return SceneManager.LoadSceneAsync(GameScenePath);
            yield return null;

            IObjectResolver container = Object.FindAnyObjectByType<GameLifetimeScope>().Container;
            var gameState = container.Resolve<IGameStateEvents>();
            var car = container.Resolve<Car>();
            var track = container.Resolve<LevelTrack>();
            Vector3 startPosition = car.Position;

            Assert.That(gameState.CurrentState, Is.EqualTo(GameStateId.Ready));

            yield return Tap();
            Assert.That(gameState.CurrentState, Is.EqualTo(GameStateId.Playing));

            Time.timeScale = TimeScale;
            float deadline = Time.realtimeSinceStartup + RoundTimeout;
            yield return new WaitUntil(() =>
                gameState.CurrentState is GameStateId.Won or GameStateId.Lost ||
                Time.realtimeSinceStartup > deadline);

            GameStateId result = gameState.CurrentState;
            Debug.Log($"[Smoke] Round ended: {result}, car HP {car.Health.Current}/{car.Health.Max}, " +
                      $"distance {track.GetDistance(car.Position):0.0}/{track.Length}");

            Assert.That(result, Is.EqualTo(GameStateId.Won).Or.EqualTo(GameStateId.Lost), "Round did not finish in time.");
            Assert.That(track.GetDistance(car.Position), Is.GreaterThan(5f), "Car did not drive.");

            if (result == GameStateId.Won)
            {
                Assert.That(track.IsFinishReached(car.Position), Is.True);
            }
            else
            {
                Assert.That(car.IsAlive, Is.False);
            }

            // taps during the restart grace period must be ignored.
            yield return Tap();
            Assert.That(gameState.CurrentState, Is.EqualTo(result));

            yield return new WaitForSecondsRealtime(1f);
            yield return Tap();

            Assert.That(gameState.CurrentState, Is.EqualTo(GameStateId.Ready));
            Assert.That(car.IsAlive, Is.True);
            Assert.That(car.Health.Normalized, Is.EqualTo(1f));
            Assert.That(Vector3.Distance(car.Position, startPosition), Is.LessThan(0.01f));
        }

        private IEnumerator Tap()
        {
            // queue only, so the press is processed by the regular player-loop input update.
            Press(_mouse.leftButton, queueEventOnly: true);
            yield return null;
            yield return null;
            Release(_mouse.leftButton, queueEventOnly: true);
            yield return null;
        }
    }
}
