using MailAutoconf.Tests.Helper;

namespace MailAutoconf.Tests
{
	/// <summary>
	/// This class contains tests of the server settings source AutodiscoverXml. These tests 
	/// do not retrieve data from websites. They simulate them by injecting the XML formatted 
	/// data directly.
	/// </summary>
	[TestClass]
	public class AutodiscoverXmlTests
	{
		[TestMethod]
		public void Pop3ImapSmtp()
		{
			var serverSettings = new TestAutodiscoverXml(
				mailAddress: "john.doe@company.com", 
				timeoutMs: 1)
			{
				XmlString = xml,
			}
			.GetServerSettings(null);

			Assert2.Pop3ImapSmtp(serverSettings);
		}

		[TestMethod]
		public void ImapSmtp()
		{
			var serverSettings = new TestAutodiscoverXml(
				mailAddress: "john.doe@company.com",
				timeoutMs: 1)
			{
				XmlString = xmlWithoutPop3,
			}
			.GetServerSettings(null);

			Assert2.ImapSmtp(serverSettings);
		}

		[TestMethod]
		public void NoEntries()
		{
			var serverSettings = new TestAutodiscoverXml(
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
			var serverSettings = new TestAutodiscoverXml(
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

		private const string xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Autodiscover xmlns=""http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006"">
  <Response xmlns=""http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a"">
    <Account>
      <AccountType>email</AccountType>
      <Action>settings</Action>
      <Protocol>
        <Type>IMAP</Type>
        <Server>imap.company.com</Server>
        <Port>993</Port>
        <DomainRequired>off</DomainRequired>
        <LoginName></LoginName>
        <SPA>off</SPA>
        <SSL>on</SSL>
        <AuthRequired>on</AuthRequired>
      </Protocol>
      <Protocol>
        <Type>POP3</Type>
        <Server>pop3.company.com</Server>
        <Port>995</Port>
        <DomainRequired>off</DomainRequired>
        <LoginName></LoginName>
        <SPA>off</SPA>
        <SSL>on</SSL>
        <AuthRequired>on</AuthRequired>
      </Protocol>
      <Protocol>
        <Type>SMTP</Type>
        <Server>smtp.company.com</Server>
        <Port>465</Port>
        <DomainRequired>off</DomainRequired>
        <LoginName></LoginName>
        <SPA>off</SPA>
        <SSL>on</SSL>
        <AuthRequired>on</AuthRequired>
        <UsePOPAuth>off</UsePOPAuth>
        <SMTPLast>off</SMTPLast>
      </Protocol>
    </Account>
  </Response>
</Autodiscover>
";

		private const string xmlWithoutPop3 = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Autodiscover xmlns=""http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006"">
  <Response xmlns=""http://schemas.microsoft.com/exchange/autodiscover/outlook/responseschema/2006a"">
    <Account>
      <AccountType>email</AccountType>
      <Action>settings</Action>
      <Protocol>
        <Type>IMAP</Type>
        <Server>imap.company.com</Server>
        <Port>993</Port>
        <DomainRequired>off</DomainRequired>
        <LoginName></LoginName>
        <SPA>off</SPA>
        <SSL>on</SSL>
        <AuthRequired>on</AuthRequired>
      </Protocol>
      <Protocol>
        <Type>SMTP</Type>
        <Server>smtp.company.com</Server>
        <Port>465</Port>
        <DomainRequired>off</DomainRequired>
        <LoginName></LoginName>
        <SPA>off</SPA>
        <SSL>on</SSL>
        <AuthRequired>on</AuthRequired>
        <UsePOPAuth>off</UsePOPAuth>
        <SMTPLast>off</SMTPLast>
      </Protocol>
    </Account>
  </Response>
</Autodiscover>
";

		private const string pageNotFoundHtml = @"<html><body><p>page not found</p></body></html>";
	}
}