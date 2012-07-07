using System;
using System.Configuration;
using System.Reflection;
using System.ServiceProcess;
using System.ComponentModel;
using System.Configuration.Install;
using log4net;
using System.Diagnostics;

namespace Bespoke.DynamicDnsUpdater.WindowsService
{
	[RunInstaller(true)]
	public class BespokeDynamicDnsUpdaterWindowsServiceInstaller : Installer
	{
		private log4net.ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private ServiceProcessInstaller processInstaller;
		private ServiceInstaller serviceInstaller;

		public BespokeDynamicDnsUpdaterWindowsServiceInstaller()
		{
			processInstaller = new ServiceProcessInstaller();
			processInstaller.Account = ServiceAccount.LocalSystem;

			serviceInstaller = new ServiceInstaller();
			serviceInstaller.ServiceName = Constants.ServiceName;
			serviceInstaller.DisplayName = "Bespoke Dynamic DNS Updater Service";
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
				const string applicationName = "Bespoke.DynamicDnsUpdater.WindowsService.exe";
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

            SetRecoveryOptions();

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

        private void SetRecoveryOptions()
        {
            try
            {
                int exitCode;
                using (var process = new Process())
                {
                    var startInfo = process.StartInfo;
                    startInfo.FileName = "sc";
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    const int reset = 86400; //1 day until the recovery attempt counter resets
                    const int restartDelay1 = 60000; //1 Minute
                    const int restartDelay2 = 300000; //5 Minutes
                    const int restartDelay3 = 1800000; //30 Minutes

                    //http://technet.microsoft.com/en-us/library/cc742019%28v=ws.10%29.aspx
                    startInfo.Arguments = string.Format("failure {0} reset= {1} actions= restart/{2}/restart/{3}/restart{4}", Constants.ServiceName, reset, restartDelay1, restartDelay2, restartDelay3);

                    process.Start();
                    process.WaitForExit();

                    exitCode = process.ExitCode;

                    process.Close();
                }

                if (exitCode != 0)
                    throw new InvalidOperationException("Exit Code: " + exitCode);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
	}
}