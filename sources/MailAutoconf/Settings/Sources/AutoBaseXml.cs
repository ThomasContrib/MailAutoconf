using MailAutoconf.Util;
using System.Collections.Concurrent;
using System.Net.Mail;

namespace MailAutoconf.Settings.Sources
{
	/// <summary>
	/// This class is the base class for AutoconfigXml, AutodiscoverXml and IspDb (the latter 
	/// derived from AutoconfigXml).
	/// </summary>
	/// <remarks>
	/// The primary constructor internally saves mailAddress and timeoutMs to provide them to 
	/// GetServerSettings().
	/// </remarks>
	/// <param name="mailAddress">the mail address for which this class searches server settings</param>
	/// <param name="timeoutMs">the timeout for the network connections in milliseconds</param>
	public abstract class AutoBaseXml(string mailAddress, int timeoutMs) : IServerSettingsSource
	{
		/// <summary>
		/// the mail address for which this class searches server settings
		/// </summary>
		protected readonly string mailAddress = mailAddress;

		/// <summary>
		/// the domain of mailAddress
		/// </summary>
		protected readonly string domain = new MailAddress(mailAddress).Host;

		/// <summary>
		/// the timeout for the network connections in milliseconds
		/// </summary>
		private readonly int timeoutMs = timeoutMs;

		/// <summary>
		/// the URIs to be requested by GetServerSettings()
		/// </summary>
		protected IEnumerable<string> uris;

		/// <summary>
		/// the HttpClient to be used to get the uris
		/// </summary>
		private readonly HttpClient httpClient = Http.CreateClient(timeoutMs);

		/// <summary>
		/// Gets and returns the server settings by searching autodiscover XML files. Does not use/need
		/// the passed server settings. Returns null if nothing found.
		/// </summary>
		public ServerSettings GetServerSettings(ServerSettings _)
		{
			var xmlContents = GetXmlContents();

			var serverSettings = xmlContents
				.Select(XmlToServerSettings)
				.FirstOrDefault()
				?? new();

			serverSettings.SourceName = GetType().Name; // AutoconfigXml, AutodiscoverXml or IspDb
			serverSettings.MailAddress ??= mailAddress;

			return serverSettings;
		}

		private string[] GetXmlContents() => GetXmlContentsAsync().GetAwaiter().GetResult().NotNull().ToArray();

		private async Task<string[]> GetXmlContentsAsync() => await Task.WhenAll(uris.Select(UriToXmlAsync));

		/// <summary>
		/// Gets and returns the content (should be XML) of the passed URI via HTTP. This method has 
		/// 'protected' visibility, so it can be overridden by another class, e.g. for testing purposes.
		/// </summary>
		protected virtual async Task<string> UriToXmlAsync(string uri)
		{
			try
			{
				var response = await httpClient.GetAsync(uri);

				// throw an exception if the status code is not OK
				response.EnsureSuccessStatusCode();

				var xml = await response.Content.ReadAsStringAsync();
				return xml;
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Extracts and returns the server settings from the passed XML string.
		/// </summary>
		protected abstract ServerSettings XmlToServerSettings(string xml);
	}
}