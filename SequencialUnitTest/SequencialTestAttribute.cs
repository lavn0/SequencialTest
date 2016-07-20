using System;

namespace SequencialUnitTest
{
	public class SequencialTestAttribute : Attribute
	{
		public int SequencialNumber { get; }

		public SequencialTestAttribute(int sequencialNumber)
		{
			this.SequencialNumber = sequencialNumber;
		}
	}
}
