using Bespoke.DynamicDnsUpdater.Client;
using Bespoke.DynamicDnsUpdater.Tests.Base;
using NUnit.Framework;

namespace Bespoke.DynamicDnsUpdater.Tests
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

        [Test]
        public void CanGetIpAddressForHostname()
        {
            var resolver = new IpAddressResolver();
            var ip = resolver.GetIpAddressForHostname("bespokeindustries.com");

            Assert.AreEqual("204.246.37.132", ip);
        }

        [Test]
        public void GetIpAddressForInvalidHostnameReturnsNull()
        {
            var resolver = new IpAddressResolver();
            var ip = resolver.GetIpAddressForHostname("bogus93937329923.bespokeindustries.com");

            Assert.IsNull(ip);
        }
    }
}
