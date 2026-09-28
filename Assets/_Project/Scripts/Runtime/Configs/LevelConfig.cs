using UnityEngine;

namespace ZombieCar.Configs
{
    [CreateAssetMenu(menuName = "Zombie Car/Level Config", fileName = "LevelConfig")]
    public sealed class LevelConfig : ScriptableObject
    {
        [Header("Track")]
        [Tooltip("Distance from the start point to the finish line.")]
        [SerializeField, Min(10f)] private float _trackLength = 250f;
        [Tooltip("Half width of the area where enemies can be placed.")]
        [SerializeField, Min(0f)] private float _roadHalfWidth = 9f;

        [Header("Ground Tiles")]
        [SerializeField] private GameObject _groundTilePrefab;
        [Tooltip("Tile length along the track. 0 = measure from the tile's renderers.")]
        [SerializeField, Min(0f)] private float _groundTileLength;
        [SerializeField, Min(0)] private int _tilesBehind = 1;
        [SerializeField, Min(1)] private int _tilesAhead = 3;

        [Header("Enemy Placement")]
        [SerializeField, Min(0f)] private float _firstWaveDistance = 30f;
        [Tooltip("No enemies are placed closer than this to the finish line.")]
        [SerializeField, Min(0f)] private float _finishSafeZone = 12f;
        [SerializeField, Min(1f)] private float _waveSpacing = 12f;
        [SerializeField, Min(0f)] private float _waveJitter = 3f;
        [SerializeField] private Vector2Int _enemiesPerWave = new(1, 3);
        [Tooltip("0 = new random layout on every restart.")]
        [SerializeField] private int _randomSeed;

        public float TrackLength => _trackLength;
        public float RoadHalfWidth => _roadHalfWidth;
        public GameObject GroundTilePrefab => _groundTilePrefab;
        public float GroundTileLength => _groundTileLength;
        public int TilesBehind => _tilesBehind;
        public int TilesAhead => _tilesAhead;
        public float FirstWaveDistance => _firstWaveDistance;
        public float FinishSafeZone => _finishSafeZone;
        public float WaveSpacing => _waveSpacing;
        public float WaveJitter => _waveJitter;
        public Vector2Int EnemiesPerWave => _enemiesPerWave;
        public int RandomSeed => _randomSeed;

        private void OnValidate()
        {
            _enemiesPerWave.x = Mathf.Max(0, _enemiesPerWave.x);
            _enemiesPerWave.y = Mathf.Max(_enemiesPerWave.x, _enemiesPerWave.y);
        }
    }
}
