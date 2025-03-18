using System.Diagnostics;

namespace MailAutoconf.Util
{
	internal class StopwatchConsole
	{
		private readonly string name;
		private readonly Stopwatch stopwatch = new();

		public StopwatchConsole(string name)
		{
			stopwatch.Start();
			this.name = name;
		}

		public void Stop()
		{
			stopwatch.Stop();
			Console.WriteLine($"{name}: {stopwatch.ElapsedMilliseconds} ms");
		}
	}
}
