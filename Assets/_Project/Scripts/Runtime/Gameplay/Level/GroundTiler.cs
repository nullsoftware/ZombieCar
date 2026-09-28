using System;
using UnityEngine;
using VContainer.Unity;
using ZombieCar.Configs;
using ZombieCar.Core.Lifecycle;
using ZombieCar.Gameplay.Vehicles;
using Object = UnityEngine.Object;

namespace ZombieCar.Gameplay.Level
{
    /// <summary>
    /// Endless ground made of a few seamless tiles. When a tile falls far enough
    /// behind the car it is moved to the front, so the track never runs out.
    /// </summary>
    public sealed class GroundTiler : IInitializable, ITickable, IResettable
    {
        private readonly LevelConfig _config;
        private readonly LevelTrack _track;
        private readonly LevelView _view;
        private readonly Car _car;

        private Transform[] _tiles = Array.Empty<Transform>();
        private float _tileLength;
        private int _firstTile;
        private float _firstTileCenter;

        public GroundTiler(LevelConfig config, LevelTrack track, LevelView view, Car car)
        {
            _config = config;
            _track = track;
            _view = view;
            _car = car;
        }

        public void Initialize()
        {
            if (_config.GroundTilePrefab == null)
            {
                Debug.LogWarning($"{nameof(GroundTiler)}: no ground tile prefab in {nameof(LevelConfig)}.");
                return;
            }

            _tiles = new Transform[_config.TilesBehind + _config.TilesAhead + 1];

            for (int i = 0; i < _tiles.Length; i++)
            {
                _tiles[i] = Object.Instantiate(_config.GroundTilePrefab, _view.GroundRoot).transform;
                _tiles[i].name = $"GroundTile_{i}";
            }

            _tileLength = _config.GroundTileLength > 0f ? _config.GroundTileLength : MeasureLength(_tiles[0]);
            ResetState();
        }

        public void ResetState()
        {
            if (_tiles.Length == 0)
            {
                return;
            }

            _firstTile = 0;
            _firstTileCenter = -_config.TilesBehind * _tileLength;

            for (int i = 0; i < _tiles.Length; i++)
            {
                PlaceTile(_tiles[i], _firstTileCenter + i * _tileLength);
            }
        }

        public void Tick()
        {
            if (_tiles.Length == 0)
            {
                return;
            }

            float carDistance = _track.GetDistance(_car.Position);
            float recycleDistance = (_config.TilesBehind + 0.5f) * _tileLength;

            while (carDistance - _firstTileCenter > recycleDistance)
            {
                float newCenter = _firstTileCenter + _tiles.Length * _tileLength;
                PlaceTile(_tiles[_firstTile], newCenter);

                _firstTile = (_firstTile + 1) % _tiles.Length;
                _firstTileCenter += _tileLength;
            }
        }

        private void PlaceTile(Transform tile, float centerDistance)
        {
            tile.SetPositionAndRotation(_track.GetPoint(centerDistance, 0f), _track.StartPose.rotation);
        }

        private float MeasureLength(Transform tile)
        {
            PlaceTile(tile, 0f);
            Renderer[] renderers = tile.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                throw new InvalidOperationException("Ground tile prefab has no renderers to measure; set the tile length in the config.");
            }

            Bounds bounds = renderers[0].bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            // Project the world AABB onto the track direction.
            Vector3 forward = _track.Forward;
            Vector3 size = bounds.size;
            return Mathf.Abs(forward.x) * size.x + Mathf.Abs(forward.y) * size.y + Mathf.Abs(forward.z) * size.z;
        }
    }
}
