using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ZombieCar.Cameras;
using ZombieCar.Configs;
using ZombieCar.Controls;
using ZombieCar.Core.Combat;
using ZombieCar.Effects;
using ZombieCar.GameFlow;
using ZombieCar.GameFlow.States;
using ZombieCar.Gameplay.Enemies;
using ZombieCar.Gameplay.Enemies.Spawning;
using ZombieCar.Gameplay.Enemies.States;
using ZombieCar.Gameplay.Level;
using ZombieCar.Gameplay.Vehicles;
using ZombieCar.Gameplay.Weapons;
using ZombieCar.UI;

namespace ZombieCar.Bootstrap
{
    /// <summary>
    /// Composition root of the game scene. It only wires dependencies and holds no game logic.
    /// </summary>
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [Header("Configs")]
        [SerializeField] private CarConfig _carConfig;
        [SerializeField] private TurretConfig _turretConfig;
        [SerializeField] private ProjectileConfig _projectileConfig;
        [SerializeField] private EnemyConfig _enemyConfig;
        [SerializeField] private LevelConfig _levelConfig;
        [SerializeField] private VfxConfig _vfxConfig;
        [SerializeField] private GameFlowConfig _gameFlowConfig;

        [Header("Scene")]
        [SerializeField] private Car _car;
        [SerializeField] private TurretView _turretView;
        [SerializeField] private LevelView _levelView;
        [SerializeField] private CameraRig _cameraRig;
        [SerializeField] private GameHudView _hudView;

        protected override void Configure(IContainerBuilder builder)
        {
            ValidateReferences();

            RegisterConfigs(builder);
            RegisterSceneComponents(builder);

            builder.RegisterEntryPoint<PointerInputService>();
            builder.RegisterEntryPoint<VfxService>();
            builder.Register<CameraDirector>(Lifetime.Singleton).As<ICameraDirector>();

            RegisterLevel(builder);
            RegisterVehicle(builder);
            RegisterWeapons(builder);
            RegisterEnemies(builder);

            builder.RegisterEntryPoint<GameHudPresenter>();

            RegisterGameFlow(builder);
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            builder.RegisterInstance(_carConfig);
            builder.RegisterInstance(_turretConfig);
            builder.RegisterInstance(_projectileConfig);
            builder.RegisterInstance(_enemyConfig);
            builder.RegisterInstance(_levelConfig);
            builder.RegisterInstance(_vfxConfig);
            builder.RegisterInstance(_gameFlowConfig);
        }

        private void RegisterSceneComponents(IContainerBuilder builder)
        {
            builder.RegisterComponent(_car).As<ITargetable>();
            builder.RegisterComponent(_turretView);
            builder.RegisterComponent(_levelView);
            builder.RegisterComponent(_cameraRig);
            builder.RegisterComponent(_hudView);
        }

        private static void RegisterLevel(IContainerBuilder builder)
        {
            builder.Register<LevelTrack>(Lifetime.Singleton);
            builder.Register<LevelOutcomeEvaluator>(Lifetime.Singleton).As<ILevelOutcome>();
            builder.RegisterEntryPoint<GroundTiler>();
            builder.RegisterEntryPoint<FinishLinePresenter>();
        }

        private static void RegisterVehicle(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<CarController>();
            builder.RegisterEntryPoint<CarDamageFeedback>();
        }

        private static void RegisterWeapons(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ProjectileSystem>();
            builder.RegisterEntryPoint<TurretAimController>();
            builder.RegisterEntryPoint<TurretShooter>();
        }

        private static void RegisterEnemies(IContainerBuilder builder)
        {
            builder.Register<EnemyTargeting>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.Register<EnemyIdleState>(Lifetime.Singleton).As<IEnemyState>();
            builder.Register<EnemyChaseState>(Lifetime.Singleton).As<IEnemyState>();
            builder.Register<EnemyAttackState>(Lifetime.Singleton).As<IEnemyState>();
            builder.Register<EnemyDeadState>(Lifetime.Singleton).As<IEnemyState>();
            builder.Register<EnemyStateMachine>(Lifetime.Singleton);

            builder.Register<RandomEnemySpawnLayout>(Lifetime.Singleton).As<IEnemySpawnLayout>();
            builder.Register<EnemySpawner>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.RegisterEntryPoint<EnemyAiSystem>();
        }

        private static void RegisterGameFlow(IContainerBuilder builder)
        {
            builder.Register<ReadyState>(Lifetime.Singleton).As<IGameState>();
            builder.Register<PlayingState>(Lifetime.Singleton).As<IGameState>();
            builder.Register<WonState>(Lifetime.Singleton).As<IGameState>();
            builder.Register<LostState>(Lifetime.Singleton).As<IGameState>();

            builder.Register<GameStateMachine>(Lifetime.Singleton).AsSelf().As<IGameStateEvents>();
            builder.RegisterEntryPoint<GameLoop>();
        }

        private void ValidateReferences()
        {
            Require(_carConfig, nameof(_carConfig));
            Require(_turretConfig, nameof(_turretConfig));
            Require(_projectileConfig, nameof(_projectileConfig));
            Require(_enemyConfig, nameof(_enemyConfig));
            Require(_levelConfig, nameof(_levelConfig));
            Require(_vfxConfig, nameof(_vfxConfig));
            Require(_gameFlowConfig, nameof(_gameFlowConfig));
            Require(_car, nameof(_car));
            Require(_turretView, nameof(_turretView));
            Require(_levelView, nameof(_levelView));
            Require(_cameraRig, nameof(_cameraRig));
            Require(_hudView, nameof(_hudView));
        }

        private void Require(UnityEngine.Object reference, string fieldName)
        {
            if (reference == null)
            {
                throw new InvalidOperationException(
                    $"{name}: '{fieldName}' is not assigned.");
            }
        }
    }
}
