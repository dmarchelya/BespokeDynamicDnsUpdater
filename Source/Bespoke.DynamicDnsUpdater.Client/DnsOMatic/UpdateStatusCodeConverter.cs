namespace Bespoke.DynamicDnsUpdater.Client.DnsOMatic
{
	public static class UpdateStatusCodeConverter
	{
		/// <summary>
		/// Takes the update status code string that is returned from DNS-O-Matic and
		/// converts it into its proper enum value.
		/// </summary>
		/// <param name="nativeUpdateStatusCode"></param>
		/// <returns></returns>
		public static UpdateStatusCode GetUpdateStatusCode(string nativeUpdateStatusCode)
		{
			switch(nativeUpdateStatusCode)
			{
				case "good" :
					return UpdateStatusCode.Good;
				case "nochg" :
					return UpdateStatusCode.NoChange;
				case "badauth" :
					return UpdateStatusCode.BadAuth;
				case "notfqdn" :
					return UpdateStatusCode.NotFullyQualifiedDomainName;
				case "nohost" :
					return UpdateStatusCode.NoHost;
				case "numhost" :
					return UpdateStatusCode.NumberOfHosts;
				case "abuse" :
					return UpdateStatusCode.Abuse;
				case "badagent" :
					return UpdateStatusCode.BadAgent;
				case "dnserr" :
					return UpdateStatusCode.DnsErrror;
				case "911" :
					return UpdateStatusCode.DnsOMaticError;
				default :
					return UpdateStatusCode.Unknown;
			}
		}
	}
}
