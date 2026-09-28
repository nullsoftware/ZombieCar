using System;
using System.Collections.Generic;

namespace ZombieCar.Core.Pooling
{
    /// <summary>
    /// Generic object pool that does not allocate once warmed up.
    /// Unlike <c>UnityEngine.Pool.ObjectPool</c> it tracks active items, which gives
    /// cheap <see cref="ReleaseAll"/> for level restarts and protects against double release.
    /// </summary>
    public sealed class ObjectPool<T> : IObjectPool<T>, IDisposable where T : class
    {
        private readonly Func<T> _createFunc;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly Action<T> _onDestroy;
        private readonly int _maxInactive;

        private readonly Stack<T> _inactive;
        private readonly HashSet<T> _active;
        private readonly List<T> _releaseBuffer;

        public ObjectPool(
            Func<T> createFunc,
            Action<T> onGet = null,
            Action<T> onRelease = null,
            Action<T> onDestroy = null,
            int initialCapacity = 16,
            int maxInactive = 1024)
        {
            _createFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
            _onGet = onGet;
            _onRelease = onRelease;
            _onDestroy = onDestroy;
            _maxInactive = Math.Max(1, maxInactive);

            _inactive = new Stack<T>(initialCapacity);
            _active = new HashSet<T>();
            _releaseBuffer = new List<T>(initialCapacity);
        }

        public int CountActive => _active.Count;
        public int CountInactive => _inactive.Count;

        public void Prewarm(int count)
        {
            for (int i = _inactive.Count + _active.Count; i < count; i++)
            {
                _inactive.Push(_createFunc());
            }
        }

        public T Get()
        {
            T item = _inactive.Count > 0 ? _inactive.Pop() : _createFunc();
            _active.Add(item);
            _onGet?.Invoke(item);
            return item;
        }

        public void Release(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!_active.Remove(item))
            {
                throw new InvalidOperationException($"Trying to release {item} that is not active in this pool.");
            }

            _onRelease?.Invoke(item);

            if (_inactive.Count < _maxInactive)
            {
                _inactive.Push(item);
            }
            else
            {
                _onDestroy?.Invoke(item);
            }
        }

        public void ReleaseAll()
        {
            // Copy first: Release mutates the active set.
            _releaseBuffer.AddRange(_active);

            foreach (T item in _releaseBuffer)
            {
                Release(item);
            }

            _releaseBuffer.Clear();
        }

        public void Dispose()
        {
            if (_onDestroy != null)
            {
                foreach (T item in _active)
                {
                    _onDestroy(item);
                }

                foreach (T item in _inactive)
                {
                    _onDestroy(item);
                }
            }

            _active.Clear();
            _inactive.Clear();
        }
    }
}
