using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public abstract class TimeoutTest : IntegrationTest
    {
        public TimeoutTest(string masterType, ITestOutputHelper output)
            : base(masterType, 9, output)
        {
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ThrowsOnTimeout()
        {
            // Arrange
            Target.Transport.Retries = 0;
            Target.Transport.ReadTimeout = 1000;

            // Act/Assert
            await Assert.ThrowsAsync<TimeoutException>(
                () => Target.ReadHoldingRegistersAsync(SlaveId, 0, 1));

            // simulator is seamingly single threaded
            // wait for it to not affect next tests
            await Task.Delay(3000);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ReadsDelayedResponse()
        {
            // Arrange
            Target.Transport.Retries = 0;
            Target.Transport.ReadTimeout = 4500;

            // Act
            var actual = await Target.ReadHoldingRegistersAsync(SlaveId, 0, 1);

            // Assert
            Assert.Single(actual);
            Assert.Equal(10, actual.First());
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ReadsSuccessfullyAfterTimeoutFromInvalidSlave()
        {
            // Arrange
            byte validSlave = 2;
            byte invalidSlave = 10;

            Target.Transport.Retries = 0;
            Target.Transport.ReadTimeout = 1000;

            // Act
            await Assert.ThrowsAsync<TimeoutException>(
                () => Target.ReadHoldingRegistersAsync(invalidSlave, 0, 1));

            var actual = await Target.ReadInputRegistersAsync(validSlave, 0, 1);

            // Assert
            Assert.Single(actual);
            Assert.Equal(100, actual.First());
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ReadsSuccessfullyAfterTimeoutFromDelayedSlave()
        {
            // Arrange
            byte validSlave = 2;

            Target.Transport.Retries = 0;
            Target.Transport.ReadTimeout = 1000;
            Target.Transport.RetryOnOldTransactionIdThreshold = 3;
            Target.Transport.RetryOnInvalidResponseCount = 3;

            // Act
            await Assert.ThrowsAsync<TimeoutException>(
                () => Target.ReadHoldingRegistersAsync(SlaveId, 0, 1));

            // simulator is seamingly single threaded
            // wait for it to not affect next tests
            await Task.Delay(3000);

            var actual = await Target.ReadInputRegistersAsync(validSlave, 0, 1);

            // Assert
            Assert.Single(actual);
            Assert.Equal(100, actual.First());
        }
    }
}