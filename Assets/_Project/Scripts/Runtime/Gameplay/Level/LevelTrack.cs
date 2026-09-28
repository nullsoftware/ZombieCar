using UnityEngine;
using ZombieCar.Configs;

namespace ZombieCar.Gameplay.Level
{
    /// <summary>
    /// A straight track from the start point to the finish line, with helpers
    /// to convert between world positions and distance along the track.
    /// </summary>
    public sealed class LevelTrack
    {
        public LevelTrack(LevelConfig config, LevelView view)
        {
            Transform start = view.StartPoint;
            StartPose = new Pose(start.position, start.rotation);
            Length = config.TrackLength;
            RoadHalfWidth = config.RoadHalfWidth;
        }

        public Pose StartPose { get; }
        public float Length { get; }
        public float RoadHalfWidth { get; }
        public Vector3 Forward => StartPose.forward;
        public Vector3 Right => StartPose.right;
        public Vector3 FinishPosition => GetPoint(Length, 0f);

        public Vector3 GetPoint(float distance, float lateralOffset) =>
            StartPose.position + Forward * distance + Right * lateralOffset;

        public float GetDistance(Vector3 position) => Vector3.Dot(position - StartPose.position, Forward);

        public float GetProgress(Vector3 position) => Mathf.Clamp01(GetDistance(position) / Length);

        public bool IsFinishReached(Vector3 position) => GetDistance(position) >= Length;
    }
}
