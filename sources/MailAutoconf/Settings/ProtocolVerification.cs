namespace MailAutoconf.Settings
{
	/// <summary>
	/// the state of the protocol verification. ProtocolSettings (in ServerSettings) can be 
	/// verified by means of the PortsChecker. It tries to connect to the POP3, IMAP or SMTP 
	/// server using these settings (server name, user name and password) and sets the property
	/// ProtocolSettings.Verification accordingly.
	/// </summary>
	public enum ProtocolVerification
	{
		/// <summary>
		/// protocol not (yet) verified
		/// </summary>
		Unverified,

		/// <summary>
		/// protocol verification passed
		/// </summary>
		Passed,

		/// <summary>
		/// protocol verification failed
		/// </summary>
		Failed
	};
}
