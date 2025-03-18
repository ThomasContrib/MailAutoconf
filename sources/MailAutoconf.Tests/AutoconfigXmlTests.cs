using MailAutoconf.Tests.Helper;

namespace MailAutoconf.Tests
{
	/// <summary>
	/// This class contains tests of the server settings source AutoconfigXml. These tests 
	/// do not retrieve data from websites. They simulate them by injecting the XML formatted 
	/// data directly.
	/// </summary>
	[TestClass]
	public class AutoconfigXmlTests
	{
		[TestMethod]
		public void Pop3ImapSmtp()
		{
			var serverSettings = new TestAutoconfigXml(
				mailAddress: "john.doe@company.com", 
				timeoutMs: 1)
			{
				XmlString = xml,
			}
			.GetServerSettings(null);

			Assert2.Pop3ImapSmtp(serverSettings);
			Assert2.UserName(serverSettings);
		}

		[TestMethod]
		public void ImapSmtp()
		{
			var serverSettings = new TestAutoconfigXml(
				mailAddress: "john.doe@company.com",
				timeoutMs: 1)
			{
				XmlString = xmlWithoutPop3,
			}
			.GetServerSettings(null);

			Assert2.ImapSmtp(serverSettings);
			Assert2.UserName(serverSettings);
		}

		[TestMethod]
		public void NoEntries()
		{
			var serverSettings = new TestAutoconfigXml(
				mailAddress: "john.doe@company.com",
				timeoutMs: 1)
			{
				XmlString = null,
			}
			.GetServerSettings(null);

			Assert.IsFalse(serverSettings.Pop3.ServerAndPortSet);
			Assert.IsFalse(serverSettings.Imap.ServerAndPortSet);
			Assert.IsFalse(serverSettings.Smtp.ServerAndPortSet);
		}

		[TestMethod]
		public void InvalidEntries()
		{
			var serverSettings = new TestAutoconfigXml(
				mailAddress: "john.doe@company.com",
				timeoutMs: 1)
			{
				XmlString = pageNotFoundHtml,
			}
			.GetServerSettings(null);

			Assert.IsFalse(serverSettings.Pop3.ServerAndPortSet);
			Assert.IsFalse(serverSettings.Imap.ServerAndPortSet);
			Assert.IsFalse(serverSettings.Smtp.ServerAndPortSet);
		}

		private const string xml = @"<clientConfig version=""1.1"">
	<emailProvider id=""automx2-1"">
		<identity />
		<domain>company.com</domain>
		<displayName>John Doe</displayName>
		<displayShortName>John Doe</displayShortName>
		<incomingServer type=""pop3"">
			<hostname>pop3.company.com</hostname>
			<port>995</port>
			<socketType>SSL</socketType>
			<username>%EMAILADDRESS%</username>
			<authentication>plain</authentication>
		</incomingServer>
		<incomingServer type=""imap"">
			<hostname>imap.company.com</hostname>
			<port>993</port>
			<socketType>SSL</socketType>
			<username>%EMAILADDRESS%</username>
			<authentication>plain</authentication>
		</incomingServer>
		<outgoingServer type=""smtp"">
			<hostname>smtp.company.com</hostname>
			<port>465</port>
			<socketType>SSL</socketType>
			<username>%EMAILADDRESS%</username>
			<authentication>plain</authentication>
		</outgoingServer>
	</emailProvider>
</clientConfig>";

		private const string xmlWithoutPop3 = @"<clientConfig version=""1.1"">
	<emailProvider id=""automx2-1"">
		<identity />
		<domain>company.com</domain>
		<displayName>John Doe</displayName>
		<displayShortName>John Doe</displayShortName>
		<incomingServer type=""imap"">
			<hostname>imap.company.com</hostname>
			<port>993</port>
			<socketType>SSL</socketType>
			<username>%EMAILADDRESS%</username>
			<authentication>plain</authentication>
		</incomingServer>
		<outgoingServer type=""smtp"">
			<hostname>smtp.company.com</hostname>
			<port>465</port>
			<socketType>SSL</socketType>
			<username>%EMAILADDRESS%</username>
			<authentication>plain</authentication>
		</outgoingServer>
	</emailProvider>
</clientConfig>";

		private const string pageNotFoundHtml = @"<html><body><p>page not found</p></body></html>";
	}
}