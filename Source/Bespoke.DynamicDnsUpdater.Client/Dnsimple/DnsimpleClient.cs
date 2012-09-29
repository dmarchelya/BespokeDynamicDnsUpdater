
using System;
using System.Collections.Generic;
using Bespoke.DynamicDnsUpdater.Common;
using DNSimple;
using NLog;
using RestSharp;

namespace Bespoke.DynamicDnsUpdater.Client.Dnsimple
{
	public class DnsimpleClient : DynamicDnsClientBase
	{
		private Logger logger = LogManager.GetCurrentClassLogger();
		private DNSimpleRestClient client;
 
		public DnsimpleClient(string username, string password)
		{
			client = new DNSimpleRestClient(username, password);	
		}

		public DnsimpleClient()
			: this(Config.DnsimpleUsername, Config.DnsimplePassword)
		{
		}

		public override bool UpdateHostname(string hostname, string ipAddress)
		{
			try
			{
				if (HasIpAddresssChanged(hostname, ipAddress) == false) return true; // No change, no need to update

				var domainName = DomainName.Parse(hostname);

				dynamic records = client.ListRecords(domainName.Domain);

				var recordsResponse = records as IDictionary<String, object>;
				if (recordsResponse != null && recordsResponse.ContainsKey("error"))
				{
					logger.Error(records.error);
					return false;
				}

				if (records != null)
				{
					for (var i = 0; i < records.Length; i++)
					{
						var hostForRecord = records[i].record.name;

						if (domainName.Host == hostForRecord)
						{
							var ipAddressForRecord = records[i].record.content;

							if (ipAddress == ipAddressForRecord)
							{
								return true; //No need to update.
							}
							else
							{
								dynamic record = client.UpdateRecord(domainName.Domain, records[i].record.id, hostForRecord, ipAddress);

								var recordResponse = record as IDictionary<String, object>;
								if (recordResponse != null && recordResponse.ContainsKey("error"))
								{
									logger.Error(record.error);
									return false;
								}
								else
								{
									logger.Info(string.Format("Updated DNS Record: {0} to IP Address: {1}", hostname, ipAddress));
									return true;			
								}
							}
						}
					}
				}

				//At this point we have checked all of the existing records and have not found the host name,
				//so we will try to create one.

				client.AddRecord(domainName.Domain, domainName.Host, DnsRecordType.A, ipAddress);
				return true;
			}
			catch (Exception ex)
			{
				logger.Error(ex);
				return false;
			}
			
		}
	}
}
