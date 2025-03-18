using MailAutoconf.Settings.Sources;

namespace MailAutoconf.Tests.Helper
{
    internal class TestIspDb(string mailAddress, int timeoutMs) : IspDb(mailAddress, timeoutMs)
    {
        public string XmlString { get; set; }

        protected override Task<string> UriToXmlAsync(string uri) => Task.FromResult(XmlString);
    }
}
