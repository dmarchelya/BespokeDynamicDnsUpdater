using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.ServiceProcess;
using System.Text;
using Bespoke.DynamicDnsUpdater.Client;
using Bespoke.DynamicDnsUpdater.Common;
using Microsoft.Deployment.WindowsInstaller;

namespace Bespoke.DynamicDnsUpdater.WindowsServiceInstallerCustomActions
{
	public class CustomActions
	{
		[CustomAction]
		public static ActionResult UpdateAppConfig(Session session)
		{
			//We only update the app config when its destination path has been provided
			//as well as the DNS-O-Matic settings
			if (!string.IsNullOrWhiteSpace(session.CustomActionData["INSTALLDIR"]))
			{
				var dnsClientType = GetDnsClientType(session);

				string targetDir = session.CustomActionData["INSTALLDIR"];

				const string applicationName = "Bespoke.DynamicDnsUpdater.WindowsService.exe";
				var exePath = System.IO.Path.Combine(targetDir, applicationName);

				//var exePath = targetDir + @"\" + applicationName;
				var config = ConfigurationManager.OpenExeConfiguration(exePath);

				//Parameters are passed from the Installer
				//otherwise we'll assume the values are already in the config file.

				SetDnsClientTypeId(session, config);

				SetHostnames(session, config);

				if (dnsClientType == DynamicDnsUpdaterClientType.Route53)
				{
					SetAccessKeyId(session, config, dnsClientType);
					SetSecretAccessKey(session, config, dnsClientType);
				}
				else
				{
					SetUsername(session, dnsClientType, config);
					SetPassword(session, config, dnsClientType);
				}

				config.Save(ConfigurationSaveMode.Modified);
			}

			return ActionResult.Success;
		}

		private static void SetDnsClientTypeId(Session session, Configuration config)
		{
			string dnsClientTypeId = session.CustomActionData["DNSCLIENTTYPEID"];
			if (!string.IsNullOrWhiteSpace(dnsClientTypeId))
			{
				config.AppSettings.Settings.Remove("DynamicDnsUpdaterClientTypeId");
				config.AppSettings.Settings.Add("DynamicDnsUpdaterClientTypeId", dnsClientTypeId);
			}
		}

		private static void SetHostnames(Session session, Configuration config)
		{
			string hostnames = session.CustomActionData["HOSTNAMES"];
			if (!string.IsNullOrWhiteSpace(hostnames))
			{
				config.AppSettings.Settings.Remove("HostnamesToUpdate");
				config.AppSettings.Settings.Add("HostnamesToUpdate", hostnames);
			}
		}

		private static void SetPassword(Session session, Configuration config, DynamicDnsUpdaterClientType dnsClientType)
		{
			string dnsClientPassword = session.CustomActionData["DNSCLIENTPASSWORD"];
			if (!string.IsNullOrWhiteSpace(dnsClientPassword))
			{
				bool encryptionEnabled = Convert.ToBoolean(config.AppSettings.Settings["EncryptionEnabled"].Value);

				string password = string.Empty;

				if (encryptionEnabled)
				{
					var encryptionService = new EncryptionService(Convert.FromBase64String(Common.Constants.EncryptionKey),
					                                              Convert.FromBase64String(Common.Constants.InitializationVector));
					password = encryptionService.EncryptToBase64String(dnsClientPassword);
				}
				else
				{
					password = dnsClientPassword;
				}

				switch (dnsClientType)
				{
					case DynamicDnsUpdaterClientType.DnsOMatic:
						config.AppSettings.Settings.Remove("DnsOMaticPassword");
						config.AppSettings.Settings.Add("DnsOMaticPassword", password);
						break;
					case DynamicDnsUpdaterClientType.Dnsimple:
						config.AppSettings.Settings.Remove("DnsimplePassword");
						config.AppSettings.Settings.Add("DnsimplePassword", password);
						break;
					default:
						break;
				}
			}
		}

		private static void SetUsername(Session session, DynamicDnsUpdaterClientType dnsClientType, Configuration config)
		{
			string dnsClientUsername = session.CustomActionData["DNSCLIENTUSERNAME"];
			if (!string.IsNullOrWhiteSpace(dnsClientUsername))
			{
				switch (dnsClientType)
				{
					case DynamicDnsUpdaterClientType.DnsOMatic:
						config.AppSettings.Settings.Remove("DnsOMaticUsername");
						config.AppSettings.Settings.Add("DnsOMaticUsername", dnsClientUsername);
						break;
					case DynamicDnsUpdaterClientType.Dnsimple:
						config.AppSettings.Settings.Remove("DnsimpleUsername");
						config.AppSettings.Settings.Add("DnsimpleUsername", dnsClientUsername);
						break;
					default:
						break;
				}
			}
		}

		private static void SetAccessKeyId(Session session, Configuration config, DynamicDnsUpdaterClientType dnsClientType)
		{
			switch (dnsClientType)
			{
				case DynamicDnsUpdaterClientType.Route53:
					string accessKeyId = session.CustomActionData["ACCESSKEYID"];
					if (!string.IsNullOrWhiteSpace(accessKeyId))
					{
						config.AppSettings.Settings.Remove("AwsAccessKeyId");
						config.AppSettings.Settings.Add("AwsAccessKeyId", accessKeyId);
					}
					break;
				default:
					break;
			}
		}

		private static void SetSecretAccessKey(Session session, Configuration config, DynamicDnsUpdaterClientType dnsClientType)
		{
			switch (dnsClientType)
			{
				case DynamicDnsUpdaterClientType.Route53:
					string secretAccessKey = session.CustomActionData["SECRETACCESSKEY"];
					if (!string.IsNullOrWhiteSpace(secretAccessKey))
					{
						bool encryptionEnabled = Convert.ToBoolean(config.AppSettings.Settings["EncryptionEnabled"].Value);

						if (encryptionEnabled)
						{
							var encryptionService = new EncryptionService(Convert.FromBase64String(Constants.EncryptionKey), Convert.FromBase64String(Constants.InitializationVector));
							secretAccessKey = encryptionService.EncryptToBase64String(secretAccessKey);
						}

						config.AppSettings.Settings.Remove("AwsSecretAccessKey");
						config.AppSettings.Settings.Add("AwsSecretAccessKey", secretAccessKey);
					}
					break;
				default:
					break;
			}
		}

		private static DynamicDnsUpdaterClientType GetDnsClientType(Session session)
		{
			string dnsClientTypeIdString = session.CustomActionData["DNSCLIENTTYPEID"];
			DynamicDnsUpdaterClientType dnsClientType;

			if (!string.IsNullOrWhiteSpace(dnsClientTypeIdString))
			{
				try
				{
					dnsClientType = (DynamicDnsUpdaterClientType) Enum.Parse(typeof (DynamicDnsUpdaterClientType), dnsClientTypeIdString);
				}
				catch (Exception)
				{
					dnsClientType = DynamicDnsUpdaterClientType.DnsOMatic;
				}
			}
			else
			{
				dnsClientType = DynamicDnsUpdaterClientType.DnsOMatic;
			}
			return dnsClientType;
		}

		[CustomAction]
		public static ActionResult StartService(Session session)
		{
			var controller = new ServiceController("BespokeDynamicDnsUpdaterService");
			controller.Start();

			return ActionResult.Success;
		}
	}
}
