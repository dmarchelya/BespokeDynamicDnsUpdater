using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Bespoke.DynamicDnsUpdater.Tests.DnsimpleClient
{
	[TestFixture]
	public class DnsimpleClientTests
	{
		[Test]
		[Ignore("Manual Test")]
		public void CanUpdateHostname()
		{
			var client = new Client.Dnsimple.DnsimpleClient("", "");

			bool updated = client.UpdateHostname("ddns.sndbx.io");

			Assert.IsTrue(updated);
		}
	}
}
