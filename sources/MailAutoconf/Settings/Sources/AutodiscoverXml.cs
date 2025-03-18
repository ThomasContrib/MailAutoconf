using MailAutoconf.Types;
using MailAutoconf.Util;
using System.Xml;

namespace MailAutoconf.Settings.Sources
{
	/// <summary>
	/// This class uses Exchange Autodiscover to determine the server settings for IMAP and SMTP.
	/// https://tech-tip.de/was-ist-autodiscover-und-wie-funktioniert-es/
	/// </summary>
	public class AutodiscoverXml : AutoBaseXml
	{
		/// <summary>
		/// Initializes the URIs to be requested by GetServerSettings().
		/// </summary>
		/// <param name="mailAddress">the mail address for which this class searches server settings</param>
		/// <param name="timeoutMs">the timeout for the network connections in milliseconds</param>
		public AutodiscoverXml(string mailAddress, int timeoutMs) : base(mailAddress, timeoutMs)
		{
			uris =
			[
				$"https://{domain}/autodiscover/autodiscover.xml",
				$"https://autodiscover.{domain}/autodiscover/autodiscover.xml",
				$"http://autodiscover.{domain}/autodiscover/autodiscover.xml",
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

			string protocolPath = "/Autodiscover/Response/Account/Protocol";

			bool IsPop3(XmlNode node)
				=> node.ChildNodes.Cast<XmlNode>().Any(node => node.Name.EqualTo("Type") && node.InnerXml.EqualTo("POP3"));

			bool IsImap(XmlNode node)
				=> node.ChildNodes.Cast<XmlNode>().Any(node => node.Name.EqualTo("Type") && node.InnerXml.EqualTo("IMAP"));

			bool IsSmtp(XmlNode node)
				=> node.ChildNodes.Cast<XmlNode>().Any(node => node.Name.EqualTo("Type") && node.InnerXml.EqualTo("SMTP"));

			ProtocolSettings ToProtocolSettings(XmlNode node)
			{
				SocketType GetSocketType()
				{
					var sslNode = node["SSL"];
					return sslNode == null
						? SocketType.Unknown
						: sslNode.InnerText.EqualTo("on") ? SocketType.SslTls : SocketType.Plain;
				}

				return new()
				{
					Server = node["Server"].InnerText,
					Port = int.Parse(node["Port"].InnerText),
					SocketType = GetSocketType(),
					Credentials = new() { UserName = UserName.FromPlaceholder(node["LoginName"]?.InnerText) ?? new() },
					Authentication = node["AuthRequired"].InnerText.EqualTo("on")
						? "authentication required"
						: "authentication not required",
				};
			}

			var settings = doc
				.SelectNodesNsIgnore(protocolPath)
				.Cast<XmlNode>();

			ServerSettings serverSettings = new()
			{
				Pop3 = settings.Where(IsPop3).Select(ToProtocolSettings).FirstOrDefault() ?? new(),
				Imap = settings.Where(IsImap).Select(ToProtocolSettings).FirstOrDefault() ?? new(),
				Smtp = settings.Where(IsSmtp).Select(ToProtocolSettings).FirstOrDefault() ?? new(),
			};

			serverSettings.Pop3.Credentials?.UserName?.UpdateWithMailAddress(mailAddress);
			serverSettings.Imap.Credentials?.UserName?.UpdateWithMailAddress(mailAddress);
			serverSettings.Smtp.Credentials?.UserName?.UpdateWithMailAddress(mailAddress);

			return serverSettings;
		}
	}
}