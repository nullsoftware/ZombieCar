using System.Collections.Generic;
using UnityEngine;
using ZombieCar.Configs;
using ZombieCar.Gameplay.Level;
using Random = System.Random;

namespace ZombieCar.Gameplay.Enemies.Spawning
{
    /// <summary>
    /// Scatters groups of enemies along the track, from the first-wave distance up to the finish safe zone.
    /// A non-zero seed in the level config gives the same layout every run.
    /// </summary>
    public sealed class RandomEnemySpawnLayout : IEnemySpawnLayout
    {
        private const float MaxFacingDeviation = 60f;
        private const float WaveDepth = 1.5f;
        private const float MinWaveSpacing = 1f;

        private readonly LevelConfig _config;
        private readonly LevelTrack _track;

        public RandomEnemySpawnLayout(LevelConfig config, LevelTrack track)
        {
            _config = config;
            _track = track;
        }

        public void Build(List<Pose> results)
        {
            results.Clear();
            Random random = _config.RandomSeed != 0 ? new Random(_config.RandomSeed) : new Random();

            float halfWidth = _config.RoadHalfWidth;
            float lastDistance = _track.Length - _config.FinishSafeZone;
            // Enemies face the car coming towards them.
            Quaternion facingCar = _track.StartPose.rotation * Quaternion.Euler(0f, 180f, 0f);

            for (float distance = _config.FirstWaveDistance; distance <= lastDistance; distance += NextSpacing(random))
            {
                int count = random.Next(_config.EnemiesPerWave.x, _config.EnemiesPerWave.y + 1);

                for (int i = 0; i < count; i++)
                {
                    Vector3 position = _track.GetPoint(
                        distance + Range(random, -WaveDepth, WaveDepth),
                        Range(random, -halfWidth, halfWidth));
                    Quaternion rotation = facingCar * Quaternion.Euler(0f, Range(random, -MaxFacingDeviation, MaxFacingDeviation), 0f);
                    results.Add(new Pose(position, rotation));
                }
            }
        }

        private float NextSpacing(Random random)
        {
            float jitter = _config.WaveJitter;
            return Mathf.Max(MinWaveSpacing, _config.WaveSpacing + Range(random, -jitter, jitter));
        }

        private static float Range(Random random, float min, float max) =>
            min + (float)random.NextDouble() * (max - min);
    }
}
