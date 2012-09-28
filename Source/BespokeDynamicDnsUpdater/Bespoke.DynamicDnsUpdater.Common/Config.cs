using System;
using System.Configuration;
using NLog;

namespace Bespoke.DynamicDnsUpdater.Common
{
	public static class Config
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public static int DynamicDnsUpdaterClientTypeId
		{
			get { return StringUtility.ConvertToInt(ConfigurationManager.AppSettings["DynamicDnsUpdaterClientTypeId"], 1); }
		}

		public static bool EncryptionEnabled
		{
			get { return StringUtility.ConvertToBool(ConfigurationManager.AppSettings["EncryptionEnabled"], true); }
		}

		public static string DnsOMaticUsername
		{
			get { return ConfigurationManager.AppSettings["DnsOMaticUsername"]; }
			set { ConfigurationManager.AppSettings["DnsOMaticUsername"] = value; }
		}

		public static string DnsOMaticPassword
		{
			get
			{
				if(EncryptionEnabled)
				{
					try
					{
						var encryptionService = new EncryptionService(Convert.FromBase64String(Constants.EncryptionKey), Convert.FromBase64String(Constants.InitializationVector));
						return encryptionService.DecryptBase64StringToString(ConfigurationManager.AppSettings["DnsOMaticPassword"]);
					}
					catch (Exception ex)
					{
						logger.Error(ex);
						return null;
					}
				}
				else
				{
					return ConfigurationManager.AppSettings["DnsOMaticPassword"];										
				}
			}
			set
			{
				if(EncryptionEnabled)
				{
					var encryptionService = new EncryptionService(Convert.FromBase64String(Constants.EncryptionKey), Convert.FromBase64String(Constants.InitializationVector));
					ConfigurationManager.AppSettings["DnsOMaticPassword"] = encryptionService.EncryptToBase64String(value);
				}
				else
				{
					ConfigurationManager.AppSettings["DnsOMaticPassword"] = value;					
				}
			}
		}

		public static string HostnamesToUpdate
		{
			get { return ConfigurationManager.AppSettings["HostnamesToUpdate"]; }
			set { ConfigurationManager.AppSettings["HostnamesToUpdate"] = value; }
		}

		public static string AwsAccessKeyId
		{
			get { return ConfigurationManager.AppSettings["AwsAccessKeyId"]; }
		}

		public static string AwsSecretAccessKey
		{
			get
			{
				if(EncryptionEnabled)
				{
					var encryptionService = new EncryptionService(Convert.FromBase64String(Constants.EncryptionKey), Convert.FromBase64String(Constants.InitializationVector));
					return encryptionService.DecryptBase64StringToString(ConfigurationManager.AppSettings["AwsSecretAccessKey"]);
				}
				else
				{
					return ConfigurationManager.AppSettings["AwsSecretAccessKey"];										
				}
			}
		}

		public static string DnsimpleUsername
		{
			get { return ConfigurationManager.AppSettings["DnsimpleUsername"]; }
		}

		public static string DnsimplePassword
		{
			get { return ConfigurationManager.AppSettings["DnsimplePassword"]; }
		}
	}
}
