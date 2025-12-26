using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AMLogger;

namespace CustomizationTool;

public class PatchClass
{
	private Logger LogInstance;

	private string LogLineHead = "[PATCHER] : ";

	private string SysDir = "";

	public string bin = AppDomain.CurrentDomain.BaseDirectory + "bin\\";

	private RichTextBox PatcherRTB;

	private string RootDir = "";

	private string newestSys = "";

	private bool ifvar = true;

	public string ImageType = "";

	public PatchClass(Logger logInstance, RichTextBox tb)
	{
		LogInstance = logInstance;
		PatcherRTB = tb;
	}

	private void BackgroundShell(string executable, string command)
	{
		string shellOutput = "";
		Thread newThread = new Thread((ThreadStart)delegate
		{
			ProcessStartInfo startInfo = new ProcessStartInfo("\"" + executable + "\"", command)
			{
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			};
			Process process = new Process();
			process.StartInfo = startInfo;
			process.Start();
			process.WaitForExit();
			shellOutput = process.StandardOutput.ReadToEnd();
			process.Dispose();
		});
		newThread.Start();
		while (newThread.IsAlive)
		{
			Application.DoEvents();
		}
		newThread.Abort();
	}

	public string PatchAuthor(string patch)
	{
		string PatchData = "";
		StreamReader streamReader = new StreamReader(patch);
		PatchData = streamReader.ReadToEnd().Replace("\r\n", "\n");
		streamReader.Close();
		streamReader.Dispose();
		string[] array = PatchData.Split('\n');
		foreach (string line in array)
		{
			if (line.StartsWith("AUTHOR="))
			{
				return line.Split('=')[1];
			}
		}
		return "";
	}

	public string PatchVersion(string patch)
	{
		string PatchData = "";
		StreamReader streamReader = new StreamReader(patch);
		PatchData = streamReader.ReadToEnd().Replace("\r\n", "\n");
		streamReader.Close();
		streamReader.Dispose();
		string[] array = PatchData.Split('\n');
		foreach (string line in array)
		{
			if (line.StartsWith("VERSION="))
			{
				return line.Split('=')[1];
			}
		}
		return "";
	}

	public string PatchDescription(string patch)
	{
		string PatchData = "";
		StreamReader streamReader = new StreamReader(patch);
		PatchData = streamReader.ReadToEnd().Replace("\r\n", "\n");
		streamReader.Close();
		streamReader.Dispose();
		string[] array = PatchData.Split('\n');
		foreach (string line in array)
		{
			if (line.StartsWith("DESCRIPTION="))
			{
				return line.Split('=')[1];
			}
		}
		return "";
	}

	public void CopyFilesRecursively(string Input, string Output)
	{
		Parallel.ForEach(Directory.GetFileSystemEntries(Input, "*", SearchOption.AllDirectories), delegate(string fileName)
		{
			string text = Regex.Replace(fileName, "^" + Regex.Escape(Input), Output);
			if (File.Exists(fileName))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(text));
				File.Copy(fileName, text, overwrite: true);
			}
			else
			{
				Directory.CreateDirectory(text);
			}
		});
	}

	private void DeleteDirectory(string inputDirectory)
	{
		Thread newThread = new Thread((ThreadStart)delegate
		{
			ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", "/C \"rmdir /S /Q \"" + inputDirectory + "\"\"")
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden
			};
			Process process = new Process();
			process.StartInfo = startInfo;
			process.Start();
			process.WaitForExit();
			process.Dispose();
		});
		newThread.Start();
		while (newThread.IsAlive)
		{
			Application.DoEvents();
		}
	}

	public bool RunPatch(string InputPatch, string RootDirectory)
	{
		LogInstance.Log(LogLineHead + "Running patch: " + Path.GetFileNameWithoutExtension(InputPatch) + "..");
		DateTime startTime = DateTime.Now;
		PatcherRTB.Text = "Running patch: " + Path.GetFileNameWithoutExtension(InputPatch) + "..\n\n";
		RootDir = RootDirectory;
		string PatchData = "";
		StreamReader streamReader = new StreamReader(InputPatch);
		PatchData = streamReader.ReadToEnd().Replace("\r\n", "\n");
		streamReader.Close();
		streamReader.Dispose();
		bool ran = true;
		string[] array = PatchData.Split('\n');
		foreach (string line in array)
		{
			if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
			{
				continue;
			}
			if (ifvar)
			{
				if (line.StartsWith("Platform") && !CheckPlatform(line.Split('|')[1]))
				{
					ran = false;
					UpdateRTB("Incorrect platorm.\nThis script is designed for " + line.Split('|')[1]);
					break;
				}
				if (line.StartsWith("EndScript"))
				{
					if (line.Split('|')[1] == "SUCCESS")
					{
						ran = true;
					}
					else if (line.Split('|')[1] == "FAILED")
					{
						ran = false;
					}
					UpdateRTB(line.Split('|')[2]);
					break;
				}
				if (line.StartsWith("WriteLine"))
				{
					UpdateRTB(line.Split('|')[1] + "\n");
				}
				if (line.StartsWith("SearchDeleteFile"))
				{
					SearchDeleteFile(RootDir + line.Split('|')[1].Replace("/", "\\"), line.Split('|')[2]);
				}
				if (line.StartsWith("SearchDeleteDir"))
				{
					SearchDeleteDir(RootDir + line.Split('|')[1].Replace("/", "\\"), line.Split('|')[2]);
				}
				if (line.StartsWith("DeleteFile"))
				{
					DeleteFile(RootDir + line.Split('|')[1].Replace("/", "\\"));
				}
				if (line.StartsWith("DeleteDir"))
				{
					DeleteDir(RootDir + line.Split('|')[1].Replace("/", "\\"));
				}
				if (line.StartsWith("CopyFile"))
				{
					CopyFile(RootDir + line.Split('|')[1].Replace("/", "\\"), RootDir + line.Split('|')[2].Replace("/", "\\"), line.Split('|')[3]);
				}
				if (line.StartsWith("CopyDir"))
				{
					CopyDir(RootDir + line.Split('|')[1].Replace("/", "\\"), RootDir + line.Split('|')[2].Replace("/", "\\"), line.Split('|')[3]);
				}
				if (line.StartsWith("MoveFile"))
				{
					MoveFile(RootDir + line.Split('|')[1].Replace("/", "\\"), RootDir + line.Split('|')[2].Replace("/", "\\"), line.Split('|')[3]);
				}
				if (line.StartsWith("MoveDir"))
				{
					MoveDir(RootDir + line.Split('|')[1].Replace("/", "\\"), RootDir + line.Split('|')[2].Replace("/", "\\"), line.Split('|')[3]);
				}
				if (line.StartsWith("CreateFile"))
				{
					CreateFile(RootDir + line.Split('|')[1].Replace("/", "\\"));
				}
				if (line.StartsWith("CreateDir"))
				{
					CreateDir(RootDir + line.Split('|')[1].Replace("/", "\\"));
				}
				if (line.StartsWith("AppendFile"))
				{
					AppendText(RootDir + line.Split('|')[1].Replace("/", "\\"), line.Split('|')[2]);
				}
				if (line.StartsWith("ReplaceLine"))
				{
					ReplaceLine(RootDir + line.Split('|')[1].Replace("/", "\\"), line.Split('|')[2], line.Split('|')[3], line.Split('|')[4]);
				}
				if (line.StartsWith("ReplaceText"))
				{
					ReplaceText(RootDir + line.Split('|')[1].Replace("/", "\\"), line.Split('|')[2], line.Split('|')[3]);
				}
				if (line.StartsWith("Download"))
				{
					DownloadFile(RootDir + line.Split('|')[1].Replace("/", "\\"), line.Split('|')[2], line.Split('|')[3]);
				}
				if (line.StartsWith("Unzip"))
				{
					UnzipFile(RootDir + line.Split('|')[1].Replace("/", "\\"), RootDir + line.Split('|')[2].Replace("/", "\\"));
				}
			}
			ifvar = true;
			if (line.StartsWith("IfDirExists") && !Directory.Exists(RootDir + line.Split('|')[1].Replace("/", "\\")))
			{
				ifvar = false;
			}
			if (line.StartsWith("IfFileExists") && !File.Exists(RootDir + line.Split('|')[1].Replace("/", "\\")))
			{
				ifvar = false;
			}
			if (line.StartsWith("IfDirNotExists") && Directory.Exists(RootDir + line.Split('|')[1].Replace("/", "\\")))
			{
				ifvar = false;
			}
			if (line.StartsWith("IfFileNotExists") && File.Exists(RootDir + line.Split('|')[1].Replace("/", "\\")))
			{
				ifvar = false;
			}
			if (line.StartsWith("IfContains") && !IfContains(RootDir + line.Split('|')[1].Replace("/", "\\"), line.Split('|')[2]))
			{
				ifvar = false;
			}
			if (line.StartsWith("IfNotContains") && !IfNotContains(RootDir + line.Split('|')[1].Replace("/", "\\"), line.Split('|')[2]))
			{
				ifvar = false;
			}
		}
		UpdateRTB("\nFinished running patch in " + (DateTime.Now - startTime).TotalMilliseconds.ToString().Split('.').First() + "ms");
		return ran;
	}

	private bool CheckPlatform(string platform)
	{
		if (platform.ToLower() == ImageType.ToLower())
		{
			return true;
		}
		return false;
	}

	private bool IfContains(string file, string phrase)
	{
		bool returnval = false;
		if (File.Exists(file))
		{
			StreamReader streamReader = new StreamReader(file);
			if (streamReader.ReadToEnd().Contains(phrase))
			{
				returnval = true;
			}
			streamReader.Close();
			streamReader.Dispose();
		}
		return returnval;
	}

	private bool IfNotContains(string file, string phrase)
	{
		bool returnval = true;
		if (File.Exists(file))
		{
			StreamReader streamReader = new StreamReader(file);
			if (streamReader.ReadToEnd().Contains(phrase))
			{
				returnval = false;
			}
			streamReader.Close();
			streamReader.Dispose();
		}
		return returnval;
	}

	private void UpdateRTB(string inputstring)
	{
		PatcherRTB.Text += inputstring;
		PatcherRTB.Text = PatcherRTB.Text.Replace("/", "\\");
		PatcherRTB.Text = PatcherRTB.Text.Replace(RootDir, "");
		PatcherRTB.Text = PatcherRTB.Text.Replace("\\", "/");
	}

	private void SearchDeleteFile(string inputDirectory, string searchString)
	{
		LogInstance.Log(LogLineHead + "Finding and deleting files with search string " + searchString);
		if (!Directory.Exists(inputDirectory))
		{
			return;
		}
		string[] files = Directory.GetFiles(inputDirectory, searchString, SearchOption.AllDirectories);
		foreach (string file in files)
		{
			try
			{
				LogInstance.Log(LogLineHead + "Deleting file " + file);
				File.Delete(file);
				UpdateRTB("Deleted file " + file + "\n");
			}
			catch
			{
			}
		}
	}

	private void SearchDeleteDir(string inputDirectory, string searchString)
	{
		LogInstance.Log(LogLineHead + "Finding and deleting directories with search string " + searchString);
		if (!Directory.Exists(inputDirectory))
		{
			return;
		}
		string[] directories = Directory.GetDirectories(inputDirectory, searchString, SearchOption.AllDirectories);
		foreach (string dir in directories)
		{
			try
			{
				DeleteDirectory(dir);
				UpdateRTB("Deleted directory " + dir + "\n");
			}
			catch
			{
			}
		}
	}

	private void DeleteFile(string inputFile)
	{
		LogInstance.Log(LogLineHead + "Deleting file " + inputFile);
		if (File.Exists(inputFile))
		{
			File.Delete(inputFile);
			UpdateRTB("Deleted file " + inputFile + "\n");
		}
	}

	private void DeleteDir(string inputDir)
	{
		LogInstance.Log(LogLineHead + "Deleting directory " + inputDir);
		if (Directory.Exists(inputDir))
		{
			DeleteDirectory(inputDir);
			UpdateRTB("Deleted directory " + inputDir + "\n");
		}
	}

	private void CopyFile(string inputFile, string outputFile, string overwrite)
	{
		LogInstance.Log(LogLineHead + "Copying file " + inputFile);
		if (File.Exists(inputFile))
		{
			if (File.Exists(outputFile))
			{
				if (overwrite.ToUpper() == "Y")
				{
					try
					{
						File.Delete(outputFile);
						File.Copy(inputFile, outputFile);
						UpdateRTB("Replaced file " + outputFile + " with " + inputFile + "\n");
						return;
					}
					catch
					{
						UpdateRTB("Failed to copy file " + inputFile + " to " + outputFile + "\n");
						return;
					}
				}
				UpdateRTB("File " + outputFile + " already exists\n");
			}
			else
			{
				File.Copy(inputFile, outputFile);
				UpdateRTB("Copied file " + inputFile + " to " + outputFile + "\n");
			}
		}
		else
		{
			UpdateRTB("File " + inputFile + " not found\n");
		}
	}

	private void CopyDir(string inputDir, string outputDir, string overwrite)
	{
		LogInstance.Log(LogLineHead + "Copying directory " + inputDir);
		if (Directory.Exists(inputDir))
		{
			if (Directory.Exists(outputDir))
			{
				if (overwrite.ToUpper() == "Y")
				{
					try
					{
						DeleteDirectory(outputDir);
						CopyFilesRecursively(inputDir, outputDir);
						UpdateRTB("Replaced directory " + outputDir + " with " + inputDir + "\n");
						return;
					}
					catch
					{
						UpdateRTB("Failed to copy directory " + inputDir + " to " + outputDir + "\n");
						return;
					}
				}
				UpdateRTB("Directory " + outputDir + " already exists\n");
			}
			else
			{
				CopyFilesRecursively(inputDir, outputDir);
				UpdateRTB("Copied directory " + inputDir + " to " + outputDir + "\n");
			}
		}
		else
		{
			UpdateRTB("Directory " + inputDir + " not found\n");
		}
	}

	private void MoveFile(string inputFile, string outputFile, string overwrite)
	{
		LogInstance.Log(LogLineHead + "Moving file " + inputFile);
		if (File.Exists(inputFile))
		{
			if (File.Exists(outputFile))
			{
				if (overwrite.ToUpper() == "Y")
				{
					try
					{
						LogInstance.Log(LogLineHead + "Replacing file " + outputFile + " with " + inputFile);
						File.Delete(outputFile);
						File.Move(inputFile, outputFile);
						UpdateRTB("Replaced file " + outputFile + " with " + inputFile + "\n");
						return;
					}
					catch
					{
						UpdateRTB("Failed to move file " + inputFile + " to " + outputFile + "\n");
						return;
					}
				}
				UpdateRTB("File " + outputFile + " already exists\n");
			}
			else
			{
				File.Move(inputFile, outputFile);
				UpdateRTB("Moved file " + inputFile + " to " + outputFile + "\n");
			}
		}
		else
		{
			UpdateRTB("File " + inputFile + " not found\n");
		}
	}

	private void MoveDir(string inputDir, string outputDir, string overwrite)
	{
		LogInstance.Log(LogLineHead + "Moving directory " + outputDir);
		if (Directory.Exists(inputDir))
		{
			if (Directory.Exists(outputDir))
			{
				if (overwrite.ToUpper() == "Y")
				{
					try
					{
						DeleteDirectory(outputDir);
						Directory.Move(inputDir, outputDir);
						UpdateRTB("Replaced directory " + outputDir + " with " + inputDir + "\n");
						return;
					}
					catch
					{
						UpdateRTB("Failed to move directory " + inputDir + " to " + outputDir + "\n");
						return;
					}
				}
				UpdateRTB("Directory " + outputDir + " already exists\n");
			}
			else
			{
				Directory.Move(inputDir, outputDir);
				UpdateRTB("Moved directory " + inputDir + " to " + outputDir + "\n");
			}
		}
		else
		{
			UpdateRTB("Directory " + inputDir + " not found\n");
		}
	}

	private void CreateFile(string inputFile)
	{
		LogInstance.Log(LogLineHead + "Creating file " + inputFile);
		if (!File.Exists(inputFile))
		{
			File.Create(inputFile);
			UpdateRTB("Created file " + inputFile + "\n");
		}
		else
		{
			UpdateRTB("File " + inputFile + " already exists\n");
		}
	}

	private void CreateDir(string inputDir)
	{
		LogInstance.Log(LogLineHead + "Creating directory " + inputDir);
		if (!Directory.Exists(inputDir))
		{
			Directory.CreateDirectory(inputDir);
			UpdateRTB("Created directory " + inputDir + "\n");
		}
		else
		{
			UpdateRTB("Directory " + inputDir + " already exists\n");
		}
	}

	private void AppendText(string inputFile, string text)
	{
		LogInstance.Log(LogLineHead + "Appending file " + inputFile);
		if (File.Exists(inputFile))
		{
			File.AppendAllText(inputFile, text.Replace("\\n", "\n"));
			UpdateRTB("Appended file " + inputFile + " with " + text + "\n");
		}
		else
		{
			UpdateRTB("File " + inputFile + " does not exists\n");
		}
	}

	private void ReplaceLine(string inputFile, string position, string searchString, string replaceString)
	{
		LogInstance.Log(LogLineHead + "Replacing lines in file " + inputFile);
		if (File.Exists(inputFile))
		{
			StreamReader streamReader = new StreamReader(inputFile);
			string data = streamReader.ReadToEnd();
			streamReader.Close();
			streamReader.Dispose();
			string newdata = "";
			string[] array = data.Split('\n');
			foreach (string line in array)
			{
				string newline = line;
				if (position == "START" && line.StartsWith(searchString))
				{
					newline = replaceString.Replace("\\n", "\n");
				}
				if (position == "END" && line.EndsWith(searchString))
				{
					newline = replaceString.Replace("\\n", "\n");
				}
				if (position == "CONTAINS" && line.Contains(searchString))
				{
					newline = replaceString.Replace("\\n", "\n");
				}
				newdata = newdata + newline + "\n";
			}
			try
			{
				File.Delete(inputFile);
				StreamWriter streamWriter = new StreamWriter(inputFile);
				streamWriter.Write(newdata.Remove(newdata.Length - 1));
				streamWriter.Close();
				streamWriter.Dispose();
				UpdateRTB("Replaced lines in file " + inputFile + "\n");
				return;
			}
			catch
			{
				UpdateRTB("Failed to replace lines in file " + inputFile + "\n");
				return;
			}
		}
		UpdateRTB("File " + inputFile + " does not exists\n");
	}

	private void ReplaceText(string inputFile, string searchString, string replaceString)
	{
		LogInstance.Log(LogLineHead + "Replacing text in file " + inputFile);
		if (File.Exists(inputFile))
		{
			StreamReader streamReader = new StreamReader(inputFile);
			string data = streamReader.ReadToEnd();
			streamReader.Close();
			streamReader.Dispose();
			try
			{
				File.Delete(inputFile);
				StreamWriter streamWriter = new StreamWriter(inputFile);
				streamWriter.Write(data.Replace(searchString, replaceString.Replace("\\n", "\n")));
				streamWriter.Close();
				streamWriter.Dispose();
				UpdateRTB("Replaced text in file " + inputFile + "\n");
				return;
			}
			catch
			{
				UpdateRTB("Failed to text lines in file " + inputFile + "\n");
				return;
			}
		}
		UpdateRTB("File " + inputFile + " does not exists\n");
	}

	private void DownloadFile(string outputFile, string overwrite, string inputURL)
	{
		LogInstance.Log(LogLineHead + "Downloading file " + Path.GetFileName(outputFile));
		try
		{
			if (!Directory.Exists(Path.GetDirectoryName(outputFile)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
			}
			if (File.Exists(outputFile))
			{
				if (overwrite.ToUpper() == "Y")
				{
					if (File.Exists(outputFile))
					{
						File.Delete(outputFile);
					}
					Thread DLThread = new Thread((ThreadStart)delegate
					{
						WebClient obj2 = new WebClient
						{
							Headers = { { "a", "a" } }
						};
						ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
						obj2.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
						obj2.Proxy = null;
						obj2.DownloadFile(inputURL, outputFile);
					});
					DLThread.Start();
					while (DLThread.IsAlive)
					{
						Application.DoEvents();
					}
					UpdateRTB("Downloaded file to " + outputFile + "\n");
				}
				else
				{
					UpdateRTB("File " + Path.GetFileName(outputFile) + " already exists, use overwrite flag\n");
				}
			}
			else
			{
				Thread DLThread2 = new Thread((ThreadStart)delegate
				{
					WebClient obj2 = new WebClient
					{
						Headers = { { "a", "a" } }
					};
					ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
					obj2.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
					obj2.Proxy = null;
					obj2.DownloadFile(inputURL, outputFile);
				});
				DLThread2.Start();
				while (DLThread2.IsAlive)
				{
					Application.DoEvents();
				}
				UpdateRTB("Downloaded file to " + outputFile + "\n");
			}
		}
		catch
		{
			UpdateRTB("Failed to download file " + Path.GetFileName(outputFile) + "\n");
		}
	}

	private void UnzipFile(string InputFile, string OutputFolder)
	{
		LogInstance.Log(LogLineHead + "Unzipping file " + InputFile);
		if (File.Exists(InputFile))
		{
			if (Directory.Exists(OutputFolder))
			{
				Thread newThread = new Thread((ThreadStart)delegate
				{
					BackgroundShell(bin + "7za", " x -y \"" + InputFile + "\" -o\"" + OutputFolder + "\"");
				});
				newThread.Start();
				while (newThread.IsAlive)
				{
					Application.DoEvents();
				}
				UpdateRTB("Unzipped file " + InputFile + "\n");
			}
			else
			{
				UpdateRTB("Directory " + OutputFolder + " does not exist\n");
			}
		}
		else
		{
			UpdateRTB("File " + InputFile + " does not exist\n");
		}
	}
}
