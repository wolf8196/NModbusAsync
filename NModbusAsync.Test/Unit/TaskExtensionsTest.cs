using System;
using System.Threading;
using System.Threading.Tasks;
using NModbusAsync.Utility;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    public class TaskExtensionsTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ReturnsFalseOnTimeoutAsync()
        {
            // Arrange/Act
            var waitResult = await Task.Run(() =>
            {
                while (true)
                {
                }
            }).WaitAsync(200);

            // Assert
            Assert.False(waitResult);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ReturnsTrueOnSuccess()
        {
            // Arrange/Act
            var waitResult = await Task.Run(() =>
            {
            }).WaitAsync(200);

            // Assert
            Assert.True(waitResult);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowsException()
        {
            // Arrange/Act
            var ex = await Assert.ThrowsAsync<AggregateException>(async () =>
             {
                 var waitResult = await Task.Run(() =>
                 {
                     throw new Exception("Test");
                 }).WaitAsync(10000);
             });

            // Assert
            Assert.Single(ex.InnerExceptions);
            Assert.Equal("Test", ex.InnerExceptions[0].Message);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ThrowsOnCancellation()
        {
            // Arrange/Act/Assert
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(100);
            var ex = await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                var waitResult = await Task.Run(() =>
                {
                    while (true)
                    {
                    }
                }).WaitAsync(10000, cts.Token);
            });
        }
    }
}