using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoExtension.RunAs
{
	[Serializable]
	public class RunAsTestClassAttribute : TestClassExtensionAttribute
	{
		public override Uri ExtensionId { get; } = new Uri("urn: Microsoft.RunAsAttribute");

		public override TestExtensionExecution GetExecution()
		{
			return new RunAsTestExtensionExecution();
		}

		public override object GetClientSide()
		{
			return base.GetClientSide();
		}
	}
}
