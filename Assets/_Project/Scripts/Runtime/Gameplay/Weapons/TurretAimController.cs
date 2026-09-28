using UnityEngine;
using VContainer.Unity;
using ZombieCar.Configs;
using ZombieCar.Controls;
using ZombieCar.Core.Lifecycle;

namespace ZombieCar.Gameplay.Weapons
{
    /// <summary>
    /// Turns horizontal drag into turret yaw, clamped around the car's forward direction.
    /// </summary>
    public sealed class TurretAimController : ITickable, IGameplaySystem, IResettable
    {
        private readonly TurretView _view;
        private readonly TurretConfig _config;
        private readonly IAimInput _aimInput;

        private bool _isActive;
        private float _targetYaw;
        private float _currentYaw;

        public TurretAimController(TurretView view, TurretConfig config, IAimInput aimInput)
        {
            _view = view;
            _config = config;
            _aimInput = aimInput;
        }

        public void StartGameplay() => _isActive = true;

        public void StopGameplay() => _isActive = false;

        public void ResetState()
        {
            _isActive = false;
            _targetYaw = 0f;
            _currentYaw = 0f;
            _view.SetYaw(0f);
        }

        public void Tick()
        {
            if (!_isActive)
            {
                return;
            }

            float maxYaw = _config.MaxYawAngle;
            _targetYaw = Mathf.Clamp(_targetYaw + _aimInput.AimDelta.x * _config.AimSensitivity, -maxYaw, maxYaw);
            _currentYaw = Mathf.MoveTowards(_currentYaw, _targetYaw, _config.TurnSpeed * Time.deltaTime);
            _view.SetYaw(_currentYaw);
        }
    }
}
