using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace DemoExtension.RunAs
{
	public enum LogonType
	{
		Interactive = 2,
	}

	public enum LogonProvider
	{
		Default = 0,
	}

	public sealed class RunProcessAs : IDisposable
	{
		WindowsImpersonationContext impersonationContext = null;

		public RunProcessAs(string user, string domain, string password)
		{
			Impersonate(user, domain, password, LogonType.Interactive, LogonProvider.Default);
		}

		public RunProcessAs(string user, string domain,
			string password, LogonType logonType,
			LogonProvider logonProvider)
		{
			Impersonate(user, domain, password, logonType, logonProvider);
		}

		~RunProcessAs()
		{
		}

		public void Dispose()
		{
			if (constructorThread != Thread.CurrentThread)
			{
				throw new ApplicationException("Dispose should be called on the same thread as instance constructor.");
			}

			Exception ex = null;
			if (IntPtr.Zero != token)
			{
				impersonationContext.Undo();
				if (!NativeMethods.CloseHandle(token))
				{
					ex = new Win32Exception(Marshal.GetLastWin32Error());
				}

				token = IntPtr.Zero;
			}
			GC.KeepAlive(this);
			GC.SuppressFinalize(this);
			if (ex != null) throw ex;
		}

		private void Impersonate(string user, string domain,
			string password, LogonType logonType,
			LogonProvider logonProvider)
		{
			if (null == user) throw new ArgumentNullException();
			if (null == domain) throw new ArgumentNullException();
			if (null == password) throw new ArgumentNullException();
			// 
			if (!NativeMethods.LogonUser(user, domain, password, logonType, logonProvider, out token))
			{
				throw new Win32Exception(Marshal.GetLastWin32Error());
			}

			impersonationContext = WindowsIdentity.Impersonate(token);
			if (impersonationContext == null)
			{
				NativeMethods.CloseHandle(token);
				token = IntPtr.Zero;
				throw new Exception("Failed to impersonate specified user");
			}

			constructorThread = Thread.CurrentThread;
			GC.KeepAlive(this);
		}

		private IntPtr token = IntPtr.Zero;
		private Thread constructorThread = null;
	}

	abstract class NativeMethods
	{
		[DllImport("advapi32.dll", SetLastError = true)]
		internal extern static bool LogonUser(string user, string domain, string password, LogonType logonType, LogonProvider provider, out IntPtr token);
		[DllImport("kernel32.dll", SetLastError = true)]
		internal extern static bool CloseHandle(IntPtr handle);
		[DllImport("advapi32.dll", SetLastError = true)]
		internal extern static bool ImpersonateLoggedOnUser(IntPtr token);
		[DllImport("advapi32.dll", SetLastError = true)]
		internal extern static bool RevertToSelf();
	}
}
