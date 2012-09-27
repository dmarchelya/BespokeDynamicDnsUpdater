using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bespoke.DynamicDnsUpdater.Common
{
	public class StringUtility
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
	}
}
