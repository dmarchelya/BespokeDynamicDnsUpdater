using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;

namespace Bespoke.DynamicDnsUpdater.Client.DnsOMatic
{
	//DNS-O-Matic API Reference
	//https://www.dnsomatic.com/wiki/api

	//DNS-O-Matic API Reference specifically states that it is similar (based on?) the DynDns API, which has better documentation
	//http://dyn.com/support/developers/api/

	public class DnsOMaticClient
	{

		#region Construtor

		public DnsOMaticClient(string username, string password)
		{
			Username = username;
			Password = password;
		}

		#endregion Constructor

		#region Constants

		const string updateAllMagicHostname = "all.dnsomatic.com";

		#endregion Constants

		#region Fields

		private log4net.ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private Dictionary<string, UpdateStatusCode> updateStatusCodes = new Dictionary<string, UpdateStatusCode>();
		private Dictionary<string, string> lastUpdatedIpAddresses = new Dictionary<string, string>();

		#endregion Fields

		#region Properties

		/// <summary>
		/// The DNS-O-Matic Username
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// The DNS-O-Matic Password
		/// </summary>
		public string Password { get; set; }

		public bool? PublicIpAddressResolved { get; set; }

		/// <summary>
		/// The Update Status Codes, keyed on the hostname that they represent.
		/// </summary>
		public Dictionary<string, UpdateStatusCode> UpdateStatusCodes
		{
			get { return updateStatusCodes; }
		}

		/// <summary>
		/// The Update Status Code that was returned, when a single hostname is updated.
		/// Throws InvalidOperationException when attempting to update multiple hostnames.
		/// </summary>
		public UpdateStatusCode UpdateStatusCode
		{
			get
			{
				if (UpdateStatusCodes.Keys.Count > 1)
				{
					throw new InvalidOperationException("Multiple Hostnames were updated, use UpdateStatusCodes instead.");
				}

				return UpdateStatusCodes[UpdateStatusCodes.Keys.ElementAt(0)];
			}
		}

		/// <summary>
		/// A Dictionary of the last ip address that was successfully updated for a given hostname.
		/// </summary>
		public Dictionary<string, string> LastUpdateIpAddresses
		{
			get { return lastUpdatedIpAddresses; }
		}

		/// <summary>
		/// The IP Address that was set, when a single hostname is updated.
		/// Throws InvalidOperationException when attempting to update multiple hostnames.
		/// </summary> 
		public string LastUpdateIpAddress
		{
			get
			{
				if (LastUpdateIpAddresses.Keys.Count > 1)
				{
					throw new InvalidOperationException("Multiple Hostnames were updated, use LastUpdateIpAddress instead.");
				}

				return LastUpdateIpAddresses[LastUpdateIpAddresses.Keys.ElementAt(0)];
			}
		}

		#endregion Properties

		#region Methods

        /// <summary>
        /// Initialize the LastUpdateIpAddresses collection with the current IP Address (in DNS) for each of the given hostnames.
        /// This can be used when the client is run for the first time, when we don't have a record of what the previous IP
        /// Address was that was sent to DNS-O-Matic.  This way we won't have to attempt to update the IP Address, if the current
        /// DNS entry matches the current IP Address.
        /// </summary>
        /// <param name="hostnames">The hostnames to update.</param>
        public void InitializeLastUpdateIpAddresses(string hostnames)
        {
            var resolver = new IpAddressResolver();
            var hostnamesList = HostnamesToList(hostnames);

            foreach(var hostname in hostnamesList)
            {
                var ipAddress = resolver.GetIpAddressForHostname(hostname);

                if(ipAddress != null)
                {
                    LastUpdateIpAddresses[hostname] = ipAddress;
                }
            }
        }

		/// <summary>
		/// Updates the given hostnames via DNS-O-Matic with the public facing
		/// IP Address for the system that the request is made from.
		/// </summary>
		/// <param name="hostnames">A comma delimited list of hostnames to update.</param>
		/// <returns>True, if all of the hostnames updated correctly, otherwise false.</returns>
		public bool UpdateHostnames(string hostnames)
		{
			var resolver = new IpAddressResolver();

			var ip = resolver.GetPublicIpAddress();

			if (ip == null) return false;

			logger.Info(string.Format("Resolved public IP Address as {0}", ip));

			return UpdateHostnames(hostnames, ip);
		}

		/// <summary>
		/// Updates the given hostnames via DNS-O-Matic with the public facing
		/// IP Address for the system that the request is made from.
		/// </summary>
		/// <param name="hostnames">A comma delimited list of hostnames to update.</param>
		/// <param name="ipAddress">The IP Address to update to.</param>
		/// <returns>True, if all of the hostnames updated correctly, otherwise false.</returns>
		public bool UpdateHostnames(string hostnames, string ipAddress)
		{
		    var hostnamesList = HostnamesToList(hostnames);

			return UpdateHostnames(hostnamesList, ipAddress);
		}

		/// <summary>
		/// Updates the specified hostnames via DNS-O-Matic with the public facing
		/// IP Address for the system that the request is made from.
		/// </summary>
		/// <param name="hostnames">The hostnames to update.</param>
		/// <returns>
		/// True if all updates were successful, otherwise false.
		/// Check UpdateStatusCodes dictionary for individual hostname status.
		/// </returns>
		public bool UpdateHostnames(List<string> hostnames)
		{
			var resolver = new IpAddressResolver();

			var ip = resolver.GetPublicIpAddress();

			if (ip == null) return false;

			logger.Info(string.Format("Resolved public IP Address as {0}", ip));

			return UpdateHostnames(hostnames, ip);
		}

		/// <summary>
		/// Updates the specified hostnames via DNS-O-Matic with the IP Address
		/// that is specified.
		/// </summary>
		/// <param name="hostnames">The hostnames to update.</param>
		/// <param name="ipAddress">The IP Address to update to.</param>
		/// <returns>
		/// True if all updates were successful, otherwise false.
		/// Check UpdateStatusCodes dictionary for individual hostname status.
		/// </returns>
		public bool UpdateHostnames(List<string> hostnames, string ipAddress)
		{
			bool allSuccess = true;
			foreach(var hostname in hostnames)
			{
				bool success = UpdateHostname(hostname, ipAddress);

				if (success == false) allSuccess = false;
			}

			return allSuccess;
		}

		/// <summary>
		/// Updates the specified hostname via DNS-O-Matic with the public facing IP
		/// Address for the system that the request is made from.
		/// </summary>
		/// <param name="hostname">The hostname to update.</param>
		/// <returns>True if the update was successful</returns>
		public bool UpdateHostname(string hostname)
		{
			var resolver = new IpAddressResolver();

			var ip = resolver.GetPublicIpAddress();

			if (ip == null) return false;

			logger.Info(string.Format("Resolved public IP Address as {0}", ip));

			return UpdateHostname(hostname, ip);
		}

		/// <summary>
		/// Updates the specified hostname via DNS-O-Matic to the IP Address that is given.
		/// </summary>
		/// <param name="hostname">The hostname to update.</param>
		/// <param name="ipAddress">The IP address to use.</param>		
		/// <returns>True if the update was successful or there was no change in the IP Address.</returns>
		public bool UpdateHostname(string hostname, string ipAddress)
		{
			try
			{
				if(LastUpdateIpAddresses.ContainsKey(hostname)  && LastUpdateIpAddresses[hostname] == ipAddress)
				{
					logger.Info(string.Format("No need to update hostname {0}, the IP Address ({1}) hasn't changed.", hostname, ipAddress));
					return true;
				}

				var updateUriFormat = "https://updates.dnsomatic.com/nic/update?hostname={0}&myip={1}&wildcard=NOCHG&mx=NOCHG&backmx=NOCHG";

				var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(updateUriFormat, hostname.Trim(), ipAddress));

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

					logger.Info(string.Format("DNS-O-Matic update response for hostname {0}: {1}", hostname, responseBody));

					var regex = new Regex(@"\w+");
					var match = regex.Match(responseBody);

					UpdateStatusCode statusCode;

					if (match.Success)
					{
						statusCode = UpdateStatusCodeConverter.GetUpdateStatusCode(match.Value);
					}
					else
					{
						statusCode = UpdateStatusCode.Unknown;
					}

					UpdateStatusCodes[hostname] = statusCode;

					if (statusCode == UpdateStatusCode.Good || statusCode == UpdateStatusCode.NoChange)
					{
						LastUpdateIpAddresses[hostname] = ipAddress;
						return true;
					}

					return false;
				}
			}
			catch (Exception ex)
			{	
				logger.Error(ex);

				return false;
			}
		}


		/// <summary>
		/// Updates the all of the hostnames registered with DNS-O-Matic with the public facing IP
		/// Address for the system that the request is made from.
		/// </summary>
		/// <returns>True if the update was successful</returns>
		public bool UpdateAll()
		{
			return UpdateHostname(updateAllMagicHostname);
		}

		/// <summary>
		/// Updates the all of the hostnames registered with DNS-O-Matic with the specified
		/// IP Address.
		/// </summary>
		/// <param name="ipAddress">The IP Address to use for updating the hostnames.</param>
		/// <returns>True if the update was successful</returns>
		public bool UpdateAll(string ipAddress)
		{
			return UpdateHostname(updateAllMagicHostname, ipAddress);
		}

		#endregion Methods

        #region Helper Methods

        /// <summary>
        /// Takes a comma seperated string of hostnames and converts it to a List of hostnames.
        /// </summary>
        /// <param name="hostnames"></param>
        /// <returns></returns>
        private static List<string> HostnamesToList(string hostnames)
        {
            var hostnamesList = new List<string>();

            hostnames.Split(',').ToList().ForEach(h => hostnamesList.Add(h.Trim()));

            return hostnamesList;
        }

        #endregion Helper Methods
    }
}
