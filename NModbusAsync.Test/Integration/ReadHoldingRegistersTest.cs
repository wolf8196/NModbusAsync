using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public class ReadHoldingRegistersTest : IntegrationTest
    {
        public ReadHoldingRegistersTest()
            : base(TcpMaster, 1)
        {
            // Arrange
        }

        [Theory]
        [MemberData((nameof(GetReadResults)))]
        [Trait("Category", "Integration")]
        public async Task ReadSuccessfully(ushort startAddress, ushort numberOfPoints, ushort[] expected)
        {
            // Act
            var actual = await Target.ReadHoldingRegistersAsync(SlaveId, startAddress, numberOfPoints);

            // Assert
            Assert.Equal(expected, actual);
        }

        public static TheoryData<ushort, ushort, ushort[]> GetReadResults()
        {
            return new TheoryData<ushort, ushort, ushort[]>
            {
                { 0, 1, new ushort[] { 10 } },
                { 4, 3, new ushort[] { 20, 30, 40 } },
                { 10, 125, Enumerable.Range(2, 125).Select(x => (ushort)x).ToArray() },
            };
        }
    }
}