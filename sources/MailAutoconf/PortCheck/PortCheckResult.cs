using MailAutoconf.Settings;
using MailAutoconf.Types;

namespace MailAutoconf.PortCheck
{
	/// <summary>
	/// This class contains the results of a port probe.
	/// </summary>
	public class PortCheckResult
    {
		/// <summary>
		/// the settings of the probed port (e.g. server name and port)
		/// </summary>
		public ProtocolSettings ProtocolSettings { get; internal set; }

		/// <summary>
		/// the probed port protocol (POP3, IMAP or SMTP)
		/// </summary>
		public Protocol Protocol { get; internal set; }

		/// <summary>
		/// true if connecting was successful
		/// </summary>
		public bool ConnectSuccessful { get; internal set; }

		/// <summary>
		/// true if authenticating was successful
		/// </summary>
		public bool AuthenticateSuccessful { get; internal set; }

		/// <summary>
		/// true if the network connection timed out
		/// </summary>
		public bool TimedOut { get; internal set; }

		/// <summary>
		/// true if connected and authenticated successfully and if connection not timed out
		/// </summary>
		public bool Verified => ConnectSuccessful && AuthenticateSuccessful && !TimedOut;

		/// <summary>
		/// a string that usually contains information acquired during port probing and may help the 
		/// debug this process
		/// </summary>
		public ResultString ResultString { get; set; }

		/// <summary>
		/// Returns information about protocol, settings and authentication verification
		/// </summary>
		public override string ToString() => $"{Protocol}, {ProtocolSettings}, verified: {Verified}";
	}
}
