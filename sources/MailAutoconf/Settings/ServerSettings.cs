using MailAutoconf.Util;

namespace MailAutoconf.Settings
{
	/// <summary>
	/// This class contains the settings for a network connection with a protocol like IMAP or SMTP.
	/// </summary>
	public class ServerSettings
	{
		/// <summary>
		/// e.g. Options, AutoconfigXml etc.
		/// </summary>
		public string SourceName { get; set; }

		/// <summary>
		/// the mail address for which these settings are intended
		/// </summary>
		public string MailAddress { get; set; }

		/// <summary>
		/// the detail settings for the POP3 server
		/// </summary>
		public ProtocolSettings Pop3 { get; set; } = new();

		/// <summary>
		/// the detail settings for the IMAP server
		/// </summary>
		public ProtocolSettings Imap { get; set; } = new();

		/// <summary>
		/// the detail settings for the SMTP server
		/// </summary>
		public ProtocolSettings Smtp { get; set; } = new();

		/// <summary>
		/// true if the settings contain enough information for the mail configuration
		/// </summary>
		public bool EnoughForMailConfiguration => 
			(Imap?.EnoughForMailConfiguration ?? false) 
			&& (Smtp?.EnoughForMailConfiguration ?? false);

		/// <summary>
		/// Returns information about servers (POP3, IMAP and SMTP) as string.
		/// </summary>
		/// <returns></returns>
		public override string ToString() => new string[]
		{
			SourceName,
			Pop3?.Server ?? "POP3 unknown",
			Imap?.Server ?? "IMAP unknown",
			Smtp?.Server ?? "SMTP unknown",
			EnoughForMailConfiguration ? "(enough for mail configuration)" : null,
		}
		.NotEmpty()
		.CommaDelimitedString();

		/// <summary>
		/// Clones and returns these settings.
		/// </summary>
		public ServerSettings Clone() => (ServerSettings)MemberwiseClone();
	}
}
