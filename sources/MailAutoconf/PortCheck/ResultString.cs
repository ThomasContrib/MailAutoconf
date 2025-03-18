using MailAutoconf.Util;

namespace MailAutoconf.PortCheck
{
	/// <summary>
	/// This class provides some fields and properties which are used to create the result string 
	/// of PortCheckResult.
	/// </summary>
	/// <param name="userName">the user name used by the PortChecker</param>
	/// <param name="password">the password used by the PortChecker</param>
	public class ResultString(string userName, string password)
	{
		/// <summary>
		/// the message of the exception encountered by the Pop3/Imap/SmtpPortChecker.
		/// Null if there was no exception.
		/// </summary>
		public string ExceptionMessage { get; set; } = null;

		/// <summary>
		/// some more (debug) protocol information encountered by the Pop3/Imap/SmtpPortChecker
		/// while connecting to the server
		/// </summary>
		public string DetailInfo { get; set; } = null;

		/// <summary>
		/// Returns a string containing the information from the fields and properties.
		/// </summary>
		public override string ToString()
		{
			return new string[]
			{
				ExceptionMessage,
				$"UserName = {userName}, Password = {password}",
				DetailInfo,
			}
			.NotEmpty()
			.NewLineDelimitedString();
		}
	}
}
