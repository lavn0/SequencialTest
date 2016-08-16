using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoExtension.RunAs
{
	public class RunAsTestExtensionExecution : TestExtensionExecution
	{
		public override void Initialize(TestExecution execution)
		{
		}

		public override void Dispose()
		{
		}

		public override ITestMethodInvoker CreateTestMethodInvoker(TestMethodInvokerContext context)
		{
			return new RunAsTestMethodInvoker(context);
		}
	}
}