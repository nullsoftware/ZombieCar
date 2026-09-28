using UnityEngine;
using ZombieCar.Core.Pooling;

namespace ZombieCar.Effects
{
    [DisallowMultipleComponent]
    public sealed class PooledEffect : MonoBehaviour, IPoolable
    {
        private const float FallbackLifetime = 2f;

        [Tooltip("How long the effect stays out of the pool. 0 = work it out from the particle systems.")]
        [SerializeField, Min(0f)] private float _lifetime;

        [Tooltip("WARNING: Do not toch this. Filled automatically.")]
        [SerializeField] private ParticleSystem[] _particleSystems;


        public float Lifetime { get; private set; }

        private void OnValidate()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystem>(true);
        }

        private void Awake()
        {
            Lifetime = _lifetime > 0f ? _lifetime : EstimateLifetime();
        }

        public void OnSpawned()
        {
            foreach (ParticleSystem system in _particleSystems)
            {
                system.Clear(false);
                system.Play(false);
            }
        }

        public void OnDespawned()
        {
            foreach (ParticleSystem system in _particleSystems)
            {
                system.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private float EstimateLifetime()
        {
            float lifetime = 0f;

            foreach (ParticleSystem system in _particleSystems)
            {
                ParticleSystem.MainModule main = system.main;

                if (main.loop)
                {
                    return FallbackLifetime;
                }

                float systemLifetime = main.startDelay.constantMax + main.duration + main.startLifetime.constantMax;
                lifetime = Mathf.Max(lifetime, systemLifetime);
            }

            return lifetime > 0f ? lifetime : FallbackLifetime;
        }
    }
}
