using System.Configuration;

namespace Bespoke.DynamicDnsUpdater.Common
{
	public static class Config
	{
		public static string DnsOMaticUsername
		{
			get { return ConfigurationManager.AppSettings["DnsOMaticUsername"]; }
			set { ConfigurationManager.AppSettings["DnsOMaticUsername"] = value; }
		}

		public static string DnsOMaticPassword
		{
			get { return ConfigurationManager.AppSettings["DnsOMaticPassword"]; }
			set { ConfigurationManager.AppSettings["DnsOMaticPassword"] = value; }
		}

		public static string HostnamesToUpdate
		{
			get { return ConfigurationManager.AppSettings["HostnamesToUpdate"]; }
			set { ConfigurationManager.AppSettings["HostnamesToUpdate"] = value; }
		}
	}
}
