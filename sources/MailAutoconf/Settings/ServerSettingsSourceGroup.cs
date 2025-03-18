using MailAutoconf.Settings.Sources;

namespace MailAutoconf.Settings
{
	/// <summary>
	/// This class contains a group of sources for server settings information. Add() adds sources and
	/// GetSettings() gets the settings from these sources by requesting them in parallel.
	/// </summary>
	public class ServerSettingsSourceGroup
	{
		private readonly List<IServerSettingsSource> sources = [];

		/// <summary>
		/// Adds the source (implementing IServerSettingsSource) to the internal list of sources.
		/// </summary>
		/// <param name="source"></param>
		public void Add(IServerSettingsSource source) => sources.Add(source);

		/// <summary>
		/// Gets and returns the server settings by requesting the added sources in parallel. Therefore
		/// the resulting settings may not be ordered in the order of the added sources.
		/// </summary>
		/// <param name="startSettings">the server settings already known (e.g. IMAP and SMTP server, 
		/// user name and password), but usually not complete (e.g. network port and socket type are 
		/// unknown or not yet verified), and therefore to be taken a start value and to be completed</param>
		/// <returns>the server settings found by requesting all sources</returns>
		public IEnumerable<ServerSettings> GetSettings(ServerSettings startSettings)
		{
			ServerSettings GetServerSettings(IServerSettingsSource source)
				=> source.GetServerSettings(startSettings);

			return sources
				.AsParallel()
				.Select(GetServerSettings);
		}
	}
}