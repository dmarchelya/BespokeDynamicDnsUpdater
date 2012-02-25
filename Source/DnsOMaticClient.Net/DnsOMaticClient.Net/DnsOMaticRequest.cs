using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using log4net;

namespace DnsOMaticClient.Net
{
	//DNS-O-Matic API Reference
	//https://www.dnsomatic.com/wiki/api

	//DNS-O-Matic API Reference specifically states that it is similar (based on?) the DynDns API, which has better documentation
	//http://dyn.com/support/developers/api/

	public class DnsOMaticRequest
	{
		private log4net.ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		
		public DnsOMaticRequest(string username, string password)
		{
			Username = username;
			Password = password;
		}
		
		/// <summary>
		/// The DNS-O-Matic Username
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// The DNS-O-Matic Password
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Updates the specified hostname via DNS-O-Matic with public facing IP
		/// address for the system that the request is made from.
		/// </summary>
		/// <param name="hostname">The hostname to update.</param>
		public void Update(string hostname)
		{
			var resolver = new IpAddressResolver();

			var ip = resolver.GetPublicIpAddress();

			if (ip == null) return;

			logger.Info(string.Format("Resolved public IP Address as {0}", ip));

			Update(hostname, ip);
		}

		/// <summary>
		/// Updates the specified hostname via DNS-O-Matic to the ip address that is given.
		/// </summary>
		/// <param name="hostname">The hostname to update.</param>
		/// <param name="ipAddress">The IP address to use.</param>
		public void Update(string hostname, string ipAddress)
		{
			var updateUriFormat = "https://updates.dnsomatic.com/nic/update?hostname={0}&myip={1}&wildcard=NOCHG&mx=NOCHG&backmx=NOCHG";

			var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(updateUriFormat, hostname, ipAddress));
			
			request.Credentials = new NetworkCredential(Username, Password);

			var assembly = Assembly.GetExecutingAssembly();
			var product = (AssemblyProductAttribute)assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0];
			var version = assembly.GetName().Version;

			request.UserAgent = string.Format("Open Source - {0} - {1}", product.Product, version);

			var response = request.GetResponse();

			string responseBody = string.Empty;
			using (StreamReader stream = new StreamReader(response.GetResponseStream()))
			{
				responseBody = stream.ReadToEnd();

				logger.Info(string.Format("DNS-O-Matic update response: {0}", responseBody));
			}
			
			//TODO: Handle Response Code
		}
	}
}
