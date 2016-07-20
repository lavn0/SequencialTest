using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SequencialUnitTest
{
	public class SequencialTestClassAttribute : Attribute
	{
		static SequencialTestClassAttribute()
		{
			var methods = typeof(UnitTest1).GetMethods();
			var sortedList = new SortedList<int, MethodInfo>();
			foreach (var method in methods)
			{
				var att = method.GetCustomAttribute<SequencialTestAttribute>();
				if (att == null)
				{
					continue;
				}

				var sequenceNumber = att.SequencialNumber;
				sortedList.Add(sequenceNumber, method);
			}

			var text = "TestMethodName\r\n" + string.Join("\r\n", sortedList.Values.Select(m => m.Name));
			File.WriteAllText("test.csv", text);
		}
	}
}
