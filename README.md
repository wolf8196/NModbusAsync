# NModbusAsync
NModbusAsync is a striped to the bone version of NModbus library.

The only thing that left is Modbus over TCP and Modbus RTU over TPC masters.

The main difference and advantage of NModbusAsync is that all operations now  utilize only asynchronous I/O.

## Prerequisites
.NET Standart 2.1

## Usage examples

```
// create & connect your TCP client (or something derived from it)
// you are responsible for managing it
var tcpClient = new TcpClient();
tcpClient.Connect(IPAddress.Loopback, 502);

// create master factory
var masterFactory = new ModbusFactory(); // optional Microsoft logger can be passed

// call CreateTcpMaster or CreateRtuOverTcpMaster to create the master
var master = masterFactory.CreateTcpMaster(tcpClient); // optional Microsoft logger can be passed as well

// call methods on modbus master to read/write from coils/registers
var actual = await master.ReadInputRegistersAsync(slaveAddress: 1, startAddress: 1, numberOfPoints: 2, token: default);

// a bit hacky way to get your tcpClient back from master
var resource = (master.Transport.PipeResource as IPipeResource<TcpClient>)?.Resource;

// configuring communication settings
// detailed explanation in xml comments
modbusMaster.Transport.ReadTimeout = ReadTimeout;
modbusMaster.Transport.WriteTimeout = WriteTimeout;
modbusMaster.Transport.RetryOnOldTransactionIdThreshold = RetryOnOldTransactionIdThreshold; // tcp only
modbusMaster.Transport.RetryOnInvalidResponseCount = RetryOnInvalidResponseCount; // rtu only
modbusMaster.Transport.SlaveBusyUsesRetryCount = SlaveBusyUsesRetryCount;
modbusMaster.Transport.WaitToRetryMilliseconds = WaitToRetryMilliseconds;
modbusMaster.Transport.Retries = Retries;
```

## Acknowledgments
Most of the code was originally written by the authors of [NModbus](https://github.com/NModbus/NModbus) library.

## License
This project is licensed under the MIT License - see the [LICENSE](https://github.com/wolf8196/NModbusAsync/blob/master/LICENSE) file for details.
