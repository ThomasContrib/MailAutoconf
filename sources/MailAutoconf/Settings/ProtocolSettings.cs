using MailAutoconf.Types;
using MailAutoconf.Util;

namespace MailAutoconf.Settings
{
	/// <summary>
	/// This class contains the settings for a network connection with a protocol like IMAP or SMTP.
	/// </summary>
    public class ProtocolSettings
	{
		/// <summary>
		/// the server name
		/// </summary>
		public string Server { get; set; }

		/// <summary>
		/// the server network port
		/// </summary>
		public int? Port { get; set; }

		/// <summary>
		/// the type of the network socket (e.g. SSL/TLS)
		/// </summary>
		public SocketType SocketType { get; set; }
		
		/// <summary>
		/// the user name and password to connect to the IMAP or SMTP server
		/// </summary>
		public Credentials Credentials { get; set; } = new();

		/// <summary>
		/// some authentication information from the public mail settings databases (e.g. OAuth2)
		/// </summary>
		public string Authentication { get; set; } = null;

		/// <summary>
		/// true if server and port are not null
		/// </summary>
		public bool ServerAndPortSet => Server != null && Port != null;

		/// <summary>
		/// true if the settings contain enough information for the mail configuration
		/// </summary>
		public bool EnoughForMailConfiguration => ServerAndPortSet && SocketType != SocketType.Unknown;

		/// <summary>
		/// true if these protocol settings have been successfully verified by connecting and authenticating 
		/// to the server
		/// </summary>
		public ProtocolVerification Verification { get; set; } = ProtocolVerification.Unverified;

		/// <summary>
		/// Returns server, port, socket type etc. as string.
		/// </summary>
		public override string ToString() => new string[]
		{
			Server,
			Port.ToString(),
			SocketType.ToString(),
			$"User: {Credentials?.UserName?.Name ?? "Unknown"}",
			Authentication,
			$"Verification: {Verification}",
		}
		.NotEmpty()
		.CommaDelimitedString();
	}
}
