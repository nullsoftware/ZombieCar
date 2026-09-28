using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using ZombieCar.Configs;
using ZombieCar.Core.Lifecycle;
using ZombieCar.Core.Pooling;

namespace ZombieCar.Effects
{
    /// <summary>
    /// Keeps one pool per effect type and returns effects to it once their lifetime is up.
    /// </summary>
    public sealed class VfxService : IVfxService, ITickable, IResettable, IDisposable
    {
        private readonly ComponentPool<PooledEffect>[] _pools;
        private readonly List<ActiveEffect> _active = new(32);

        public VfxService(VfxConfig config)
        {
            _pools = new ComponentPool<PooledEffect>[Enum.GetValues(typeof(VfxType)).Length];

            foreach (VfxConfig.Entry entry in config.Entries)
            {
                if (entry.Prefab == null)
                {
                    continue;
                }

                int index = (int)entry.Type;

                if (_pools[index] != null)
                {
                    Debug.LogWarning($"{nameof(VfxConfig)} has more than one entry for {entry.Type}; the first one is used.");
                    continue;
                }

                _pools[index] = new ComponentPool<PooledEffect>(entry.Prefab, entry.PrewarmCount);
            }
        }

        public void Play(VfxType type, Vector3 position, Quaternion rotation)
        {
            ComponentPool<PooledEffect> pool = _pools[(int)type];

            if (pool == null)
            {
                return;
            }

            PooledEffect effect = pool.Get(position, rotation);
            _active.Add(new ActiveEffect(effect, pool, Time.time + effect.Lifetime));
        }

        public void Tick()
        {
            float now = Time.time;

            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (now >= _active[i].ReleaseTime)
                {
                    ReleaseAt(i);
                }
            }
        }

        public void ResetState()
        {
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                ReleaseAt(i);
            }
        }

        public void Dispose()
        {
            foreach (ComponentPool<PooledEffect> pool in _pools)
            {
                pool?.Dispose();
            }

            _active.Clear();
        }

        private void ReleaseAt(int index)
        {
            ActiveEffect active = _active[index];
            int lastIndex = _active.Count - 1;
            _active[index] = _active[lastIndex];
            _active.RemoveAt(lastIndex);
            active.Pool.Release(active.Effect);
        }

        private readonly struct ActiveEffect
        {
            public ActiveEffect(PooledEffect effect, ComponentPool<PooledEffect> pool, float releaseTime)
            {
                Effect = effect;
                Pool = pool;
                ReleaseTime = releaseTime;
            }

            public PooledEffect Effect { get; }
            public ComponentPool<PooledEffect> Pool { get; }
            public float ReleaseTime { get; }
        }
    }
}
