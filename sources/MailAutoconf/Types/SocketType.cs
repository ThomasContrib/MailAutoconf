namespace MailAutoconf.Types
{
    /// <summary>
    /// the type for network connections
    /// </summary>
    public enum SocketType
    {
        /// <summary>
        ///  initial state if type not yet known
        /// </summary>
        Unknown,

        /// <summary>
        /// SSL or TLS
        /// </summary>
        SslTls,

		/// <summary>
		/// upgrades plain text connection to an encrypted one
		/// </summary>
		StartTls,

        /// <summary>
        /// no encryption
        /// </summary>
        Plain,
    }

    /// <summary>
    /// This class provides Parse() to convert a string to SocketType.
    /// </summary>
    public class SocketType_
    {
        /// <summary>
        /// Converts the string (PLAIN, STARTTLS or SSL) to a SocketType. Converts null to SocketType.Unknown.
        /// </summary>
        public static SocketType Parse(string s)
            => s.ToUpper() switch
            {
                "PLAIN" => SocketType.Plain,
                "STARTTLS" => SocketType.StartTls,
                "SSL" => SocketType.SslTls,
                null => SocketType.Unknown,
                _ => throw new ArgumentException($"Unknown socket type: {s}"),
            };
    }
}