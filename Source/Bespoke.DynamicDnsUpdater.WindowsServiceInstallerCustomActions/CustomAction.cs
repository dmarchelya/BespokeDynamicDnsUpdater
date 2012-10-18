using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.ServiceProcess;
using System.Text;
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
				string targetDir = session.CustomActionData["INSTALLDIR"];

				const string applicationName = "Bespoke.DynamicDnsUpdater.WindowsService.exe";
				var exePath = System.IO.Path.Combine(targetDir, applicationName);

				//var exePath = targetDir + @"\" + applicationName;
				var config = ConfigurationManager.OpenExeConfiguration(exePath);

				//Context.Parameters are passed from the Setup project
				//otherwise we'll assume the values are already in the config file.
				if (!string.IsNullOrWhiteSpace(session.CustomActionData["DNSCLIENTUSERNAME"]))
				{
					config.AppSettings.Settings.Remove("DnsOMaticUsername");
					config.AppSettings.Settings.Add("DnsOMaticUsername", session.CustomActionData["DNSCLIENTUSERNAME"]);
				}

				if (!string.IsNullOrWhiteSpace(session.CustomActionData["DNSCLIENTPASSWORD"]))
				{
					bool encryptionEnabled = Convert.ToBoolean(config.AppSettings.Settings["EncryptionEnabled"].Value);

					string password = string.Empty;

					if (encryptionEnabled)
					{
						var encryptionService = new EncryptionService(Convert.FromBase64String(Common.Constants.EncryptionKey), Convert.FromBase64String(Common.Constants.InitializationVector));
						password = encryptionService.EncryptToBase64String(session.CustomActionData["DNSCLIENTPASSWORD"]);
					}
					else
					{
						password = session.CustomActionData["DNSCLIENTPASSWORD"];
					}

					config.AppSettings.Settings.Remove("DnsOMaticPassword");
					config.AppSettings.Settings.Add("DnsOMaticPassword", password);
				}

				if (!string.IsNullOrWhiteSpace(session.CustomActionData["HOSTNAMES"]))
				{
					config.AppSettings.Settings.Remove("HostnamesToUpdate");
					config.AppSettings.Settings.Add("HostnamesToUpdate", session.CustomActionData["HOSTNAMES"]);
				}

				config.Save(ConfigurationSaveMode.Modified);
			}

			return ActionResult.Success;
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
