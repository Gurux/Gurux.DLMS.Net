# Gurux DLMS Console Client

A command-line application for reading DLMS/COSEM electricity, gas and water meters over a network or serial connection.

## Requirements

- .NET 10 SDK.
- The connection settings, addresses and credentials required by your meter.

Different meters use different settings. Every value must match what the meter expects. Even one incorrect value can cause the connection to fail or the meter to send no response, resulting in a timeout error.

Run the following commands from this directory. Replace the example settings with those for your meter.

## Build and help

```powershell
dotnet build Gurux.DLMS.Client.Example.Net.csproj
dotnet run --project Gurux.DLMS.Client.Example.Net.csproj -- --help
```

The build restores the required NuGet packages automatically. Arguments after `--` are passed to the application.

## Connect and read

### Network (TCP, WRAPPER)

```powershell
dotnet run --project Gurux.DLMS.Client.Example.Net.csproj -- -h localhost -p 4063 -i WRAPPER -c 1 -s 1 -a Low -P Gurux
```

### Serial port (HDLC)

```powershell
dotnet run --project Gurux.DLMS.Client.Example.Net.csproj -- -S COM1:9600:8None1 -i HDLC -c 16 -s 1 -a Low -P password
```

For an optical connection using IEC Mode E, use `-i HdlcWithModeE` and the serial settings required by your meter.

Not all meters support `HdlcWithModeE`. If your meter does not support it, use `-i HDLC` even when connecting with an optical probe.

### Read specific attributes

Use `-g` with an OBIS logical name and attribute index. Quote multiple entries and separate them with semicolons:

```powershell
dotnet run --project Gurux.DLMS.Client.Example.Net.csproj -- -h localhost -p 4063 -i WRAPPER -c 1 -a Low -P Gurux -g "0.0.1.0.0.255:2;1.0.1.8.0.255:2"
```

These example entries read the clock and an energy register if the meter exposes them. Without `-g`, the application attempts to read all available readable attributes.

## Common options

Options are case-sensitive; for example, `-p` is the network port and `-P` is the password.

| Option | Description |
| --- | --- |
| `-h HOST -p PORT` | Network host and port. TCP is used unless `-u` selects UDP. |
| `-S PORT[:BAUD:FRAMING]` | Serial port, for example `COM1:9600:8None1`. |
| `-i TYPE` | Interface, for example `HDLC`, `WRAPPER` or `HdlcWithModeE`. |
| `-c ADDRESS -s ADDRESS` | Client and server addresses (defaults: 16 and 1). |
| `-r ln` / `-r sn` | Logical name or short name referencing (default: `ln`). |
| `-a TYPE` | Authentication, for example `None`, `Low` or `HighGMAC`. |
| `-P PASSWORD` | Authentication password; use `0x` for hexadecimal bytes. |
| `-g "LN:INDEX;LN:INDEX"` | Read only the selected attributes. |
| `-o PATH` | Load/save the XML association cache and read results. Use a separate file for each meter and configuration. |
| `-x MS` | Connection/response timeout in milliseconds (default: 5000). |
| `-z MS` | Delay after opening the connection (default: 1000); `0` disables it. |
| `-t Verbose` | Show detailed communication traces. |

Secured meters may also require `-C`, `-V`, `-T`, `-A`, `-B` and invocation-counter settings (`-F` or `-v`). Use `--help` for the full option list and configure these values according to your meter's requirements.

## Reading and troubleshooting

Always let the application finish reading all requested data and close the connection normally. Some meters do not recover properly if reading is interrupted, which can cause the next connection attempt to fail.

If a connection fails, check the host/port or serial settings, interface type, client/server addresses and authentication settings. For timeouts, try a larger value such as `-x 10000`. Use `-t Verbose` for diagnostics.

Exit codes:

- `0`: completed successfully or displayed help.
- `1`: configuration or operation failed.
- `130`: operation canceled.

Warnings are written to standard error. Cache failures and skipped individual attribute reads may produce warnings even when a broad read completes successfully.

## Development and support

For protocol documentation and support, see the [Gurux website](https://www.gurux.org/) and [Gurux forum](https://www.gurux.org/forum).
