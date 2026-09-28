using UnityEngine;
using VContainer.Unity;
using ZombieCar.Configs;
using ZombieCar.Core.Lifecycle;
using ZombieCar.Effects;

namespace ZombieCar.Gameplay.Weapons
{
    /// <summary>
    /// Fires projectiles at a fixed rate for as long as gameplay runs.
    /// </summary>
    public sealed class TurretShooter : ITickable, IGameplaySystem
    {
        private readonly TurretView _view;
        private readonly TurretConfig _config;
        private readonly IProjectileLauncher _launcher;
        private readonly IVfxService _vfx;

        private bool _isFiring;
        private float _cooldown;

        public TurretShooter(TurretView view, TurretConfig config, IProjectileLauncher launcher, IVfxService vfx)
        {
            _view = view;
            _config = config;
            _launcher = launcher;
            _vfx = vfx;
        }

        public void StartGameplay()
        {
            _isFiring = true;
            _cooldown = 0f;
        }

        public void StopGameplay() => _isFiring = false;

        public void Tick()
        {
            if (!_isFiring)
            {
                return;
            }

            _cooldown -= Time.deltaTime;

            if (_cooldown > 0f)
            {
                return;
            }

            // Keep the rest of the interval so the rate does not depend on frame rate.
            _cooldown = Mathf.Max(0f, _cooldown + _config.FireInterval);
            Fire();
        }

        private void Fire()
        {
            Transform muzzle = _view.Muzzle;
            _launcher.Launch(muzzle.position, muzzle.forward);
            _vfx.Play(VfxType.MuzzleFlash, muzzle.position, muzzle.rotation);
        }
    }
}
