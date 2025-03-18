using MailAutoconf.Types;

namespace MailAutoconf.PortCheck
{
	/// <summary>
	/// This interface is implemented by classes that are used by PortChecker to probe a single POP3, 
	/// IMAP or SMTP port.
	/// </summary>
	public interface IPortChecker
	{
		/// <summary>
		/// the protocol handled by the implementing PortChecker
		/// </summary>
		Protocol Protocol { get; }

		/// <summary>
		/// the port to probe
		/// </summary>
		int Port { get; }

		/// <summary>
		/// Probes the TCP port (e.g. passed to the constructor of the implementing class). Tries to connect 
		/// and authenticate at this ports. Returns an array with the results. It contains an entry per mode 
		/// to probe.
		/// </summary>
		/// <param name="server">the server to probe</param>
		/// <param name="userName">the user name for authentification (POP3, IMAP or SMTP)</param>
		/// <param name="password">the password for authentification (POP3, IMAP or SMTP)</param>
		/// <param name="timeoutMs">the timeout for the network connection in milliseconds</param>
		/// <returns>array with the probe results</returns>
		Task<PortCheckResult>[] CheckPort(string server, string userName, string password, int timeoutMs);
	}
}
