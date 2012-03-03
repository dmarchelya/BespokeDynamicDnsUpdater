using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DnsOMaticClient.Net.Tests.Base;
using NUnit.Framework;

namespace DnsOMaticClient.Net.Tests
{
	[TestFixture]
	public class DnsOMaticClientTests : BaseFixture
	{
		private string dnsOMaticUsername = "dnsomaticclient";
		private string dnsOMaticPassword = "XBychON2";
		private string hostnameToUpdate = "dnsomaticclient.no-ip.info";
		private string hostnameToUpdate2 = "dnsomaticclient2.no-ip.info";

		[Test]
		[Ignore("Manual Test")]
		public void CanUpdateDnsOMaticWithSpecifiedIp()
		{
			var client = new DnsOMaticClient(dnsOMaticUsername, dnsOMaticPassword);

			bool updated = client.UpdateHostname(hostnameToUpdate, "192.168.1.1");

			Assert.IsTrue(updated);
			Assert.AreEqual(UpdateStatusCode.Good, client.UpdateStatusCode);
		}

		[Test]
		[Ignore("Manual Test")]
		public void CanUpdateDnsOMaticWithRetrievedIp()
		{
			var client = new DnsOMaticClient(dnsOMaticUsername, dnsOMaticPassword);

			bool updated = client.UpdateHostname(hostnameToUpdate);

			Assert.IsTrue(updated);
			Assert.AreEqual(UpdateStatusCode.Good, client.UpdateStatusCode);
		}

		[Test]
		[Ignore("Manual Test")]
		public void CanUpdateMultipleHostnames()
		{
			var client = new DnsOMaticClient(dnsOMaticUsername, dnsOMaticPassword);

			bool updated = client.UpdateHostnames(new List<string>() {hostnameToUpdate, hostnameToUpdate2});

			Assert.IsTrue(updated);

			foreach(var pair in client.UpdateStatusCodes)
			{
				Assert.AreEqual(UpdateStatusCode.Good, pair.Value);	
			}
		}

		[Test]
		[Ignore("Manual Test")]
		public void CanUpdateAllHostnamesWithSpecifiedIPAddress()
		{
			var client = new DnsOMaticClient(dnsOMaticUsername, dnsOMaticPassword);

			bool updated = client.UpdateAll("192.168.1.1");

			Assert.IsTrue(updated);

			Assert.AreEqual(UpdateStatusCode.Good, client.UpdateStatusCode);
		}

		[Test]
		[Ignore("Manual Test")]
		public void CanUpdateAllHostnamesWithRetrievedIPAddress()
		{
			var client = new DnsOMaticClient(dnsOMaticUsername, dnsOMaticPassword);

			bool updated = client.UpdateAll();

			Assert.IsTrue(updated);

			Assert.AreEqual(UpdateStatusCode.Good, client.UpdateStatusCode);
		}

		[Test]
		public void CanCreateAndTrimList()
		{
			string hostnames = "test.com , test2.com, test3.com ";
			var hostnamesList = new List<string>();

			hostnames.Split(',').ToList().ForEach(h => hostnamesList.Add(h.Trim()));

			Assert.AreEqual("test.com", hostnamesList[0]);
			Assert.AreEqual("test2.com", hostnamesList[1]);
			Assert.AreEqual("test3.com", hostnamesList[2]);			
		}

	}
}
