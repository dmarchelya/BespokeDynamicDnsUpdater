using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DnsOMaticClient.Net.Tests.Base
{
	public class BaseFixture
	{
		/// <summary>
		/// We set the Expected Public IP Address in the app.config so testers in different
		/// environments can run the unit test suite and expect the tests to pass.
		/// </summary>
		public string ExpectedPublicIpAddress
		{
			get { return ConfigurationManager.AppSettings["ExpectedPublicIpAddress"]; }
		}
	}
}
