using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bespoke.DynamicDnsUpdater.Client
{
	//TODO: Implement as enum
	public static class DnsRecordTypes
	{
		public const string Soa = "SOA";
		public const string A = "A";
		public const string Txt = "TXT";
		public const string Ns = "NS";
		public const string Cname = "CNAME";
		public const string Mx = "MX";
		public const string Ptr = "PTR";
		public const string Srv = "SRV";
		public const string Spf = "SPF";
		public const string Aaaa = "AAAA";
	}
}
