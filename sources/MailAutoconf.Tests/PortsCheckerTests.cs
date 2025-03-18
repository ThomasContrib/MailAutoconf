using MailAutoconf.PortCheck;
using MailAutoconf.Settings;
using MailAutoconf.Types;

namespace MailAutoconf.Tests
{
	/// <summary>
	/// This class tests the PortsChecker.
	/// </summary>
    [TestClass]
    public class PortsCheckerTests
    {
        /// <summary>
        /// Ensures that the ports checker probes no port for server settings without any servers 
        /// set.
        /// </summary>
        [TestMethod]
        public void CheckPorts_EmptySettings()
        {
            var portsChecker = (TestPortsChecker)new TestPortsChecker(timeoutMs: 2000)
                .AddPorts(PortsChecker.DefaultPop3ImapSmtpPorts);

            ServerSettings serverSettings = new();

            portsChecker.CheckPorts(serverSettings);

            // Ensure that no port is probed because of the empty server settings.
            Assert.AreEqual(0, portsChecker.Probed.Length);
        }

        /// <summary>
        /// Ensures that the ports checker probes two ports (143 and 993) for server settings with
        /// only an IMAP server set. Furthermore ensures that the ports checker does not touch
        /// the Verification property of the protocol settings.
        /// </summary>
		[TestMethod]
		public void CheckPorts_SettingsForImapOnly()
		{
			var portsChecker = (TestPortsChecker)new TestPortsChecker(timeoutMs: 2000)
				.AddPorts(PortsChecker.DefaultPop3ImapSmtpPorts);

            ServerSettings serverSettings = new()
            {
                Imap = new() { Server = "test.server.com" },
            };

            Assert.AreEqual(ProtocolVerification.Unverified, serverSettings.Imap.Verification);

			portsChecker.CheckPorts(serverSettings);

			// Ensure that two ports (143, 993) are probed because of the IMAP protocol settings.
			Assert.AreEqual(2, portsChecker.Probed.Length);
            
            Assert.AreEqual(Protocol.IMAP, portsChecker.Probed[0].Settings.Protocol);
            Assert.AreEqual(143, portsChecker.Probed[0].PortChecker.Port);

            Assert.AreEqual(Protocol.IMAP, portsChecker.Probed[1].Settings.Protocol);
            Assert.AreEqual(993, portsChecker.Probed[1].PortChecker.Port);

			Assert.AreEqual(ProtocolVerification.Unverified, serverSettings.Imap.Verification);
		}

        /// <summary>
        /// Ensures that the ports checker probes two ports (100 and 200) for server settings
        /// with IMAP and SMTP server set. The ports to probe are not the default ones. The
        /// ports of the server settings (100 and 200) are used instead.
        /// </summary>
		[TestMethod]
		public void CheckPorts_ServerSettingsPortsOnly()
		{
			ServerSettings serverSettings = new()
			{
				Imap = new() { Server = "test.server.com", Port = 100 },
				Smtp = new() { Server = "test.server.com", Port = 200 },
			};

			var portsChecker = (TestPortsChecker)new TestPortsChecker(timeoutMs: 2000)
				.AddPorts(PortsChecker.PortsFrom(serverSettings));

			portsChecker.CheckPorts(serverSettings);

			// Ensure that two ports (100, 200) are probed because of the IMAP/SMTP protocol settings.
			Assert.AreEqual(2, portsChecker.Probed.Length);

			Assert.AreEqual(Protocol.IMAP, portsChecker.Probed[0].Settings.Protocol);
			Assert.AreEqual(100, portsChecker.Probed[0].PortChecker.Port);

            Assert.AreEqual(Protocol.SMTP, portsChecker.Probed[1].Settings.Protocol);
            Assert.AreEqual(200, portsChecker.Probed[1].PortChecker.Port);
        }

		/// <summary>
		/// Ensures that the ports checker sets the Verification property correctly for IMAP but 
		/// not for the other protocol settings. Only IMAP is configured within the server settings.
		/// </summary>
		[TestMethod]
		public void CheckPorts_Verify()
		{
			ServerSettings serverSettings = new()
			{
				Imap = new() { Server = "test.server.com", Port = 100 },
			};

			var portsChecker = (TestPortsChecker)new TestPortsChecker(timeoutMs: 2000)
				.AddPorts(PortsChecker.PortsFrom(serverSettings));

			portsChecker.CheckPorts(serverSettings, verify: true);

			// Ensure that two ports (100, 200) are probed because of the IMAP/SMTP protocol settings.
			Assert.AreEqual(1, portsChecker.Probed.Length);

			Assert.AreEqual(ProtocolVerification.Unverified, serverSettings.Pop3.Verification);
			Assert.AreEqual(ProtocolVerification.Passed, serverSettings.Imap.Verification);
			Assert.AreEqual(ProtocolVerification.Unverified, serverSettings.Smtp.Verification);
		}

		/// <summary>
		/// This is a helper class for testing purposes. It is derived from PortsChecker and 
		/// overrides CheckPort() to keep track of when this method is called and with what 
		/// parameters.
		/// </summary>
		private class TestPortsChecker(int timeoutMs) : PortsChecker(timeoutMs)
		{
            private readonly List<PortCheckerAndSettings> probed = [];
            public PortCheckerAndSettings[] Probed => probed.ToArray();

			protected override Task<PortCheckResult>[] CheckPort(IPortChecker portChecker, ProtocolAndSettings settings)
			{
                probed.Add(new(portChecker, settings));

                Task<PortCheckResult> CheckPort_()
                {
                    return Task.FromResult(new PortCheckResult()
                    {
                        AuthenticateSuccessful = true,
                        ConnectSuccessful = true,
                        Protocol = settings.Protocol,
                        ProtocolSettings = settings.ProtocolSettings,
                        TimedOut = false,
                    });
                }
                return [ CheckPort_() ];
			}
		}

		private class PortCheckerAndSettings(IPortChecker portChecker, ProtocolAndSettings settings)
		{
			public IPortChecker PortChecker { get; } = portChecker;
			public ProtocolAndSettings Settings { get; } = settings;
		}
	}
}
