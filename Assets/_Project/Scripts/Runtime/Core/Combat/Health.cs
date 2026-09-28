using System;

namespace ZombieCar.Core.Combat
{
    /// <summary>
    /// Plain health model shared by the car and the enemies.
    /// </summary>
    public sealed class Health : IReadOnlyHealth
    {
        public event Action<float> OnDamaged;
        public event Action OnChanged;
        public event Action OnDied;

        public Health(float max)
        {
            SetMax(max);
            Current = Max;
        }

        public float Current { get; private set; }
        public float Max { get; private set; }
        public float Normalized => Current / Max;
        public bool IsAlive => Current > 0f;

        /// <summary>
        /// Applies damage. Returns <c>true</c> if this hit killed the owner.
        /// </summary>
        public bool TakeDamage(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return false;
            }

            Current = Math.Max(0f, Current - amount);
            OnDamaged?.Invoke(amount);
            OnChanged?.Invoke();

            if (IsAlive)
            {
                return false;
            }

            OnDied?.Invoke();
            return true;
        }

        public void Restore()
        {
            Current = Max;
            OnChanged?.Invoke();
        }

        public void Restore(float max)
        {
            SetMax(max);
            Restore();
        }

        private void SetMax(float max)
        {
            if (max <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(max), max, "Max health must be positive.");
            }

            Max = max;
        }
    }
}
