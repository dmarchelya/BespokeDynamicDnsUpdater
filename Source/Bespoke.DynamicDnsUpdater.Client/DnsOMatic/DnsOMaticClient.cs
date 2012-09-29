using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Bespoke.DynamicDnsUpdater.Common;
using NLog;

namespace Bespoke.DynamicDnsUpdater.Client.DnsOMatic
{
	//DNS-O-Matic API Reference
	//https://www.dnsomatic.com/wiki/api

	//DNS-O-Matic API Reference specifically states that it is similar (based on?) the DynDns API, which has better documentation
	//http://dyn.com/support/developers/api/

	public class DnsOMaticClient : DynamicDnsClientBase
	{
		#region Construtor

		public DnsOMaticClient(string username, string password)
		{
			Username = username;
			Password = password;
		}

		public DnsOMaticClient()
			:this(Config.DnsOMaticUsername, Config.DnsOMaticPassword)
		{
		}

		#endregion Constructor

		#region Constants

		const string updateAllMagicHostname = "all.dnsomatic.com";

		#endregion Constants

		#region Fields

		private Logger logger = LogManager.GetCurrentClassLogger();
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

		#endregion Properties

		#region Methods

		/// <summary>
		/// Updates the specified hostname via DNS-O-Matic to the IP Address that is given.
		/// </summary>
		/// <param name="hostname">The hostname to update.</param>
		/// <param name="ipAddress">The IP address to use.</param>		
		/// <returns>True if the update was successful or there was no change in the IP Address.</returns>
		public override bool UpdateHostname(string hostname, string ipAddress)
		{
			try
			{
				if (HasIpAddresssChanged(hostname, ipAddress) == false) return true; // No change, no need to update

				var updateUriFormat = "https://updates.dnsomatic.com/nic/update?hostname={0}&myip={1}&wildcard=NOCHG&mx=NOCHG&backmx=NOCHG";

				var request = (HttpWebRequest)HttpWebRequest.Create(string.Format(updateUriFormat, hostname.Trim(), ipAddress));

				request.Credentials = new NetworkCredential(Username, Password);

				var assembly = Assembly.GetExecutingAssembly();
				var product = (AssemblyProductAttribute)assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0];
				var version = assembly.GetName().Version;

				request.UserAgent = string.Format("Bespoke Dynamic DNS Updater - {0} - {1}", product.Product, version);

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
    }
}
