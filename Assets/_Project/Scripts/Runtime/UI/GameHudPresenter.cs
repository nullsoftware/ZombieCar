using System;
using VContainer.Unity;
using ZombieCar.GameFlow;
using ZombieCar.Gameplay.Level;
using ZombieCar.Gameplay.Vehicles;

namespace ZombieCar.UI
{
    public sealed class GameHudPresenter : IInitializable, ITickable, IDisposable
    {
        private readonly GameHudView _view;
        private readonly IGameStateEvents _gameState;
        private readonly Car _car;
        private readonly LevelTrack _track;

        public GameHudPresenter(GameHudView view, IGameStateEvents gameState, Car car, LevelTrack track)
        {
            _view = view;
            _gameState = gameState;
            _car = car;
            _track = track;
        }

        public void Initialize()
        {
            _gameState.OnStateChanged += HandleStateChanged;
            _car.Health.OnChanged += HandleHealthChanged;

            HandleStateChanged(_gameState.CurrentState);
            HandleHealthChanged();
        }

        public void Tick() => _view.SetProgress(_track.GetProgress(_car.Position));

        public void Dispose()
        {
            _gameState.OnStateChanged -= HandleStateChanged;
            _car.Health.OnChanged -= HandleHealthChanged;
        }

        private void HandleStateChanged(GameStateId state)
        {
            _view.ShowScreen(state switch
            {
                GameStateId.Ready => HudScreen.Start,
                GameStateId.Won => HudScreen.Win,
                GameStateId.Lost => HudScreen.Lose,
                _ => HudScreen.None,
            });

            _view.SetHudVisible(state != GameStateId.Ready);
        }

        private void HandleHealthChanged() => _view.SetHealth(_car.Health.Normalized);
    }
}
