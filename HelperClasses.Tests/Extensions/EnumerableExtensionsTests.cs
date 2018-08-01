using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace HelperClasses.Tests.Extensions
{
    public class EnumerableExtensionsTests
    {
        private List<TestObject> _actionCalls;

        public EnumerableExtensionsTests()
        {
            _actionCalls = new List<TestObject>();
        }

        [Fact]
        public void ForEach_NullArray_ActionNotCalled()
        {
            // Arrange
            IEnumerable<TestObject> target = null;

            // Act
            target.ForEach(Action);

            // Assert
            Assert.Empty(_actionCalls);
        }

        [Fact]
        public void ForEach_EmptyArray_ActionNotCalled()
        {
            // Arrange
            var target = (new TestObject[0]).Select(obj => obj);

            // Act
            target.ForEach(Action);

            // Assert
            Assert.Empty(_actionCalls);
        }

        [Fact]
        public void ForEach_NullAction_ActionNotCalled()
        {
            // Arrange
            var target = (new[]
            {
                new TestObject(1)
            }).Select(obj => obj);
            Action<TestObject> action = null;

            // Act
            target.ForEach(action);

            // Assert
            Assert.Empty(_actionCalls);
        }

        [Fact]
        public void ForEach_SingleItemArray_ActionCalledOnce()
        {
            // Arrange
            var target = (new[]
            {
                new TestObject(64)
            }).Select(obj => obj);

            // Act
            target.ForEach(Action);

            // Assert
            var actionObj = _actionCalls.Single();
            Assert.Equal(64, actionObj.Id);
        }

        [Fact]
        public void ForEach_MultipleItemArray_ActionCalledSeveralTimes()
        {
            // Arrange
            var target = (new[]
            {
                new TestObject(1),
                new TestObject(2),
                new TestObject(3),
                new TestObject(4),
                new TestObject(5),
            }).Select(obj => obj);

            // Act
            target.ForEach(Action);

            // Assert
            Assert.Equal(5, _actionCalls.Count);
        }

        /// <summary>
        /// This test is designed to give an idea of performance, but this should not
        /// be reliend on too much. Current 10M records as below takes about 250ms.
        /// </summary>
        [Fact]
        public void ForEach_MassiveItemArray_ActionCalledALot()
        {
            // Arrange
            const int count = 10000000;
            var stopwatch = new Stopwatch();
            var target = new List<TestObject>();
            for (var i = 1; i <= count; i++)
            {
                target.Add(new TestObject(i));
            }

            // Act
            stopwatch.Start();
            ((IEnumerable<TestObject>)target).ForEach(Action);
            stopwatch.Stop();

            // Assert
            Console.WriteLine($"*** Load Test Time (milliseconds): {stopwatch.Elapsed} ***");
            Assert.Equal(count, _actionCalls.Count);
            Assert.True(stopwatch.ElapsedMilliseconds < 500);
        }

        private void Action(TestObject obj)
        {
            _actionCalls.Add(obj);
        }
    }

    public class TestObject
    {
        public TestObject(int id)
        {
            Id = id;
        }

        public int Id { get; }
    }
}
