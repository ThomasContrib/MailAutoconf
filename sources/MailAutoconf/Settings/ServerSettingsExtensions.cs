using MailAutoconf.Util;

namespace MailAutoconf.Settings
{
	/// <summary>
	/// This class provides SelectBest() to extract the best settings from an enumeration of settings.
	/// </summary>
	public static class ServerSettingsExtensions
	{
		/// <summary>
		/// Extracts and returns the best settings from allServerSettings.
		/// </summary>
		public static ServerSettings SelectBest(this IEnumerable<ServerSettings> allServerSettings)
		{
			static bool HasMailAddress(ServerSettings serverSettings)
				=> serverSettings.MailAddress != null;

			var settingsWithMailAddress = allServerSettings
				.NotNull()
				.Where(HasMailAddress)
				.FirstOrDefault();

			var settingsWithPop3 = allServerSettings
				.NotNull()
				.Where(settings => settings.Pop3?.ServerAndPortSet ?? false)
				.OrderBy(settings => Score.Port(settings.Pop3) + Score.SocketType(settings.Pop3))
				.FirstOrDefault();

			var settingsWithImap = allServerSettings
				.NotNull()
				.Where(settings => settings.Imap?.ServerAndPortSet ?? false)
				.OrderBy(settings => Score.Port(settings.Imap) + Score.SocketType(settings.Imap))
				.FirstOrDefault();

			var settingsWithSmtp = allServerSettings
				.NotNull()
				.Where(settings => settings.Smtp?.ServerAndPortSet ?? false)
				.OrderBy(settings => Score.Port(settings.Smtp) + Score.SocketType(settings.Smtp))
				.FirstOrDefault();

			static ProtocolSettings BestOfProtocols(IEnumerable<ProtocolSettings> protocolSettings)
				=> protocolSettings
					.OrderBy(Score.ScoreProtocol)
					.FirstOrDefault();

			static string FirstNotEmpty(IEnumerable<string> strings) => strings.NotEmpty().FirstOrDefault();

			static Credentials BestOfCredentials(IEnumerable<ProtocolSettings> protocolSettings)
				=> protocolSettings
					.OrderBy(Score.UserName)
					.FirstOrDefault()?
					.Credentials;

			static ServerSettings Combine(params ServerSettings[] serverSettings)
			{
				var pop3Credentials = BestOfCredentials(serverSettings.Select(settings => settings?.Pop3));
				var imapCredentials = BestOfCredentials(serverSettings.Select(settings => settings?.Imap));
				var smtpCredentials = BestOfCredentials(serverSettings.Select(settings => settings?.Smtp));

				var result = new ServerSettings()
				{
					SourceName = serverSettings.Select(settings => settings?.SourceName)
					.NotNull()
					.Distinct()
					.DelimitedString(", "),
					MailAddress = FirstNotEmpty(serverSettings.Select(settings => settings?.MailAddress)),
					Pop3 = BestOfProtocols(serverSettings.Select(settings => settings?.Pop3)) ?? new(),
					Imap = BestOfProtocols(serverSettings.Select(settings => settings?.Imap)) ?? new(),
					Smtp = BestOfProtocols(serverSettings.Select(settings => settings?.Smtp)) ?? new(),
				};

				result.Pop3.Credentials = pop3Credentials;
				result.Imap.Credentials = imapCredentials;
				result.Smtp.Credentials = smtpCredentials;

				return result;
			}

			var settingsToCombine = new ServerSettings[] 
			{
				settingsWithMailAddress,
				settingsWithPop3,
				settingsWithImap,
				settingsWithSmtp,
			}
			.NotNull()
			.ToArray();

			return Combine(settingsToCombine);
		}
	}
}
