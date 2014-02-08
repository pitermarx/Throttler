using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using NUnit.Framework;

using Ploeh.AutoFixture;

namespace Throttler
{
    [TestFixture]
    public class ThrottlerTests
    {
        private Throttler<int> throttled;
        private AutoResetEvent waiter;
        private int length;
        private void Reseter(List<int> list)
        {
            length = list.Count;
            waiter.Set();
        }

        [SetUp]
        public void Setup()
        {
            length = 0;
            waiter = new AutoResetEvent(false);
        }

        [Test]
        public void ThrottledMetthodIsNotCalledOnEmptyQueue()
        {
            // arrange
            throttled = new Throttler<int>(Reseter, 100, TimeSpan.FromMinutes(5));

            // test
            throttled.Flush();

            // assert
            Assert.IsFalse(waiter.WaitOne(2000));
        }

        [Test]
        public void ThrottledMetthodIsNotCalled()
        {
            // arrange
            throttled = new Throttler<int>(Reseter, 100, TimeSpan.FromMinutes(5));

            // test
            throttled.Call(new int());
            throttled.Call(new List<int>(5));

            // assert
            Assert.IsFalse(waiter.WaitOne(2000));
        }

        [Test]
        public void ThrottledMetthodIsCalledOnFlush()
        {
            // arrange
            throttled = new Throttler<int>(Reseter, 100, TimeSpan.FromMinutes(5));

            // test
            throttled.Call(new int());
            throttled.Call(new int());
            throttled.Call(new int());
            throttled.Call(new List<int>());
            throttled.Flush();

            // assert
            Assert.IsTrue(waiter.WaitOne(2000));
            Assert.IsTrue(length == 3);
        }

        [Test]
        public void ThrottledMetthodIsCalledOnTimeout()
        {
            // arrange
            throttled = new Throttler<int>(Reseter, 100, TimeSpan.FromMilliseconds(200));

            // test
            throttled.Call(new int());
            throttled.Call(new List<int>());

            // assert
            Assert.IsTrue(waiter.WaitOne(2000));
            Assert.IsTrue(length == 1);
            Assert.IsFalse(waiter.WaitOne(2000));
        }

        [Test]
        public void ThrottledMetthodIsCalledOnTimeout2()
        {
            // arrange
            throttled = new Throttler<int>(Reseter, 100, TimeSpan.FromMilliseconds(200));

            // test
            throttled.Call(new int());
            throttled.Call(new List<int>());

            // assert
            Assert.IsTrue(waiter.WaitOne(2000));
            Assert.IsTrue(length == 1);

            // just one more
            throttled.Call(new int());
            Assert.IsTrue(waiter.WaitOne(2000));
        }

        [Test]
        public void ThrottledMetthodIsCalledOnQueueFull()
        {
            // arrange
            throttled = new Throttler<int>(Reseter, 100, TimeSpan.FromMinutes(5));
            throttled.Call(new Fixture().CreateMany<int>(99).ToList());

            // assert 1
            Assert.IsFalse(waiter.WaitOne(2000));

            // test
            throttled.Call(new int());

            // assert 2
            Assert.IsTrue(waiter.WaitOne(2000));
            Assert.IsTrue(length == 100);
        }

        [Test]
        public void ThrottledMetthodThrows()
        {
            // arrange
            throttled = new Throttler<int>(list => { throw new Exception("ERROR"); }, 100, TimeSpan.FromMilliseconds(100))
            {
                OnError = (ex, list) => waiter.Set()
            };

            throttled.Call(new int());

            // assert 1
            Assert.IsTrue(waiter.WaitOne(2000));
        }
    }
}
