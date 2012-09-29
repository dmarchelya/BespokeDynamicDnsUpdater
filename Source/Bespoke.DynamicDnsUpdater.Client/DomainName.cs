using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bespoke.DynamicDnsUpdater.Client
{
	public class DomainName
	{
		public string Host { get; set; }

		/// <summary>
		/// Domain Name with the TLD included.
		/// </summary>
		public string Domain { get; set; }

		/// <summary>
		/// Top Level Domain Name
		/// </summary>
		public string Tld { get; set; }

		public static DomainName Parse(string domainNameString)
		{
			if(string.IsNullOrWhiteSpace(domainNameString))
			{
				throw new ArgumentException("domainName cannot be null or empty.");
			}

			domainNameString = domainNameString.TrimEnd('.');

			var segments = domainNameString.Count(character => character == '.') + 1;

			var domainName = new DomainName();

			//What about domain names such as .co.uk?
			if(segments == 1)
			{
				domainName.Tld = domainNameString;
			}
			else if(segments == 2)
			{
				//Domain + TLD
				domainName.Domain = domainNameString;
				domainName.Tld = domainNameString.Substring(domainNameString.LastIndexOf('.') + 1);
			}
			else if(segments == 3)
			{
				domainName.Host = domainNameString.Substring(0,domainNameString.IndexOf('.'));
				domainName.Domain = domainNameString.Substring(domainNameString.IndexOf('.') + 1);
				domainName.Tld = domainNameString.Substring(domainNameString.LastIndexOf('.') + 1);
			}
			else
			{
				//TODO: Support domain names with more segments.
				throw new ArgumentException("Invalid domain name");
			}

			return domainName;
		}
	}
}
