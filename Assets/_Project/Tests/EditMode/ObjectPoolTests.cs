using System;
using System.Collections.Generic;
using NUnit.Framework;
using ZombieCar.Core.Pooling;

namespace ZombieCar.Tests
{
    public sealed class ObjectPoolTests
    {
        private sealed class Item
        {
        }

        [Test]
        public void Get_ReusesReleasedItem()
        {
            var pool = new ObjectPool<Item>(() => new Item());
            Item first = pool.Get();

            pool.Release(first);
            Item second = pool.Get();

            Assert.That(second, Is.SameAs(first));
            Assert.That(pool.CountActive, Is.EqualTo(1));
            Assert.That(pool.CountInactive, Is.Zero);
        }

        [Test]
        public void Prewarm_CreatesItemsUpFront()
        {
            int created = 0;
            var pool = new ObjectPool<Item>(() =>
            {
                created++;
                return new Item();
            });

            pool.Prewarm(5);
            for (int i = 0; i < 5; i++)
            {
                pool.Get();
            }

            Assert.That(created, Is.EqualTo(5));
        }

        [Test]
        public void ReleaseAll_ReturnsEveryActiveItem()
        {
            var released = new List<Item>();
            var pool = new ObjectPool<Item>(() => new Item(), onRelease: released.Add);
            pool.Get();
            pool.Get();
            pool.Get();

            pool.ReleaseAll();

            Assert.That(released, Has.Count.EqualTo(3));
            Assert.That(pool.CountActive, Is.Zero);
            Assert.That(pool.CountInactive, Is.EqualTo(3));
        }

        [Test]
        public void Release_ThrowsOnDoubleRelease()
        {
            var pool = new ObjectPool<Item>(() => new Item());
            Item item = pool.Get();
            pool.Release(item);

            Assert.Throws<InvalidOperationException>(() => pool.Release(item));
        }

        [Test]
        public void Release_DestroysItemsAboveCapacity()
        {
            var destroyed = new List<Item>();
            var pool = new ObjectPool<Item>(() => new Item(), onDestroy: destroyed.Add, maxInactive: 1);
            Item first = pool.Get();
            Item second = pool.Get();

            pool.Release(first);
            pool.Release(second);

            Assert.That(pool.CountInactive, Is.EqualTo(1));
            Assert.That(destroyed, Is.EqualTo(new[] { second }));
        }
    }
}
