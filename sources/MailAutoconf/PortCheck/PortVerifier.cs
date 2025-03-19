using MailAutoconf.Settings;

namespace MailAutoconf.PortCheck
{
	/// <summary>
	/// This class implements Verify() to verify server settings for POP3, IMAP and SMTP.
	/// </summary>
    public class PortVerifier
    {
		/// <summary>
		/// Verifies serverSettings.Pop3/Imap/Smtp: Connects and authenticates to the specified servers. 
		/// Reports the result by setting serverSettings.Pop3/Imap/Smtp.Verification accordingly.
		/// Returns the port check results from the PortsChecker used.
		/// </summary>
		/// <param name="serverSettings">the settings with the POP3, IMAP and SMTP server</param>
		/// <param name="forceVerification">If true always performs the the verification. If false only
		/// performs the verification if it has not yet be done (default value).</param>
		/// <param name="timeoutMs">the timeout for the network connection in milliseconds (default value: 2000 ms)</param>
		public static PortCheckResult[] Verify(ServerSettings serverSettings, bool forceVerification = false, int timeoutMs = 2000)
		{
			var portsChecker = new PortsChecker(timeoutMs)
				.AddPorts(PortsChecker.PortsFrom(serverSettings));

			ProtocolSettingsFilter filter = forceVerification
				? ProtocolSettingsFilter.None
				: ProtocolSettingsFilter.UnverifiedOnly;

			return portsChecker.CheckPorts(serverSettings, filter, verify: true);
		}
	}
}
