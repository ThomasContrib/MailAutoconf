using MailAutoconf.Settings;
using MailAutoconf.Settings.Sources;

namespace MailAutoconf.Tests
{
	/// <summary>
	/// This class contains tests of the ServerSettingsBuilder.
	/// </summary>
	[TestClass]
	public class SettingsBuilderTests
	{
		[TestMethod]
		public void DefaultSources()
		{
			var allServerSettings = new ServerSettingsBuilder()
				.AddDefaultSources(
					mailAddress: "john.doe@yahoo.com",
					timeoutMs: 2000)
				.GetSettings();

			//static void WriteConsole(ServerSettings serverSettings) => Console.WriteLine(serverSettings);
			//allServerSettings.ToList().ForEach(WriteConsole);

			var bestSettings = allServerSettings.SelectBest();

			Assert.AreEqual("imap.mail.yahoo.com", bestSettings.Imap.Server);
			Assert.AreEqual(993, bestSettings.Imap.Port);

			Assert.AreEqual("smtp.mail.yahoo.com", bestSettings.Smtp.Server);
			Assert.AreEqual(465, bestSettings.Smtp.Port);
		}

		[TestMethod]
		public void SomeSources()
		{
			string mailAddress = "john.doe@yahoo.com";
			int timeoutMs = 2000;

			var allServerSettings = new ServerSettingsBuilder()
				.AddSource(new AutoconfigXml(mailAddress, timeoutMs))
				.AddSource(new AutodiscoverXml(mailAddress, timeoutMs))
				.AddSource(new IspDb(mailAddress, timeoutMs))
				.GetSettings();

			//static void WriteConsole(ServerSettings serverSettings) => Console.WriteLine(serverSettings);
			//allServerSettings.ToList().ForEach(WriteConsole);

			var bestSettings = allServerSettings.SelectBest();

			Assert.AreEqual("imap.mail.yahoo.com", bestSettings.Imap.Server);
			Assert.AreEqual(993, bestSettings.Imap.Port);

			Assert.AreEqual("smtp.mail.yahoo.com", bestSettings.Smtp.Server);
			Assert.AreEqual(465, bestSettings.Smtp.Port);
		}
	}
}
