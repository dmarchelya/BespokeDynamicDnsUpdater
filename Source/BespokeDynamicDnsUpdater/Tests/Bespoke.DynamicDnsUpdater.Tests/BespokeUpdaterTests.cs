using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bespoke.DynamicDnsUpdater.Client;
using Bespoke.DynamicDnsUpdater.Client.Route53;
using NUnit.Framework;

namespace Bespoke.DynamicDnsUpdater.Tests
{
	[TestFixture]
	public class BespokeUpdaterTests
	{
		[Test]
		public void CanGetDnsOMaticClientFromId()
		{
			const int id = 1;
			var client = BespokeUpdater.GetClient(id);

			Assert.AreEqual(typeof(DynamicDnsUpdater.Client.DnsOMatic.DnsOMaticClient), client.GetType());
		}

		[Test]
		public void CanGetRoute53ClientFromId()
		{
			const int id = 2;
			var client = BespokeUpdater.GetClient(id);

			Assert.AreEqual(typeof(Route53Client), client.GetType());
		}
	}
}
