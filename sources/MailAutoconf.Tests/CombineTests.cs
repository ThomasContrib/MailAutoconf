using MailAutoconf.Settings;

namespace MailAutoconf.Tests
{
	/// <summary>
	/// This class contains tests that ensure the correct combination of a collection of
	/// server settings.
	/// </summary>
	[TestClass]
	public class CombineTests
	{
		[TestMethod]
		public void Combine_TwoSettings()
		{
			IEnumerable<ServerSettings> allServerSettings = 
			[
				new() 
				{
					MailAddress = "john.doe@company.com",
					Imap = new() 
					{
						Credentials = new() { UserName = new() { Name = "JohnDoe", Type = UserNameType.UserName } },
					}
				},
				new()
				{
					Imap = new()
					{
						Server = "imap.company.com",
						Port = 993,
					},
					Smtp = new()
					{
						Server = "smtp.company.com",
						Port = 587,
					}
				},
			];

			var bestSettings = allServerSettings.SelectBest();

			Assert.AreEqual("john.doe@company.com", bestSettings.MailAddress);
			Assert.AreEqual("JohnDoe", bestSettings.Imap.Credentials.UserName.Name);

			Assert.IsNotNull(bestSettings.Smtp);
			Assert.IsNull(bestSettings.Smtp.Credentials?.UserName?.Name);

			Assert.AreEqual("imap.company.com", bestSettings.Imap.Server);
			Assert.AreEqual("smtp.company.com", bestSettings.Smtp.Server);
		}

		[TestMethod]
		public void Combine_Imap()
		{
			IEnumerable<ServerSettings> allServerSettings =
			[
				new()
				{
					Imap = new()
					{
						Server = "imap.company.com",
						Port = 993,
						SocketType = Types.SocketType.Unknown,
					},
				},
				new()
				{
					Imap = new()
					{
						Server = "imap.company.com",
						Port = 993,
						SocketType = Types.SocketType.Plain,
					},
				},
			];

			var bestSettings = allServerSettings.SelectBest();

			Assert.AreEqual("imap.company.com", bestSettings.Imap.Server);
			Assert.AreEqual(993, bestSettings.Imap.Port);
			Assert.AreEqual(Types.SocketType.Plain, bestSettings.Imap.SocketType);
		}
	}
}
