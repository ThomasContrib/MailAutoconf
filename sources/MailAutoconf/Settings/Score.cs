using MailAutoconf.PortCheck;
using MailAutoconf.Types;

namespace MailAutoconf.Settings
{
	/// <summary>
	/// This class contains methods to score protocol settings etc. This score values are used to find
	/// the best settings and to order results.
	/// </summary>
    internal class Score
	{
		public static int Port(ProtocolSettings protocolSettings) 
			=> Port(protocolSettings?.Port ?? 0);

		public static int Port(int portNo) => portNo switch
		{
			993 => 10,
			143 => 20,

			587 => 10,
			465 => 20,
			25 => 30,
			2525 => 40,

			_ => 90,
		};

		public static int SocketType(ProtocolSettings protocolSettings) 
			=> SocketType(protocolSettings?.SocketType ?? Types.SocketType.Unknown);

		public static int SocketType(SocketType socketType) => socketType switch
		{
			Types.SocketType.SslTls => 1,
			Types.SocketType.StartTls => 2,
			Types.SocketType.Plain => 3,
			Types.SocketType.Unknown => 900, // big number: an unknown socket type is worse than a port number
			_ => 900,
		};

		// Returns a score value for settings. Lower value mean the result is better than a result
		// with a higher value. This value sorts the result by its relevance.
		public static int ScoreProtocol(ProtocolSettings protocolSettings)
		{
			int verificationScore = protocolSettings?.Verification switch
			{
				ProtocolVerification.Passed => 10000,
				ProtocolVerification.Failed => 30000,
				ProtocolVerification.Unverified => 20000,
				null => throw new ArgumentException("protocolSettings missing"),
				_ => throw new ArgumentException($"Unknown verification: {protocolSettings?.Verification}"),
			};

			int serverScore = protocolSettings?.Server != null 
				? 1000 
				: 2000;

			return verificationScore + serverScore + SocketType(protocolSettings) + Port(protocolSettings);
		}

		/// <summary>
		/// This method returns a value for a PortCheckResult. Lower value mean the result is better than
		/// a result with a higher value. This value sorts the result by its relevance.
		/// </summary>
		public static int PortCheckResult(PortCheckResult portCheckResult)
		{
			int timeoutScore = portCheckResult.TimedOut 
				? 200000 
				: 100000;

			int connectScore = portCheckResult.ConnectSuccessful 
				? 10000 
				: 20000;

			int authenticateScore = portCheckResult.AuthenticateSuccessful 
				? 1000 
				: 2000;

			int socketTypeScore = SocketType(portCheckResult.ProtocolSettings);
			int portScore = Port(portCheckResult.ProtocolSettings);

			return timeoutScore + connectScore + authenticateScore + socketTypeScore + portScore;
		}

		public static int UserName(ProtocolSettings protocolSettings) => UserName(protocolSettings?.Credentials);

		public static int UserName(Credentials credentials)
		{
			if (credentials?.UserName == null || credentials?.Password == null)
			{
				return 6;
			}
			if (credentials.UserName.AuthenticationSuccessful)
			{
				return 1;
			}
			else if (string.IsNullOrEmpty(credentials.UserName.Name))
			{
				return 5;
			}
			else if (credentials.UserName.Type == UserNameType.Unknown)
			{
				return 4;
			}
			else if (credentials.UserName.NameIsPlaceholder)
			{
				return 1;
			}
			else
			{
				return 2;
			}
		}
	}
}
