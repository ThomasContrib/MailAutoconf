using MailAutoconf.Settings;

namespace MailAutoconf.PortCheck
{
	/// <summary>
	/// This class embeds en enumerations of PortCheckResults in ServerSettings. See PortCheckResults.From()
	/// for further details.
	/// </summary>
	internal class ServerSettingsPortCheck(IEnumerable<PortCheckResult> portCheckResults) : ServerSettings
	{
		public IEnumerable<PortCheckResult> PortCheckResults { get; } = portCheckResults;
	}
}