namespace MailAutoconf.Settings.Sources
{
	/// <summary>
	/// This class provides the server settings as passed to the constructor without any changes.
	/// </summary>
	/// <param name="serverSettings">the server settings to be returned by GetServerSettings()</param>
	public class StaticSource(ServerSettings serverSettings) : IServerSettingsSource
	{
		/// <summary>
		/// Returns the server settings passed to the constructor.
		/// </summary>
		public ServerSettings GetServerSettings(ServerSettings _) => serverSettings;
	}
}
