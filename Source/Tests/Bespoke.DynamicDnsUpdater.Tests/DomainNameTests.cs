using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bespoke.DynamicDnsUpdater.Client;
using NUnit.Framework;

namespace Bespoke.DynamicDnsUpdater.Tests
{
	[TestFixture]
	public class DomainNameTests
	{
		[Test]
		public void CanParseDomainNameWithHost()
		{
			const string domainNameString = "host.test.com";

			var domainName = DomainName.Parse(domainNameString);

			Assert.AreEqual("host", domainName.Host);
			Assert.AreEqual("test.com", domainName.Domain);
			Assert.AreEqual("com", domainName.Tld);
		}

		[Test]
		public void CanParseDomainName()
		{
			const string domainNameString = "test.com";

			var domainName = DomainName.Parse(domainNameString);

			Assert.IsNull(domainName.Host);
			Assert.AreEqual("test.com", domainName.Domain);
			Assert.AreEqual("com", domainName.Tld);
		}
	}
}
