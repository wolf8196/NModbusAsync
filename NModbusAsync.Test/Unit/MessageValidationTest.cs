using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NModbusAsync.Messages;
using Xunit;

namespace NModbusAsync.Test.Unit
{
    [ExcludeFromCodeCoverage]
    public class MessageValidationTest
    {
        [Theory]
        [MemberData(nameof(GetTestValidationData))]
        [Trait("Category", "Unit")]
        public void ValidatesResponse(IModbusRequest request, IModbusResponse response)
        {
            // Arrange/Act/Assert
            Assert.Throws<IOException>(() => request.Validate(response));
        }

        public static TheoryData<IModbusRequest, IModbusResponse> GetTestValidationData()
        {
            var theoryData = new TheoryData<IModbusRequest, IModbusResponse>
            {
                {
                    new ReadCoilsRequest(1, 1, 1),
                    ModbusResponseFactory.CreateResponse<ReadCoilsResponse>(new ReadOnlySpan<byte>(new byte[] { 2, 1, 1, 1 }))
                },
                {
                    new ReadCoilsRequest(1, 1, 1),
                    ModbusResponseFactory.CreateResponse<ReadCoilsResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 2, 1, 1 }))
                },
                {
                    new ReadCoilsRequest(1, 1, 9),
                    ModbusResponseFactory.CreateResponse<ReadCoilsResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 1, 1, 1 }))
                },

                {
                    new ReadInputsRequest(1, 1, 1),
                    ModbusResponseFactory.CreateResponse<ReadInputsResponse>(new ReadOnlySpan<byte>(new byte[] { 2, 1, 1, 1 }))
                },
                {
                    new ReadInputsRequest(1, 1, 1),
                    ModbusResponseFactory.CreateResponse<ReadInputsResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 1, 1, 1 }))
                },
                {
                    new ReadInputsRequest(1, 1, 9),
                    ModbusResponseFactory.CreateResponse<ReadInputsResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 1, 1, 1 }))
                },

                {
                    new ReadHoldingRegistersRequest(1, 1, 1),
                    ModbusResponseFactory.CreateResponse<ReadHoldingRegistersResponse>(new ReadOnlySpan<byte>(new byte[] { 2, 1, 1, 1 }))
                },
                {
                    new ReadHoldingRegistersRequest(1, 1, 1),
                    ModbusResponseFactory.CreateResponse<ReadHoldingRegistersResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 1, 1, 1 }))
                },
                {
                    new ReadHoldingRegistersRequest(1, 1, 2),
                    ModbusResponseFactory.CreateResponse<ReadHoldingRegistersResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 3, 2, 1, 1 }))
                },


                {
                    new ReadInputRegistersRequest(1, 1, 1),
                    ModbusResponseFactory.CreateResponse<ReadInputRegistersResponse>(new ReadOnlySpan<byte>(new byte[] { 2, 1, 1, 1 }))
                },
                {
                    new ReadInputRegistersRequest(1, 1, 1),
                    ModbusResponseFactory.CreateResponse<ReadInputRegistersResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 1, 1, 1 }))
                },
                {
                    new ReadInputRegistersRequest(1, 1, 2),
                    ModbusResponseFactory.CreateResponse<ReadInputRegistersResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 4, 2, 1, 1 }))
                },


                {
                    new WriteSingleCoilRequest(1, 1, true),
                    ModbusResponseFactory.CreateResponse<WriteSingleCoilResponse>(new ReadOnlySpan<byte>(new byte[] { 2, 1, 0, 1, byte.MaxValue, 0 }))
                },
                {
                    new WriteSingleCoilRequest(1, 1, true),
                    ModbusResponseFactory.CreateResponse<WriteSingleCoilResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 1, 0, 1, byte.MaxValue, 0 }))
                },
                {
                    new WriteSingleCoilRequest(1, 1, true),
                    ModbusResponseFactory.CreateResponse<WriteSingleCoilResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 5, 0, 2, byte.MaxValue, 0 }))
                },
                {
                    new WriteSingleCoilRequest(1, 1, false),
                    ModbusResponseFactory.CreateResponse<WriteSingleCoilResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 5, 0, 1, byte.MaxValue, 0 }))
                },
                {
                    new WriteSingleCoilRequest(1, 1, true),
                    ModbusResponseFactory.CreateResponse<WriteSingleCoilResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 5, 0, 1, 0, 0 }))
                },
                
                {
                    new WriteSingleRegisterRequest(1, 1, ushort.MaxValue),
                    ModbusResponseFactory.CreateResponse<WriteSingleRegisterResponse>(new ReadOnlySpan<byte>(new byte[] { 2, 1, 0, 1, 0, 0 }))
                },
                {
                    new WriteSingleRegisterRequest(1, 1, ushort.MaxValue),
                    ModbusResponseFactory.CreateResponse<WriteSingleRegisterResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 1, 0, 1, 0, 0 }))
                },
                {
                    new WriteSingleRegisterRequest(1, 1, ushort.MaxValue),
                    ModbusResponseFactory.CreateResponse<WriteSingleRegisterResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 6, 0, 2, 0, 0 }))
                },
                {
                    new WriteSingleRegisterRequest(1, 1, ushort.MaxValue),
                    ModbusResponseFactory.CreateResponse<WriteSingleRegisterResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 6, 0, 1, 0, 0 }))
                },
                
                {
                    new WriteMultipleCoilsRequest(1, 1, new bool[] { true }),
                    ModbusResponseFactory.CreateResponse<WriteMultipleCoilsResponse>(new ReadOnlySpan<byte>(new byte[] { 2, 1, 0, 1, 0, 1 }))
                },
                {
                    new WriteMultipleCoilsRequest(1, 1, new bool[] { true }),
                    ModbusResponseFactory.CreateResponse<WriteMultipleCoilsResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 1, 0, 1, 0, 1 }))
                },
                {
                    new WriteMultipleCoilsRequest(1, 1, new bool[] { true }),
                    ModbusResponseFactory.CreateResponse<WriteMultipleCoilsResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 15, 0, 2, 0, 1 }))
                },
                {
                    new WriteMultipleCoilsRequest(1, 1, new bool[] { true }),
                    ModbusResponseFactory.CreateResponse<WriteMultipleCoilsResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 15, 0, 1, 0, 2 }))
                },

                {
                    new WriteMultipleRegistersRequest(1, 1, new ushort[] { ushort.MaxValue }),
                    ModbusResponseFactory.CreateResponse<WriteMultipleRegistersResponse>(new ReadOnlySpan<byte>(new byte[] { 2, 1, 0, 1, 0, 1 }))
                },
                {
                    new WriteMultipleRegistersRequest(1, 1, new ushort[] { ushort.MaxValue }),
                    ModbusResponseFactory.CreateResponse<WriteMultipleRegistersResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 1, 0, 1, 0, 1 }))
                },
                {
                    new WriteMultipleRegistersRequest(1, 1, new ushort[] { ushort.MaxValue }),
                    ModbusResponseFactory.CreateResponse<WriteMultipleRegistersResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 16, 0, 2, 0, 1 }))
                },
                {
                    new WriteMultipleRegistersRequest(1, 1, new ushort[] { ushort.MaxValue }),
                    ModbusResponseFactory.CreateResponse<WriteMultipleRegistersResponse>(new ReadOnlySpan<byte>(new byte[] { 1, 16, 0, 1, 0, 2 }))
                }
            };

            return theoryData;
        }
    }
}