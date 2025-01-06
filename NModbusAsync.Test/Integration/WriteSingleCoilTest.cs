using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration
{
    public abstract class WriteSingleCoilTest : IntegrationTest
    {
        protected WriteSingleCoilTest(string masterType, ITestOutputHelper output)
            : base(masterType, 5, output)
        {
            // Arrange
        }

        [Theory]
        [MemberData(nameof(GetWriteData))]
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