using System;
using System.Configuration;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using DnsOMaticClient.Net;
using log4net;

namespace DnsOMaticClient.Net.WindowsService
{
	public class DnsOMaticClientWindowsService : ServiceBase
	{
		private log4net.ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private DnsOMaticClient dnsOMaticClient;
		private TimeSpan updateStartDelay = TimeSpan.FromSeconds(15);
		private TimeSpan updateInterval = TimeSpan.FromSeconds(5);
		private Timer timer = null;
		private string username = string.Empty;
		private string password = string.Empty;
		private string hostnameToUpdate = string.Empty;

		public DnsOMaticClientWindowsService()
		{
			ServiceName = Constants.ServiceName;
		}

		public static void Main()
		{
			Run(new DnsOMaticClientWindowsService());
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				username = ConfigurationManager.AppSettings["DnsOMaticUsername"];
				password = ConfigurationManager.AppSettings["DnsOMaticPassword"];
				hostnameToUpdate = ConfigurationManager.AppSettings["HostNameToUpdate"];

				if (string.IsNullOrEmpty(username))
				{
					throw new ArgumentException("Username was not provided");
				}

				if (string.IsNullOrEmpty(password))
				{
					throw new ArgumentException("Password was not provided");
				}

				if (string.IsNullOrEmpty(hostnameToUpdate))
				{
					throw new ArgumentException("Hostname To Update was not provided");
				}

				dnsOMaticClient = new DnsOMaticClient(username, password);

				timer = new Timer(Update, null, updateStartDelay, updateInterval);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
			}
		}

		protected override void OnStop()
		{
			if(timer != null)
			{
				timer.Dispose();
			}
		}

		private void Update(object data)
		{
			dnsOMaticClient.Update(hostnameToUpdate);
		}
	}
}