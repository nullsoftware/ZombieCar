namespace ZombieCar.Core.Combat
{
    /// <summary>
    /// Shared health/damage contract for everything that can be hurt (the car, enemies, ...).
    /// </summary>
    public interface IDamageable
    {
        bool IsAlive { get; }

        void TakeDamage(float amount);
    }
}
