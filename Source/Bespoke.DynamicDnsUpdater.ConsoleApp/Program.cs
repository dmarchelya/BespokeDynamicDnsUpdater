using System;
using System.Configuration;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using Bespoke.DynamicDnsUpdater;
using Bespoke.DynamicDnsUpdater.Client;
using Bespoke.DynamicDnsUpdater.Client.DnsOMatic;
using Bespoke.DynamicDnsUpdater.Common;
using System.Linq;

namespace BespokeDynamicDnsUpdater.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			//Thread.Sleep(10000);

			string firstArg = args.FirstOrDefault();

			if (firstArg != null && firstArg == "-config")
			{
				UpdateConfigSettings(args);
				Console.ReadLine();
			}
			else
			{
				string ipAddress = null;
				bool forceUpdate = false;

				var ipAddressArg = args.SingleOrDefault(a => a.StartsWith("/ipaddress:"));

				if(ipAddressArg != null)
				{
					ipAddress = ipAddressArg.Substring(11, ipAddressArg.Length - 11);
				}

				if(firstArg == "-force")
				{
					forceUpdate = true;
				}

				UpdateHostnames(ipAddress, forceUpdate);
			}
		}

		private static void UpdateConfigSettings(string[] args)
		{
			string clientTypeIdString = string.Empty;
			string username = string.Empty;
			string password = string.Empty;
			string hostnames = string.Empty;
			string encryptionEnabled = string.Empty;
			string secretAccessKeyId = string.Empty;
			string secretAccessKey = string.Empty;

			foreach(var arg in args)
			{
				//we replace any spaces, as its possible that the user had added a space after the colon
				var param = arg.Replace(" ", string.Empty); 
				
				if(param.ToLower().StartsWith("/clienttypeid:"))
				{
					clientTypeIdString = param.Substring(14, param.Length - 14);
				}
				else if(param.ToLower().StartsWith("/username:"))
				{
					username = param.Substring(10, param.Length - 10);
				}
				else if (param.ToLower().StartsWith("/password:"))
				{
					password = param.Substring(10, param.Length - 10);
				}
				else if (param.ToLower().StartsWith("/hostnames:"))
				{
					hostnames = param.Substring(11, param.Length - 11);
				}
				else if(param.ToLower().StartsWith("/encryptionenabled:"))
				{
					encryptionEnabled = param.Substring(19, param.Length - 19);
				}
				else if(param.ToLower().StartsWith("/secretaccesskeyid:"))
				{
					secretAccessKeyId = param.Substring(19, param.Length - 19);
				}
				else if(param.ToLower().StartsWith("/secretaccesskey:"))
				{
					secretAccessKey = param.Substring(17, param.Length - 17);
				}
			}

			if(string.IsNullOrWhiteSpace(clientTypeIdString))
			{
				Console.WriteLine("ClientTypeId not specified.");
				return;
			}

			int clientTypeId = StringUtility.ConvertToInt(clientTypeIdString, -1);

			if (clientTypeId == -1)
			{
				Console.WriteLine("Invalid ClientTypeId specified.");
			}

			DynamicDnsUpdaterClientType clientType;

			try
			{
				clientType = (DynamicDnsUpdaterClientType)clientTypeId;
			}
			catch (Exception)
			{
				Console.WriteLine("Invalid ClientTypeId specified");
				return;
			}

			if(!string.IsNullOrWhiteSpace(password) && encryptionEnabled == "true")
			{
				var encryptionService = new EncryptionService(Convert.FromBase64String(Bespoke.DynamicDnsUpdater.Common.Constants.EncryptionKey), Convert.FromBase64String(Bespoke.DynamicDnsUpdater.Common.Constants.InitializationVector));
				password = encryptionService.EncryptToBase64String(password);
			}

			if(!string.IsNullOrWhiteSpace(secretAccessKey) && encryptionEnabled == "true")
			{
				var encryptionService = new EncryptionService(Convert.FromBase64String(Bespoke.DynamicDnsUpdater.Common.Constants.EncryptionKey), Convert.FromBase64String(Bespoke.DynamicDnsUpdater.Common.Constants.InitializationVector));
				secretAccessKey = encryptionService.EncryptToBase64String(secretAccessKey);
			}

			var pathToApp = System.Reflection.Assembly.GetExecutingAssembly().Location;
			var config = ConfigurationManager.OpenExeConfiguration(pathToApp);

			switch(clientType)
			{
				case DynamicDnsUpdaterClientType.DnsOMatic:
					if (!string.IsNullOrWhiteSpace(username))
					{
						config.AppSettings.Settings.Remove("DnsOMaticUsername");
						config.AppSettings.Settings.Add("DnsOMaticUsername", username);
					}
					if (!string.IsNullOrWhiteSpace(password))
					{
						config.AppSettings.Settings.Remove("DnsOMaticPassword");
						config.AppSettings.Settings.Add("DnsOMaticPassword", password);
					}
					break;
				case DynamicDnsUpdaterClientType.Route53:
					if(!string.IsNullOrWhiteSpace(secretAccessKeyId))
					{
						config.AppSettings.Settings.Remove("AwsAccessKeyId");
						config.AppSettings.Settings.Add("AwsAccessKeyId", secretAccessKeyId);
					}
					if(!string.IsNullOrWhiteSpace(secretAccessKey))
					{
						config.AppSettings.Settings.Remove("AwsSecretAccessKey");
						config.AppSettings.Settings.Add("AwsSecretAccessKey", secretAccessKey);
					}
					break;
				case DynamicDnsUpdaterClientType.Dnsimple:
					if (!string.IsNullOrWhiteSpace(username))
					{
						config.AppSettings.Settings.Remove("DnsimpleUsername");
						config.AppSettings.Settings.Add("DnsimpleUsername", username);
					}
					if (!string.IsNullOrWhiteSpace(password))
					{
						config.AppSettings.Settings.Remove("DnsimplePassword");
						config.AppSettings.Settings.Add("DnsimplePassword", password);
					}
					break;
			}

			config.AppSettings.Settings.Remove("DynamicDnsUpdaterClientTypeId");
			config.AppSettings.Settings.Add("DynamicDnsUpdaterClientTypeId", clientTypeIdString);

			if (!string.IsNullOrWhiteSpace(hostnames))
			{
				config.AppSettings.Settings.Remove("HostnamesToUpdate");
				config.AppSettings.Settings.Add("HostnamesToUpdate", hostnames);
			}

			if (encryptionEnabled != "true")
				encryptionEnabled = "false";

			config.AppSettings.Settings.Remove("EncryptionEnabled");
			config.AppSettings.Settings.Add("EncryptionEnabled", encryptionEnabled);

			config.Save(ConfigurationSaveMode.Modified);
			Console.WriteLine("Config settings updated.");
		}

		private static void UpdateHostnames(string ipAddress, bool forceUpdate)
		{
			string hostnamesToUpdate = ConfigurationManager.AppSettings["HostNamesToUpdate"];

			var updater = new BespokeUpdater(Config.DynamicDnsUpdaterClientTypeId);
			
			//We can force the update if we don't initialize the last ip address that was used.
			//this is useful because of DNS Caching
			if(!forceUpdate)
			{
				updater.Client.InitializeLastUpdateIpAddresses(hostnamesToUpdate);				
			}

			if(!string.IsNullOrWhiteSpace(ipAddress))
			{
				updater.Client.UpdateHostnames(hostnamesToUpdate, ipAddress);				
			}
			else
			{
				updater.Client.UpdateHostnames(hostnamesToUpdate);
			}

			//Console.ReadLine();
		}
	}
}
