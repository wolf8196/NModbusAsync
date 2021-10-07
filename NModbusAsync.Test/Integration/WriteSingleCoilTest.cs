using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public class WriteSingleCoilTest : IntegrationTest
    {
        public WriteSingleCoilTest()
            : base(TcpMaster, 5)
        {
            // Arrange
        }

        [Theory]
        [MemberData((nameof(GetWriteData)))]
        [Trait("Category", "Integration")]
        public async Task WritesSuccessfully(ushort startAddress, bool expected)
        {
            // Act
            await Target.WriteSingleCoilAsync(SlaveId, startAddress, expected);
            var actual = await Target.ReadCoilsAsync(SlaveId, startAddress, 1);

            // Assert
            Assert.Equal(expected, actual[0]);
        }

        public static TheoryData<ushort, bool> GetWriteData()
        {
            return new TheoryData<ushort, bool>
            {
                { 0, true },
                { 1, false },
                { 3, false },
                { 5, true }
            };
        }
    }
}