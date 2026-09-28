using UnityEngine;

namespace ZombieCar.Gameplay.Level
{
    /// <summary>
    /// Scene anchors of the level: where the car starts, where the ground tiles go, and the finish line.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LevelView : MonoBehaviour
    {
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _groundRoot;
        [SerializeField] private Transform _finishLine;

        public Transform StartPoint => _startPoint;
        public Transform GroundRoot => _groundRoot;
        public Transform FinishLine => _finishLine;
    }
}
