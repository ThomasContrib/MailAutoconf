using MailAutoconf.Settings;
using MailAutoconf.Types;

namespace MailAutoconf.PortCheck
{
	/// <summary>
	/// This class combines ProtocolSettings and Protocol to be passed as single parameter within 
	/// PortsChecker.
	/// </summary>
	public class ProtocolAndSettings(ProtocolSettings protocolSettings, Protocol protocol)
	{
		/// <summary>
		/// the protocol settings
		/// </summary>
		public ProtocolSettings ProtocolSettings { get; } = protocolSettings;

		/// <summary>
		/// the protocol (POP3, IMAP or SMTP)
		/// </summary>
		public Protocol Protocol { get; } = protocol;
	}
}
