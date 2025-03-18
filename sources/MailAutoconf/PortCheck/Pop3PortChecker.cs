using MailKit;
using System.Text;
using MailKit.Security;
using MailAutoconf.Types;
using MailAutoconf.Util;
using MailKit.Net.Pop3;

namespace MailAutoconf.PortCheck
{
	/// <summary>
	/// This class probes POP3 ports. It tries to connect and authenticate at these ports.
	/// CheckPort() returns an array with the results. It contains an entry per mode to
	/// probe.
	/// </summary>
	/// <param name="port">the POP3 port to probe</param>
	/// <param name="modes">the modes to use for probing (plain, SSL etc.)</param>
	public class Pop3PortChecker(int port, params SocketType[] modes) : IPortChecker
    {
		/// <summary>
		/// the protocol handled by this PortChecker (POP3)
		/// </summary>
		public Protocol Protocol { get; } = Protocol.POP3;

		/// <summary>
		/// the port to probe
		/// </summary>
		public int Port { get; } = port;

		/// <summary>
		/// Probes the POP3 port passed to the constructor. Tries to connect and authenticate at this ports. 
		/// Returns an array with the results. It contains an entry per mode to probe.
		/// </summary>
		/// <param name="server">the server to probe</param>
		/// <param name="userName">the user name for authentification (POP3)</param>
		/// <param name="password">the password for authentification (POP3)</param>
		/// <param name="timeoutMs">the timeout for the network connection in milliseconds</param>
		/// <returns>array with the probe results</returns>
		public Task<PortCheckResult>[] CheckPort(string server, string userName, 
            string password, int timeoutMs)
            => modes.Select(mode => CheckPort(server, userName, password, mode, timeoutMs)).ToArray();

        private async Task<PortCheckResult> CheckPort(string server, string userName, string password, 
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
				Protocol = Protocol.POP3,
				ConnectSuccessful = false,
				AuthenticateSuccessful = false,
				TimedOut = false,
				ResultString = new(userName, password)
			};

			MemoryStream logCollector = new();
            Pop3Client client = new(new ProtocolLogger(logCollector))
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
                result.ProtocolSettings.Credentials.UserName.AuthenticationSuccessful = true;
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
