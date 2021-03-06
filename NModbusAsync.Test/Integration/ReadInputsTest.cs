﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public class ReadInputsTest : IntergrationTest
    {
        public ReadInputsTest()
            : base(4)
        {
            // Arrange
        }

        [Theory]
        [MemberData((nameof(GetReadResults)))]
        [Trait("Category", "Intergration")]
        public async Task ReadSuccessfully(ushort startAddress, ushort numberOfPoints, bool[] expected)
        {
            // Act
            var result = await Target.ReadInputsAsync(SlaveId, startAddress, numberOfPoints);

            // Assert
            Assert.Equal(expected, result);
        }

        public static TheoryData<ushort, ushort, bool[]> GetReadResults()
        {
            return new TheoryData<ushort, ushort, bool[]>
            {
                { 0, 1, new bool[] { true } },
                { 1, 1, new bool[] { false } },
                { 6, 8, new bool[] { true, true, false, true, false, true, false, false } }
            };
        }
    }
}