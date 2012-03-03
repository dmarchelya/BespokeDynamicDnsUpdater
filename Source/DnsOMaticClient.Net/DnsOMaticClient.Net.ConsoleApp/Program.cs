using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DnsOMaticClient.Net.ConsoleApp
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
