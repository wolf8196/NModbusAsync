using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public class WriteSingleHoldingRegisterTest : IntergrationTest
    {
        public WriteSingleHoldingRegisterTest()
            : base(6)
        {
            // Arrange
        }

        [Theory]
        [MemberData((nameof(GetWriteData)))]
        [Trait("Category", "Intergration")]
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