using UnityEngine;

namespace ZombieCar.Gameplay.Enemies.Animation
{
    /// <summary>
    /// Base for enemy animator components. An enemy can carry several; <see cref="Enemy"/> uses the best
    /// available one and disables the rest.
    /// </summary>
    public abstract class EnemyAnimator : MonoBehaviour, IEnemyAnimator
    {
        /// <summary>
        /// Whether this animator has something to play (e.g. its controller has clips).
        /// </summary>
        public abstract bool IsAvailable { get; }

        /// <summary>
        /// Placeholders are only used when no real animator is available.
        /// </summary>
        public virtual bool IsFallback => false;

        public abstract void ResetPose();

        public abstract void PlayIdle();

        public abstract void PlayRun();

        public abstract void PlayAttack();

        public abstract void PlayHit();

        public abstract void PlayDeath();
    }
}
