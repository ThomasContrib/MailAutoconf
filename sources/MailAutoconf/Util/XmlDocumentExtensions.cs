using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace MailAutoconf.Util
{
	/// <summary>
	/// This extension class contains some methods to manipulate XML content. It uses System.Xml.
	/// </summary>
	internal static class XmlDocumentExtensions
	{
		/// <summary>
		/// Returns the same result as SelectNodes(xpath), but ignores namespaces and the result is a generic enumerable.
		/// </summary>
		public static IEnumerable<XmlNode> SelectNodesNsIgnore(this XmlDocument doc, string xpath)
		{
			return doc.SelectNodes(xpath.NsIgnore()).ToEnum();
		}

		/// <summary>
		/// Modifies xpath such as methods like XmlDocument.SelectNodes(...) ignore any namespaces.
		/// 
		/// Example:
		/// //Folder//LineStyle/color
		/// -> //*[local-name()='Folder']//*[local-name()='LineStyle']/*[local-name()='color']
		/// 
		/// See https://stackoverflow.com/a/12607946 for details.
		/// </summary>
		public static string NsIgnore(this string xpath)
			=> Regex.Replace(xpath, pattern: @"(\/+)([^\/]+)", replacement: @"$1*[local-name()='$2']");

		/// <summary>
		/// Converts an XmlNodeList to <![CDATA[IEnumerable<XmlNode>]]>.
		/// </summary>
		public static IEnumerable<XmlNode> ToEnum(this XmlNodeList nodeList) => nodeList.Cast<XmlNode>();

		/// <summary>
		/// Returns a valid xpath for node, e.g. /kml/Folder/Style/LineStyle/color.
		/// </summary>
		public static string XPath(this XmlNode node)
		{
			List<string> names = [];
			while (node != null && node is XmlElement)
			{
				names.Add(node.Name);
				node = node.ParentNode;
			}
			names.Reverse();
			return $"/{string.Join('/', names)}";
		}
	}
}
