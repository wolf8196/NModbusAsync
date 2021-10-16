using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using NModbusAsync.Utility;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    [ExcludeFromCodeCoverage]
    public class ReadOnlySequenceExtensionsTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void ReturnsSpanWithoutCopyingOnSingleSegment()
        {
            // Arrange
            var array = new byte[5] { 1, 2, 3, 4, 5 };
            var sequence = new ReadOnlySequence<byte>(array);

            // Act
            var span = sequence.Slice(3, 2).ToSpan();
            array[4] = byte.MaxValue;

            // Assert
            Assert.Equal(array[4], span[1]);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReturnsCopiedSpanOnTwoSegmentsIntersection()
        {
            // Arrange
            var array1 = new byte[5] { 1, 2, 3, 4, 5 };
            var array2 = new byte[5] { 6, 7, 8, 9, 10 };
            var segment1 = new ReadOnlySequenceSegmentMock<byte>(array1);
            var segment2 = new ReadOnlySequenceSegmentMock<byte>(array2);
            segment1.SetNext(segment2);
            var sequence = new ReadOnlySequence<byte>(segment1, 0, segment2, 4);

            // Act
            var span = sequence.Slice(4, 2).ToSpan();
            array1[4] = byte.MaxValue;

            // Assert
            Assert.Equal(5, span[0]);
            Assert.Equal(byte.MaxValue, array1[4]);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReturnsMemoryWithoutCopyingOnSingleSegment()
        {
            // Arrange
            var array = new byte[5] { 1, 2, 3, 4, 5 };
            var sequence = new ReadOnlySequence<byte>(array);

            // Act
            var memory = sequence.Slice(3, 2).ToMemory();
            array[4] = byte.MaxValue;

            // Assert
            Assert.Equal(array[4], memory.Span[1]);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void ReturnsCopiedMemoryOnTwoSegmentsIntersection()
        {
            // Arrange
            var array1 = new byte[5] { 1, 2, 3, 4, 5 };
            var array2 = new byte[5] { 6, 7, 8, 9, 10 };
            var segment1 = new ReadOnlySequenceSegmentMock<byte>(array1);
            var segment2 = new ReadOnlySequenceSegmentMock<byte>(array2);
            segment1.SetNext(segment2);
            var sequence = new ReadOnlySequence<byte>(segment1, 0, segment2, 4);

            // Act
            var memory = sequence.Slice(4, 2).ToMemory();
            array1[4] = byte.MaxValue;

            // Assert
            Assert.Equal(5, memory.Span[0]);
            Assert.Equal(byte.MaxValue, array1[4]);
        }

        private class ReadOnlySequenceSegmentMock<T> : ReadOnlySequenceSegment<T>
        {
            public ReadOnlySequenceSegmentMock(T[] array)
            {
                Memory = new System.ReadOnlyMemory<T>(array);
                RunningIndex = 0;
            }

            public int Length => Memory.Length;

            public void SetNext(ReadOnlySequenceSegmentMock<T> next)
            {
                Next = next;
                next.RunningIndex = RunningIndex + Length;
            }
        }
    }
}