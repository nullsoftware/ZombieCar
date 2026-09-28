using NUnit.Framework;
using ZombieCar.Core.Combat;

namespace ZombieCar.Tests
{
    public sealed class HealthTests
    {
        [Test]
        public void TakeDamage_ReducesCurrentHealth()
        {
            var health = new Health(100f);

            health.TakeDamage(30f);

            Assert.That(health.Current, Is.EqualTo(70f));
            Assert.That(health.Normalized, Is.EqualTo(0.7f).Within(0.0001f));
            Assert.That(health.IsAlive, Is.True);
        }

        [Test]
        public void TakeDamage_ClampsAtZeroAndRaisesDiedOnce()
        {
            var health = new Health(50f);
            int diedCount = 0;
            health.OnDied += () => diedCount++;

            bool killed = health.TakeDamage(80f);
            bool killedAgain = health.TakeDamage(10f);

            Assert.That(killed, Is.True);
            Assert.That(killedAgain, Is.False);
            Assert.That(health.Current, Is.Zero);
            Assert.That(health.IsAlive, Is.False);
            Assert.That(diedCount, Is.EqualTo(1));
        }

        [Test]
        public void TakeDamage_IgnoresNonPositiveAmounts()
        {
            var health = new Health(10f);
            int changedCount = 0;
            health.OnChanged += () => changedCount++;

            health.TakeDamage(0f);
            health.TakeDamage(-5f);

            Assert.That(health.Current, Is.EqualTo(10f));
            Assert.That(changedCount, Is.Zero);
        }

        [Test]
        public void Restore_RevivesWithNewMax()
        {
            var health = new Health(10f);
            health.TakeDamage(10f);

            health.Restore(25f);

            Assert.That(health.IsAlive, Is.True);
            Assert.That(health.Current, Is.EqualTo(25f));
            Assert.That(health.Max, Is.EqualTo(25f));
        }
    }
}
