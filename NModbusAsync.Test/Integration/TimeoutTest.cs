using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public class TimeoutTest : IntergrationTest
    {
        public TimeoutTest()
            : base(9)
        {
            // Arrange
        }

        [Fact]
        [Trait("Category", "Intergration")]
        public async Task ThrowsOnTimeout()
        {
            // Arrange
            Target.Transport.Retries = 0;
            Target.Transport.ReadTimeout = 1000;

            // Act/Assert
            await Assert.ThrowsAsync<TimeoutException>(
                () => Target.ReadHoldingRegistersAsync(SlaveId, 1, 1));
        }
    }
}