using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration
{
    [ExcludeFromCodeCoverage]
    public abstract class IntegrationTest : IDisposable
    {
        public const string TcpMaster = "Tcp";
        public const string RtuOverTcpMaster = "RtuOverTcp";

        protected IntegrationTest(string masterType, byte slaveId, ITestOutputHelper output)
        {
            MasterType = masterType;
            SlaveId = slaveId;
            var tcpClient = new TcpClient
            {
                LingerState = new LingerOption(true, 0),
                NoDelay = true,
            };

            Logger = CreateLogger(output);

            switch (MasterType)
            {
                case TcpMaster:
                    {
                        tcpClient.Connect(IPAddress.Loopback, 502);
                        Target = new ModbusFactory(Logger).CreateTcpMaster(tcpClient);
                        break;
                    }

                case RtuOverTcpMaster:
                    {
                        tcpClient.Connect(IPAddress.Loopback, 503);
                        Target = new ModbusFactory(Logger).CreateRtuOverTcpMaster(tcpClient);
                        break;
                    }

                default:
                    throw new NotImplementedException($"Master type is not implemented. Master type: {MasterType}");
            }
        }

        public string MasterType { get; }

        protected byte SlaveId { get; }

        protected IModbusMaster Target { get; }

        protected ILogger<IModbusMaster> Logger { get; }

        public void Dispose()
        {
            Target.Dispose();
        }

        private ILogger<IModbusMaster> CreateLogger(ITestOutputHelper output)
        {
            var serilogLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(
                output,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            return new SerilogLoggerFactory(serilogLogger)
                .CreateLogger<IModbusMaster>();
        }
    }
}