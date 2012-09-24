using System;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using Bespoke.DynamicDnsUpdater.Client;
using Bespoke.DynamicDnsUpdater.Client.DnsOMatic;
using Bespoke.DynamicDnsUpdater.Common;
using NLog;

namespace Bespoke.DynamicDnsUpdater.WindowsService
{
	public class BespokeDynamicDnsUpdaterWindowsService : ServiceBase
	{
		private Logger logger = LogManager.GetCurrentClassLogger();

		private BespokeUpdater updater;
		private TimeSpan updateStartDelay = TimeSpan.FromSeconds(15);
		private TimeSpan updateInterval = TimeSpan.FromMinutes(5);
		private Timer timer = null;
		private string username = string.Empty;
		private string password = string.Empty;
		private string hostnamesToUpdate = string.Empty;

		public BespokeDynamicDnsUpdaterWindowsService()
		{
			ServiceName = Constants.ServiceName;
		}

		public static void Main()
		{
			Run(new BespokeDynamicDnsUpdaterWindowsService());
		}

		protected override void OnStart(string[] args)
		{
		
			Thread.Sleep(10000);

			try
			{
				hostnamesToUpdate = Config.HostnamesToUpdate;

				if (string.IsNullOrEmpty(hostnamesToUpdate))
				{
					throw new ArgumentException("Hostnames To Update were not provided");
				}	

				if(Config.DynamicDnsUpdaterClientTypeId == (int)DynamicDnsUpdaterClientType.DnsOMatic)
				{
					username = Config.DnsOMaticUsername;
					password = Config.DnsOMaticPassword;

					if (string.IsNullOrEmpty(username))
					{
						throw new ArgumentException("Username was not provided");
					}

					if (string.IsNullOrEmpty(password))
					{
						throw new ArgumentException("Password was not provided");
					}
				}

				updater = new BespokeUpdater(Config.DynamicDnsUpdaterClientTypeId);
			    updater.Client.InitializeLastUpdateIpAddresses(hostnamesToUpdate);

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
			updater.Client.UpdateHostnames(hostnamesToUpdate);
		}
	}
}