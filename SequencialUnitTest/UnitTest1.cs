using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace SequencialUnitTest
{
	[SequencialTestClass]
	[TestClass]
	public class UnitTest1
	{
		public TestContext TestContext { get; set; }

		[TestMethod]
		[DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", @"test.csv", "test#csv", DataAccessMethod.Sequential)]
		public void RootTest()
		{
			var testMethodName = TestContext.DataRow["TestMethodName"].ToString();
			Debug.WriteLine(testMethodName);

			this.GetType().GetMethod(testMethodName).Invoke(this, null);
		}

		[SequencialTest(2)]
		public void TestMethod2()
		{
			Debug.WriteLine(nameof(TestMethod2));
		}

		[SequencialTest(3)]
		public void TestMethod3()
		{
			Debug.WriteLine(nameof(TestMethod3));
		}

		[SequencialTest(1)]
		public void TestMethod1()
		{
			Debug.WriteLine(nameof(TestMethod1));
		}
	}
}
