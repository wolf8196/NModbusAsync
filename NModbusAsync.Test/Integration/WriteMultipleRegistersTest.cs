using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration
{
    public abstract class WriteMultipleRegistersTest : IntegrationTest
    {
        protected WriteMultipleRegistersTest(string masterType, ITestOutputHelper output)
            : base(masterType, 8, output)
        {
            // Arrange
        }

        [Theory]
        [MemberData(nameof(GetWriteData))]
        [Trait("Category", "Integration")]
        public async Task WritesSuccessfully(ushort startAddress, ushort[] expected)
        {
            // Act
            await Target.WriteMultipleRegistersAsync(SlaveId, startAddress, expected);
            var actual = await Target.ReadHoldingRegistersAsync(SlaveId, startAddress, (ushort)expected.Length);

            // Assert
            Assert.Equal(expected, actual);
        }

        public static TheoryData<ushort, ushort[]> GetWriteData()
        {
            return new TheoryData<ushort, ushort[]>
            {
                { 0, new ushort[] { 10 } },
                { 4, new ushort[] { 20, 30, 40 } },
                { 10, Enumerable.Range(2, 123).Select(x => (ushort)x).ToArray() },
            };
        }
    }
}