using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public abstract class WriteSingleHoldingRegisterTest : IntegrationTest
    {
        protected WriteSingleHoldingRegisterTest(string masterType, ITestOutputHelper output)
            : base(masterType, 6, output)
        {
            // Arrange
        }

        [Theory]
        [MemberData(nameof(GetWriteData))]
        [Trait("Category", "Integration")]
        public async Task WritesSuccessfully(ushort startAddress, ushort expected)
        {
            // Act
            await Target.WriteSingleRegisterAsync(SlaveId, startAddress, expected);
            var actual = await Target.ReadHoldingRegistersAsync(SlaveId, startAddress, 1);

            // Assert
            Assert.Equal(expected, actual[0]);
        }

        public static TheoryData<ushort, ushort> GetWriteData()
        {
            return new TheoryData<ushort, ushort>
            {
                { 0, 1000 },
                { 1, 2000 },
                { 3, 0 },
                { 5, ushort.MaxValue }
            };
        }
    }
}