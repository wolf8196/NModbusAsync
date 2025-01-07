using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using NModbusAsync.IO;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    public class TransactionIdProviderTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void Test()
        {
            // Arrange
            var target = new TransactionIdProvider();
            var collection = new ConcurrentBag<ushort>();

            // Act
            Parallel.For(
                0,
                ushort.MaxValue * 3,
                (i) =>
                {
                    collection.Add(target.NewId());
                });

            // Assert
            var range = Enumerable.Range(0, ushort.MaxValue).Select(x => (ushort)x);
            Assert.Equal(range.Concat(range).Concat(range).OrderBy(x => x), collection.OrderBy(x => x));
            Assert.All(collection.GroupBy(x => x).ToList(), (elem) => Assert.Equal(3, elem.Count()));
        }
    }
}