using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public abstract class ReadCoilsTest : IntegrationTest
    {
        protected ReadCoilsTest(string masterType)
            : base(masterType, 3)
        {
            // Arrange
        }

        [Theory]
        [MemberData((nameof(GetReadResults)))]
        [Trait("Category", "Integration")]
        public async Task ReadsSuccessfully(ushort startAddress, ushort numberOfPoints, bool[] expected)
        {
            // Act
            var actual = await Target.ReadCoilsAsync(SlaveId, startAddress, numberOfPoints);

            // Assert
            Assert.Equal(expected, actual);
        }

        public static TheoryData<ushort, ushort, bool[]> GetReadResults()
        {
            return new TheoryData<ushort, ushort, bool[]>
            {
                { 0, 1, new bool[] { true } },
                { 1, 1, new bool[] { false } },
                { 6, 8, new bool[] { true, true, false, true, false, true, false, false } },
                { 20, 2000, Enumerable.Repeat(true, 200)
                    .Concat(Enumerable.Repeat(false, 100))
                    .Concat(Enumerable.Repeat(true, 500))
                    .Concat(Enumerable.Repeat(false, 300))
                    .Concat(Enumerable.Repeat(true, 200))
                    .Concat(Enumerable.Repeat(false, 150))
                    .Concat(Enumerable.Repeat(false, 250))
                    .Concat(Enumerable.Repeat(true, 300)).ToArray() }
            };
        }
    }
}