using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Bespoke.DynamicDnsUpdater.Client
{
	public class IpAddressResolver
	{
        /// <summary>
        /// Submits a dns request for the given hostname and returns the IP address that it resolves to.
        /// </summary>
        /// <param name="hostname">The hostname to get an IP address for.</param>
        /// <returns>The IP Address.</returns>
        public string GetIpAddressForHostname(string hostname)
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(hostname);

                if (hostEntry.AddressList.Count() > 0)
                {
                    return hostEntry.AddressList.First().ToString();
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

		public string GetPublicIpAddress()
		{
			//Since this is a DNS-O-Matic Updater Client, we use the DNS-O-Matic service to retrieve the Public IP Address
			//TODO: Make the IP Address Resolution Service configurable.
			return GetPublicIpAddressFromDnsOMatic();
		}

		/// <summary>
		/// Retrieve the public IP address from DNS-O-Matic at http://myip.dnsomatic.com/
		/// </summary>
		/// <returns>The public IP Address.</returns>
		public string GetPublicIpAddressFromDnsOMatic()
		{
			string uri = "http://myip.dnsomatic.com/";
			string responseBody = string.Empty;

			try
			{
				WebRequest request = WebRequest.Create(uri);
				using (WebResponse response = request.GetResponse())
				{
					using (StreamReader stream = new StreamReader(response.GetResponseStream()))
					{
						responseBody = stream.ReadToEnd();
					}
				}
			}
			catch (Exception)
			{
				//TODO: Log Exception
			}

			var options = RegexOptions.IgnoreCase | RegexOptions.Compiled;

			var regex = new Regex(@"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}", options);

			var match = regex.Match(responseBody);

			if (match.Success)
			{
				return match.Value;
			}

			return null;
		}

		/// <summary>
		/// Get the public IP Address from DynDns at: http://checkip.dyndns.org/
		/// </summary>
		/// <returns>The public IP Address.</returns>
		public string GetPublicIpAddressFromDynDns()
		{
			string uri = "http://checkip.dyndns.org/";
			string responseBody = string.Empty;

			try
			{
				WebRequest request = WebRequest.Create(uri);
				using (WebResponse response = request.GetResponse())
				{
					using (StreamReader stream = new StreamReader(response.GetResponseStream()))
					{
						responseBody = stream.ReadToEnd();
					}
				}
			}
			catch (Exception)
			{
				//TODO: Log Exception
			}

			var options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
			
			var regex = new Regex(@"<body>Current IP Address:\s?(\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3})\s?</body>",options);

			var match = regex.Match(responseBody);

			if(match.Success && match.Groups.Count > 1)
			{
				return match.Groups[1].Value;
			}

			return null;
		}

	}
}
