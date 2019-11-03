using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public class WriteMultipleRegistersTest : IntergrationTest
    {
        public WriteMultipleRegistersTest()
            : base(8)
        {
            // Arrange
        }

        [Theory]
        [MemberData(nameof(GetWriteData))]
        [Trait("Category", "Intergration")]
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
                { 10, Enumerable.Range(2, 123).Select(x => (ushort)x).ToArray()},
            };
        }
    }
}