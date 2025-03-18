# MailAutoconf

MailAutoconf detects settings for POP3, IMAP and SMTP servers starting with an email address. It searches various data sources as described in [Mail Autoconfig](https://benbucksch.github.io/autoconfig-spec/draft-ietf-mailmaint-autoconfig.html) or probes known email service network ports.

### Example

##### Detect settings for john.doe@yahoo.com

```c#
var serverSettings = new ServerSettingsFinder(mailAddress: "john.doe@yahoo.com")
	.GetSettings()
	.SelectBest();

Console.WriteLine($"POP3: {serverSettings.Pop3}");
Console.WriteLine($"IMAP: {serverSettings.Imap}");
Console.WriteLine($"SMTP: {serverSettings.Smtp}");
```

##### Output at console

```
POP3: pop.mail.yahoo.com, 995, SslTls, User: john.doe@yahoo.com, OAuth2, password-cleartext, Verification: Unverified
IMAP: imap.mail.yahoo.com, 993, SslTls, User: john.doe@yahoo.com, OAuth2, password-cleartext, Verification: Unverified
SMTP: smtp.mail.yahoo.com, 465, SslTls, User: john.doe@yahoo.com, OAuth2, password-cleartext, Verification: Unverified
```

### Use cases

##### Detect settings for john.doe@yahoo.com and verify user name and password

```C#
Credentials credentials = new()
{
	UserName = new() { Name = "john.doe@yahoo.com" },
	Password = "pass1"
};

var startSettings = new ServerSettings()
{
	Pop3 = new ProtocolSettings() { Credentials = credentials, },
	Imap = new ProtocolSettings() { Credentials = credentials, },
	Smtp = new ProtocolSettings() { Credentials = credentials, },
};

ServerSettings serverSettings
	= new ServerSettingsFinder(mailAddress: "john.doe@yahoo.com", startSettings)
		.GetSettings()
		.SelectBest();

PortVerifier.Verify(serverSettings);

Console.WriteLine($"POP3: {serverSettings.Pop3}");
Console.WriteLine($"IMAP: {serverSettings.Imap}");
Console.WriteLine($"SMTP: {serverSettings.Smtp}");
```

`PortVerifier.Verify()` verifies the server settings: It attempts to authenticate using the credentials provided and stores the result in `serverSettings.Pop3/Imap/Smtp.Verification`.

##### Email server names known, ports to be searched

The settings for known email service providers are stored in databases. If this information is missing, you may know the names of the email servers but you may not know which ports to use. MailAutoconf can detect them:

```C#
var startSettings = new ServerSettings()
{
	Pop3 = new ProtocolSettings() { Server = "pop.mail.yahoo.com", },
	Imap = new ProtocolSettings() { Server = "imap.mail.yahoo.com", },
	Smtp = new ProtocolSettings() { Server = "smtp.mail.yahoo.com", },
};

var serverSettings
	= new ServerSettingsFinder(mailAddress: "john.doe@yahoo.com", startSettings)
		.GetSettings()
    	.SelectBest();

Console.WriteLine($"POP3: {serverSettings.Pop3}");
Console.WriteLine($"IMAP: {serverSettings.Imap}");
Console.WriteLine($"SMTP: {serverSettings.Smtp}");
```

##### Probe email server ports

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

// Show result at console
static string PassedFailed(bool successful) => successful ? "passed" : "failed";

foreach (var result in portCheckResults)
{
	string prefix = "";
	if (result.ConnectSuccessful) prefix += ">";
	if (result.AuthenticateSuccessful) prefix += ">";

	string timeoutString = result.TimedOut ? ", timed out" : "";
	Console.WriteLine($"{prefix,2}{result.Protocol}, Port: {result.ProtocolSettings?.Port,4}, " +
		$"Mode: {result.ProtocolSettings?.SocketType,8}, " +
		$"connect: {PassedFailed(result.ConnectSuccessful)}, " +
		$"authenticate: {PassedFailed(result.AuthenticateSuccessful)}{timeoutString}");
}
```

### OAuth2

In the examples above with Yahoo, you may have noticed the `OAuth2` in the output. Big players started to force the users to [OAuth](https://en.wikipedia.org/wiki/OAuth). They argue with security. From the perspective of a developer who implements email clients, this makes authentication very complicated. He has to register and to ask for access with each of these players. Furthermore he has to dig through extensive specifications. Usually only large projects have the capacity to do this.

`MailAutoconf` is not able to authenticate with `OAuth2`, but it can probe the working email server ports and detect if `OAuth2` is required. `ProtocolSettings.Authentication` (e.g. serverSettings.Imap.Authentication) provides such information as string.

### Main classes

* `ServerSettingsFinder`: Finds server settings for an email address and optionally for some more information about the server settings. Usage see above
* [`ServerSettingsBuilder`](doc/ServerSettingsBuilder.md): The `ServerSettingsFinder` internally uses the `ServerSettingsBuilder`.  The builder chains some sources for email server settings.
* [`PortsChecker`](doc/PortsChecker.md): Probes email server ports

