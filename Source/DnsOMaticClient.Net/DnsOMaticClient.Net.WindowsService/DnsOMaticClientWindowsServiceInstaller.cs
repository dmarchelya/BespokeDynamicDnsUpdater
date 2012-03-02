using System.ServiceProcess;
using System.ComponentModel;
using System.Configuration.Install;

namespace DnsOMaticClient.Net.WindowsService
{
	[RunInstaller(true)]
	public class DnsOMaticClientWindowsServiceInstaller : Installer
	{
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
	}
}