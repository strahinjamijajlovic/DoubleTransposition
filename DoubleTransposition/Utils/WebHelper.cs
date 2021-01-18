using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DoubleTransposition.Utils
{
	public static class WebHelper
	{
		public static string GetDescription(this Enum value)
		{
			var fi = value.GetType().GetField(value.ToString());
			var attributes = fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
			return attributes?.FirstOrDefault()?.Description ?? value.ToString();
		}
	}
}
