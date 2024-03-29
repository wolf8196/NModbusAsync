﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging.Abstractions;
using NModbusAsync.Test.Helpers;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public abstract class IntegrationTest
    {
        public const string TcpMaster = "Tcp";
        public const string RtuOverTpcMaster = "RtuOverTcp";

        protected IntegrationTest(string masterType, byte slaveId)
        {
            MasterType = masterType;
            SlaveId = slaveId;
            var tcpClient = new TcpClient();

            switch (MasterType)
            {
                case TcpMaster:
                    {
                        tcpClient.Connect(IPAddress.Loopback, 502);
                        Target = new ModbusFactory(NullLogger<IModbusMaster>.Instance).CreateTcpMaster(tcpClient);
                        break;
                    }

                case RtuOverTpcMaster:
                    {
                        tcpClient.Connect(IPAddress.Loopback, 503);
                        Target = new ModbusFactory(NullLogger<IModbusMaster>.Instance).CreateRtuOverTcpMaster(tcpClient);
                        break;
                    }

                default:
                    throw new NotImplementedException($"Master type is not implemented. Master type: {MasterType}");
            }
        }

        public string MasterType { get; }

        protected byte SlaveId { get; }

        protected IModbusMaster Target { get; }
    }
}