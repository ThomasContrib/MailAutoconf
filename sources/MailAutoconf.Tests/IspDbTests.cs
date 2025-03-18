using MailAutoconf.Tests.Helper;

namespace MailAutoconf.Tests
{
	/// <summary>
	/// This class contains tests of the server settings source IspDb. These tests do not 
    /// retrieve data from websites. They simulate them by injecting the XML formatted 
	/// data directly.
	/// </summary>
	[TestClass]
	public class IspDbTests
	{
		[TestMethod]
		public void Pop3ImapSmtp()
		{
			var serverSettings = new TestIspDb(
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
			var serverSettings = new TestIspDb(
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
			var serverSettings = new TestIspDb(
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

		private const string xml = @"<clientConfig version=""1.1"">
  <emailProvider id=""company.com"">
    <domain>company.com</domain>
    <domain>company.com.mx</domain>
    <!-- MX -->
    <domain>mail.am0.companydns.net</domain>
    <domain>am0.companydns.net</domain>

    <displayName>company! Mail</displayName>
    <displayShortName>company</displayShortName>

    <incomingServer type=""imap"">
      <hostname>imap.company.com</hostname>
      <port>993</port>
      <socketType>SSL</socketType>
      <username>%EMAILADDRESS%</username>
      <authentication>OAuth2</authentication>
      <authentication>password-cleartext</authentication>
    </incomingServer>
    <incomingServer type=""pop3"">
      <hostname>pop3.company.com</hostname>
      <port>995</port>
      <socketType>SSL</socketType>
      <username>%EMAILADDRESS%</username>
      <authentication>OAuth2</authentication>
      <authentication>password-cleartext</authentication>
    </incomingServer>
    <outgoingServer type=""smtp"">
      <hostname>smtp.company.com</hostname>
      <port>465</port>
      <socketType>SSL</socketType>
      <username>%EMAILADDRESS%</username>
      <authentication>OAuth2</authentication>
      <authentication>password-cleartext</authentication>
    </outgoingServer>

    <documentation url=""https://help.company.com/kb/new-mail-for-desktop/imap-server-settings-company-mail-sln4075.html"">
      <descr lang=""en"">How to setup email applications with imap to receive company! mail?</descr>
    </documentation>
    <documentation url=""https://help.company.com/kb/new-mail-for-desktop/pop-access-settings-instructions-company-mail-sln4724.html"">
      <descr lang=""en"">How to setup email applications with pop to receive company! mail?</descr>
    </documentation>
  </emailProvider>

  <oAuth2>
    <issuer>login.company.com</issuer>
    <scope>mail-w</scope>
    <authURL>https://api.login.company.com/oauth2/request_auth</authURL>
    <tokenURL>https://api.login.company.com/oauth2/get_token</tokenURL>
  </oAuth2>

  <webMail>
    <loginPage url=""https://mail.company.com""/>
    <loginPageInfo url=""https://mail.company.com/"">
      <username>%EMAILADDRESS%</username>
      <usernameField id=""login-username""/>
      <passwordField id=""login-passwd""/>
      <loginButton id=""login-signin""/>
    </loginPageInfo>
  </webMail>
</clientConfig>
";

		private const string xmlWithoutPop3 = @"<clientConfig version=""1.1"">
  <emailProvider id=""company.com"">
    <domain>company.com</domain>
    <domain>company.com.mx</domain>
    <!-- MX -->
    <domain>mail.am0.companydns.net</domain>
    <domain>am0.companydns.net</domain>

    <displayName>company! Mail</displayName>
    <displayShortName>company</displayShortName>

    <incomingServer type=""imap"">
      <hostname>imap.company.com</hostname>
      <port>993</port>
      <socketType>SSL</socketType>
      <username>%EMAILADDRESS%</username>
      <authentication>OAuth2</authentication>
      <authentication>password-cleartext</authentication>
    </incomingServer>
    <outgoingServer type=""smtp"">
      <hostname>smtp.company.com</hostname>
      <port>465</port>
      <socketType>SSL</socketType>
      <username>%EMAILADDRESS%</username>
      <authentication>OAuth2</authentication>
      <authentication>password-cleartext</authentication>
    </outgoingServer>

    <documentation url=""https://help.company.com/kb/new-mail-for-desktop/imap-server-settings-company-mail-sln4075.html"">
      <descr lang=""en"">How to setup email applications with imap to receive company! mail?</descr>
    </documentation>
    <documentation url=""https://help.company.com/kb/new-mail-for-desktop/pop-access-settings-instructions-company-mail-sln4724.html"">
      <descr lang=""en"">How to setup email applications with pop to receive company! mail?</descr>
    </documentation>
  </emailProvider>

  <oAuth2>
    <issuer>login.company.com</issuer>
    <scope>mail-w</scope>
    <authURL>https://api.login.company.com/oauth2/request_auth</authURL>
    <tokenURL>https://api.login.company.com/oauth2/get_token</tokenURL>
  </oAuth2>

  <webMail>
    <loginPage url=""https://mail.company.com""/>
    <loginPageInfo url=""https://mail.company.com/"">
      <username>%EMAILADDRESS%</username>
      <usernameField id=""login-username""/>
      <passwordField id=""login-passwd""/>
      <loginButton id=""login-signin""/>
    </loginPageInfo>
  </webMail>
</clientConfig>
";

	}
}