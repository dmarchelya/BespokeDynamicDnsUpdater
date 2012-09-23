using System;
using System.Configuration;
using Bespoke.DynamicDnsUpdater;
using Bespoke.DynamicDnsUpdater.Client;
using Bespoke.DynamicDnsUpdater.Client.DnsOMatic;
using Bespoke.DynamicDnsUpdater.Common;

namespace BespokeDynamicDnsUpdater.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			string hostnamesToUpdate = ConfigurationManager.AppSettings["HostNamesToUpdate"];

			var updater = new BespokeUpdater(Config.DynamicDnsUpdaterClientTypeId);

			updater.Client.UpdateHostnames(hostnamesToUpdate);

			Console.ReadLine();
		}
	}
}
