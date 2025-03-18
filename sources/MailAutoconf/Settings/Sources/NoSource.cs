namespace MailAutoconf.Settings.Sources
{
	/// <summary>
	/// This class implements a non-existent source. GetServerSettings() returns null.
	/// </summary>
	public class NoSource : IServerSettingsSource
	{
		/// <summary>
		/// Returns null.
		/// </summary>
		public ServerSettings GetServerSettings(ServerSettings _) => null;
	}
}
