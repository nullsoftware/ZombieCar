using System;
using UnityEngine;
using VContainer.Unity;
using ZombieCar.Cameras;
using ZombieCar.Configs;
using ZombieCar.Effects;

namespace ZombieCar.Gameplay.Vehicles
{
    /// <summary>
    /// Camera shake and effects when the car takes damage or gets destroyed.
    /// </summary>
    public sealed class CarDamageFeedback : IInitializable, IDisposable
    {
        private readonly Car _car;
        private readonly CarConfig _config;
        private readonly ICameraDirector _camera;
        private readonly IVfxService _vfx;

        public CarDamageFeedback(Car car, CarConfig config, ICameraDirector camera, IVfxService vfx)
        {
            _car = car;
            _config = config;
            _camera = camera;
            _vfx = vfx;
        }

        public void Initialize()
        {
            _car.Health.OnDamaged += HandleDamaged;
            _car.Health.OnDied += HandleDied;
        }

        public void Dispose()
        {
            _car.Health.OnDamaged -= HandleDamaged;
            _car.Health.OnDied -= HandleDied;
        }

        private void HandleDamaged(float amount) => _camera.Shake(_config.HitShakeForce);

        private void HandleDied()
        {
            _camera.Shake(_config.DestroyedShakeForce);
            _vfx.Play(VfxType.CarDestroyed, _car.Position, Quaternion.identity);
        }
    }
}
