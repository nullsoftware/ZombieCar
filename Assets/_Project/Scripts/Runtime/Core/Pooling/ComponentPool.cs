using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZombieCar.Core.Pooling
{
    /// <summary>
    /// Prefab-based pool for components. Instances live under their own root object,
    /// are positioned before activation and get <see cref="IPoolable"/> callbacks.
    /// </summary>
    public sealed class ComponentPool<T> : IObjectPool<T>, IDisposable where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _root;
        private readonly ObjectPool<T> _pool;

        public ComponentPool(T prefab, int prewarmCount = 0, int maxInactive = 256, Transform parent = null)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            _prefab = prefab;
            _root = new GameObject($"[Pool] {prefab.name}").transform;
            _root.SetParent(parent, false);

            _pool = new ObjectPool<T>(Create, onRelease: Deactivate, onDestroy: DestroyItem,
                initialCapacity: Math.Max(prewarmCount, 4), maxInactive: maxInactive);
            _pool.Prewarm(prewarmCount);
        }

        public int CountActive => _pool.CountActive;
        public int CountInactive => _pool.CountInactive;

        public T Get()
        {
            T item = _pool.Get();
            Activate(item);
            return item;
        }

        public T Get(Vector3 position, Quaternion rotation)
        {
            T item = _pool.Get();
            // Position first so OnEnable, trails and particles start at the right place.
            item.transform.SetPositionAndRotation(position, rotation);
            Activate(item);
            return item;
        }

        public void Release(T item) => _pool.Release(item);

        public void ReleaseAll() => _pool.ReleaseAll();

        public void Dispose()
        {
            _pool.Dispose();

            if (_root != null)
            {
                Object.Destroy(_root.gameObject);
            }
        }

        private T Create()
        {
            T item = Object.Instantiate(_prefab, _root);
            item.gameObject.SetActive(false);
            return item;
        }

        private static void Activate(T item)
        {
            item.gameObject.SetActive(true);

            if (item is IPoolable poolable)
            {
                poolable.OnSpawned();
            }
        }

        private static void Deactivate(T item)
        {
            if (item is IPoolable poolable)
            {
                poolable.OnDespawned();
            }

            item.gameObject.SetActive(false);
        }

        private static void DestroyItem(T item)
        {
            if (item != null)
            {
                Object.Destroy(item.gameObject);
            }
        }
    }
}
