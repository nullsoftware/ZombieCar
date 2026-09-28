using Unity.Cinemachine;
using UnityEngine;

namespace ZombieCar.Cameras
{
    /// <summary>
    /// Scene references for the Cinemachine setup: an intro shot behind the stationary car
    /// and a higher gameplay follow camera. The brain blends between them.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CameraRig : MonoBehaviour
    {
        [SerializeField] private CinemachineBrain _brain;
        [SerializeField] private CinemachineCamera _introCamera;
        [SerializeField] private CinemachineCamera _gameplayCamera;
        [Tooltip("Optional. Needs a CinemachineImpulseListener on the cameras.")]
        [SerializeField] private CinemachineImpulseSource _impulseSource;

        public CinemachineBrain Brain => _brain;
        public CinemachineCamera IntroCamera => _introCamera;
        public CinemachineCamera GameplayCamera => _gameplayCamera;
        public CinemachineImpulseSource ImpulseSource => _impulseSource;
    }
}
