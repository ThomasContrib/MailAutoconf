namespace MailAutoconf.Settings
{
	/// <summary>
	/// This class provides user name and password to connect to the IMAP or SMTP server.
	/// </summary>
    public class Credentials
    {
		/// <summary>
		/// the user name to connect to the IMAP or SMTP server
		/// </summary>
		public UserName UserName { get; set; } = new();

		/// <summary>
		/// the password to connect to the IMAP or SMTP server
		/// </summary>
		public string Password { get; set; }
	}
}
