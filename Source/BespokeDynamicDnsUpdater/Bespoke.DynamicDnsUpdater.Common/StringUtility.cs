using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bespoke.DynamicDnsUpdater.Common
{
	public static class StringUtility
	{
		public static int ConvertToInt(string intString, int defaultValue)
		{
			int parsedInt;
			bool parsed = int.TryParse(intString, out parsedInt);

			if(parsed)
			{
				return parsedInt;
			}
			else
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Attempts to convert the given string value to a boolean.  If this fails, then it return the value provided
		/// in the defaultOnFailure parameter.
		/// </summary>
		/// <param name="boolString"></param>
		/// <param name="defaultOnFailure"></param>
		/// <returns></returns>
		public static bool ConvertToBool(string boolString, bool defaultOnFailure)
		{
			bool parsed;
			bool success = bool.TryParse(boolString, out parsed);
			if (success)
			{
				return parsed;
			}
			else
			{
				return defaultOnFailure;
			}
		}


	}
}
