using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration
{
    public abstract class ReadInputRegistersTest : IntegrationTest
    {
        protected ReadInputRegistersTest(string masterType, ITestOutputHelper output)
            : base(masterType, 2, output)
        {
            // Arrange
        }

        [Theory]
        [MemberData(nameof(GetReadResults))]
        [Trait("Category", "Integration")]
        public async Task ReadSuccessfully(ushort startAddress, ushort numberOfPoints, ushort[] expected)
        {
            // Act
            var actual = await Target.ReadInputRegistersAsync(SlaveId, startAddress, numberOfPoints);

            // Assert
            Assert.Equal(expected, actual);
        }

        public static TheoryData<ushort, ushort, ushort[]> GetReadResults()
        {
            return new TheoryData<ushort, ushort, ushort[]>
            {
                { 0, 1, new ushort[] { 100 } },
                { 4, 3, new ushort[] { 200, 300, 400 } },
                {
                    10,
                    125,
                    new ushort[] { 126,  125, 124, 123, 122, 121, 120, }
                    .Concat(Enumerable.Repeat((ushort)0, 114))
                    .Concat(new ushort[] { 5, 4, 3, 2 }).ToArray()
                },
            };
        }
    }
}