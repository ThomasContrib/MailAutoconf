using MailAutoconf.Types;
using MailAutoconf.Util;
using System.Xml;

namespace MailAutoconf.Settings.Sources
{
	/// <summary>
	/// This class uses Mail Autoconfig to determine the server settings for POP3, IMAP and SMTP.
	/// https://benbucksch.github.io/autoconfig-spec/draft-ietf-mailmaint-autoconfig.html
	/// </summary>
	public class AutoconfigXml : AutoBaseXml
	{
		/// <summary>
		/// Initializes the URIs to be requested by GetServerSettings().
		/// </summary>
		/// <param name="mailAddress">the mail address for which this class searches server settings</param>
		/// <param name="timeoutMs">the timeout for the network connections in milliseconds</param>
		public AutoconfigXml(string mailAddress, int timeoutMs) : base(mailAddress, timeoutMs)
		{
			// https://benbucksch.github.io/autoconfig-spec/draft-ietf-mailmaint-autoconfig.html#name-mail-provider
			uris =
			[
				$"https://autoconfig.{domain}/mail/config-v1.1.xml?emailaddress={mailAddress}",
				$"https://{domain}/.well-known/autoconfig/mail/config-v1.1.xml",
				$"http://autoconfig.{domain}/mail/config-v1.1.xml",
			];
		}

		/// <summary>
		/// Extracts and returns the server settings from the passed XML string.
		/// </summary>
		protected override ServerSettings XmlToServerSettings(string xml)
		{
			if (xml == null) return new();

			XmlDocument doc = new();
			try
			{
				doc.LoadXml(xml);
			}
			catch (XmlException)
			{
				// xml does not seem to contain valid XML, therefore return null.
				return null;
			}

			var incoming = doc
				.SelectNodes("/clientConfig/emailProvider/incomingServer")
				.Cast<XmlNode>();

			var outgoing = doc
				.SelectNodes("/clientConfig/emailProvider/outgoingServer")
				.Cast<XmlNode>();

			bool IsPop3(XmlNode node)
				=> node.Attributes.Cast<XmlAttribute>().Any(attr => attr.Name.EqualTo("type") && attr.Value.EqualTo("pop3"));

			bool IsImap(XmlNode node)
				=> node.Attributes.Cast<XmlAttribute>().Any(attr => attr.Name.EqualTo("type") && attr.Value.EqualTo("imap"));

			bool IsSmtp(XmlNode node)
				=> node.Attributes.Cast<XmlAttribute>().Any(attr => attr.Name.EqualTo("type") && attr.Value.EqualTo("smtp"));

			ProtocolSettings ToProtocolSettings(XmlNode node)
			{
				return node == null
					? new()
					: new()
					{
						Server = node["hostname"]?.InnerText,
						Port = int.Parse(node["port"]?.InnerText),
						SocketType = SocketType_.Parse(node["socketType"]?.InnerText),
						Credentials = new() { UserName = UserName.FromPlaceholder(node["username"]?.InnerText) ?? new() },
						Authentication = node.SelectNodes($"authentication")
						.Cast<XmlNode>()
						.Select(node => node.InnerText)
						.Distinct()
						.CommaDelimitedString(),
					};
			}

			ServerSettings serverSettings = new()
			{
				Pop3 = incoming.Where(IsPop3).Select(ToProtocolSettings).FirstOrDefault() ?? new(),
				Imap = incoming.Where(IsImap).Select(ToProtocolSettings).FirstOrDefault() ?? new(),
				Smtp = outgoing.Where(IsSmtp).Select(ToProtocolSettings).FirstOrDefault() ?? new(),
			};

			serverSettings.Pop3.Credentials?.UserName?.UpdateWithMailAddress(mailAddress);
			serverSettings.Imap.Credentials?.UserName?.UpdateWithMailAddress(mailAddress);
			serverSettings.Smtp.Credentials?.UserName?.UpdateWithMailAddress(mailAddress);

			return serverSettings;
		}
	}
}