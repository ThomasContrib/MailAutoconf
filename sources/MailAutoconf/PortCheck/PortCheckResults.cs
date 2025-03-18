using MailAutoconf.Settings;
using MailAutoconf.Types;

namespace MailAutoconf.PortCheck
{
	/// <summary>
	/// This class provides methods to process enumerations of PortCheckResults.
	/// </summary>
	public class PortCheckResults
    {
		/// <summary>
		/// Combines the collection of PortCheckResults to a recommendation for the best server 
		/// settings.
		/// </summary>
		public static ServerSettings GetBestServerSettings(IEnumerable<PortCheckResult> portCheckResults)
        {
			var bestPop3Settings = portCheckResults
				.Where(result => result.Protocol == Protocol.POP3)
				.OrderBy(Score.PortCheckResult)
				.FirstOrDefault();

			var bestImapSettings = portCheckResults
                .Where(result => result.Protocol == Protocol.IMAP)
                .OrderBy(Score.PortCheckResult)
                .FirstOrDefault();

            var bestSmtpSettings = portCheckResults
                .Where(result => result.Protocol == Protocol.SMTP)
                .OrderBy(Score.PortCheckResult)
                .FirstOrDefault();

            return new ServerSettingsPortCheck(portCheckResults)
            {
                SourceName = "PortCheck",
                Pop3 = bestPop3Settings?.ProtocolSettings,
                Imap = bestImapSettings?.ProtocolSettings,
                Smtp = bestSmtpSettings?.ProtocolSettings,
            };
        }

		/// <summary>
		/// Retrieves and returns the enumeration of PortCheckResults from an enumeration of ServerSettings.
        /// These results are embedded in the ServerSettings object of type ServerSettingsPortCheck. If
        /// the passed enumeration does not contain such an object, an empty enumeration is returned.
		/// </summary>
		public static IEnumerable<PortCheckResult> From(IEnumerable<ServerSettings> allServerSettings)
		{
            static bool IsPortCheckResult(ServerSettings serverSettings) 
                => serverSettings is ServerSettingsPortCheck;

            var portCheckResult = allServerSettings.FirstOrDefault(IsPortCheckResult);

            return portCheckResult == null
                ? []
                : ((ServerSettingsPortCheck)portCheckResult).PortCheckResults;
		}
    }
}