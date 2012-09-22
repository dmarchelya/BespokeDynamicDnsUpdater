using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Amazon.Route53;
using Amazon.Route53.Model;
using log4net;

namespace Bespoke.DynamicDnsUpdater.Client.Route53
{
	public class Route53Client
	{
		private log4net.ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// http://docs.amazonwebservices.com/sdkfornet/latest/apidocs/?topic=html/P_Amazon_Route53_Model_Change_Action.htm
		/// </summary>
		public class ChangeActions
		{
			//TODO: Convert to enum
			public const string Create = "CREATE";
			public const string Delete = "DELETE";
		}

		private AmazonRoute53Client client;

		public Route53Client(string awsAccessKeyId, string awsSecretAccessKey)
		{
			client = new AmazonRoute53Client(awsAccessKeyId, awsSecretAccessKey);	
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="hostname">Fully qualified host name, i.e. host.domain.com</param>
		public void UpdateHostName(string hostname, string ipAddress)
		{	
			var zonesResponse = client.ListHostedZones();
			var zones = zonesResponse.ListHostedZonesResult.HostedZones;

			try
			{
				var deleteRequest = GetChangeResourceRecordSetsRequest(hostname, ipAddress, ChangeActions.Delete, zones);
				var deleteResponse = client.ChangeResourceRecordSets(deleteRequest);
			}
			catch (AmazonRoute53Exception ex)
			{
				//Ignore, if delete fails, its probably because the record didn't already exists
				logger.Warn(ex);
			}
			catch(Exception ex)
			{
				logger.Error(ex);
			}

			var createRequest = GetChangeResourceRecordSetsRequest(hostname, ipAddress, ChangeActions.Create, zones);
			var createResponse = client.ChangeResourceRecordSets(createRequest);

			//ChangeInfo info: http://docs.amazonwebservices.com/sdkfornet/latest/apidocs/?topic=html/T_Amazon_Route53_Model_ChangeInfo.htm
			//response.ChangeResourceRecordSetsResult.ChangeInfo.Status

			//TODO: Interrogate response.
		}

		private ChangeResourceRecordSetsRequest GetChangeResourceRecordSetsRequest(string hostname, string ipAddress, string action, List<HostedZone> zones)
		{
			string domain = DomainName.Parse(hostname).Domain;
			string hostedZoneId = zones.Single(z => z.Name.StartsWith(domain)).Id;

			var record = new ResourceRecord() { Value = ipAddress };

			var recordSet = new ResourceRecordSet()
			                	{
			                		Name = hostname,
			                		TTL = 300, //5 min.
			                		Type = DnsRecordTypes.A,
			                		ResourceRecords = new List<ResourceRecord> {record}
			                	};

			var change = new Change() {Action = action, ResourceRecordSet = recordSet};

			var request = new ChangeResourceRecordSetsRequest()
			              	{
			              		ChangeBatch = new ChangeBatch()
			              		              	{
			              		              		Changes = new List<Change> {change},
			              		              		Comment = string.Format("Automatically updated by {0}", this.ToString())
			              		              	},
			              		HostedZoneId = hostedZoneId
			              	};
			return request;
		}
	}
}
