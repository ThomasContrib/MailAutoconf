using MailAutoconf.Settings;

namespace MailAutoconf.Tests.Helper
{
    internal class Assert2
    {
        public static void Pop3ImapSmtp(ServerSettings serverSettings)
        {
            Assert.AreEqual("pop3.company.com", serverSettings.Pop3.Server);
            Assert.AreEqual(995, serverSettings.Pop3.Port);

            Assert.AreEqual("imap.company.com", serverSettings.Imap.Server);
            Assert.AreEqual(993, serverSettings.Imap.Port);

            Assert.AreEqual("smtp.company.com", serverSettings.Smtp.Server);
            Assert.AreEqual(465, serverSettings.Smtp.Port);
        }

        public static void ImapSmtp(ServerSettings serverSettings)
        {
            Assert.IsFalse(serverSettings.Pop3.ServerAndPortSet);

            Assert.AreEqual("imap.company.com", serverSettings.Imap.Server);
            Assert.AreEqual(993, serverSettings.Imap.Port);

            Assert.AreEqual("smtp.company.com", serverSettings.Smtp.Server);
            Assert.AreEqual(465, serverSettings.Smtp.Port);
        }

        public static void UserName(ServerSettings serverSettings)
        {
            Assert.AreEqual("john.doe@company.com", serverSettings.Imap.Credentials.UserName.Name);
            Assert.AreEqual(UserNameType.MailAddress, serverSettings.Imap.Credentials.UserName.Type);

            Assert.AreEqual("john.doe@company.com", serverSettings.Smtp.Credentials.UserName.Name);
            Assert.AreEqual(UserNameType.MailAddress, serverSettings.Imap.Credentials.UserName.Type);
        }
    }
}
