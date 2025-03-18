using MailAutoconf.Settings;

namespace MailAutoconf.PortCheck
{
	/// <summary>
	/// This class implements a filter used by the PortsChecker to filter protocol settings.
	/// 
	/// Example: You want to probe ports not only if they are open but also if you can authenticate
	/// with user name and password. Therefore you filter the protocol settings with ProtocolSettings.
	/// WithUserOnly because it makes no sense to probe ports with protocol settings that have no
	/// user name or password set.
	/// 
	/// portsChecker.CheckPorts(serverSettings, ProtocolSettings.WithUserOnly);
	/// </summary>
	public class ProtocolSettingsFilter
	{
		private readonly List<Func<ProtocolSettings, bool>> acceptConditions = [];

		/// <summary>
		/// Adds the condition passed as function to the internal list of conditions.
		/// </summary>
		public ProtocolSettingsFilter And(Func<ProtocolSettings, bool> accepts)
		{
			acceptConditions.Add(accepts);
			return this;
		}

		/// <summary>
		/// Adds all conditions of the passed filter to the internal list of conditions.
		/// </summary>
		public ProtocolSettingsFilter And(ProtocolSettingsFilter filter)
		{
			acceptConditions.AddRange(filter.acceptConditions);
			return this;
		}

		/// <summary>
		/// Returns true if all the conditions added with And() are fulfilled.
		/// </summary>
		public bool Accepts(ProtocolAndSettings settings) 
			=> acceptConditions.All(acceptCondition => acceptCondition(settings.ProtocolSettings));

		/// <summary>
		/// a filter the accepts all protocol settings, i.e. it does not filter
		/// </summary>
		public static readonly ProtocolSettingsFilter None = new();
	
		/// <summary>
		/// a filter that only accepts procotol settings with user name and password set
		/// </summary>
		public static readonly ProtocolSettingsFilter WithUserOnly = new ProtocolSettingsFilter()
			.And(protocolSettings =>
				protocolSettings.Credentials != null &&
				protocolSettings.Credentials.UserName != null &&
				protocolSettings.Credentials.Password != null);

		/// <summary>
		/// a filter that only accepts protocol settings with Verification == Unverified
		/// </summary>
		public static readonly ProtocolSettingsFilter UnverifiedOnly = new ProtocolSettingsFilter()
			.And(protocolSettings => protocolSettings.Verification == ProtocolVerification.Unverified);
	}
}
