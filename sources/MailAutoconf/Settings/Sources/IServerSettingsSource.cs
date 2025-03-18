namespace MailAutoconf.Settings.Sources
{
	/// <summary>
	/// This interface is implemented by classes that retrieve server settings from Mail Autoconfig, Exchange
	/// Autodiscover, ISP Database etc.
	/// </summary>
	public interface IServerSettingsSource
	{
		/// <summary>
		/// Gets and returns the server settings.
		/// </summary>
		/// <param name="startSettings">the server settings already known (e.g. IMAP and SMTP server, 
		/// user name and password), but usually not complete (e.g. network port and socket type are 
		/// unknown or not yet verified), and therefore to be taken a start value and to be completed</param>
		/// <returns>the server settings found</returns>
		ServerSettings GetServerSettings(ServerSettings startSettings);
	}
}