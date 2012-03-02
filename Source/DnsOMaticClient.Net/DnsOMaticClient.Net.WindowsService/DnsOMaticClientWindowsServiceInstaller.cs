using System;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;
using System.ComponentModel;
using System.Configuration.Install;
using DnsOMaticClient.Net.Common;
using log4net;

namespace DnsOMaticClient.Net.WindowsService
{
	[RunInstaller(true)]
	public class DnsOMaticClientWindowsServiceInstaller : Installer
	{
		private log4net.ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private ServiceProcessInstaller processInstaller;
		private ServiceInstaller serviceInstaller;

		public DnsOMaticClientWindowsServiceInstaller()
		{
			processInstaller = new ServiceProcessInstaller();
			processInstaller.Account = ServiceAccount.LocalSystem;

			serviceInstaller = new ServiceInstaller();
			serviceInstaller.ServiceName = Constants.ServiceName;
			serviceInstaller.DisplayName = "DNS-O-Matic Client .Net Service";
			serviceInstaller.Description = "An Open Source DNS-O-Matic Client that runs on a specified interval and updates dynamic DNS hostnames if necessary.";
			serviceInstaller.StartType = ServiceStartMode.Automatic;

			Installers.Add(processInstaller);
			Installers.Add(serviceInstaller);
		}

		public override void Install(System.Collections.IDictionary stateSaver)
		{
			UpdateAppConfig();

			base.Install(stateSaver);
		}

		private void UpdateAppConfig()
		{
			//We only update the app config when its destination path has been provided
			//as well as the DNS-O-Matic settings
			if (!string.IsNullOrWhiteSpace(Context.Parameters["TargetDir"]))
			{
				string targetDir = Context.Parameters["TargetDir"];
				logger.Error(targetDir);
				string applicationName = "DnsOMaticClient.Net.WindowsService.exe";
				var exePath = System.IO.Path.Combine(targetDir, applicationName);

				//var exePath = targetDir + @"\" + applicationName;
				var config = ConfigurationManager.OpenExeConfiguration(exePath);

				//Context.Parameters are passed from the Setup project
				//otherwise we'll assume the values are already in the config file.
				if (!string.IsNullOrWhiteSpace(Context.Parameters["Username"]))
				{
					config.AppSettings.Settings.Remove("DnsOMaticUsername");
					config.AppSettings.Settings.Add("DnsOMaticUsername", Context.Parameters["Username"]);
				}

				if (!string.IsNullOrWhiteSpace(Context.Parameters["Password"]))
				{
					config.AppSettings.Settings.Remove("DnsOMaticPassword");
					config.AppSettings.Settings.Add("DnsOMaticPassword", Context.Parameters["Password"]);
				}

				if (!string.IsNullOrWhiteSpace(Context.Parameters["Hostnames"]))
				{
					config.AppSettings.Settings.Remove("HostnamesToUpdate");
					config.AppSettings.Settings.Add("HostnamesToUpdate", Context.Parameters["Hostnames"]);
				}

				config.Save(ConfigurationSaveMode.Modified);
			}
		}

		protected override void OnAfterInstall(System.Collections.IDictionary savedState)
		{
			base.OnAfterInstall(savedState);

			ServiceController controller = new ServiceController(Constants.ServiceName);
			controller.Start();
		}


		protected override void OnBeforeUninstall(System.Collections.IDictionary savedState)
		{
			ServiceController controller = new ServiceController(Constants.ServiceName);
			
			try
			{
				if (controller.Status == ServiceControllerStatus.Running || controller.Status == ServiceControllerStatus.Paused)
				{
					controller.Stop();
					controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
					controller.Close();
				}
			} 
			finally
			{
				base.OnBeforeUninstall(savedState);				
			}
		}

		public override void Uninstall(System.Collections.IDictionary savedState)
		{
			base.Uninstall(savedState);
		}

	}
}