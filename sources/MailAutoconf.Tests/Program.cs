namespace MailAutoconf.Tests
{
	internal class Program
	{
		public static void Main()
		{
			//new AutoconfigXmlTests().ImapSmtp();
			//new AutoconfigXmlTests().InvalidEntries();
			//new AutodiscoverXmlTests().ImapSmtp();
			//new AutodiscoverXmlTests().NoEntries();
			//new CombineTests().Combine_TwoSettings();
			new SettingsBuilderTests().DefaultSources();
			new SettingsBuilderTests().SomeSources();
			//new PortsCheckerTests().CheckPorts_EmptySettings();
		}
	}
}
