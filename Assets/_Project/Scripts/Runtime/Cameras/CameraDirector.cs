using Unity.Cinemachine;

namespace ZombieCar.Cameras
{
    /// <summary>
    /// Switches Cinemachine cameras by priority and triggers impulse shakes.
    /// </summary>
    public sealed class CameraDirector : ICameraDirector
    {
        private const int LivePriority = 20;
        private const int StandbyPriority = 10;

        private readonly CameraRig _rig;

        public CameraDirector(CameraRig rig)
        {
            _rig = rig;
        }

        public void CutToIntro()
        {
            SetLive(_rig.IntroCamera, _rig.GameplayCamera);

            // The car was just teleported back to the start: drop damping history and skip the blend.
            _rig.IntroCamera.PreviousStateIsValid = false;
            _rig.GameplayCamera.PreviousStateIsValid = false;
            _rig.Brain.ResetState();
        }

        public void BlendToGameplay() => SetLive(_rig.GameplayCamera, _rig.IntroCamera);

        public void Shake(float force)
        {
            if (_rig.ImpulseSource != null && force > 0f)
            {
                _rig.ImpulseSource.GenerateImpulseWithForce(force);
            }
        }

        private static void SetLive(CinemachineCamera live, CinemachineCamera standby)
        {
            live.Priority = LivePriority;
            standby.Priority = StandbyPriority;
        }
    }
}
