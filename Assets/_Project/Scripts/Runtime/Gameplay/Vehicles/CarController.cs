using UnityEngine;
using VContainer.Unity;
using ZombieCar.Configs;
using ZombieCar.Core.Lifecycle;
using ZombieCar.Gameplay.Level;

namespace ZombieCar.Gameplay.Vehicles
{
    /// <summary>
    /// Drives the car forward during gameplay, brakes when it ends
    /// and puts the car back at the start line on reset.
    /// </summary>
    public sealed class CarController : IFixedTickable, IGameplaySystem, IResettable
    {
        private readonly Car _car;
        private readonly CarConfig _config;
        private readonly LevelTrack _track;

        private bool _isDriving;
        private float _speed;

        public CarController(Car car, CarConfig config, LevelTrack track)
        {
            _car = car;
            _config = config;
            _track = track;
        }

        public void StartGameplay() => _isDriving = true;

        public void StopGameplay() => _isDriving = false;

        public void ResetState()
        {
            _isDriving = false;
            _speed = 0f;
            _car.Teleport(_track.StartPose);
            _car.RestoreHealth();
        }

        public void FixedTick()
        {
            float deltaTime = Time.fixedDeltaTime;
            float targetSpeed = _isDriving ? _config.MaxSpeed : 0f;
            float rate = _isDriving ? _config.Acceleration : _config.Braking;

            _speed = Mathf.MoveTowards(_speed, targetSpeed, rate * deltaTime);

            if (_speed > 0f)
            {
                _car.Drive(_speed * deltaTime);
            }
        }
    }
}
