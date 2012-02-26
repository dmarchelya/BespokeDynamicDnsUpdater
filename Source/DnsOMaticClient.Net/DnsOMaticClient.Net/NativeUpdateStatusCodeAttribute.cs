using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DnsOMaticClient.Net
{
	/// <summary>
	/// The NativeUpdateStatusCodeAttribute.Value represents the string value that is
	/// returned from a DNS-O-Matic update request and specifies the update status.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class NativeUpdateStatusCodeAttribute : Attribute
	{
		private string value;

		public NativeUpdateStatusCodeAttribute(string value)
		{
			this.value = value;
		}

		public string Value
		{
			get { return value; }
		}
	}
}
