using UnityEngine;

namespace ZombieCar.Configs
{
    [CreateAssetMenu(menuName = "Zombie Car/Game Flow Config", fileName = "GameFlowConfig")]
    public sealed class GameFlowConfig : ScriptableObject
    {
        [Tooltip("Taps are ignored for this long after Win/Lose so the result screen is not skipped by accident.")]
        [SerializeField, Min(0f)] private float _restartInputDelay = 0.8f;

        public float RestartInputDelay => _restartInputDelay;
    }
}
