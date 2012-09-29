using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bespoke.DynamicDnsUpdater.Common;
using NLog;

namespace Bespoke.DynamicDnsUpdater.Client
{
	public abstract class DynamicDnsClientBase
	{
		#region Fields

		private Logger logger = LogManager.GetCurrentClassLogger();
		private Dictionary<string, string> lastUpdatedIpAddresses = new Dictionary<string, string>();

		#endregion Fields

		#region Properties

		public bool? PublicIpAddressResolved { get; set; }

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

		public abstract bool UpdateHostname(string hostname, string ipAddress);

		#region Public Methods

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
		/// Updates the given hostnames via DNS-O-Matic with the public facing
		/// IP Address for the system that the request is made from.
		/// </summary>
		/// <param name="hostnames">A comma delimited list of hostnames to update.</param>
		/// <returns>True, if all of the hostnames updated correctly, otherwise false.</returns>
		public  bool UpdateHostnames(string hostnames)
		{
			var resolver = new IpAddressResolver();

			var ip = resolver.GetPublicIpAddress();

			if (ip == null) return false;

			logger.Info(string.Format("Resolved public IP Address as {0}", ip));

			return UpdateHostnames(hostnames, ip);
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
			foreach (var hostname in hostnames)
			{
				bool success = UpdateHostname(hostname, ipAddress);

				if (success == false) allSuccess = false;
			}

			return allSuccess;
		}

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

			foreach (var hostname in hostnamesList)
			{
				var ipAddress = resolver.GetIpAddressForHostname(hostname);

				if (ipAddress != null)
				{
					LastUpdateIpAddresses[hostname] = ipAddress;
				}
			}
		}

		public void InitializeLastUpdateIpAddresses()
		{
			InitializeLastUpdateIpAddresses(Config.HostnamesToUpdate);
		}
	
		/// <summary>
		/// Checks if the given IP Address has changed from the last update.
		/// </summary>
		/// <param name="hostname">The hostname that we are checking for an update.</param>
		/// <param name="ipAddress">The current IP Address.</param>
		/// <returns>True, if the ip address has changed, otherwise false.</returns>
		public bool HasIpAddresssChanged(string hostname, string ipAddress)
		{
			if (LastUpdateIpAddresses.ContainsKey(hostname) && LastUpdateIpAddresses[hostname] == ipAddress)
			{
				logger.Info(string.Format("No need to update hostname {0}, the IP Address ({1}) hasn't changed.", hostname, ipAddress));
				return false;
			}

			return true;
		}
		
		#endregion Public Methods

		#region Helper Methods

		/// <summary>
		/// Takes a comma seperated string of hostnames and converts it to a List of hostnames.
		/// </summary>
		/// <param name="hostnames"></param>
		/// <returns></returns>
		protected static List<string> HostnamesToList(string hostnames)
		{
			var hostnamesList = new List<string>();

			hostnames.Split(',').ToList().ForEach(h => hostnamesList.Add(h.Trim()));

			return hostnamesList;
		}

		#endregion Helper Methods
	}
}
