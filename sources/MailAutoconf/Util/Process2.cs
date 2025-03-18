using System.Diagnostics;

namespace MailAutoconf.Util
{
	internal class Process2
	{
		public static void Start(string fileName)
		{
			ProcessStartInfo startInfo = new(fileName)
			{
				UseShellExecute = true,
			};
			Process.Start(startInfo);
		}
	}
}
