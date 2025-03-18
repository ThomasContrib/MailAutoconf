using MailAutoconf.PortCheck;
using MailAutoconf.Settings;

namespace MailAutoconf.Example
{
	internal class Program
	{
		static void Main()
		{
			// ----- Mail server settings: Search them by requesting some databases -----
			SearchServerSettings();

			// ----- Mail server settings: Start with some values and complete them by requesting some databases -----
			CompleteServerSettings();

			// ----- Mail server settings: Search them by requesting some databases and verify user/password -----
			SearchServerSettingsAndVerifyUserPassword();

			// ----- Probe mail ports -----
			ProbeMailPorts();
		}

		private static void SearchServerSettings()
		{
			ServerSettings serverSettings = new ServerSettingsFinder(mailAddress: "john.doe@yahoo.com")
				.GetSettings()
				.SelectBest();

			TitleAtConsole("Mail server settings: Search them by requesting some databases");
			ShowAtConsole(serverSettings);
		}

		private static void CompleteServerSettings()
		{
			var startSettings = new ServerSettings()
			{
				Pop3 = new ProtocolSettings() { Server = "pop.mail.yahoo.com", },
				Imap = new ProtocolSettings() { Server = "imap.mail.yahoo.com", },
				Smtp = new ProtocolSettings() { Server = "smtp.mail.yahoo.com", },
			};

			ServerSettings serverSettings = new ServerSettingsFinder(mailAddress: "john.doe@yahoo.com", startSettings)
				.GetSettings()
				.SelectBest();

			TitleAtConsole("Mail server settings: Start with some values and complete them by requesting some databases");
			ShowAtConsole(serverSettings);
		}

		private static void SearchServerSettingsAndVerifyUserPassword()
		{
			Credentials credentials = new()
			{
				UserName = new() { Name = "john.doe@yahoo.com" },
				Password = "pass1"
			};

			var startSettings = new ServerSettings()
			{
				Pop3 = new ProtocolSettings() { Credentials = credentials, },
				Imap = new ProtocolSettings() { Credentials = credentials, },
				Smtp = new ProtocolSettings() { Credentials = credentials, },
			};

			ServerSettings serverSettings
				= new ServerSettingsFinder(mailAddress: "john.doe@yahoo.com", startSettings)
					.GetSettings()
					.SelectBest();

			PortVerifier.Verify(serverSettings);

			TitleAtConsole("Mail server settings: Search them by requesting some databases and verify user/password");
			ShowAtConsole(serverSettings);
		}

		private static void ProbeMailPorts()
		{
			var serverSettings = new ServerSettings()
			{
				Pop3 = new ProtocolSettings() { Server = "pop.mail.yahoo.com", },
				Imap = new ProtocolSettings() { Server = "imap.mail.yahoo.com", },
				Smtp = new ProtocolSettings() { Server = "smtp.mail.yahoo.com", },
			};

			var portCheckResults = new PortsChecker(timeoutMs: 2000)
				.AddPorts(PortsChecker.DefaultPop3ImapSmtpPorts)
				.CheckPorts(serverSettings);

			TitleAtConsole("Probe mail ports");
			ShowAtConsole(portCheckResults);
		}

		private static void ShowAtConsole(IEnumerable<PortCheckResult> portCheckResults)
		{
			static string PassedFailed(bool successful) => successful ? "passed" : "failed";

			foreach (var result in portCheckResults)
			{
				string prefix = "";
				if (result.ConnectSuccessful) prefix += ">";
				if (result.AuthenticateSuccessful) prefix += ">";

				string timeoutString = result.TimedOut ? ", timed out" : "";
				Console.WriteLine($"{prefix,2}{result.Protocol}, Port: {result.ProtocolSettings?.Port,4}, " +
					$"Mode: {result.ProtocolSettings?.SocketType,8}, " +
					$"connect: {PassedFailed(result.ConnectSuccessful)}, " +
					$"authenticate: {PassedFailed(result.AuthenticateSuccessful)}{timeoutString}");

				//Console.WriteLine(result.ResultString);
			}
			Console.WriteLine();
		}

		private static void TitleAtConsole(string title)
		{
			Console.WriteLine(new string('-', 80));
			Console.WriteLine(title);
			Console.WriteLine();
		}

		private static void ShowAtConsole(ServerSettings serverSettings)
		{
			Console.WriteLine($"POP3: {serverSettings?.Pop3}");
			Console.WriteLine($"IMAP: {serverSettings?.Imap}");
			Console.WriteLine($"SMTP: {serverSettings?.Smtp}");
			Console.WriteLine();
		}
	}
}
