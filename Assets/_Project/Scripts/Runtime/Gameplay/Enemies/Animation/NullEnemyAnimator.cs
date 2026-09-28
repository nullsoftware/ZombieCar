namespace ZombieCar.Gameplay.Enemies.Animation
{
    /// <summary>
    /// No-op animator for enemies without any animation set up.
    /// </summary>
    public sealed class NullEnemyAnimator : IEnemyAnimator
    {
        public static readonly NullEnemyAnimator Instance = new();

        private NullEnemyAnimator()
        {
        }

        public void ResetPose()
        {
        }

        public void PlayIdle()
        {
        }

        public void PlayRun()
        {
        }

        public void PlayAttack()
        {
        }

        public void PlayHit()
        {
        }

        public void PlayDeath()
        {
        }
    }
}
