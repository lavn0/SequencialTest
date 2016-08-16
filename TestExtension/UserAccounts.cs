using System;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.Globalization;
using Microsoft.Win32;
using System.Diagnostics;
using System.Security.Permissions;
using System.DirectoryServices;
using System.Security.Principal;

namespace DemoExtension.RunAs
{
	public class UserAccounts
	{
		public enum GroupType
		{
			Users
		};
		static string[] strGroupType = { "Users" };

		static string[] sidGroupType = { "S - 1 - 5 - 32 - 545" };

		//This is for localization runs where Administrators 
		//   might be Administrateurs etc 
		static int findIndexForGroupType(string groupType)
		{
			for (int i = 0; i < strGroupType.Length; i++)
			{

				if (strGroupType[i].ToLower().Equals(
								groupType.ToLower()))
				{
					return i;
				}
			}
			return -1;
		}

		public static string GetLocalGroupString(string groupType)
		{

			int index = findIndexForGroupType(groupType);
			if (index < 0)
				throw new ArgumentException("groupType");
			SecurityIdentifier sid =
				 new SecurityIdentifier(sidGroupType[index]);
			NTAccount ntaccount =
				 sid.Translate(typeof(NTAccount)) as NTAccount;

			string[] accountTokens = ntaccount.ToString().Split('\\');
			switch (accountTokens.Length)
			{
				case 2:
					return accountTokens[1];
				case 1:
					return accountTokens[0];
				default:
					throw new Exception("Account Token not in the known format");
			}
		}

		public static void CreateUserInMachine(string User,
					 string Password, GroupType groupType)
		{
			InternalCreateUserInMachine(User, Password, groupType);
		}

		static void InternalCreateUserInMachine(string User, string Password, GroupType groupType)
		{
			try
			{
				InternalRemoveUserFromMachine(User);
			}
			catch { }

			DirectoryEntry AD = new DirectoryEntry("WinNT://" +
								Environment.MachineName + ", computer");
			DirectoryEntry NewUser = AD.Children.Add(User, "user");
			NewUser.Invoke("SetPassword", new object[] { Password });
			NewUser.Invoke("Put", new object[] {
							 "Description", "Test User from .NET" });
			NewUser.CommitChanges();

			DirectoryEntry grp;

			grp = AD.Children.Find(GetLocalGroupString(
						 strGroupType[(int)groupType]), "group");
			if (grp != null)
			{
				grp.Invoke("Add", new object[] { NewUser.Path.ToString() });
			}
		}

		public static void RemoveUserFromMachine(string User)
		{
			InternalRemoveUserFromMachine(User);
		}

		static void InternalRemoveUserFromMachine(string User)
		{
			try
			{
				DirectoryEntry AD = new DirectoryEntry("WinNT://" +
									Environment.MachineName + ", computer");
				DirectoryEntry UserToRemove =
						  AD.Children.Find(User, "user");
				AD.Children.Remove(UserToRemove);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
