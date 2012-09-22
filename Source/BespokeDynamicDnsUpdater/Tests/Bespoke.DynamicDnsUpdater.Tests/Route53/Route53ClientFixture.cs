using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bespoke.DynamicDnsUpdater.Client.Route53;
using NUnit.Framework;

namespace Bespoke.DynamicDnsUpdater.Tests.Route53
{
	[TestFixture]
	public class Route53ClientFixture
	{
		[Test]
		public void CanUpdateHostname()
		{
			const string awsAccessKeyId = @"";
			const string awsSecretAccessKey = @"";
			var client = new Route53Client(awsAccessKeyId, awsSecretAccessKey);

			client.UpdateHostName("ddnsupdater.bespokeindustries.com", "127.0.0.1");
		}
	}
}
