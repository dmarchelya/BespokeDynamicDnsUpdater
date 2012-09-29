using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bespoke.DynamicDnsUpdater.Common
{
	public static class Constants
	{
		public const string EncryptionKey = @"FT+O6hirmyH5T779BPW0CrUkjgkRR8HVVwAZ6BXAxVI=";

		/// <summary>
		/// Ideally this would be regenerated each time, and stored along with each value,
		/// but this is probably overkill for this app.
		/// </summary>
		public const string InitializationVector = @"nA4mPtXU4OORINpZeZm3ww==";
	}
}
