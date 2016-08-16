using System;
using System.Security.Principal;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemoExtension.RunAs
{
	public class RunAsTestMethodInvoker : ITestMethodInvoker
	{
		private readonly TestMethodInvokerContext m_context;

		public RunAsTestMethodInvoker(TestMethodInvokerContext context)
		{
			Debug.Assert(context != null);
			m_context = context;
		}

		public TestMethodInvokerResult Invoke(params object[] args)
		{
			// Log the ID of the test user 
			Trace.WriteLine("Begin Invoke: current user is " + WindowsIdentity.GetCurrent().Name);
			RunProcessAs runas = null;

			bool runAsNormalUser;
			Boolean.TryParse(m_context.TestContext.Properties["RunAsNormalUser"] as string, out runAsNormalUser);

			if (runAsNormalUser)
			{
				Trace.WriteLine("Creating user: " + USER);
				UserAccounts.CreateUserInMachine(USER, PASSWORD, UserAccounts.GroupType.Users);

				// Impersonate a user with minimal privileges 
				Trace.WriteLine("Impersonating user: " + USER);
				runas = new RunProcessAs(USER, DOMAIN, PASSWORD);
			}

			// Invoke the user’s test method 
			Trace.WriteLine("Invoking test method");

			try
			{
				return m_context.InnerInvoker.Invoke();
			}
			finally
			{
				if (runas != null)
				{
					// Undo the impersonation 
					Trace.WriteLine("Undoing impersonation of user: " + USER);
					runas.Dispose();

					Trace.WriteLine("Removing user: " + USER);
					UserAccounts.RemoveUserFromMachine(USER);
				}

				// Log the ID of the test user 
				Trace.WriteLine("End Invoke: current user is " + WindowsIdentity.GetCurrent().Name);
			}
		}

		private const string USER = "DemoUser";
		private const string DOMAIN = "";
		private const string PASSWORD = "abc123!!";
	}
}