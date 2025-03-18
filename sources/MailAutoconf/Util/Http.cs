namespace MailAutoconf.Util
{
	/// <summary>
	/// This class provides CreateClient() to create a HttpClient suppressing certificate errors.
	/// </summary>
	internal class Http
    {
		/// <summary>
		/// Creates and returns a HttpClient that suppresses any certificate errors.
		/// </summary>
		/// <param name="timeoutMs">the timeout for the network connection in milliseconds</param>
		public static HttpClient CreateClient(int timeoutMs)
		{
			HttpClient httpClient = new(new HttpClientHandler()
			{
				// suppress certificate errors
				ClientCertificateOptions = ClientCertificateOption.Manual,
				ServerCertificateCustomValidationCallback =
					(httpRequestMessage, cert, cetChain, policyErrors) => true,
			})
			{
				Timeout = TimeSpan.FromMilliseconds(timeoutMs)
			};
			return httpClient;
		}
	}
}
