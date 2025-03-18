using MailAutoconf.Settings.Sources;

namespace MailAutoconf.Tests.Helper
{
    internal class TestAutoconfigXml(string mailAddress, int timeoutMs) : AutoconfigXml(mailAddress, timeoutMs)
    {
        public string XmlString { get; set; }

        protected override Task<string> UriToXmlAsync(string uri) => Task.FromResult(XmlString);
    }
}
