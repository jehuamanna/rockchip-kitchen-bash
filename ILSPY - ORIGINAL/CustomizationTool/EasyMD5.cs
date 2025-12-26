using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CustomizationTool;

internal class EasyMD5
{
	private static string GetMd5Hash(byte[] data)
	{
		StringBuilder sBuilder = new StringBuilder();
		for (int i = 0; i < data.Length; i++)
		{
			sBuilder.Append(data[i].ToString("x2"));
		}
		return sBuilder.ToString();
	}

	private static bool VerifyMd5Hash(byte[] data, string hash)
	{
		return StringComparer.OrdinalIgnoreCase.Compare(GetMd5Hash(data), hash) == 0;
	}

	public static string Hash(string data)
	{
		using MD5 md5 = MD5.Create();
		return GetMd5Hash(md5.ComputeHash(Encoding.UTF8.GetBytes(data)));
	}

	public static string Hash(FileStream data)
	{
		using MD5 md5 = MD5.Create();
		return GetMd5Hash(md5.ComputeHash(data));
	}

	public static bool Verify(string data, string hash)
	{
		using MD5 md5 = MD5.Create();
		return VerifyMd5Hash(md5.ComputeHash(Encoding.UTF8.GetBytes(data)), hash);
	}

	public static bool Verify(FileStream data, string hash)
	{
		using MD5 md5 = MD5.Create();
		return VerifyMd5Hash(md5.ComputeHash(data), hash);
	}
}
