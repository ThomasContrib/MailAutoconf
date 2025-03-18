using MailAutoconf.Settings;
using MailAutoconf.Types;
using MailAutoconf.Util;

namespace MailAutoconf.PortCheck
{
	/// <summary>
	/// This class probes TCP ports (POP3, IMAP and SMTP) to determine if the server is ready 
	/// to answer requests at these ports. It tries to connect and authenticate. Furthermore it
	/// maintains a list of port checkers. Each checker (e.g. ImapPortChecker or SmtpPortChecker) 
	/// is responsible to probe one port. 
	/// 
	/// Checkers can be added with AddPort() or AddPorts(). You may add some default ones with 
	/// .AddPorts(DefaultImapSmtpPorts) or you add ports exactly for some server settings with
	/// .AddPorts(PortsChecker.PortsFrom(serverSettings)). CheckPort() uses all of these checkers 
	/// to probe the required ports.
	/// 
	/// Note that the protocol settings (POP3, IMAP or SMTP) within the server settings must
	/// provide a server name. Otherwise the PortsChecker does nothing.
	/// </summary>
	/// <param name="timeoutMs">the timeout for the network connections to the probed ports in 
	/// milliseconds</param>
	public class PortsChecker(int timeoutMs)
    {
        private readonly List<IPortChecker> portCheckers = [];

		/// <summary>
		/// some default IMAP and SMTP ports to be probed (IMAP and SMTP)
		/// </summary>
        public static IEnumerable<IPortChecker> DefaultPop3ImapSmtpPorts =>
        [
			new Pop3PortChecker(110, SocketType.Plain, SocketType.StartTls, SocketType.SslTls),
			new Pop3PortChecker(995, SocketType.Plain, SocketType.StartTls, SocketType.SslTls),

            new ImapPortChecker(143, SocketType.Plain, SocketType.StartTls, SocketType.SslTls),
            new ImapPortChecker(993, SocketType.Plain, SocketType.StartTls, SocketType.SslTls),

            new SmtpPortChecker(25, SocketType.Plain, SocketType.StartTls, SocketType.SslTls),
            new SmtpPortChecker(465, SocketType.Plain, SocketType.StartTls, SocketType.SslTls),
            new SmtpPortChecker(587, SocketType.Plain, SocketType.StartTls, SocketType.SslTls),
            new SmtpPortChecker(2525, SocketType.Plain, SocketType.StartTls, SocketType.SslTls),
        ];

		/// <summary>
		/// Returns the port checkers for the ports used in the passed server settings.
		/// </summary>
		public static IEnumerable<IPortChecker> PortsFrom(ServerSettings serverSettings)
		{
			// Returns true if the port is not null
			static bool PortAvailable(ProtocolAndSettings protocolAndSettings)
				=> protocolAndSettings.ProtocolSettings.Port != null;

			// Returns the suitable port checker for protocolAndSettings.
			static IPortChecker ToPortChecker(ProtocolAndSettings protocolAndSettings)
			{
				// Take the socket type as stored in the protocol settings. If it is unknown, take all
				// known (SslTls, StartTls and Plain).
				SocketType[] socketType = protocolAndSettings.ProtocolSettings.SocketType == SocketType.Unknown
					? [ SocketType.SslTls, SocketType.StartTls, SocketType.Plain]
					: [ protocolAndSettings.ProtocolSettings.SocketType ];

				int port = (int)protocolAndSettings.ProtocolSettings.Port;

				return protocolAndSettings.Protocol switch
				{
					Protocol.POP3 => new Pop3PortChecker(port, socketType),
					Protocol.IMAP => new ImapPortChecker(port, socketType),
					Protocol.SMTP => new SmtpPortChecker(port, socketType),
					_ => throw new ArgumentException($"Unknown protocol: {protocolAndSettings.Protocol}"),
				};
			}

			// Extract the protocolSettings from the serverSettings (POP3, IMAP and SMTP)
			var allSettings = ToProtocolAndSettings(serverSettings);

			return allSettings
				.Where(PortAvailable)
				.Select(ToPortChecker);
		}

		/// <summary>
		/// Extracts the protocol settings from the passed server settings and returns them as array.
		/// ProtocolAndSettings contain these protocol settings and the protocol (POP3, IMAP or SMTP).
		/// </summary>
		private static ProtocolAndSettings[] ToProtocolAndSettings(ServerSettings serverSettings)
			=>  [
					new ProtocolAndSettings(serverSettings.Pop3, Protocol.POP3),
					new ProtocolAndSettings(serverSettings.Imap, Protocol.IMAP),
					new ProtocolAndSettings(serverSettings.Smtp, Protocol.SMTP),
				];

		/// <summary>
		/// Adds a port checker to the list of checkers used by CheckPort().
		/// </summary>
		public PortsChecker AddPort(IPortChecker portChecker)
        {
            portCheckers.Add(portChecker);
            return this;
        }

		/// <summary>
		/// Adds port checkers to the list of checkers used by CheckPort().
		/// </summary>
		public PortsChecker AddPorts(IEnumerable<IPortChecker> portCheckers)
        {
            foreach (var portChecker in portCheckers)
            {
                AddPort(portChecker);
            }
			return this;
		}

		/// <summary>
		/// Probes the ports added with AddPort() or AddPorts(). Gets server, user name and password
		/// from the passed serverSettings.
		/// </summary>
		/// <param name="serverSettings">the server settings to be probed</param>
		/// <param name="filter">a filter which the protocol settings (POP3, IMAP, SMPT) in server 
		/// settings have to pass the their ports are going to be probed</param>
		/// <param name="verify">if true, sets the property serverSettings.Pop3/Imap/Smtp.Verification 
		/// according to the portCheckResults</param>
		public PortCheckResult[] CheckPorts(ServerSettings serverSettings, 
			ProtocolSettingsFilter filter = null, bool verify = false)
        {
			//StopwatchConsole stopwatch = new("CheckPorts");

			try
			{
				filter ??= ProtocolSettingsFilter.None;

				var filteredSettings = ToProtocolAndSettings(serverSettings)
					.Where(filter.Accepts);

				var tasks = filteredSettings
					.SelectMany(GetCheckPortsTasks)
					.ToArray();

				Task.WaitAll(tasks);

				var portCheckResults = tasks
					.Select(task => task.Result)
					.ToArray();

				if (verify)
				{
					Verify(filteredSettings, portCheckResults);
				}

				return portCheckResults;
			}
			finally
			{
				//stopwatch.Stop();
			}
		}

		/// <summary>
		/// Sets the property settings.ProtocolSettings.Verification according to the portCheckResults.
		/// </summary>
		private static void Verify(IEnumerable<ProtocolAndSettings> settings, PortCheckResult[] portCheckResults)
		{
			foreach (var item in settings)
			{
				var firstResult = portCheckResults
					.FirstOrDefault(portCheckResult => portCheckResult.Protocol == item.Protocol);

				item.ProtocolSettings.Verification =
					firstResult == null
						? ProtocolVerification.Unverified
						: firstResult.Verified
							? ProtocolVerification.Passed
							: ProtocolVerification.Failed;
			}
		}

		private Task<PortCheckResult>[] GetCheckPortsTasks(ProtocolAndSettings settings)
		{
			bool probingUseless = settings.ProtocolSettings.Server == null;
			if (probingUseless) return [];

			bool ProtocolMatches(IPortChecker portChecker) => portChecker.Protocol == settings.Protocol;

			Task<PortCheckResult>[] GetCheckPortTasks(IPortChecker portChecker)
				=> CheckPort(portChecker, settings);

			var matchingPortCheckers = portCheckers
				.Where(ProtocolMatches);

			var tasks = matchingPortCheckers
				.SelectMany(GetCheckPortTasks)
				.ToArray();

			return tasks;
		}

		/// <summary>
		/// Creates and returns a task to probe the port using portChecker with the passed settings. 
		/// Marked 'protected virtual' for testing purposes, i.e. that a test class can intercept 
		/// calls of this method.
		/// </summary>
		protected virtual Task<PortCheckResult>[] CheckPort(IPortChecker portChecker, ProtocolAndSettings settings)
			=> portChecker.CheckPort(
					settings.ProtocolSettings.Server,
					settings.ProtocolSettings.Credentials.UserName.Name,
					settings.ProtocolSettings.Credentials.Password,
					timeoutMs);
	}
}