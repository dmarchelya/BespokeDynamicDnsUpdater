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
	public class DynamicDnsClientBaseTests
	{
		[Test]
		public void CanCheckValidIpAddress()
		{
			var client = new Route53Client();

			bool valid = client.IsValidIpAddress("127.0.0.1");

			Assert.IsTrue(valid);
		}

		[Test]
		public void CanCheckInvalidIpAddress()
		{
			var client = new Route53Client();

			bool valid = client.IsValidIpAddress("asdfasfd");

			Assert.IsFalse(valid);
		}

		[Test]
		public void OutOfRangeIpAddressIsInvalid()
		{

			var client = new Route53Client();

			bool valid = client.IsValidIpAddress("300.0.0.1");

			Assert.IsFalse(valid);
		}
	}
}
