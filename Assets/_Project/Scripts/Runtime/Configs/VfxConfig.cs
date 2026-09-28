using System;
using System.Collections.Generic;
using UnityEngine;
using ZombieCar.Effects;

namespace ZombieCar.Configs
{
    /// <summary>
    /// Maps each <see cref="VfxType"/> to a pooled effect prefab.
    /// Entries without a prefab are allowed and simply play nothing.
    /// </summary>
    [CreateAssetMenu(menuName = "Zombie Car/VFX Config", fileName = "VfxConfig")]
    public sealed class VfxConfig : ScriptableObject
    {
        [SerializeField] private Entry[] _entries = Array.Empty<Entry>();

        public IReadOnlyList<Entry> Entries => _entries;

        [Serializable]
        public struct Entry
        {
            [SerializeField] private VfxType _type;
            [SerializeField] private PooledEffect _prefab;
            [SerializeField, Min(0)] private int _prewarmCount;

            public VfxType Type => _type;
            public PooledEffect Prefab => _prefab;
            public int PrewarmCount => _prewarmCount;
        }
    }
}
