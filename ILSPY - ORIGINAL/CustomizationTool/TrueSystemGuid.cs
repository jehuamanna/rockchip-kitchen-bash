using System;
using System.Collections.Generic;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace CustomizationTool;

public class TrueSystemGuid
{
	private static string _systemGuid = string.Empty;

	private static List<string> ListOfCpuProperties = new List<string> { "UniqueId", "ProcessorId", "Name", "Manufacturer" };

	private static List<string> ListOfMainboardProperties = new List<string> { "Model", "Manufacturer", "Name", "SerialNumber" };

	private static List<string> ListOfGpuProperties = new List<string> { "Name" };

	public static string Value()
	{
		if (string.IsNullOrEmpty(_systemGuid))
		{
			string lCpuId = GetCpuId();
			string lMainboard = GetMainboardId();
			string lGpuId = GetGpuId();
			_systemGuid = GetHash("CPU: " + lCpuId + "\nMainboard: " + lMainboard + "\nGPU: " + lGpuId);
		}
		return _systemGuid;
	}

	private static string GetHash(string s)
	{
		try
		{
			return new Guid(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(s))).ToString().ToUpper();
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private static string GetIdentifier(string pWmiClass, List<string> pProperties)
	{
		string lResult = string.Empty;
		try
		{
			foreach (ManagementObject lItem in new ManagementClass(pWmiClass).GetInstances())
			{
				foreach (string lProperty in pProperties)
				{
					try
					{
						if (!(lProperty == "MACAddress"))
						{
							goto IL_0076;
						}
						if (!string.IsNullOrWhiteSpace(lResult))
						{
							return lResult;
						}
						if (!(lItem["IPEnabled"].ToString() != "True"))
						{
							goto IL_0076;
						}
						goto end_IL_003a;
						IL_0076:
						object lItemProperty = lItem[lProperty];
						if (lItemProperty != null)
						{
							string lValue = lItemProperty.ToString();
							if (!string.IsNullOrWhiteSpace(lValue))
							{
								lResult = lResult + lValue + "; ";
							}
						}
						end_IL_003a:;
					}
					catch
					{
					}
				}
			}
		}
		catch
		{
		}
		return lResult.TrimEnd(' ', ';');
	}

	private static string GetCpuId()
	{
		return GetIdentifier("Win32_Processor", ListOfCpuProperties);
	}

	private static string GetMainboardId()
	{
		return GetIdentifier("Win32_BaseBoard", ListOfMainboardProperties);
	}

	private static string GetGpuId()
	{
		return GetIdentifier("Win32_VideoController", ListOfGpuProperties);
	}
}
