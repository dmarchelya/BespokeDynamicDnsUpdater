using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DnsOMaticClient.Net.Tests.Base;
using NUnit.Framework;

namespace DnsOMaticClient.Net.Tests
{
	[TestFixture]
	public class IpAddressResolverTests : BaseFixture
	{
		[Test]
		public void CanGetPublicIpAddressFromDynDns()
		{
			var resolver = new IpAddressResolver();
			var ip = resolver.GetPublicIpAddressFromDynDns();

			Assert.AreEqual(ExpectedPublicIpAddress, ip);
		}

		[Test]
		public void CanGetPublicIpAddressFromDnsOMatic()
		{
			var resolver = new IpAddressResolver();
			var ip = resolver.GetPublicIpAddressFromDnsOMatic();

			Assert.AreEqual(ExpectedPublicIpAddress, ip);
		}
	}
}
