using MailAutoconf.PortCheck;

namespace MailAutoconf.Settings.Sources
{
	/// <summary>
	/// This class uses the PortChecker to determine the server settings for IMAP and SMTP. The
	/// PortChecker probes TCP ports (currently IMAP and SMTP) to determine if the server is ready 
	/// to answer requests at these ports.
	/// </summary>
	/// <param name="portChecker">the PortChecker to be used to determine the server settings</param>
	public class PortCheckSource(PortsChecker portChecker) : IServerSettingsSource
	{
		/// <summary>
		/// Calls CheckPort(serverSettings) and extracts the best settings from all results.
		/// </summary>
		public ServerSettings GetServerSettings(ServerSettings startSettings)
			=> PortCheckResults.GetBestServerSettings(portChecker.CheckPorts(startSettings));
	}
}
