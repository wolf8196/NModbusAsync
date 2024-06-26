﻿using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace NModbusAsync.Test.Integration.Tcp
{
    [ExcludeFromCodeCoverage]
    public class TcpReadInputsTest : ReadInputsTest
    {
        public TcpReadInputsTest(ITestOutputHelper output)
        : base(TcpMaster, output)
        {
        }
    }
}