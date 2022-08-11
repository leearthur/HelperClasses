using System.Diagnostics;
using Xunit;

namespace HelperClasses.Tests.Extensions
{
    public class EnumerableExtensionsTests
    {
        private readonly List<TestObject> _actionCalls;

        public EnumerableExtensionsTests()
        {
            _actionCalls = new();
        }

        [Fact]
        public void ForEach_EmptyArray_ActionNotCalled()
        {
            var target = Array.Empty<TestObject>().Select(obj => obj);

            target.ForEach(Action);

            Assert.Empty(_actionCalls);
        }

        [Fact]
        public void ForEach_SingleItemArray_ActionCalledOnce()
        {
            var target = (new[]
            {
                new TestObject(64)
            }).Select(obj => obj);

            target.ForEach(Action);

            var actionObj = _actionCalls.Single();
            Assert.Equal(64, actionObj.Id);
        }

        [Fact]
        public void ForEach_MultipleItemArray_ActionCalledSeveralTimes()
        {
            var target = (new[]
            {
                new TestObject(1),
                new TestObject(2),
                new TestObject(3),
                new TestObject(4),
                new TestObject(5),
            }).Select(obj => obj);

            target.ForEach(Action);

            Assert.Equal(5, _actionCalls.Count);
        }

        /// <summary>
        /// This test is designed to give an idea of performance, but this should not
        /// be reliend on too much. Current 2.25M records as below takes about 250ms.
        /// </summary>
        [Fact]
        public void ForEach_MassiveItemArray_ActionCalledALot()
        {
            const int count = 2250000;
            var stopwatch = new Stopwatch();
            var target = new List<TestObject>();
            for (var i = 1; i <= count; i++)
            {
                target.Add(new TestObject(i));
            }

            stopwatch.Start();
            (target as IEnumerable<TestObject>).ForEach(Action);
            stopwatch.Stop();

            Console.WriteLine($"*** Load Test Time (milliseconds): {stopwatch.Elapsed} ***");
            Assert.Equal(count, _actionCalls.Count);
            Assert.True(stopwatch.ElapsedMilliseconds < 500);
        }

        private void Action(TestObject obj) => _actionCalls.Add(obj);
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
