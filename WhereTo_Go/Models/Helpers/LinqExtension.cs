using System;
using System.Collections.Generic;
using System.Text;

namespace WhereTo_Go
{
	public static class LinqExtension
	{
		public static string GenerateFBQuery(this IEnumerable<string> source)
		{
			StringBuilder result = new StringBuilder();
			result.Append ("?ids=");
			foreach(string item in source)
			{
				result.Append(string.Format("{0},",item));
			}
			result.Length--;
			return result.ToString ();
		}
	}
}

