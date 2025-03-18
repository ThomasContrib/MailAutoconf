namespace MailAutoconf.Settings
{
	/// <summary>
	/// the type of the user name (class UserName)
	/// </summary>
	public enum UserNameType
	{
		/// <summary>
		/// type not (yet) known
		/// </summary>
		Unknown,

		/// <summary>
		/// e.g. john.doe@company.com
		/// </summary>
		MailAddress,

		/// <summary>
		/// e.g. john.doe
		/// </summary>
		MailLocalPart,

		/// <summary>
		/// e.g. company.com
		/// </summary>
		MailDomain,

		/// <summary>
		/// e.g. John Doe
		/// </summary>
		RealName,

		/// <summary>
		/// e.g. JohnDoe
		/// </summary>
		UserName
	}
}
