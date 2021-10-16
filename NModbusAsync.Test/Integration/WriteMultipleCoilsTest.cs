using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public abstract class WriteMultipleCoilsTest : IntegrationTest
    {
        protected WriteMultipleCoilsTest(string masterType)
            : base(masterType, 7)
        {
            // Arrange
        }

        [Theory]
        [MemberData(nameof(GetWriteData))]
        [Trait("Category", "Integration")]
        public async Task WritesSuccessfully(ushort startAddress, bool[] expected)
        {
            // Act
            await Target.WriteMultipleCoilsAsync(SlaveId, startAddress, expected);
            var actual = await Target.ReadCoilsAsync(SlaveId, startAddress, (ushort)expected.Length);

            // Assert
            Assert.Equal(expected, actual);
        }

        public static TheoryData<ushort, bool[]> GetWriteData()
        {
            return new TheoryData<ushort, bool[]>
            {
                { 0, new bool[] { true } },
                { 1, new bool[] { false } },
                { 6, new bool[] { true, true, false, true, false, true, false, false } },
                {
                    20, Enumerable.Repeat(true, 200)
                        .Concat(Enumerable.Repeat(false, 100))
                        .Concat(Enumerable.Repeat(true, 500))
                        .Concat(Enumerable.Repeat(false, 300))
                        .Concat(Enumerable.Repeat(true, 200))
                        .Concat(Enumerable.Repeat(false, 150))
                        .Concat(Enumerable.Repeat(false, 250))
                        .Concat(Enumerable.Repeat(true, 268))
                        .ToArray()
                }
            };
        }
    }
}