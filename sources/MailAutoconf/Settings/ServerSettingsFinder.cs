using MailAutoconf.PortCheck;
using MailAutoconf.Settings.Sources;

namespace MailAutoconf.Settings
{
	/// <summary>
	/// This class provides mail server settings for a mail address. It uses a default configuration 
	/// of the ServerSettingsBuilder to get this information.
	/// </summary>
	/// <param name="mailAddress">the mail address for which the server settings have to be searched</param>
	/// <param name="startSettings">some start values for searching the server settings</param>
	/// <param name="timeoutMs">the timeout for the network connections in milliseconds (default: 2000 ms)</param>
	public class ServerSettingsFinder(string mailAddress, ServerSettings startSettings = null, 
		int timeoutMs = 2000)
    {
		/// <summary>
		/// Returns the server settings for the mail address passed to the constructor.
		/// </summary>
		public IEnumerable<ServerSettings> GetSettings()
		{
			var portCheckSource = new PortCheckSource(
				new PortsChecker(timeoutMs)
					.AddPorts(PortsChecker.DefaultPop3ImapSmtpPorts));

			var allServerSettings = new ServerSettingsBuilder()

				.AddSource(new StaticSource(startSettings))
				.Evaluate()

				.AddSource(portCheckSource)
				.Evaluate()

				// GetSettings() will evaluate the static source above first. If there is enough information,
				// GetSettings() will return it. Otherwise it requests the default sources below (AutoconfigXml,
				// AutodiscoverXml, IspDb) to get the required information.
				.AddDefaultSources(mailAddress, timeoutMs)
				.Evaluate()

				// If the sources above did not deliver the required information, GetSettings() will probe the
				// usual server ports. This happens when the mail server is not listed in any of the public
				// databases. You have to provide the server names in this case and the PortCheckSource searches
				// for working server ports.
				.AddSource(portCheckSource)

				.GetSettings();

			return allServerSettings;
		}
	}
}
