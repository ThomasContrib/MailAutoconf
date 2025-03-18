using MailKit;
using System.Text;
using MailKit.Security;
using MailKit.Net.Smtp;
using MailAutoconf.Types;
using MailAutoconf.Util;

namespace MailAutoconf.PortCheck
{
	/// <summary>
	/// This class probes SMTP ports. It tries to connect and authenticate at these ports.
	/// CheckPort() returns an array with the results. It contains an entry per mode to
	/// probe.
	/// </summary>
	/// <param name="port">the SMTP port to probe</param>
	/// <param name="modes">the modes to use for probing (plain, SSL etc.)</param>
	public class SmtpPortChecker(int port, params SocketType[] modes) : IPortChecker
	{
		/// <summary>
		/// the protocol handled by this PortChecker (SMTP)
		/// </summary>
		public Protocol Protocol { get; } = Protocol.SMTP;

		/// <summary>
		/// the port to probe
		/// </summary>
		public int Port { get; } = port;

		/// <summary>
		/// Probes the SMTP port passed to the constructor. Tries to connect and authenticate at this ports. 
		/// Returns an array with the results. It contains an entry per mode to probe.
		/// </summary>
		/// <param name="server">the server to probe</param>
		/// <param name="userName">the user name for authentification (SMTP)</param>
		/// <param name="password">the password for authentification (SMTP)</param>
		/// <param name="timeoutMs">the timeout for the network connection in milliseconds</param>
		/// <returns>array with the probe results</returns>
		public Task<PortCheckResult>[] CheckPort(string server, string userName,
			string password, int timeoutMs)
			=> modes.Select(mode => Check(server, userName, password, mode, timeoutMs)).ToArray();

		private async Task<PortCheckResult> Check(string server, string userName, string password,
			SocketType socketType, int timeoutMs)
		{
			PortCheckResult result = new()
			{
				ProtocolSettings = new()
				{
					Server = server,
					Port = Port,
					SocketType = socketType,
					Credentials = new() { UserName = new() { Name = userName }, Password = password }
				},
				Protocol = Protocol.SMTP,
				ConnectSuccessful = false,
				AuthenticateSuccessful = false,
				TimedOut = false,
				ResultString = new(userName, password)
			};

			MemoryStream logCollector = new();
			SmtpClient client = new(new ProtocolLogger(logCollector))
			{
				Timeout = timeoutMs,
			};

			// https://stackoverflow.com/a/52653239
			try
			{
				switch (socketType)
				{
					case SocketType.Plain:
						await client.ConnectAsync(server, Port, useSsl: false);
						break;
					case SocketType.SslTls:
						await client.ConnectAsync(server, Port, useSsl: true);
						break;
					case SocketType.StartTls:
						await client.ConnectAsync(server, Port, SecureSocketOptions.StartTls);
						break;
				}
				result.ConnectSuccessful = true;
			}
			catch (Exception e)
			{
				result.ResultString.ExceptionMessage = e.Message;
				result.ResultString.DetailInfo = logCollector.GetString();
				result.TimedOut = e is TimeoutException;
				return result;
			}

			try
			{
				await client.AuthenticateAsync(userName, password);
				result.AuthenticateSuccessful = true;
			}
			catch (Exception e)
			{
				result.ResultString.ExceptionMessage = e.Message;
				result.ResultString.DetailInfo = logCollector.GetString();
				result.TimedOut = e is TimeoutException;
				return result;
			}

			result.ResultString.DetailInfo = logCollector.GetString();
			await client.DisconnectAsync(quit: true);

			return result;
		}
	}
}
