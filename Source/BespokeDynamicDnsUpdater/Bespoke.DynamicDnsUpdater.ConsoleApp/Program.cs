using System.Configuration;
using Bespoke.DynamicDnsUpdater;
using Bespoke.DynamicDnsUpdater.Client;
using Bespoke.DynamicDnsUpdater.Client.DnsOMatic;

namespace BespokeDynamicDnsUpdater.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			string username = ConfigurationManager.AppSettings["DnsOMaticUsername"];
			string password = ConfigurationManager.AppSettings["DnsOMaticPassword"];
			string hostnamesToUpdate = ConfigurationManager.AppSettings["HostNamesToUpdate"];

			var client = new DnsOMaticClient(username, password);
			var updater = new BespokeUpdater(client);

			updater.Client.UpdateHostnames(hostnamesToUpdate);

			//Console.ReadLine();
		}
	}
}
