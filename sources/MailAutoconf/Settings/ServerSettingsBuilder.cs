using MailAutoconf.Settings.Sources;

namespace MailAutoconf.Settings
{
	/// <summary>
	/// This class helps to retrieve the settings for the POP3, IMAP and SMTP server. It combines 
	/// the classes needed to get this information.
	/// 
	/// There are several sources for this information. Classes like AutoconfigXml, AutodiscoverXml
	/// etc. provide this information. They all implement IServerSettingsSource. These classes
	/// can be added with AddSource(). GetSettings() uses all of them to get the server settings.
	/// Some of these classes may be able to find the information, others may not. You may use
	/// the extension method .SelectBest() to extract the best settings from all results.
	/// 
	/// The sources are organized in groups. GetSettings() uses all sources of one group to get
	/// the mail server settings. If there is enough information, it returns the result. Otherwise 
	/// it moves on to the next group of sources and tries to get the information from there.
	/// The groups are separated by calling Evaluate(). Example:
	/// - Group 1
	///   - AddSource(new AutoconfigXml())
	///   - AddSource(new AutodiscoverXml())
	/// - Evaluate()
	/// -> Group 2
	///   - AddSource(PortChecker())
	/// </summary>
	public class ServerSettingsBuilder
	{
		/// <summary>
		/// the source groups - each entry is a group of sources
		/// </summary>
		private readonly List<ServerSettingsSourceGroup> sourceGroups = [];

		/// <summary>
		/// Creates an instance of this class. Prepares an empty list of sources (list of 
		/// IServerSettingsSource).
		/// </summary>
		public ServerSettingsBuilder()
		{
			Evaluate();
		}

		/// <summary>
		/// Adds a source (implementing IServerSettingsSource) to the list of sources.
		/// </summary>
		public ServerSettingsBuilder AddSource(IServerSettingsSource source)
		{
			sourceGroups.Last().Add(source);
			return this;
		}

		/// <summary>
		/// Returns some default sources (AutoconfigXml, AutodiscoverXml, IspDb) to the list of 
		/// sources.
		/// </summary>
		/// <param name="mailAddress">the mail address needed by the sources to get the mail 
		/// settings from online databases</param>
		/// <param name="timeoutMs">the timeout for the network connection in milliseconds</param>
		/// <returns></returns>
		public ServerSettingsBuilder AddDefaultSources(string mailAddress, int timeoutMs)
		{
			AddSource(new AutoconfigXml(mailAddress, timeoutMs));
			AddSource(new AutodiscoverXml(mailAddress, timeoutMs));
			AddSource(new IspDb(mailAddress, timeoutMs));
			return this;
		}

		/// <summary>
		/// Starts a new group of sources (implementing IServerSettingsSource).
		/// </summary>
		public ServerSettingsBuilder Evaluate()
		{
			sourceGroups.Add(new());
			return this;
		}

		/// <summary>
		/// Uses all added sources to get the mail server settings. Returns all of them as enumeration.
		/// </summary>
		public IEnumerable<ServerSettings> GetSettings()
		{
			ServerSettings serverSettings = null;
			List<ServerSettings> result = [];

			foreach (var sourceGroup in sourceGroups)
			{
				var groupResult = sourceGroup.GetSettings(serverSettings).ToArray();
				result.AddRange(groupResult);

				var bestSettings = result.SelectBest();
				if (bestSettings.EnoughForMailConfiguration) break;

				serverSettings = bestSettings;
			}

			return result.ToArray();
		}
	}
}