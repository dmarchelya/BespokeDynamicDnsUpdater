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
			serviceInstaller.ServiceName = "DnsOMaticClientWindowsService";
			serviceInstaller.DisplayName = "DNS-O-Matic Client .Net Service";
			serviceInstaller.StartType = ServiceStartMode.Automatic;

            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
	}
}