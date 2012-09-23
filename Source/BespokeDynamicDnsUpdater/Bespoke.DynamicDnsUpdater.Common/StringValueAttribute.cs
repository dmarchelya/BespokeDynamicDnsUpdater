using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bespoke.DynamicDnsUpdater.Common
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class StringValueAttribute : Attribute
	{
		public StringValueAttribute(string value)
		{
			Value = value;
		}

		public string Value { get; set; }
	}
}
