using System;
using System.Collections.Generic;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace CustomizationTool;

public class SystemGuid
{
	private static string _systemGuid = string.Empty;

	private static List<string> ListOfCpuProperties = new List<string> { "UniqueId", "ProcessorId", "Name", "Manufacturer" };

	private static List<string> ListOfBiosProperties = new List<string> { "Manufacturer", "SMBIOSBIOSVersion", "IdentificationCode", "SerialNumber", "ReleaseDate", "Version" };

	private static List<string> ListOfMainboardProperties = new List<string> { "Model", "Manufacturer", "Name", "SerialNumber" };

	private static List<string> ListOfGpuProperties = new List<string> { "Name" };

	private static List<string> ListOfNetworkProperties = new List<string> { "MACAddress" };

	public static string Value()
	{
		if (string.IsNullOrEmpty(_systemGuid))
		{
			string lCpuId = GetCpuId();
			string lBiodId = GetBiosId();
			string lMainboard = GetMainboardId();
			string lGpuId = GetGpuId();
			string lMac = GetMac();
			_systemGuid = GetHash("CPU: " + lCpuId + "\nBIOS:" + lBiodId + "\nMainboard: " + lMainboard + "\nGPU: " + lGpuId + "\nMAC: " + lMac);
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

	private static string GetBiosId()
	{
		return GetIdentifier("Win32_BIOS", ListOfBiosProperties);
	}

	private static string GetMainboardId()
	{
		return GetIdentifier("Win32_BaseBoard", ListOfMainboardProperties);
	}

	private static string GetGpuId()
	{
		return GetIdentifier("Win32_VideoController", ListOfGpuProperties);
	}

	private static string GetMac()
	{
		return GetIdentifier("Win32_NetworkAdapterConfiguration", ListOfNetworkProperties);
	}
}
