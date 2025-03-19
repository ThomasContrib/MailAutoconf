### PortsChecker

The `PortsChecker` probes email server ports.

##### Probe standard ports

```C#
var serverSettings = new ServerSettings()
{
	Pop3 = new ProtocolSettings() { Server = "pop.mail.yahoo.com", },
	Imap = new ProtocolSettings() { Server = "imap.mail.yahoo.com", },
	Smtp = new ProtocolSettings() { Server = "smtp.mail.yahoo.com", },
};

var portCheckResults = new PortsChecker(timeoutMs: 2000)
	.AddPorts(PortsChecker.DefaultPop3ImapSmtpPorts)
	.CheckPorts(serverSettings);
```

The result is an array of port check results - one result per probed port.

##### Probe other ports

```C#
var portCheckResults = new PortsChecker(timeoutMs: 2000)
	.AddPorts(PortsChecker.PortsFrom(serverSettings))
	.CheckPorts(serverSettings);
```

only probes the ports used in the passed `serverSettings`.

```C#
var portCheckResults = new PortsChecker(timeoutMs: 2000)
	.AddPort(new ImapPortChecker(port: 100, [ SocketType.SslTls ]))
	.CheckPorts(serverSettings);
```

only probes port 100 using SSL/TLS for an IMAP server.

##### Filters

```C#
var portsChecker = new PortsChecker(timeoutMs)
	.AddPorts(PortsChecker.PortsFrom(serverSettings));

ProtocolSettingsFilter filter = forceVerification
	? ProtocolSettingsFilter.None
	: ProtocolSettingsFilter.UnverifiedOnly;

portsChecker.CheckPorts(serverSettings, filter, verify: true);
```

Port probing takes some time. Filters can be used to prevent from probes that are not required. The example above shows, how the `PortVerifier` is implemented. If the verification has not to be forced for all server settings, it probes the ports only for settings that have not yet been verified. There are some predefined filters and you may also combine them:

```C#
filter = ProtocolSettingsFilter.UnverifiedOnly;
```

uses a filter that only accepts protocol settings with `Verification == Unverified`.

```
filter = ProtocolSettingsFilter.WithUserOnly;
```

uses a filter that only accepts procotol settings with user name and password set.

```C#
filter = new ProtocolSettingsFilter()
	.And(ProtocolSettingsFilter.UnverifiedOnly)
	.And(ProtocolSettingsFilter.WithUserOnly)
```

uses a filter that only accepts protocol settings with both `Verification == Unverified` AND „user name and password set”.

```C#
filter = new ProtocolSettingsFilter()
			.And(protocolSettings => protocolSettings.SocketType == SocketType.Plain);
```

uses a custom filter that only accepts protocol settings for unencrypted network connections.

## PortVerifier

The `PortVerifier` verifies the mail server settings. `Verify()` attempts to authenticate using the credentials provided and stores the result in `serverSettings.Pop3/Imap/Smtp.Verification`.

This class internally uses the `PortsChecker`, see above. `Verify()` returns the port check results from the PortsChecker used. You may use them together with other port check results (`PortCheckResult[]`):

```C#
var serverSettings = new ServerSettingsFinder(mailAddress: "john.doe@yahoo.com")
	.GetSettings()
	.SelectBest();

var verifyResults = PortVerifier.Verify(serverSettings, forceVerification: false);

var compareProtocolPortSocketType = 
    EqualityComparer<PortCheckResult>.Create((x, y) =>
                                             
        x.Protocol == y.Protocol &&
        x.ProtocolSettings.Port == y.ProtocolSettings.Port &&
        x.ProtocolSettings.SocketType == y.ProtocolSettings.SocketType,
                                             
        obj => HashCode.Combine(
            obj.Protocol, 
            obj.ProtocolSettings.Port, 
            obj.ProtocolSettings.SocketType));

var portCheckResults = PortCheckResults.From(allServerSettings)
	.Concat(verifyResults)
	.Distinct(compareProtocolPortSocketType);
```

