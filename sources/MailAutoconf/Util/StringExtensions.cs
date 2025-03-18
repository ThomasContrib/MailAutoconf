using System.Text;

namespace MailAutoconf.Util
{
	internal static class StringExtensions
	{
		public static string NewLineDelimitedString(this IEnumerable<string> values)
			=> values.DelimitedString("\n");

		public static string CommaDelimitedString(this IEnumerable<string> values)
			=> values.DelimitedString(", ");

		public static string DelimitedString(this IEnumerable<string> values, string delimiter)
		{
			if (values == null) return null;
			return string.Join(delimiter, values);
		}

		public static bool EqualTo(this string thisString, string otherString, bool ignoreCase = true) 
			=> string.Equals(
				thisString,
				otherString,
				ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture);

		public static void WriteFile(this string text, FileInfo file, bool open = false)
		{
			File.WriteAllText(file.FullName, text, Encoding.UTF8);
			if (open)
			{
				Process2.Start(file.FullName);
			}
		}

		public static IEnumerable<string> ToLines(this string text) 
			=> text.Split(["\r\n", "\n"], StringSplitOptions.None);
	}
}
