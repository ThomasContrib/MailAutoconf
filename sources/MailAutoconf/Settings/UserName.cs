using System.Net.Mail;

namespace MailAutoconf.Settings
{
	/// <summary>
	/// This class handles the user name to access POP3, IMAP and SMTP servers. It provides methods to
	/// retrieve this name from different sources.
	/// 
	/// Example: AutoconfigXml queries an online database for the user name to use for a given mail 
	/// address.
	/// 
	/// userNameFromDatabase = "%EMAILADDRESS%"
	/// 
	/// userName = UserName.FromPlaceholder(userNameFromDatabase)
	/// -> userName.Name = "%EMAILADDRESS%"
	/// 
	/// userName.UpdateWithMailAddress("john.doe@company.com") 
	/// -> userName.Name = "john.doe@company.com"
	/// </summary>
	public class UserName
    {
		/// <summary>
		/// the user name (e.g. "%EMAILADDRESS%" or "john.doe@company.com")
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// true if the user name is a placeholder like "%EMAILADDRESS%"
		/// </summary>
		public bool NameIsPlaceholder => Name != null && Name.Trim().StartsWith('%') && Name.Trim().EndsWith('%');

        /// <summary>
        /// the type of the user name like MailAddress, RealName etc.
        /// </summary>
        public UserNameType Type { get; set; }

        /// <summary>
        /// true if the user name (property Name) was successfully used to authenticate to a server
        /// </summary>
        public bool AuthenticationSuccessful { get; set; } = false;

        /// <summary>
        /// Assigns the passed placeholder to the property Name and determines the appropriate user name
        /// type.
        /// </summary>
        public static UserName FromPlaceholder(string placeholder)
            => placeholder == null
                ? null
                : new()
                {
                    Name = placeholder,
    
                    // https://benbucksch.github.io/autoconfig-spec/draft-ietf-mailmaint-autoconfig.html#section-3.3
                    Type = placeholder.ToUpper() switch
                    {
                        "%EMAILADDRESS%" => UserNameType.MailAddress,
                        "%EMAILLOCALPART%" => UserNameType.MailLocalPart,
                        "%EMAILDOMAIN%" => UserNameType.MailDomain,
                        "%REALNAME%" => UserNameType.RealName,
                        _ => UserNameType.Unknown,
                    }
                };

		/// <summary>
		/// Updates the property Name by converting the mailAddress according to the current user name
		/// type.
		/// Example: Type == MailLocalPart -> Name = new MailAddress("john.doe@company.com").User -> "john.doe"
		/// </summary>
		public void UpdateWithMailAddress(string mailAddress)
        {
            if (NameIsPlaceholder)
            {
                Name = Type switch
                {
                    UserNameType.MailAddress => mailAddress,
                    UserNameType.MailLocalPart => new MailAddress(mailAddress).User,
                    UserNameType.MailDomain => new MailAddress(mailAddress).Host,
                    UserNameType.RealName => "<real name>",
                    UserNameType.UserName => Name,
                    _ => Name,
                };
            }
        }

        /// <summary>
        /// Returns user name and type as string
        /// </summary>
        public override string ToString() => $"{Name}, {Type}";
    }
}
