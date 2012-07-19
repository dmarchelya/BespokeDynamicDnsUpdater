using System.Configuration;
using Bespoke.DynamicDnsUpdater;
using Bespoke.DynamicDnsUpdater.Client;

namespace BespokeDynamicDnsUpdater.ConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			string username = ConfigurationManager.AppSettings["DnsOMaticUsername"];
			string password = ConfigurationManager.AppSettings["DnsOMaticPassword"];
			string hostnamesToUpdate = ConfigurationManager.AppSettings["HostNamesToUpdate"];

			var request = new DnsOMaticClient(username, password);

			request.UpdateHostnames(hostnamesToUpdate);

			//Console.ReadLine();
		}
	}
}
