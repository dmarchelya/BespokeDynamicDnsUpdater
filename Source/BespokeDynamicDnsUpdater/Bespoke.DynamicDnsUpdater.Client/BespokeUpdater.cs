using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Bespoke.DynamicDnsUpdater.Client.DnsOMatic;
using log4net;

namespace Bespoke.DynamicDnsUpdater.Client
{
	public class BespokeUpdater
	{
		public BespokeUpdater(DynamicDnsClientBase dynamicDnsClient)
		{
			Client = dynamicDnsClient;
		}

		public DynamicDnsClientBase Client { get; set; }
	}
}
