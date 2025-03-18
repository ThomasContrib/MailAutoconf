### ServerSettingsBuilder

The `ServerSettingsBuilder` chains some sources for email server settings.

```C#
var portCheckSource = new PortCheckSource(
	new PortsChecker(timeoutMs)
		.AddPorts(PortsChecker.DefaultPop3ImapSmtpPorts));

IEnumerable<ServerSettings> allServerSettings = new ServerSettingsBuilder()

	.AddSource(new StaticSource(startSettings))
	.Evaluate()

	.AddSource(portCheckSource)
	.Evaluate()

	// GetSettings() will evaluate the static source above first. If there 
    // is enough information, GetSettings() will return it. Otherwise it
    // requests the default sources below (AutoconfigXml, AutodiscoverXml, 
    // IspDb) to get the required information.
	.AddDefaultSources(mailAddress, timeoutMs)
	.Evaluate()

	// If the sources above did not deliver the required information, 
    // GetSettings() will probe the usual server ports. This happens when 
    // the mail server is not listed in any of the public databases. You 
    // have to provide the server names in this case and the PortCheckSource 
    // searches for working server ports.
	.AddSource(portCheckSource)

	.GetSettings();
```

This example shows how the `ServerSettingsFinder` uses the `ServerSettingsBuilder`. The builder uses a chain of sources that can be requested for server settings. Each source gets some start values for the server settings and tries to improve them. 

The example can be read as follows: If the user calls `GetSettings()`

* Use some start values (`startSettings`) .
* Check if they contain enough information for working server settings. If yes, stop processing (`Evaluate()`) and return these settings.
* Use a PortsChecker to probe for working email server ports.
* Check if they contain enough information for working server settings. If yes, stop processing (`Evaluate()`) and return these settings.
* Request some public databases (AutoconfigXML, AutodiscoverXML, ISPDB) for server settings.
* Check if they contain enough information for working server settings. If yes, stop processing (`Evaluate()`) and return these settings.
* Use the PortsChecker again to complete the information gathered with working email server ports if they are still missing.
* Return these settings.

The result is an enumeration of server settings from the different sources. They have to be combined now to a best set of settings:

```C#
ServerSettings serverSettings = allServerSettings.SelectBest();
```

### Data Sources

There are various data sources like the ISPDB, Autoconfig or Exchange AutoDiscover. Add the ones you need to the `ServerSettingsBuilder`:

```c#
var serverSettings = new ServerSettingsBuilder()
	.AddSource(new AutoconfigXml(mailAddress, timeoutMs))
	.AddSource(new AutodiscoverXml(mailAddress, timeoutMs))
	.AddSource(new IspDb(mailAddress, timeoutMs));
```

`AddDefaultSources()` adds them all at once.

##### Custom data source

You may want to add a custom data source. Implement `IServerSettingsSource` and add the class as additional (or only) source:

```C#
class CustomSource : IServerSettingsSource
{
    ...
}

var serverSettings = new ServerSettingsBuilder()
    .AddSource(new CustomSource(mailAddress, timeoutMs))
	.AddSource(new AutoconfigXml(mailAddress, timeoutMs))
	.AddSource(new AutodiscoverXml(mailAddress, timeoutMs))
	.AddSource(new IspDb(mailAddress, timeoutMs));
```

This custom data source may try to complete email server names for email addresses not stored in any of the public databases. 

Example: john.doe@company.com

Your custom class may try to connect to these servers just by guessing their names:

* `pop.company.com`
* `imap.company.com`
* `smtp.company.com`