using MailAutoconf.Settings.Sources;

namespace MailAutoconf.Tests.Helper
{
    internal class TestAutodiscoverXml(string mailAddress, int timeoutMs) : AutodiscoverXml(mailAddress, timeoutMs)
    {
        public string XmlString { get; set; }

		protected override Task<string> UriToXmlAsync(string uri) => Task.FromResult(XmlString);
	}
}
