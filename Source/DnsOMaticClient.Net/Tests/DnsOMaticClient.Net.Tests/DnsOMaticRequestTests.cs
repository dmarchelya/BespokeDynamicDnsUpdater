using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DnsOMaticClient.Net.Tests.Base;
using NUnit.Framework;

namespace DnsOMaticClient.Net.Tests
{
	[TestFixture]
	public class DnsOMaticRequestTests : BaseFixture
	{
		private string dnsOMaticUsername = "dnsomaticclient";
		private string dnsOMaticPassword = "XBychON2";
		private string hostnameToUpdate = "dnsomaticclient.no-ip.info";

		[Test]
		[Ignore("Manual Test")]
		public void CanUpdateDnsOMaticWithSpecifiedIp()
		{
			var request = new DnsOMaticRequest(dnsOMaticUsername, dnsOMaticPassword);

			request.Update(hostnameToUpdate, "192.168.1.1");

			//TODO: Verify response code
		}

		[Test]
		[Ignore("Manual Test")]
		public void CanUpdateDnsOMaticWithRetrievedIp()
		{
			var request = new DnsOMaticRequest(dnsOMaticUsername, dnsOMaticPassword);

			request.Update(hostnameToUpdate);	

			//TODO: verify response code
		}
	}
}
