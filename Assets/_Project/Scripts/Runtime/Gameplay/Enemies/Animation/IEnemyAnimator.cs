namespace ZombieCar.Gameplay.Enemies.Animation
{
    /// <summary>
    /// Animation commands the enemy AI sends. The AI does not care whether they are
    /// played by Mecanim clips, procedural motion, or nothing at all.
    /// </summary>
    public interface IEnemyAnimator
    {
        void ResetPose();

        void PlayIdle();

        void PlayRun();

        void PlayAttack();

        void PlayHit();

        void PlayDeath();
    }
}
