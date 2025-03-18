namespace MailAutoconf.Settings.Sources
{
	/// <summary>
	/// This class uses the central ISP database to determine the server settings for IMAP and SMTP.
	/// https://benbucksch.github.io/autoconfig-spec/draft-ietf-mailmaint-autoconfig.html#section-4.2
	/// </summary>
	public class IspDb : AutoconfigXml
	{
		/// <summary>
		/// This class uses Mail Autoconfig to determine the server settings for POP3, IMAP and SMTP.
		/// https://benbucksch.github.io/autoconfig-spec/draft-ietf-mailmaint-autoconfig.html
		/// </summary>
		/// <remarks>
		/// The primary constructor internally saves mailAddress and timeoutMs to provide them to 
		/// GetServerSettings().
		/// </remarks>
		/// <param name="mailAddress">the mail address for which this class searches server settings</param>
		/// <param name="timeoutMs">the timeout for the network connections in milliseconds</param>
		public IspDb(string mailAddress, int timeoutMs) : base(mailAddress, timeoutMs)
		{
			// https://benbucksch.github.io/autoconfig-spec/draft-ietf-mailmaint-autoconfig.html#name-central-database
			uris = 
			[
				$"https://v1.ispdb.net/{domain}"
			];
		}
	}
}