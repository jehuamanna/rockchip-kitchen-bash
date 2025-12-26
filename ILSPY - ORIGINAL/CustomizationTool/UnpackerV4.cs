using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AMLogger;
using CustomizationTool.Properties;

namespace CustomizationTool;

public class UnpackerV4
{
	public enum ImgType
	{
		Amlogic,
		Rockchip,
		Allwinner,
		None
	}

	public enum UpdateFormat
	{
		Image,
		Zip
	}

	public enum Architecture
	{
		Unknown,
		x86,
		x64,
		arm64
	}

	private struct SYSTEM_INFO
	{
		public short wProcessorArchitecture;

		public short wReserved;

		public int dwPageSize;

		public IntPtr lpMinimumApplicationAddress;

		public IntPtr lpMaximumApplicationAddress;

		public IntPtr dwActiveProcessorMask;

		public int dwNumberOfProcessors;

		public int dwProcessorType;

		public int dwAllocationGranularity;

		public short wProcessorLevel;

		public short wProcessorRevision;
	}

	private Logger LogInstance;

	private string LogLineHead = "[UNPACKER] : ";

	public string bin = AppDomain.CurrentDomain.BaseDirectory + "bin\\";

	public string binarch = AppDomain.CurrentDomain.BaseDirectory + "bin\\common\\";

	public string binpython = AppDomain.CurrentDomain.BaseDirectory + "bin\\common\\python\\";

	public string ShellOutput = "";

	private bool debug;

	private bool stdout;

	private bool redirectOutput = true;

	private string ShellWorkDir = AppDomain.CurrentDomain.BaseDirectory;

	private const int PROCESSOR_ARCHITECTURE_AMD64 = 9;

	private const int PROCESSOR_ARCHITECTURE_INTEL = 0;

	private const int PROCESSOR_ARCHITECTURE_ARM64 = 12;

	private const int IMAGE_FILE_MACHINE_ARM64 = 43620;

	private const int IMAGE_FILE_MACHINE_I386 = 332;

	private const int IMAGE_FILE_MACHINE_AMD64 = 34404;

	public static Architecture ProcessArchitecture
	{
		get
		{
			SYSTEM_INFO si = default(SYSTEM_INFO);
			GetSystemInfo(ref si);
			return GetArchitecture(ref si);
		}
	}

	public static Architecture MachineArchitecture
	{
		get
		{
			SYSTEM_INFO si = default(SYSTEM_INFO);
			GetNativeSystemInfo(ref si);
			return GetArchitecture(ref si);
		}
	}

	public UnpackerV4(Logger logInstance)
	{
		LogInstance = logInstance;
		binarch = binarch + MachineArchitecture.ToString() + "\\";
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

	private void BackgroundShell(string executable, string command)
	{
		if (debug)
		{
			LogInstance.Log(LogLineHead + "[EXEC] \"" + executable + "\" " + command);
		}
		redirectOutput = true;
		string shellOutput = "";
		Thread newThread = new Thread((ThreadStart)delegate
		{
			ProcessStartInfo startInfo = new ProcessStartInfo("\"" + executable + "\"", command)
			{
				UseShellExecute = false,
				RedirectStandardOutput = redirectOutput,
				RedirectStandardError = redirectOutput,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				WorkingDirectory = ShellWorkDir
			};
			Process process = new Process();
			process.StartInfo = startInfo;
			process.Start();
			process.WaitForExit();
			if (redirectOutput)
			{
				shellOutput = process.StandardOutput.ReadToEnd();
			}
			if (redirectOutput)
			{
				shellOutput = shellOutput + "\n" + process.StandardError.ReadToEnd();
			}
			process.Dispose();
		});
		newThread.Start();
		while (newThread.IsAlive)
		{
			Application.DoEvents();
		}
		newThread.Abort();
		if (stdout)
		{
			LogInstance.Log(LogLineHead + "[STDOUT] " + shellOutput);
		}
		ShellOutput = shellOutput;
	}

	private void DeleteDirectory(string inputDirectory)
	{
		LogInstance.Log(LogLineHead + "Rescursive delete \"" + inputDirectory + "\"");
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

	public bool RepackZipFile(string inputDirectory, string outputFile, string compression)
	{
		LogInstance.Log(LogLineHead + "Packing upgrade zip file..");
		if (Directory.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp"))
		{
			DeleteDirectory(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp");
		}
		Directory.CreateDirectory(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp");
		if (File.Exists(inputDirectory + "\\boot.PARTITION"))
		{
			File.Copy(inputDirectory + "\\boot.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\boot.img");
		}
		if (File.Exists(inputDirectory + "\\dtbo.PARTITION"))
		{
			File.Copy(inputDirectory + "\\dtbo.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\dtbo.img");
		}
		if (File.Exists(inputDirectory + "\\logo.PARTITION"))
		{
			File.Copy(inputDirectory + "\\logo.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\logo.img");
		}
		if (File.Exists(inputDirectory + "\\recovery.PARTITION"))
		{
			File.Copy(inputDirectory + "\\recovery.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\recovery.img");
		}
		if (File.Exists(inputDirectory + "\\vbmeta.PARTITION"))
		{
			File.Copy(inputDirectory + "\\vbmeta.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\vbmeta.img");
		}
		if (File.Exists(inputDirectory + "\\super.PARTITION"))
		{
			File.Copy(inputDirectory + "\\super.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\super.img");
		}
		if (File.Exists(inputDirectory + "\\boot_a.PARTITION"))
		{
			File.Copy(inputDirectory + "\\boot_a.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\boot.img");
		}
		if (File.Exists(inputDirectory + "\\dtbo_a.PARTITION"))
		{
			File.Copy(inputDirectory + "\\dtbo_a.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\dtbo.img");
		}
		if (File.Exists(inputDirectory + "\\logo_a.PARTITION"))
		{
			File.Copy(inputDirectory + "\\logo_a.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\logo.img");
		}
		if (File.Exists(inputDirectory + "\\recovery_a.PARTITION"))
		{
			File.Copy(inputDirectory + "\\recovery_a.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\recovery.img");
		}
		if (File.Exists(inputDirectory + "\\vbmeta_a.PARTITION"))
		{
			File.Copy(inputDirectory + "\\vbmeta_a.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\vbmeta.img");
		}
		if (File.Exists(inputDirectory + "\\super_a.PARTITION"))
		{
			File.Copy(inputDirectory + "\\super_a.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\super.img");
		}
		if (File.Exists(inputDirectory + "\\boot.img"))
		{
			File.Copy(inputDirectory + "\\boot.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\boot.img");
		}
		if (File.Exists(inputDirectory + "\\dtbo.img"))
		{
			File.Copy(inputDirectory + "\\dtbo.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\dtbo.img");
		}
		if (File.Exists(inputDirectory + "\\logo.img"))
		{
			File.Copy(inputDirectory + "\\logo.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\logo.img");
		}
		if (File.Exists(inputDirectory + "\\recovery.img"))
		{
			File.Copy(inputDirectory + "\\recovery.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\recovery.img");
		}
		if (File.Exists(inputDirectory + "\\vbmeta.img"))
		{
			File.Copy(inputDirectory + "\\vbmeta.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\vbmeta.img");
		}
		if (File.Exists(inputDirectory + "\\super.img"))
		{
			File.Copy(inputDirectory + "\\super.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\super.img");
		}
		if (File.Exists(inputDirectory + "\\boot_a.img"))
		{
			File.Copy(inputDirectory + "\\boot_a.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\boot.img");
		}
		if (File.Exists(inputDirectory + "\\dtbo_a.img"))
		{
			File.Copy(inputDirectory + "\\dtbo_a.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\dtbo.img");
		}
		if (File.Exists(inputDirectory + "\\logo_a.img"))
		{
			File.Copy(inputDirectory + "\\logo_a.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\logo.img");
		}
		if (File.Exists(inputDirectory + "\\recovery_a.img"))
		{
			File.Copy(inputDirectory + "\\recovery_a.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\recovery.img");
		}
		if (File.Exists(inputDirectory + "\\vbmeta_a.img"))
		{
			File.Copy(inputDirectory + "\\vbmeta_a.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\vbmeta.img");
		}
		if (File.Exists(inputDirectory + "\\super_a.img"))
		{
			File.Copy(inputDirectory + "\\super_a.img", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\super.img");
		}
		if (File.Exists(inputDirectory + "\\ddr.USB"))
		{
			File.Copy(inputDirectory + "\\ddr.USB", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\bootloader.img");
		}
		if (File.Exists(inputDirectory + "\\DDR.USB"))
		{
			if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\bootloader.img"))
			{
				File.Delete(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\bootloader.img");
			}
			File.Copy(inputDirectory + "\\DDR.USB", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\bootloader.img");
		}
		if (File.Exists(inputDirectory + "\\_aml_dtb.PARTITION"))
		{
			File.Copy(inputDirectory + "\\_aml_dtb.PARTITION", Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\dt.img");
		}
		if (File.Exists(inputDirectory + "\\system_ext.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p system_ext \"" + inputDirectory + "\\system_ext.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\oem.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p oem \"" + inputDirectory + "\\oem.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\odm.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p odm \"" + inputDirectory + "\\odm.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\product.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p product \"" + inputDirectory + "\\product.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\vendor.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p vendor \"" + inputDirectory + "\\vendor.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\system.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p system \"" + inputDirectory + "\\system.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\system_ext_a.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p system_ext \"" + inputDirectory + "\\system_ext_a.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\oem_a.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p oem \"" + inputDirectory + "\\oem_a.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\odm_a.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p odm \"" + inputDirectory + "\\odm_a.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\product_a.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p product \"" + inputDirectory + "\\product_a.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\vendor_a.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p vendor \"" + inputDirectory + "\\vendor_a.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\system_a.PARTITION"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p system \"" + inputDirectory + "\\system_a.PARTITION\"");
		}
		if (File.Exists(inputDirectory + "\\system_ext.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p system_ext \"" + inputDirectory + "\\system_ext.img\"");
		}
		if (File.Exists(inputDirectory + "\\oem.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p oem \"" + inputDirectory + "\\oem.img\"");
		}
		if (File.Exists(inputDirectory + "\\odm.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p odm \"" + inputDirectory + "\\odm.img\"");
		}
		if (File.Exists(inputDirectory + "\\product.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p product \"" + inputDirectory + "\\product.img\"");
		}
		if (File.Exists(inputDirectory + "\\vendor.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p vendor \"" + inputDirectory + "\\vendor.img\"");
		}
		if (File.Exists(inputDirectory + "\\system.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p system \"" + inputDirectory + "\\system.img\"");
		}
		if (File.Exists(inputDirectory + "\\system_ext_a.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p system_ext \"" + inputDirectory + "\\system_ext_a.img\"");
		}
		if (File.Exists(inputDirectory + "\\oem_a.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p oem \"" + inputDirectory + "\\oem_a.img\"");
		}
		if (File.Exists(inputDirectory + "\\odm_a.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p odm \"" + inputDirectory + "\\odm_a.img\"");
		}
		if (File.Exists(inputDirectory + "\\product_a.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p product \"" + inputDirectory + "\\product_a.img\"");
		}
		if (File.Exists(inputDirectory + "\\vendor_a.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p vendor \"" + inputDirectory + "\\vendor_a.img\"");
		}
		if (File.Exists(inputDirectory + "\\system_a.img"))
		{
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "img2sdat.py\" -o \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\" -v 4 -p system \"" + inputDirectory + "\\system_a.img\"");
		}
		LogInstance.Log(LogLineHead + "Creating updater-script..");
		Directory.CreateDirectory(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\META-INF\\com\\google\\android");
		string updater_script = "";
		updater_script += "set_bootloader_env(\"upgrade_step\", \"3\");\n";
		updater_script += "show_progress(0.650000, 0);\n";
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\system_ext.new.dat"))
		{
			updater_script += "ui_print(\"Patching system_ext image unconditionally...\");\n";
			updater_script += "block_image_update(\"/dev/block/system_ext\", package_extract_file(\"system_ext.transfer.list\"), \"system_ext.new.dat\", \"system_ext.patch.dat\") ||\n";
			updater_script += "abort(\"E1001: Failed to update system_ext image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\oem.new.dat"))
		{
			updater_script += "ui_print(\"Patching oem image unconditionally...\");\n";
			updater_script += "block_image_update(\"/dev/block/oem\", package_extract_file(\"oem.transfer.list\"), \"oem.new.dat\", \"oem.patch.dat\") ||\n";
			updater_script += "abort(\"E1001: Failed to update oem image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\odm.new.dat"))
		{
			updater_script += "ui_print(\"Patching odm image unconditionally...\");\n";
			updater_script += "block_image_update(\"/dev/block/odm\", package_extract_file(\"odm.transfer.list\"), \"odm.new.dat\", \"odm.patch.dat\") ||\n";
			updater_script += "abort(\"E1001: Failed to update system_ext image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\product.new.dat"))
		{
			updater_script += "ui_print(\"Patching product image unconditionally...\");\n";
			updater_script += "block_image_update(\"/dev/block/product\", package_extract_file(\"product.transfer.list\"), \"product.new.dat\", \"product.patch.dat\") ||\n";
			updater_script += "abort(\"E1001: Failed to update product image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\vendor.new.dat"))
		{
			updater_script += "ui_print(\"Patching vendor image unconditionally...\");\n";
			updater_script += "block_image_update(\"/dev/block/vendor\", package_extract_file(\"vendor.transfer.list\"), \"vendor.new.dat\", \"vendor.patch.dat\") ||\n";
			updater_script += "abort(\"E1001: Failed to update vendor image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\system.new.dat"))
		{
			updater_script += "ui_print(\"Patching system image unconditionally...\");\n";
			updater_script += "block_image_update(\"/dev/block/system\", package_extract_file(\"system.transfer.list\"), \"system.new.dat\", \"system.patch.dat\") ||\n";
			updater_script += "abort(\"E1001: Failed to update system image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\super.img"))
		{
			updater_script += "ui_print(\"Writing raw super.img...\");\n";
			updater_script += "package_extract_file(\"super.img\", \"/dev/block/super\") || \n";
			updater_script += "abort(\"E1001: Failed to write super image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\logo.img"))
		{
			updater_script += "ui_print(\"Writing raw logo.img...\");\n";
			updater_script += "package_extract_file(\"logo.img\", \"/dev/block/logo\") ||\n";
			updater_script += "abort(\"E1001: Failed to write logo image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\dtbo.img"))
		{
			updater_script += "ui_print(\"Writing raw dtbo.img...\");\n";
			updater_script += "package_extract_file(\"dtbo.img\", \"/dev/block/dtbo\") ||\n";
			updater_script += "abort(\"E1001: Failed to write dtbo image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\dtb.img"))
		{
			updater_script += "ui_print(\"Writing raw dtb.img...\");\n";
			updater_script += "backup_data_cache(dtb, /cache/recovery/);\n";
			updater_script += "delete_file(\"/cache/recovery/dtb.img\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\recovery.img"))
		{
			updater_script += "backup_data_cache(recovery, /cache/recovery/);\n";
			updater_script += "ui_print(\"Writing raw recovery.img...\");\n";
			updater_script += "package_extract_file(\"recovery.img\", \"/dev/block/recovery\") ||\n";
			updater_script += "abort(\"E1001: Failed to write recovery image.\");\n";
			updater_script += "delete_file(\"/cache/recovery/recovery.img\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\boot.img"))
		{
			updater_script += "ui_print(\"Writing raw boot.img...\");\n";
			updater_script += "package_extract_file(\"boot.img\", \"/dev/block/boot\") ||\n";
			updater_script += "abort(\"E1001: Failed to write kernel image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\dt.img"))
		{
			updater_script += "ui_print(\"Writing raw dt.img...\");\n";
			updater_script += "write_dtb_image(package_extract_file(\"dt.img\")) ||\n";
			updater_script += "abort(\"E1001: Failed to write dt image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\vbmeta.img"))
		{
			updater_script += "ui_print(\"Writing raw vbmeta.img...\");\n";
			updater_script += "package_extract_file(\"vbmeta.img\", \"/dev/block/vbmeta\") ||\n";
			updater_script += "abort(\"E1001: Failed to write vbmeta image.\");\n";
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\bootloader.img"))
		{
			updater_script += "ui_print(\"Writing bootloader.img...\");\n";
			updater_script += "write_bootloader_image(package_extract_file(\"bootloader.img\")) ||\n";
			updater_script += "abort(\"E1001: Failed to write bootloader image.\");\n";
		}
		updater_script += "if get_update_stage() == \"2\" then\n";
		updater_script += "format(\"ext4\", \"EMMC\", \"/dev/block/metadata\", \"0\", \"/metadata\");\n";
		updater_script += "format(\"ext4\", \"EMMC\", \"/dev/block/tee\", \"0\", \"/tee\");\n";
		updater_script += "wipe_cache();\n";
		updater_script += "set_update_stage(\"0\");\n";
		updater_script += "endif;\n";
		updater_script += "set_bootloader_env(\"upgrade_step\", \"1\");\n";
		updater_script += "set_bootloader_env(\"force_auto_update\", \"false\");\n";
		updater_script += "set_progress(1.000000);\n";
		StreamWriter streamWriter = new StreamWriter(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\META-INF\\com\\google\\android\\updater-script");
		streamWriter.Write(updater_script);
		streamWriter.Close();
		streamWriter.Dispose();
		LogInstance.Log(LogLineHead + "Adding files to archive..");
		File.Copy(bin + "resources\\presigned.zip", outputFile);
		debug = false;
		BackgroundShell(bin + "7za.exe", "a \"" + outputFile + "\" \"" + Path.GetDirectoryName(inputDirectory) + "\\zip_tmp\\*\" -mx" + compression);
		debug = true;
		if (new FileInfo(outputFile).Length > 52400)
		{
			LogInstance.Log(LogLineHead + "[SUCCESS] : Packed upgrade zip");
			DeleteDirectory(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp");
			return true;
		}
		LogInstance.Log(LogLineHead + "[ERROR] : Failed to pack zip..");
		DeleteDirectory(Path.GetDirectoryName(inputDirectory) + "\\zip_tmp");
		File.Delete(outputFile);
		return false;
	}

	public bool RepackVendorImage(string inputDirectory, string outputFile)
	{
		string Chipset = "";
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\image.cfg"))
		{
			Chipset = "Amlogic";
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1"))
		{
			string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1");
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].EndsWith(".fex"))
				{
					Chipset = "Allwinner";
				}
			}
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\parameter.txt"))
		{
			Chipset = "Rockchip";
		}
		LogInstance.Log(LogLineHead + "Packing " + Chipset + " upgrade image file..");
		switch (Chipset)
		{
		case "Amlogic":
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\image.cfg"))
			{
				if (File.Exists(outputFile))
				{
					File.Delete(outputFile);
				}
				if (debug)
				{
					LogInstance.Log(LogLineHead + "[EXEC] \"" + bin + "AmlImagePack.exe\" -r \"" + inputDirectory + "\\image.cfg\" \"" + inputDirectory + "\" \"" + outputFile + "\"");
				}
				ProcessStartInfo procStartInfo = new ProcessStartInfo(bin + "AmlImagePack.exe", "-r \"" + inputDirectory + "\\image.cfg\" \"" + inputDirectory + "\" \"" + outputFile + "\"");
				procStartInfo.UseShellExecute = false;
				procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				procStartInfo.CreateNoWindow = true;
				Process process = new Process();
				process.StartInfo = procStartInfo;
				process.Start();
				process.WaitForExit();
				process.Dispose();
				if (File.Exists(outputFile))
				{
					if (new FileInfo(outputFile).Length > 52400)
					{
						LogInstance.Log(LogLineHead + "[SUCCESS] : Packed AMLOGIC based upgrade image");
						return true;
					}
					LogInstance.Log(LogLineHead + "[ERROR] : AML Failed to pack image (Output file too small)..");
					if (File.Exists(outputFile))
					{
						File.Delete(outputFile);
					}
					return false;
				}
				LogInstance.Log(LogLineHead + "[ERROR] : AML Failed to pack image (Output file was not generated)..");
				if (File.Exists(outputFile))
				{
					File.Delete(outputFile);
				}
				return false;
			}
			LogInstance.Log(LogLineHead + "[ERROR] : Missing image.cfg..");
			if (File.Exists(outputFile))
			{
				File.Delete(outputFile);
			}
			return false;
		case "Rockchip":
		{
			StreamReader streamReader = new StreamReader(inputDirectory + "\\parameter.txt");
			string data = streamReader.ReadToEnd().Replace("\r\n", "\n");
			streamReader.Close();
			streamReader.Dispose();
			string Chip = "0";
			string[] files = data.ToUpper().Split(new string[1] { "RK" }, StringSplitOptions.None);
			foreach (string line in files)
			{
				if (!(Chip == "0"))
				{
					continue;
				}
				try
				{
					Convert.ToInt32(line.ToLower().Substring(0, 4));
					if (line.StartsWith("27"))
					{
						Chip = "RK" + line.ToLower().Substring(0, 4);
					}
					if (line.StartsWith("28"))
					{
						Chip = "RK" + line.ToLower().Substring(0, 4);
					}
					if (line.StartsWith("29"))
					{
						Chip = "RK" + line.ToLower().Substring(0, 4);
					}
					if (line.StartsWith("30"))
					{
						Chip = "RK" + line.ToLower().Substring(0, 4);
					}
					if (line.StartsWith("31"))
					{
						Chip = "RK" + line.ToLower().Substring(0, 4);
					}
					if (line.StartsWith("32"))
					{
						Chip = "RK" + line.ToLower().Substring(0, 4);
					}
					if (line.StartsWith("33"))
					{
						Chip = "RK" + line.ToLower().Substring(0, 4);
					}
					if (line.StartsWith("34"))
					{
						Chip = "RK" + line.ToLower().Substring(0, 4);
					}
					if (line.StartsWith("35"))
					{
						Chip = "RK" + line.ToLower().Substring(0, 4);
					}
					if (line.StartsWith("36"))
					{
						Chip = "RK" + line.ToLower().Substring(0, 4);
					}
				}
				catch
				{
				}
			}
			if (Chip == "0")
			{
				Chip = Chip.ToLower().Substring(0, 6);
			}
			Chip = Chip.ToUpper();
			if (!string.IsNullOrWhiteSpace(Chip) && Chip.StartsWith("RK"))
			{
				if (File.Exists(inputDirectory + "\\MiniLoaderAll.bin"))
				{
					DeleteDirectory(inputDirectory + "\\image");
					Directory.CreateDirectory(inputDirectory + "\\image");
					if (File.Exists(inputDirectory + "\\update.img"))
					{
						File.Delete(inputDirectory + "\\update.img");
					}
					files = Directory.GetFiles(inputDirectory ?? "");
					foreach (string file in files)
					{
						if (Path.GetFileName(file) != "package-file")
						{
							File.Copy(file, inputDirectory + "\\image\\" + Path.GetFileName(file));
						}
					}
					if (!File.Exists(inputDirectory + "\\image\\trust.img"))
					{
						BackgroundShell("fsutil", "file createnew \"" + inputDirectory + "\\image\\trust.img\" 102400");
					}
					redirectOutput = false;
					BackgroundShell(bin + "afptool.exe", "-pack \"" + inputDirectory + "\" \"" + inputDirectory + "\\update.img\"");
					BackgroundShell(bin + "rkImageMaker", " -" + Chip + " \"" + inputDirectory + "\\image\\MiniLoaderAll.bin\" \"" + inputDirectory + "\\update.img\" \"" + outputFile + "\" -os_type:androidos");
					redirectOutput = true;
					if (File.Exists(inputDirectory + "\\update.img"))
					{
						File.Delete(inputDirectory + "\\update.img");
					}
					DeleteDirectory(inputDirectory + "\\image");
					Directory.CreateDirectory(inputDirectory + "\\image");
					if (File.Exists(outputFile))
					{
						LogInstance.Log(LogLineHead + "[SUCCESS] : Packed ROCKCHIP based upgrade image");
						return true;
					}
					LogInstance.Log(LogLineHead + "[ERROR] : RK Failed to pack image (Output file was not generated)..");
					if (File.Exists(outputFile))
					{
						File.Delete(outputFile);
					}
					return false;
				}
				LogInstance.Log(LogLineHead + "[ERROR] : RK Failed to pack image (MiniLoaderAll.bin not found)..");
				if (File.Exists(outputFile))
				{
					File.Delete(outputFile);
				}
				return false;
			}
			LogInstance.Log(LogLineHead + "[ERROR] : RK Failed to pack image (RK Chip not recognised)..");
			if (File.Exists(outputFile))
			{
				File.Delete(outputFile);
			}
			return false;
		}
		case "Allwinner":
			if (File.Exists(inputDirectory + "\\image.cfg"))
			{
				BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "awfull.py\" repack \"" + inputDirectory + "\" \"" + Path.GetDirectoryName(inputDirectory) + "\\" + Path.GetFileName(outputFile) + "\"");
				if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\" + Path.GetFileName(outputFile)))
				{
					File.Move(Path.GetDirectoryName(inputDirectory) + "\\" + Path.GetFileName(outputFile), outputFile);
					LogInstance.Log(LogLineHead + "[SUCCESS] : Packed ALLWINNER based upgrade image");
					return true;
				}
				LogInstance.Log(LogLineHead + "[ERROR] : AW AW Failed to pack image (Output file was not generated)..");
				if (File.Exists(outputFile))
				{
					File.Delete(outputFile);
				}
				return false;
			}
			LogInstance.Log(LogLineHead + "[ERROR] : AW Failed to pack image (image.cfg not found)..");
			if (File.Exists(outputFile))
			{
				File.Delete(outputFile);
			}
			return false;
		default:
			LogInstance.Log(LogLineHead + "[ERROR] : Failed to pack image (Unknown package format)..");
			if (File.Exists(outputFile))
			{
				File.Delete(outputFile);
			}
			return false;
		}
	}

	public bool RepackLogo(string inputDirectory, string OutputImage)
	{
		LogInstance.Log(LogLineHead + "Packing logo..");
		if (Settings.Default.NewLogoProcessingTools)
		{
			LogInstance.Log(LogLineHead + "Using new logo processing tools (amlogic)");
			if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\.config\\" + Path.GetFileNameWithoutExtension(OutputImage) + "_config"))
			{
				BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "respack.py\" -r \"" + Path.GetDirectoryName(inputDirectory) + "\\" + Path.GetFileName(OutputImage) + "\" -i \"" + inputDirectory + "\" -c \"" + Path.GetDirectoryName(inputDirectory) + "\\.config\\" + Path.GetFileNameWithoutExtension(OutputImage) + "_config\"");
				if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\" + Path.GetFileName(OutputImage)))
				{
					if (File.Exists(OutputImage))
					{
						File.Delete(OutputImage);
					}
					File.Move(Path.GetDirectoryName(inputDirectory) + "\\" + Path.GetFileName(OutputImage), OutputImage);
					LogInstance.Log(LogLineHead + "[SUCCESS] : Packed logo");
					return true;
				}
				LogInstance.Log(LogLineHead + "[ERROR] : Failed to pack logo");
				return false;
			}
			LogInstance.Log(LogLineHead + "[ERROR] : Could not find logo config");
			return false;
		}
		if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\.config\\logo.types"))
		{
			if (Directory.GetFiles(inputDirectory).Count() > 8)
			{
				MessageBox.Show("The logo.partition contains more than 8 files.\nThe bootlogo may be called something other than bootup.bmp.\n\nIf you have changed the logo and it remains unchanged\nyou may need to change a different file in the logo folder", "Unordinary logo format");
			}
			StreamReader streamReader = new StreamReader(Path.GetDirectoryName(inputDirectory) + "\\.config\\logo.types");
			string data = streamReader.ReadToEnd();
			streamReader.Close();
			streamReader.Dispose();
			DeleteDirectory(Path.GetDirectoryName(inputDirectory) + "\\logo_tmp");
			Directory.CreateDirectory(Path.GetDirectoryName(inputDirectory) + "\\logo_tmp");
			string[] array = data.Split('\n');
			foreach (string file in array)
			{
				if (!string.IsNullOrWhiteSpace(file))
				{
					if (file.Split('=').Last() == "gz")
					{
						File.Copy(inputDirectory + "\\" + file.Split('=').First() + ".bmp", Path.GetDirectoryName(inputDirectory) + "\\logo_tmp\\" + file.Split('=').First());
						BackgroundShell(bin + "gzip.exe", "\"" + Path.GetDirectoryName(inputDirectory) + "\\logo_tmp\\" + file.Split('=').First());
						File.Move(Path.GetDirectoryName(inputDirectory) + "\\logo_tmp\\" + file.Split('=').First() + ".gz", Path.GetDirectoryName(inputDirectory) + "\\logo_tmp\\" + file.Split('=').First());
					}
					else
					{
						File.Copy(inputDirectory + "\\" + file.Split('=').First() + ".bmp", Path.GetDirectoryName(inputDirectory) + "\\logo_tmp\\" + file.Split('=').First());
					}
				}
			}
			BackgroundShell(bin + "legacy\\imgpack.exe", " -r \"" + Path.GetDirectoryName(inputDirectory) + "\\logo_tmp\" \"" + Path.GetDirectoryName(inputDirectory) + "\\" + Path.GetFileName(OutputImage) + "\"");
			if (File.Exists(Path.GetDirectoryName(inputDirectory) + "\\" + Path.GetFileName(OutputImage)))
			{
				if (File.Exists(OutputImage))
				{
					File.Delete(OutputImage);
				}
				File.Move(Path.GetDirectoryName(inputDirectory) + "\\" + Path.GetFileName(OutputImage), OutputImage);
				if (Directory.Exists(Path.GetDirectoryName(inputDirectory) + "\\logo_tmp"))
				{
					DeleteDirectory(Path.GetDirectoryName(inputDirectory) + "\\logo_tmp");
				}
				LogInstance.Log(LogLineHead + "[SUCCESS] : Packed logo");
				return true;
			}
			LogInstance.Log(LogLineHead + "[ERROR] : Failed to pack logo");
			if (Directory.Exists(Path.GetDirectoryName(inputDirectory) + "\\logo_tmp"))
			{
				DeleteDirectory(Path.GetDirectoryName(inputDirectory) + "\\logo_tmp");
			}
			return false;
		}
		if (File.Exists(inputDirectory + "\\second.cfg"))
		{
			CopyFilesRecursively(inputDirectory, OutputImage + ".dump");
			File.Move(OutputImage + ".dump\\second.cfg", OutputImage + ".cfg");
			BackgroundShell(bin + "imgRePackerRK.exe", "/2nd \"" + OutputImage + ".cfg\"");
			DeleteDirectory(OutputImage + ".dump");
			File.Delete(OutputImage + ".cfg");
			if (File.Exists(OutputImage) && File.Exists(OutputImage + ".bak"))
			{
				File.Delete(OutputImage + ".bak");
				LogInstance.Log(LogLineHead + "[SUCCESS] : Packed logo");
				return true;
			}
			LogInstance.Log(LogLineHead + "[ERROR] : Failed to pack logo");
			if (File.Exists(OutputImage + ".bak"))
			{
				if (File.Exists(OutputImage))
				{
					File.Delete(OutputImage);
				}
				File.Move(OutputImage + ".bak", OutputImage);
			}
			return false;
		}
		LogInstance.Log(LogLineHead + "[ERROR] : Failed to pack logo");
		return false;
	}

	public bool UnpackAPK(string inputFile, string outputFolder)
	{
		if (!inputFile.Contains("\\preinstall\\"))
		{
			LogInstance.Log(LogLineHead + "Unpacking " + Path.GetFileName(inputFile) + "..");
			if (!Directory.Exists(Path.GetDirectoryName(outputFolder)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(outputFolder));
				Thread newThread = new Thread((ThreadStart)delegate
				{
					BackgroundShell("cmd.exe", " /C \"java -jar \"" + bin + "apktools\\apktool.jar\" d \"" + inputFile + ".apk\" -o\"" + outputFolder + "\"\"");
				});
				newThread.Start();
				while (newThread.IsAlive)
				{
					Application.DoEvents();
				}
			}
			if (ShellOutput.ToLower().Contains("is not recognized"))
			{
				MessageBox.Show("Java not installed.\nPlease install JRE to use this feature.");
			}
			if (Directory.Exists(outputFolder))
			{
				return true;
			}
			return false;
		}
		LogInstance.Log(LogLineHead + "Cannot unpack non system app " + Path.GetFileName(inputFile) + "..");
		return true;
	}

	public bool RepackAPK(string InputFolder, string OutputFile)
	{
		LogInstance.Log(LogLineHead + "Repacking " + Path.GetFileName(OutputFile) + "..");
		if (!File.Exists(Path.GetDirectoryName(InputFolder) + "\\AndroidManifest.xml"))
		{
			Thread newThread = new Thread((ThreadStart)delegate
			{
				BackgroundShell("cmd.exe", " /C \"java -jar \"" + bin + "apktools\\apktool.jar\" b \"" + InputFolder + "\"\"");
			});
			newThread.Start();
			while (newThread.IsAlive)
			{
				Application.DoEvents();
			}
			if (Directory.Exists(InputFolder + "\\packTemp"))
			{
				DeleteDirectory(InputFolder + "\\packTemp");
			}
			Directory.CreateDirectory(InputFolder + "\\packTemp");
			if (Directory.Exists(InputFolder + "\\res"))
			{
				CopyFilesRecursively(InputFolder + "\\res", InputFolder + "\\packTemp\\res");
				foreach (string xml in Directory.EnumerateFiles(InputFolder + "\\packTemp\\res", "*.*", SearchOption.AllDirectories))
				{
					if (!xml.EndsWith("png") && !xml.EndsWith("jpg") && !xml.EndsWith("jpeg") && !xml.EndsWith("mp4") && !xml.EndsWith("mp3") && !xml.EndsWith("gif") && !xml.EndsWith("mpeg4"))
					{
						File.Delete(xml);
					}
					string[] directories = Directory.GetDirectories(InputFolder + "\\packTemp\\res");
					foreach (string dir in directories)
					{
						if (Directory.GetFiles(dir).Count() == 0)
						{
							DeleteDirectory(dir);
						}
					}
				}
			}
			if (File.Exists(InputFolder + "\\build\\apk\\AndroidManifest.xml"))
			{
				File.Copy(InputFolder + "\\build\\apk\\AndroidManifest.xml", InputFolder + "\\packTemp\\AndroidManifest.xml");
			}
			if (!Directory.Exists(InputFolder + "\\packTemp\\res") && !File.Exists(InputFolder + "\\build\\apk\\AndroidManifest.xml"))
			{
				LogInstance.Log(LogLineHead + "ERROR: Nothing to repack..");
			}
			else
			{
				if (Directory.Exists(InputFolder + "\\packTemp\\res"))
				{
					debug = false;
					BackgroundShell("cmd.exe", " /C \"cd \"" + InputFolder + "\\packTemp\" && \"" + bin + "7z.exe\" u -tzip \"" + OutputFile + ".apk\" \"res\"");
					debug = true;
				}
				if (File.Exists(InputFolder + "\\packTemp\\AndroidManifest.xml"))
				{
					debug = false;
					BackgroundShell("cmd.exe", " /C \"cd \"" + InputFolder + "\\packTemp\" && \"" + bin + "7z.exe\" u -tzip \"" + OutputFile + ".apk\" \"AndroidManifest.xml\"");
					debug = true;
				}
			}
			DeleteDirectory(InputFolder + "\\build");
			DeleteDirectory(InputFolder + "\\dist");
			DeleteDirectory(InputFolder + "\\packTemp");
			return true;
		}
		LogInstance.Log(LogLineHead + "ERROR: Cannot find AndroidManifest.xml..");
		return false;
	}

	public bool RepackDtb(string InputDirectory, string OutputImage)
	{
		LogInstance.Log(LogLineHead + "Packing DTB..");
		if (File.Exists(Path.GetDirectoryName(InputDirectory) + "\\.config\\dtb_unpack"))
		{
			if (!Directory.Exists(Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp"))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp");
			}
			if (Directory.GetFiles(InputDirectory).Count() > 1)
			{
				string[] files = Directory.GetFiles(InputDirectory);
				foreach (string file in files)
				{
					BackgroundShell(bin + "dtc.exe", " -I dts -O dtb -o \"" + Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp\\" + Path.GetFileNameWithoutExtension(file) + ".dtb\" \"" + file + "\"");
				}
				BackgroundShell(bin + "cd.bat", "\"" + bin + "dtbtool.exe\" -p ..\\bin\\dtc\\ level3\\dtb_tmp\\ -o level3\\dtb_tmp\\_aml_dtb.PARTITION");
			}
			else
			{
				string[] files = Directory.GetFiles(InputDirectory);
				foreach (string file2 in files)
				{
					BackgroundShell(bin + "dtc.exe", " -I dts -O dtb -o \"" + Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp\\_aml_dtb.PARTITION\" \"" + file2 + "\"");
				}
			}
			if (File.Exists(Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp\\_aml_dtb.PARTITION"))
			{
				StreamReader streamReader = new StreamReader(Path.GetDirectoryName(InputDirectory) + "\\.config\\dtb_unpack");
				string content = streamReader.ReadToEnd();
				streamReader.Close();
				streamReader.Dispose();
				if (content.StartsWith("gunzip"))
				{
					BackgroundShell(bin + "gzip.exe", "\"" + Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp\\_aml_dtb.PARTITION\"");
					File.Move(Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp\\_aml_dtb.PARTITION.gz", Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp\\_aml_dtb.PARTITION");
				}
			}
			if (File.Exists(Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp\\_aml_dtb.PARTITION"))
			{
				if (File.Exists(OutputImage))
				{
					File.Delete(OutputImage);
				}
				File.Move(Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp\\_aml_dtb.PARTITION", OutputImage);
				LogInstance.Log(LogLineHead + "[SUCCESS] Packed DTB");
				DeleteDirectory(Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp");
				return true;
			}
			DeleteDirectory(Path.GetDirectoryName(InputDirectory) + "\\dtb_tmp");
			return false;
		}
		return false;
	}

	public bool RepackBootVideo(string InputFile, string OutputFile)
	{
		LogInstance.Log(LogLineHead + "Updating bootvideo..");
		if (File.Exists(OutputFile))
		{
			File.Delete(OutputFile);
		}
		File.Copy(InputFile, OutputFile);
		if (File.Exists(OutputFile))
		{
			return true;
		}
		return false;
	}

	public bool RepackBootanimation(string InputDirectory, string OutputFile)
	{
		LogInstance.Log(LogLineHead + "Packing bootanimation");
		if (!Directory.Exists(Path.GetDirectoryName(InputDirectory) + "\\bootanimation_tmp"))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(InputDirectory) + "\\bootanimation_tmp");
		}
		File.Copy(bin + "resources\\bootanimation.zip", Path.GetDirectoryName(InputDirectory) + "\\bootanimation_tmp\\bootanimation.zip");
		debug = false;
		BackgroundShell(bin + "7za.exe", "a \"" + Path.GetDirectoryName(InputDirectory) + "\\bootanimation_tmp\\bootanimation.zip\" \"" + InputDirectory + "\\*\"");
		debug = true;
		if (ShellOutput.ToLower().Contains("everything is ok"))
		{
			if (File.Exists(OutputFile))
			{
				File.Delete(OutputFile);
			}
			LogInstance.Log(LogLineHead + "[SUCCESS] Packed bootanimation");
			File.Move(Path.GetDirectoryName(InputDirectory) + "\\bootanimation_tmp\\bootanimation.zip", OutputFile);
			DeleteDirectory(Path.GetDirectoryName(InputDirectory) + "\\bootanimation_tmp");
			return true;
		}
		LogInstance.Log(LogLineHead + "[ERROR] Failed to pack bootanimation");
		DeleteDirectory(Path.GetDirectoryName(InputDirectory) + "\\bootanimation_tmp");
		return false;
	}

	public bool RepackKernel(string InputFolder, string OutputFile)
	{
		LogInstance.Log(LogLineHead + "Packing " + Path.GetFileName(InputFolder) + "..");
		ShellWorkDir = AppDomain.CurrentDomain.BaseDirectory + "bin\\aik";
		if (File.Exists(ShellWorkDir + "\\boot.img"))
		{
			File.Delete(ShellWorkDir + "\\boot.img");
		}
		BackgroundShell(ShellWorkDir + "\\cleanup.bat", "");
		if (Directory.Exists(InputFolder + "\\split_img"))
		{
			CopyFilesRecursively(InputFolder + "\\split_img", ShellWorkDir + "\\split_img");
		}
		if (Directory.Exists(InputFolder + "\\ramdisk"))
		{
			CopyFilesRecursively(InputFolder + "\\ramdisk", ShellWorkDir + "\\ramdisk");
		}
		string[] files = Directory.GetFiles(ShellWorkDir + "\\split_img");
		foreach (string file in files)
		{
			File.Move(file, Path.GetDirectoryName(file) + "\\boot.img-" + Path.GetFileName(file));
		}
		BackgroundShell(ShellWorkDir + "\\repackimg.bat", "");
		if (File.Exists(ShellWorkDir + "\\boot.img"))
		{
			if (File.Exists(OutputFile))
			{
				File.Delete(OutputFile);
			}
			File.Copy(ShellWorkDir + "\\boot.img", OutputFile);
			BackgroundShell(ShellWorkDir + "\\cleanup.bat", "");
			LogInstance.Log(LogLineHead + "[SUCCESS] Packed " + Path.GetFileName(InputFolder));
			return true;
		}
		BackgroundShell(ShellWorkDir + "\\cleanup.bat", "");
		LogInstance.Log(LogLineHead + "[ERROR] Failed to pack " + Path.GetFileName(InputFolder));
		return false;
	}

	public bool RepackExt4(string InputDirectory, string OutputFile)
	{
		LogInstance.Log(LogLineHead + "Packing " + Path.GetFileName(InputDirectory) + "..");
		if (File.Exists(Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileNameWithoutExtension(InputDirectory) + "_fs_options"))
		{
			Path.GetFileName(InputDirectory);
			if (File.Exists(Path.GetDirectoryName(InputDirectory) + "\\super\\" + Path.GetFileName(InputDirectory) + "_a.img"))
			{
				_ = Path.GetFileName(InputDirectory) + "_a";
			}
			string executable = binarch + "mkfs.erofs.exe";
			string args = "";
			string[] array = new StreamReader(Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileNameWithoutExtension(InputDirectory) + "_fs_options").ReadToEnd().Split('\n');
			foreach (string line in array)
			{
				if (line.StartsWith("mkfs.erofs"))
				{
					args = line.Replace("mkfs.erofs.exe ", "");
				}
			}
			if (debug)
			{
				LogInstance.Log(LogLineHead + "[EXEC] \"" + executable + "\" " + args);
			}
			ProcessStartInfo procStartInfo = new ProcessStartInfo(executable, args);
			procStartInfo.UseShellExecute = false;
			procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			procStartInfo.CreateNoWindow = true;
			Process process = new Process();
			process.StartInfo = procStartInfo;
			process.Start();
			process.WaitForExit();
			process.Dispose();
			if (File.Exists(InputDirectory + ".img"))
			{
				if (File.Exists(OutputFile))
				{
					File.Delete(OutputFile);
				}
				File.Move(InputDirectory + ".img", OutputFile);
				return true;
			}
			return false;
		}
		if (File.Exists(Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_size"))
		{
			StreamReader reader = new StreamReader(Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_size");
			string size = reader.ReadToEnd().Split('\n').First();
			reader.Close();
			reader.Dispose();
			if (Settings.Default.ArchMake_ext4fs)
			{
				LogInstance.Log(LogLineHead + "Packing with " + MachineArchitecture.ToString() + " make_ext4fs");
				if (debug)
				{
					LogInstance.Log(LogLineHead + "[EXEC]\"" + binarch + "make_ext4fs.exe\" -s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + size + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
				}
				BackgroundShell(binarch + "make_ext4fs.exe", "-s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + size + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
			}
			else
			{
				if (debug)
				{
					LogInstance.Log(LogLineHead + "[EXEC]\"" + bin + "make_ext4fs.exe\" -s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + size + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
				}
				BackgroundShell(bin + "make_ext4fs.exe", "-s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + size + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
			}
			if (File.Exists(InputDirectory + ".img"))
			{
				if (new FileInfo(InputDirectory + ".img").Length > 2048)
				{
					if (File.Exists(OutputFile))
					{
						File.Delete(OutputFile);
					}
					File.Move(InputDirectory + ".img", OutputFile);
					LogInstance.Log(LogLineHead + "[SUCCESS] Packed " + Path.GetFileName(InputDirectory));
					return true;
				}
				LogInstance.Log(LogLineHead + "[ERROR] Failed to pack " + Path.GetFileName(InputDirectory));
				File.Delete(InputDirectory + ".img");
				return false;
			}
			if (!File.Exists(InputDirectory + ".img"))
			{
				if (Settings.Default.ArchMake_ext4fs)
				{
					LogInstance.Log(LogLineHead + "Packing with " + MachineArchitecture.ToString() + " make_ext4fs");
					if (debug)
					{
						LogInstance.Log(LogLineHead + "[EXEC]\"" + binarch + "make_ext4fs.exe\" -s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 50000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
					}
					BackgroundShell(binarch + "make_ext4fs.exe", "-s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 50000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
				}
				else
				{
					if (debug)
					{
						LogInstance.Log(LogLineHead + "[EXEC]\"" + bin + "make_ext4fs.exe\" -s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 50000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
					}
					BackgroundShell(bin + "make_ext4fs.exe", "-s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 50000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
				}
				if (!File.Exists(InputDirectory + ".img") && File.Exists(Path.GetDirectoryName(InputDirectory) + "\\.config\\system_file_contexts") && File.Exists(Path.GetDirectoryName(InputDirectory) + "\\.config\\system_fs_config"))
				{
					if (debug)
					{
						LogInstance.Log(LogLineHead + "[EXEC]\"" + bin + "make_ext4fs.exe\" -s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\system_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\system_fs_config\" -l " + (Convert.ToInt64(size) + 50000000) + " -a system \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
					}
					BackgroundShell(bin + "make_ext4fs.exe", "-s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\system_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\system_fs_config\" -l " + (Convert.ToInt64(size) + 50000000) + " -a system \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
				}
				if (File.Exists(InputDirectory + ".img"))
				{
					if (new FileInfo(InputDirectory + ".img").Length > 2048)
					{
						if (File.Exists(OutputFile))
						{
							File.Delete(OutputFile);
						}
						File.Move(InputDirectory + ".img", OutputFile);
						LogInstance.Log(LogLineHead + "[SUCCESS] Packed " + Path.GetFileName(InputDirectory));
						return true;
					}
					LogInstance.Log(LogLineHead + "[ERROR] Failed to pack " + Path.GetFileName(InputDirectory));
					File.Delete(InputDirectory + ".img");
					return false;
				}
				if (Settings.Default.ArchMake_ext4fs)
				{
					LogInstance.Log(LogLineHead + "Packing with " + MachineArchitecture.ToString() + " make_ext4fs");
					if (debug)
					{
						LogInstance.Log(LogLineHead + "[EXEC]\"" + binarch + "make_ext4fs.exe\" -s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 100000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
					}
					BackgroundShell(binarch + "make_ext4fs.exe", "-s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 100000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
				}
				else
				{
					if (debug)
					{
						LogInstance.Log(LogLineHead + "[EXEC]\"" + bin + "make_ext4fs.exe\" -s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 100000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
					}
					BackgroundShell(bin + "make_ext4fs.exe", "-s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 100000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
				}
				if (File.Exists(InputDirectory + ".img"))
				{
					if (new FileInfo(InputDirectory + ".img").Length > 2048)
					{
						if (File.Exists(OutputFile))
						{
							File.Delete(OutputFile);
						}
						File.Move(InputDirectory + ".img", OutputFile);
						LogInstance.Log(LogLineHead + "[SUCCESS] Packed " + Path.GetFileName(InputDirectory));
						return true;
					}
					LogInstance.Log(LogLineHead + "[ERROR] Failed to pack " + Path.GetFileName(InputDirectory));
					File.Delete(InputDirectory + ".img");
					return false;
				}
				if (Settings.Default.ArchMake_ext4fs)
				{
					LogInstance.Log(LogLineHead + "Packing with " + MachineArchitecture.ToString() + " make_ext4fs");
					if (debug)
					{
						LogInstance.Log(LogLineHead + "[EXEC]\"" + binarch + "make_ext4fs.exe\" -s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 150000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
					}
					BackgroundShell(binarch + "make_ext4fs.exe", "-s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 150000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
				}
				else
				{
					if (debug)
					{
						LogInstance.Log(LogLineHead + "[EXEC]\"" + bin + "make_ext4fs.exe\" -s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 150000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
					}
					BackgroundShell(bin + "make_ext4fs.exe", "-s -J -L system -T -1 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + (Convert.ToInt64(size) + 150000000) + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
				}
				if (File.Exists(InputDirectory + ".img"))
				{
					if (new FileInfo(InputDirectory + ".img").Length > 2048)
					{
						if (File.Exists(OutputFile))
						{
							File.Delete(OutputFile);
						}
						File.Move(InputDirectory + ".img", OutputFile);
						LogInstance.Log(LogLineHead + "[SUCCESS] Packed " + Path.GetFileName(InputDirectory));
						return true;
					}
					LogInstance.Log(LogLineHead + "[ERROR] Failed to pack " + Path.GetFileName(InputDirectory));
					File.Delete(InputDirectory + ".img");
					return false;
				}
				return false;
			}
			return false;
		}
		return false;
	}

	private static string GetNumbers(string input)
	{
		return new string(input.Where((char c) => char.IsDigit(c)).ToArray());
	}

	public bool RepackSuper(string InputDirectory, string OutputFile)
	{
		if (Settings.Default.PythonSuperUtils)
		{
			LogInstance.Log(LogLineHead + "Packing super python lputils");
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "lpmake.py\" \"" + InputDirectory + "\" \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\super_config\" -o \"" + InputDirectory + ".img\"");
		}
		else
		{
			long SuperSize = 0L;
			SuperSize = Convert.ToInt64(new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\.config\\super_size").ReadToEnd().Replace("\r\n", "\n").Replace("\n", ""));
			string executable = "lpmake";
			string startcommand = "--sparse --virtual-ab --metadata-size 65536 --super-name super --metadata-slots 2 --device super:" + SuperSize + " --group slot_a:" + SuperSize;
			string slotacommands = "";
			string midcommands = " --group slot_b:4096";
			string slotbcommands = "";
			string endcommands = " --output \"" + InputDirectory + ".img\"";
			string[] files = Directory.GetFiles(InputDirectory);
			foreach (string file in files)
			{
				if (!file.EndsWith("_b.img"))
				{
					string partitionName = Path.GetFileNameWithoutExtension(file);
					long partitionSize = 0L;
					BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "du.py\" -b \"" + file + "\"");
					partitionSize = Convert.ToInt64(ShellOutput.Split(' ').First().Replace("\r\n", "")
						.Replace("\n", ""));
					if (partitionSize > 0)
					{
						slotacommands = slotacommands + "  --partition " + partitionName + ":readonly:" + (partitionSize + 500000) + ":slot_a --image " + partitionName + "=\"" + file + "\"";
					}
				}
				else
				{
					string partitionName2 = Path.GetFileNameWithoutExtension(file);
					slotbcommands = slotbcommands + "  --partition " + partitionName2 + ":readonly:0:slot_b";
				}
			}
			BackgroundShell(binarch + executable, startcommand + slotacommands + midcommands + slotbcommands + endcommands);
		}
		if (File.Exists(InputDirectory + ".img"))
		{
			FileInfo fi = new FileInfo(InputDirectory + ".img");
			if (fi.Length >= 1048576)
			{
				if (File.Exists(OutputFile))
				{
					File.Delete(OutputFile);
				}
				fi.MoveTo(OutputFile);
				LogInstance.Log(LogLineHead + "[SUCCESS] Packed " + Path.GetFileName(InputDirectory));
				return true;
			}
			File.Delete(InputDirectory + ".img");
			LogInstance.Log(LogLineHead + "[ERROR] Failed to pack super partition - output too small");
			return false;
		}
		LogInstance.Log(LogLineHead + "[ERROR] Failed to pack super partition (lpmake)");
		return false;
	}

	public bool RepackExt4_super(string InputDirectory, string OutputFile)
	{
		LogInstance.Log(LogLineHead + "Packing " + Path.GetFileName(InputDirectory) + "..");
		if (File.Exists(Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileNameWithoutExtension(InputDirectory) + "_fs_options"))
		{
			Path.GetFileName(InputDirectory);
			if (File.Exists(Path.GetDirectoryName(InputDirectory) + "\\super\\" + Path.GetFileName(InputDirectory) + "_a.img"))
			{
				_ = Path.GetFileName(InputDirectory) + "_a";
			}
			string executable = binarch + "mkfs.erofs.exe";
			string args = "";
			string[] array = new StreamReader(Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileNameWithoutExtension(InputDirectory) + "_fs_options").ReadToEnd().Split('\n');
			foreach (string line in array)
			{
				if (line.StartsWith("mkfs.erofs"))
				{
					args = line.Replace("mkfs.erofs.exe ", "");
				}
			}
			if (debug)
			{
				LogInstance.Log(LogLineHead + "[EXEC] \"" + executable + "\" " + args);
			}
			ProcessStartInfo procStartInfo = new ProcessStartInfo(executable, args);
			procStartInfo.UseShellExecute = false;
			procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			procStartInfo.CreateNoWindow = true;
			Process process = new Process();
			process.StartInfo = procStartInfo;
			process.Start();
			process.WaitForExit();
			process.Dispose();
			if (File.Exists(InputDirectory + ".img"))
			{
				if (File.Exists(OutputFile))
				{
					File.Delete(OutputFile);
				}
				File.Move(InputDirectory + ".img", OutputFile);
				return true;
			}
			return false;
		}
		if (File.Exists(Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_size"))
		{
			StreamReader reader = new StreamReader(Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_size");
			reader.ReadToEnd().Split('\n').First();
			reader.Close();
			reader.Dispose();
			debug = true;
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "du.py\" -b \"" + OutputFile + "\"");
			string imgsize = ShellOutput.Split(' ').First().Replace("\r\n", "")
				.Replace("\n", "");
			if (Convert.ToInt32(imgsize) < 1048576)
			{
				imgsize = "1048576";
			}
			else if (Convert.ToInt32(imgsize) < 536870912)
			{
				imgsize = "536870912";
			}
			else if (Convert.ToInt32(imgsize) < 1073741824)
			{
				imgsize = "1073741824";
			}
			else if (Convert.ToInt32(imgsize) < 2147483648u)
			{
				imgsize = "2147483648";
			}
			ProcessStartInfo procStartInfo2 = new ProcessStartInfo("\"" + bin + "make_ext4fs.exe\"", "-J -T 1230764400 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\" + Path.GetFileName(InputDirectory) + "_fs_config\" -l " + imgsize + " -a " + Path.GetFileName(InputDirectory) + " \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
			if (debug)
			{
				LogInstance.Log(LogLineHead + "[EXEC]" + procStartInfo2.FileName + " " + procStartInfo2.Arguments);
			}
			procStartInfo2.WindowStyle = ProcessWindowStyle.Hidden;
			procStartInfo2.WorkingDirectory = ShellWorkDir;
			Process process2 = new Process();
			process2.StartInfo = procStartInfo2;
			process2.Start();
			process2.WaitForExit();
			process2.Dispose();
			if (!File.Exists(InputDirectory + ".img") && OutputFile.Contains("odm_ext") && File.Exists(Path.GetDirectoryName(InputDirectory) + "\\.config\\system_file_contexts") && File.Exists(Path.GetDirectoryName(InputDirectory) + "\\.config\\system_fs_config"))
			{
				ProcessStartInfo procStartInfo3 = new ProcessStartInfo("\"" + bin + "make_ext4fs.exe\"", "-J -T 1230764400 -S \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\system_file_contexts\" -C \"" + Path.GetDirectoryName(InputDirectory) + "\\.config\\system_fs_config\" -l " + imgsize + " -a system \"" + InputDirectory + ".img\" \"" + InputDirectory + "\"");
				if (debug)
				{
					LogInstance.Log(LogLineHead + "[EXEC]" + procStartInfo3.FileName + " " + procStartInfo3.Arguments);
				}
				procStartInfo3.WindowStyle = ProcessWindowStyle.Hidden;
				procStartInfo3.WorkingDirectory = ShellWorkDir;
				Process process3 = new Process();
				process3.StartInfo = procStartInfo3;
				process3.Start();
				process3.WaitForExit();
				process3.Dispose();
			}
			if (File.Exists(InputDirectory + ".img"))
			{
				if (new FileInfo(InputDirectory + ".img").Length > 2048)
				{
					ProcessStartInfo procStartInfo4 = new ProcessStartInfo("\"" + bin + "e2fs\\e2fsck.exe\"", "-yf \"" + InputDirectory + ".img\"");
					procStartInfo4.WindowStyle = ProcessWindowStyle.Hidden;
					procStartInfo4.WorkingDirectory = ShellWorkDir;
					Process proc3 = new Process();
					proc3.StartInfo = procStartInfo4;
					if (debug)
					{
						LogInstance.Log(LogLineHead + "[EXEC]" + procStartInfo4.FileName + " " + procStartInfo4.Arguments);
					}
					proc3.Start();
					proc3.WaitForExit();
					procStartInfo4 = new ProcessStartInfo("\"" + bin + "e2fs\\resize2fs.exe\"", "-M \"" + InputDirectory + ".img\"");
					procStartInfo4.WindowStyle = ProcessWindowStyle.Hidden;
					procStartInfo4.WorkingDirectory = ShellWorkDir;
					proc3.StartInfo = procStartInfo4;
					if (debug)
					{
						LogInstance.Log(LogLineHead + "[EXEC]" + procStartInfo4.FileName + " " + procStartInfo4.Arguments);
					}
					proc3.Start();
					proc3.WaitForExit();
					procStartInfo4 = new ProcessStartInfo("\"" + bin + "e2fs\\e2fsck.exe\"", "-yf \"" + InputDirectory + ".img\"");
					procStartInfo4.WindowStyle = ProcessWindowStyle.Hidden;
					procStartInfo4.WorkingDirectory = ShellWorkDir;
					proc3.StartInfo = procStartInfo4;
					if (debug)
					{
						LogInstance.Log(LogLineHead + "[EXEC]" + procStartInfo4.FileName + " " + procStartInfo4.Arguments);
					}
					proc3.Start();
					proc3.WaitForExit();
					proc3.Dispose();
					if (File.Exists(OutputFile))
					{
						File.Delete(OutputFile);
					}
					File.Move(InputDirectory + ".img", OutputFile);
					LogInstance.Log(LogLineHead + "[SUCCESS] Packed " + Path.GetFileName(InputDirectory));
					return true;
				}
				LogInstance.Log(LogLineHead + "[ERROR] Failed to pack " + Path.GetFileName(InputDirectory));
				File.Delete(InputDirectory + ".img");
				return false;
			}
			return false;
		}
		return false;
	}

	public bool RepackLauncher(string InputFolder, string OutputFile)
	{
		if (Directory.Exists(InputFolder) && File.Exists(OutputFile))
		{
			Thread newThread = new Thread((ThreadStart)delegate
			{
				debug = false;
				BackgroundShell("cmd.exe", " /C \"cd \"" + InputFolder + "\" && \"" + bin + "7z.exe\" u -tzip \"" + OutputFile + "\" \"res\"");
				debug = true;
			});
			newThread.Start();
			while (newThread.IsAlive)
			{
				Application.DoEvents();
			}
		}
		return true;
	}

	public bool UnpackUpgradeZip(string InputFile, string OutputFolder)
	{
		LogInstance.Log(LogLineHead + "Unpacking " + Path.GetFileName(InputFile) + "..");
		if (!Directory.Exists(OutputFolder))
		{
			Directory.CreateDirectory(OutputFolder);
		}
		debug = false;
		BackgroundShell(bin + "7z", " e \"" + InputFile + "\" -o\"" + OutputFolder + "\" -y -r update-binary");
		debug = true;
		if (File.Exists(OutputFolder + "\\update-binary"))
		{
			File.Delete(OutputFolder + "\\update-binary");
			Thread newThread = new Thread((ThreadStart)delegate
			{
				debug = false;
				BackgroundShell(bin + "7za", " x \"" + InputFile + "\" -o\"" + OutputFolder + "\"");
				debug = true;
			});
			newThread.Start();
			while (newThread.IsAlive)
			{
				Application.DoEvents();
			}
			string[] files = Directory.GetFiles(OutputFolder);
			foreach (string file in files)
			{
				if (Path.GetFileName(file).Contains("patch.dat"))
				{
					string truePartition = Path.GetFileName(file).Split('.').First();
					if (File.Exists(OutputFolder + "\\" + truePartition + ".new.dat.br"))
					{
						LogInstance.Log(LogLineHead + "Decompressing " + truePartition + " from brotli format..");
						if (File.Exists(OutputFolder + "\\" + truePartition + ".new.dat"))
						{
							File.Delete(OutputFolder + "\\" + truePartition + ".new.dat");
						}
						Process.Start(binarch + "brotli.exe", " --decompress --in \"" + OutputFolder + "\\" + truePartition + ".new.dat.br\" --out \"" + OutputFolder + "\\" + truePartition + ".new.dat\"").WaitForExit();
					}
					if (File.Exists(OutputFolder + "\\" + truePartition + ".new.dat") && File.Exists(OutputFolder + "\\" + truePartition + ".transfer.list"))
					{
						LogInstance.Log(LogLineHead + "Converting " + truePartition + " sparse data..");
						BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "sdat2img.py\" \"" + OutputFolder + "\\" + truePartition + ".transfer.list\" \"" + OutputFolder + "\\" + truePartition + ".new.dat\" \"" + OutputFolder + "\\" + truePartition + ".img\"");
						File.Delete(file);
						File.Delete(OutputFolder + "\\" + truePartition + ".new.dat");
						File.Delete(OutputFolder + "\\" + truePartition + ".transfer.list");
						File.Exists(OutputFolder + "\\" + truePartition + ".patch.dat");
					}
				}
				if (file.Contains("contexts"))
				{
					File.Delete(file);
				}
				if (file.Contains("dtb"))
				{
					File.Move(file, OutputFolder + "\\_aml_dtb.img");
				}
			}
			if (Directory.Exists(OutputFolder + "\\META-INF"))
			{
				Directory.Delete(OutputFolder + "\\META-INF", recursive: true);
			}
			files = Directory.GetFiles(OutputFolder);
			foreach (string file2 in files)
			{
				if (Path.GetExtension(file2) == ".img")
				{
					File.Move(file2, OutputFolder + "\\" + Path.GetFileNameWithoutExtension(file2) + ".PARTITION");
				}
			}
			GenerateImageConfig(OutputFolder);
			if (File.Exists(OutputFolder + "\\image.cfg"))
			{
				LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked " + Path.GetFileName(InputFile));
				return true;
			}
			LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack " + Path.GetFileName(InputFile));
			return false;
		}
		LogInstance.Log(LogLineHead + "[ERROR] Not an upgrade zip " + Path.GetFileName(InputFile));
		return false;
	}

	public void GenerateImageConfig(string InputDirectory)
	{
		LogInstance.Log(LogLineHead + "Generating image.cfg..");
		string NormalList = "[LIST_NORMAL]\n";
		string VerifyList = "[LIST_VERIFY]\n";
		string[] files = Directory.GetFiles(InputDirectory);
		foreach (string file in files)
		{
			string FileName = Path.GetFileName(file).Split('.').First();
			string FileExtension = Path.GetFileName(file).Split('.').Last();
			if (FileName + "." + FileExtension == "_aml_dtb.PARTITION")
			{
				NormalList = NormalList + "file=\"" + FileName + "." + FileExtension + "\"\t\tmain_type=\"dtb\"\t\tsub_type=\"meson1\"\n";
			}
			if (FileExtension == "PARTITION")
			{
				VerifyList = VerifyList + "file=\"" + FileName + "." + FileExtension + "\"\t\tmain_type=\"" + FileExtension + "\"\t\tsub_type=\"" + FileName + "\"\n";
			}
			else if (FileExtension != "VERIFY" && !(FileName + "." + FileExtension).Contains("meson1"))
			{
				NormalList = NormalList + "file=\"" + FileName + "." + FileExtension + "\"\t\tmain_type=\"" + FileExtension + "\"\t\tsub_type=\"" + FileName + "\"";
			}
		}
		StreamWriter streamWriter = new StreamWriter(InputDirectory + "\\image.cfg");
		streamWriter.Write(NormalList + "\n\n" + VerifyList);
		streamWriter.Close();
		streamWriter.Dispose();
	}

	public bool UnpackVendorImage(string inputFile, string outputDirectory)
	{
		LogInstance.Log(LogLineHead + "Unpacking " + Path.GetFileName(inputFile) + "..");
		if (!Directory.Exists(outputDirectory))
		{
			Directory.CreateDirectory(outputDirectory);
		}
		BackgroundShell(bin + "AmlImagePack", "-d \"" + inputFile + "\" \"" + outputDirectory + "\"");
		if (File.Exists(outputDirectory + "\\image.cfg"))
		{
			LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked AMLOGIC base " + Path.GetFileName(inputFile));
			return true;
		}
		BackgroundShell(bin + "rkImageMaker", "-unpack \"" + inputFile + "\" \"" + outputDirectory + "\"");
		if (File.Exists(outputDirectory + "\\firmware.img"))
		{
			BackgroundShell(bin + "afptool", "-unpack \"" + outputDirectory + "\\firmware.img\" \"" + outputDirectory + "\"");
			if (File.Exists(outputDirectory + "\\package-file"))
			{
				if (File.Exists(outputDirectory + "\\image\\parameter.txt"))
				{
					if (File.Exists(outputDirectory + "\\boot.bin"))
					{
						File.Delete(outputDirectory + "\\boot.bin");
					}
					if (File.Exists(outputDirectory + "\\firmware.img"))
					{
						File.Delete(outputDirectory + "\\firmware.img");
					}
					string[] files = Directory.GetFiles(outputDirectory + "\\image");
					foreach (string file in files)
					{
						File.Move(file, outputDirectory + "\\" + Path.GetFileName(file));
					}
					LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked ROCKCHIP base " + Path.GetFileName(inputFile));
					return true;
				}
				if (File.Exists(outputDirectory + "\\parameter.txt"))
				{
					if (File.Exists(outputDirectory + "\\boot.bin"))
					{
						File.Delete(outputDirectory + "\\boot.bin");
					}
					if (File.Exists(outputDirectory + "\\firmware.img"))
					{
						File.Delete(outputDirectory + "\\firmware.img");
					}
					LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked ROCKCHIP base " + Path.GetFileName(inputFile));
					return true;
				}
				LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack " + Path.GetFileName(inputFile));
				DeleteDirectory(outputDirectory);
				return false;
			}
			LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack " + Path.GetFileName(inputFile));
			DeleteDirectory(outputDirectory);
			return false;
		}
		BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "awfull.py\" extract \"" + inputFile + "\" \"" + outputDirectory + "\"");
		if (File.Exists(outputDirectory + "\\image.cfg"))
		{
			LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked ALLWINNER base " + Path.GetFileName(inputFile));
			return true;
		}
		LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack " + Path.GetFileName(inputFile));
		DeleteDirectory(outputDirectory);
		return false;
	}

	public bool UnpackBootVideo(string InputFile, string outputFolder)
	{
		LogInstance.Log(LogLineHead + "Unpacking bootvideo..");
		if (Directory.Exists(outputFolder))
		{
			DeleteDirectory(outputFolder);
		}
		if (!Directory.Exists(outputFolder))
		{
			Directory.CreateDirectory(outputFolder);
		}
		File.Copy(InputFile, outputFolder + "\\" + Path.GetFileName(InputFile) + ".mp4");
		if (File.Exists(outputFolder + "\\" + Path.GetFileNameWithoutExtension(InputFile) + ".mp4"))
		{
			return true;
		}
		return false;
	}

	public bool UnpackBootanimation(string InputFile, string outputFolder)
	{
		LogInstance.Log(LogLineHead + "Unpacking bootanimation..");
		debug = false;
		BackgroundShell(bin + "7z", " e \"" + InputFile + "\" -o\"" + Path.GetDirectoryName(outputFolder) + "\" -y -r desc.txt");
		debug = true;
		if (File.Exists(Path.GetDirectoryName(outputFolder) + "\\desc.txt"))
		{
			File.Delete(Path.GetDirectoryName(outputFolder) + "\\desc.txt");
			if (Directory.Exists(outputFolder))
			{
				DeleteDirectory(outputFolder);
			}
			if (!Directory.Exists(outputFolder))
			{
				Directory.CreateDirectory(outputFolder);
			}
			debug = false;
			BackgroundShell(bin + "7za", " x \"" + InputFile + "\" -o\"" + outputFolder + "\" -y");
			debug = true;
			if (File.Exists(outputFolder + "\\desc.txt"))
			{
				return true;
			}
			return false;
		}
		LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack bootanimation - no desc.txt");
		return false;
	}

	public bool UnpackLauncher(string InputFolder)
	{
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\tmp"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\tmp");
		}
		Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\tmp");
		foreach (string file in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2", "*.apk", SearchOption.AllDirectories))
		{
			debug = false;
			BackgroundShell(bin + "7z", " e \"" + file + "\" -o\"" + AppDomain.CurrentDomain.BaseDirectory + "tmp\\tmp\" -y AndroidManifest.xml");
			debug = true;
			if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\tmp\\AndroidManifest.xml"))
			{
				continue;
			}
			StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "tmp\\tmp\\AndroidManifest.xml");
			if (new string((from x in reader.ReadToEnd()
				where ' ' <= x && x <= '~'
				select x).ToArray()).Contains("android.intent.category.HOME") && !new string((from x in reader.ReadToEnd()
				where ' ' <= x && x <= '~'
				select x).ToArray()).Contains("android.intent.category.HOME_MAIN") && !file.ToLower().Contains("settings") && !file.ToLower().Contains("setting") && !file.ToLower().Contains("framework") && !file.ToLower().Contains("-res") && !file.ToLower().Contains("gmscore") && !file.ToLower().Contains("velvet") && !file.ToLower().Contains("provision") && !file.ToLower().Contains("shutdown"))
			{
				string appfile = file;
				string inputFile = "";
				string outputFolder = "";
				if (File.Exists(appfile))
				{
					inputFile = appfile;
					outputFolder = AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\apps\\" + appfile.Replace(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2", "");
				}
				else if (Directory.Exists(appfile))
				{
					inputFile = appfile + "\\" + Path.GetFileNameWithoutExtension(appfile);
					outputFolder = AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\apps\\" + appfile.Replace(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2", "") + "\\" + Path.GetFileNameWithoutExtension(appfile).Replace(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2", "");
				}
				Thread packThread = new Thread((ThreadStart)delegate
				{
					if (Directory.Exists(bin + "apktools"))
					{
						if (UnpackAPK(Path.GetDirectoryName(inputFile) + "\\" + Path.GetFileNameWithoutExtension(inputFile), Path.GetDirectoryName(outputFolder) + "\\" + Path.GetFileNameWithoutExtension(outputFolder)))
						{
							LogInstance.Log(LogLineHead + "Successfully unpacked " + Path.GetFileNameWithoutExtension(appfile));
						}
						else
						{
							LogInstance.Log(LogLineHead + "Failed to unpack " + Path.GetFileNameWithoutExtension(appfile));
							MessageBox.Show("Failed to unpack launcher: " + Path.GetFileName(appfile), "Cannot unpack launcher");
							if (Directory.Exists(outputFolder))
							{
								DeleteDirectory(outputFolder);
							}
						}
					}
				});
				packThread.Start();
				while (packThread.IsAlive)
				{
					Application.DoEvents();
				}
			}
			reader.Close();
			reader.Dispose();
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\tmp"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\tmp");
		}
		return true;
	}

	public bool InstallExternal(string resource)
	{
		if (File.Exists(resource))
		{
			Thread newThread = new Thread((ThreadStart)delegate
			{
				debug = false;
				BackgroundShell(bin + "7za", " x -y \"" + resource + "\" -o\"" + bin + "\"");
				debug = true;
			});
			newThread.Start();
			while (newThread.IsAlive)
			{
				Application.DoEvents();
			}
			File.Delete(resource);
			return true;
		}
		return false;
	}

	public bool UnpackSuper(string inputImg, string outputFolder)
	{
		LogInstance.Log(LogLineHead + "Unpacking " + Path.GetFileName(inputImg) + "..");
		if (Settings.Default.PythonSuperUtils)
		{
			LogInstance.Log(LogLineHead + "Unpacking super python lputils");
			string rawInputImg = inputImg;
			string imgsize = "0";
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "imgcheck.py\" \"" + inputImg + "\"");
			if (ShellOutput.Contains("sparse"))
			{
				if (Settings.Default.PythonSimg2Img)
				{
					LogInstance.Log(LogLineHead + "Using python simg2img");
					BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "simg2img.py\" \"" + inputImg + "\" \"" + inputImg.Replace("." + inputImg.Split('.').Last(), ".raw.img") + "\"");
				}
				else
				{
					BackgroundShell(binarch + "simg2img.exe", "\"" + inputImg + "\" \"" + inputImg.Replace("." + inputImg.Split('.').Last(), ".raw.img") + "\"");
				}
				rawInputImg = inputImg.Replace("." + inputImg.Split('.').Last(), ".raw.img");
			}
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "du.py\" -b \"" + rawInputImg + "\"");
			imgsize = ShellOutput.Split(' ').First();
			if (!Directory.Exists(outputFolder + "\\super"))
			{
				Directory.CreateDirectory(outputFolder + "\\super");
			}
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "lpunpack.py\" \"" + rawInputImg + "\" \"" + outputFolder + ".raw\"");
			if (File.Exists(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size"))
			{
				File.Delete(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size");
			}
			StreamWriter streamWriter = new StreamWriter(Path.GetDirectoryName(outputFolder) + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size");
			streamWriter.WriteLine(imgsize);
			streamWriter.Close();
			streamWriter.Dispose();
			return true;
		}
		string rawInputImg2 = inputImg;
		string imgsize2 = "0";
		BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "imgcheck.py\" \"" + inputImg + "\"");
		if (ShellOutput.Contains("sparse"))
		{
			if (Settings.Default.PythonSimg2Img)
			{
				LogInstance.Log(LogLineHead + "Using python simg2img");
				BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "simg2img.py\" \"" + inputImg + "\" \"" + inputImg.Replace("." + inputImg.Split('.').Last(), ".raw.img") + "\"");
			}
			else
			{
				BackgroundShell(binarch + "simg2img.exe", "\"" + inputImg + "\" \"" + inputImg.Replace("." + inputImg.Split('.').Last(), ".raw.img") + "\"");
			}
			rawInputImg2 = inputImg.Replace("." + inputImg.Split('.').Last(), ".raw.img");
		}
		BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "du.py\" -b \"" + rawInputImg2 + "\"");
		imgsize2 = ShellOutput.Split(' ').First();
		StreamWriter streamWriter2 = new StreamWriter(Path.GetDirectoryName(outputFolder) + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size");
		streamWriter2.WriteLine(imgsize2);
		streamWriter2.Close();
		streamWriter2.Dispose();
		if (!Directory.Exists(outputFolder))
		{
			Directory.CreateDirectory(outputFolder);
		}
		try
		{
			Thread newThread = new Thread((ThreadStart)delegate
			{
				BackgroundShell(binarch + "lpunpack.exe", " \"" + rawInputImg2 + "\" \"" + outputFolder + "\"");
			});
			newThread.Start();
			while (newThread.IsAlive)
			{
				Application.DoEvents();
			}
			newThread.Abort();
		}
		catch
		{
		}
		if (Directory.GetFiles(outputFolder).Count() > 0)
		{
			LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked " + Path.GetFileName(inputImg));
			return true;
		}
		if (Directory.Exists(outputFolder))
		{
			DeleteDirectory(outputFolder);
		}
		return false;
	}

	public bool UnpackExt4(string inputImg, string outputFolder)
	{
		LogInstance.Log(LogLineHead + "Unpacking " + Path.GetFileName(inputImg) + "..");
		BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "imgcheck.py\" \"" + inputImg + "\"");
		if (ShellOutput.Contains("erofs"))
		{
			if (debug)
			{
				LogInstance.Log(LogLineHead + "[EXEC] \"" + binarch + "extract.erofs\"  -i \"" + inputImg + "\" -o \"" + outputFolder + "\" -x");
			}
			ProcessStartInfo procStartInfo = new ProcessStartInfo(binarch + "extract.erofs", " -i \"" + inputImg + "\" -o \"" + outputFolder + "\" -x");
			procStartInfo.UseShellExecute = false;
			procStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			procStartInfo.CreateNoWindow = true;
			Process process = new Process();
			process.StartInfo = procStartInfo;
			process.Start();
			process.WaitForExit();
			process.Dispose();
			bool TrimOutput = false;
			if (Path.GetFileName(inputImg).EndsWith("_a.img"))
			{
				TrimOutput = true;
			}
			if (Path.GetFileName(inputImg).EndsWith("_b.img"))
			{
				TrimOutput = true;
			}
			if (TrimOutput)
			{
				string pretrimmed = Path.GetFileName(inputImg).Substring(0, Path.GetFileName(inputImg).Length - 4);
				string partition = Path.GetFileName(inputImg).Substring(0, Path.GetFileName(inputImg).Length - 6);
				Directory.Move(outputFolder + "\\" + pretrimmed, outputFolder + "\\" + partition);
				StreamReader streamReader = new StreamReader(outputFolder + "\\config\\" + pretrimmed + "_file_contexts");
				string fc = streamReader.ReadToEnd();
				streamReader.Close();
				streamReader.Dispose();
				StreamReader streamReader2 = new StreamReader(outputFolder + "\\config\\" + pretrimmed + "_fs_config");
				string fsc = streamReader2.ReadToEnd();
				streamReader2.Close();
				streamReader2.Dispose();
				StreamReader fsoreader = new StreamReader(outputFolder + "\\config\\" + pretrimmed + "_fs_options");
				string fso = fsoreader.ReadToEnd().Replace(pretrimmed + "/", partition + "/");
				fso = fso.Replace("/", "\\");
				fso = fso.Replace("--mount-point=\\", "--mount-point=/");
				fso = fso.Replace("\\config\\" + pretrimmed, "\\.config\\" + partition);
				fso = fso.Replace("\\" + pretrimmed, "\\" + partition);
				fso = fso.Replace(pretrimmed + "_repack.img", partition + ".img");
				fso = fso.Replace("--fs-config-file=", "--fs-config-file=\"");
				fso = fso.Replace("--file-contexts=", "--file-contexts=\"");
				fso = fso.Replace("_fs_config", "_fs_config\"");
				fso = fso.Replace("_file_contexts", "_file_contexts\"");
				fso = fso.Replace(partition + ".img ", "\"" + outputFolder + "\\" + partition + ".img\" \"");
				fso += "\"";
				for (int count = 0; count < 20; count++)
				{
					fso = fso.Replace(": ", ":");
				}
				fso = fso.Replace("mkfs.erofs options:", "mkfs.erofs.exe ");
				fsoreader.Close();
				fsoreader.Dispose();
				StreamWriter streamWriter = new StreamWriter(outputFolder + "\\.config\\" + partition + "_file_contexts");
				streamWriter.Write(fc);
				streamWriter.Close();
				streamWriter.Dispose();
				StreamWriter streamWriter2 = new StreamWriter(outputFolder + "\\.config\\" + partition + "_fs_config");
				streamWriter2.Write(fsc);
				streamWriter2.Close();
				streamWriter2.Dispose();
				StreamWriter streamWriter3 = new StreamWriter(outputFolder + "\\.config\\" + partition + "_fs_options");
				streamWriter3.Write(fso);
				streamWriter3.Close();
				streamWriter3.Dispose();
				DeleteDirectory(outputFolder + "\\config");
			}
			return true;
		}
		if (Settings.Default.PythonImgextractor)
		{
			LogInstance.Log(LogLineHead + "Unpacking ext4/sparse img with python imgextractor");
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "imgextractor.py\" \"" + inputImg + "\" \"" + outputFolder + "\"");
			if (ShellOutput.Contains("Success!"))
			{
				if (File.Exists(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_file_contexts") && File.Exists(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_fs_config") && File.Exists(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size") && Directory.Exists(outputFolder + "\\" + Path.GetFileNameWithoutExtension(inputImg).Replace("_a", "").Replace("_b", "")))
				{
					File.Delete(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size");
					BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "du.py\" -b \"" + inputImg + "\"");
					string imgsize = ShellOutput.Split(' ').First();
					StreamWriter streamWriter4 = new StreamWriter(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size");
					streamWriter4.WriteLine(imgsize);
					streamWriter4.Close();
					streamWriter4.Dispose();
					LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked " + Path.GetFileName(inputImg));
					return true;
				}
				return true;
			}
			if (File.Exists(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_file_contexts"))
			{
				File.Delete(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_file_contexts");
			}
			if (File.Exists(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_fs_config"))
			{
				File.Delete(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_fs_config");
			}
			if (File.Exists(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size"))
			{
				File.Delete(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size");
			}
			if (Directory.Exists(outputFolder + "\\" + Path.GetFileNameWithoutExtension(inputImg).Replace("_a", "").Replace("_b", "")))
			{
				DeleteDirectory(outputFolder + "\\" + Path.GetFileNameWithoutExtension(inputImg).Replace("_a", "").Replace("_b", ""));
			}
			LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack " + Path.GetFileName(inputImg));
			return false;
		}
		if (!Directory.Exists(outputFolder))
		{
			Directory.CreateDirectory(outputFolder);
		}
		BackgroundShell(bin + "legacy\\imgextractor.exe", "\"" + inputImg + "\" \"" + outputFolder + "\"");
		if (Directory.GetFiles(outputFolder).Count() + Directory.GetDirectories(outputFolder).Count() == 0)
		{
			if (File.Exists(Path.GetDirectoryName(inputImg) + "\\" + Path.GetFileNameWithoutExtension(inputImg) + ".raw.img"))
			{
				File.Delete(Path.GetDirectoryName(inputImg) + "\\" + Path.GetFileNameWithoutExtension(inputImg) + ".raw.img");
			}
			if (File.Exists(Path.GetDirectoryName(outputFolder) + "\\" + Path.GetFileNameWithoutExtension(outputFolder) + "_file_contexts"))
			{
				File.Delete(Path.GetDirectoryName(outputFolder) + "\\" + Path.GetFileNameWithoutExtension(outputFolder) + "_file_contexts");
			}
			if (File.Exists(Path.GetDirectoryName(outputFolder) + "\\" + Path.GetFileNameWithoutExtension(outputFolder) + "_fs_config"))
			{
				File.Delete(Path.GetDirectoryName(outputFolder) + "\\" + Path.GetFileNameWithoutExtension(outputFolder) + "_fs_config");
			}
			if (File.Exists(Path.GetDirectoryName(outputFolder) + "\\" + Path.GetFileNameWithoutExtension(outputFolder) + "_size"))
			{
				File.Delete(Path.GetDirectoryName(outputFolder) + "\\" + Path.GetFileNameWithoutExtension(outputFolder) + "_size");
			}
			DeleteDirectory(outputFolder);
			LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked " + Path.GetFileName(inputImg) + " however its empty");
			return true;
		}
		if (File.Exists(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_file_contexts") && File.Exists(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_fs_config") && File.Exists(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size") && Directory.Exists(outputFolder + "\\" + Path.GetFileNameWithoutExtension(inputImg).Replace("_a", "").Replace("_b", "")))
		{
			File.Delete(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size");
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "du.py\" -b \"" + inputImg + "\"");
			string imgsize2 = ShellOutput.Split(' ').First();
			StreamWriter streamWriter5 = new StreamWriter(outputFolder + "\\.config\\" + Path.GetFileNameWithoutExtension(inputImg) + "_size");
			streamWriter5.WriteLine(imgsize2);
			streamWriter5.Close();
			streamWriter5.Dispose();
			LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked " + Path.GetFileName(inputImg));
			return true;
		}
		LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack " + Path.GetFileName(inputImg));
		return false;
	}

	public bool UnpackKernel(string InputFile, string OutputFolder)
	{
		LogInstance.Log(LogLineHead + "Unpacking " + Path.GetFileName(InputFile) + "..");
		ShellWorkDir = AppDomain.CurrentDomain.BaseDirectory + "bin\\aik";
		BackgroundShell(ShellWorkDir + "\\cleanup.bat", "");
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "bin\\aik\\boot.img"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "bin\\aik\\boot.img");
		}
		File.Copy(InputFile, AppDomain.CurrentDomain.BaseDirectory + "bin\\aik\\boot.img");
		BackgroundShell(ShellWorkDir + "\\unpackimg.bat", "");
		if (Directory.Exists(ShellWorkDir + "\\split_img"))
		{
			string[] files = Directory.GetFiles(ShellWorkDir + "\\split_img");
			foreach (string file in files)
			{
				File.Move(file, Path.GetDirectoryName(file) + "\\" + Path.GetFileName(file).Substring(9));
			}
		}
		if (File.Exists(ShellWorkDir + "\\boot.img"))
		{
			File.Delete(ShellWorkDir + "\\boot.img");
		}
		if (File.Exists(ShellWorkDir + "\\split_img\\kernel") && File.Exists(ShellWorkDir + "\\split_img\\ramdisk_offset"))
		{
			if (!Directory.Exists(OutputFolder))
			{
				Directory.CreateDirectory(OutputFolder);
			}
			if (Directory.Exists(ShellWorkDir + "\\split_img"))
			{
				Directory.Move(ShellWorkDir + "\\split_img", OutputFolder + "\\split_img");
			}
			if (Directory.Exists(ShellWorkDir + "\\ramdisk"))
			{
				Directory.Move(ShellWorkDir + "\\ramdisk", OutputFolder + "\\ramdisk");
			}
			if (File.Exists(ShellWorkDir + "\\boot.img"))
			{
				File.Delete(ShellWorkDir + "\\boot.img");
			}
			BackgroundShell(ShellWorkDir + "\\cleanup.bat", "");
			LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked " + Path.GetFileName(InputFile));
			return true;
		}
		if (File.Exists(ShellWorkDir + "\\boot.img"))
		{
			File.Delete(ShellWorkDir + "\\boot.img");
		}
		BackgroundShell(ShellWorkDir + "\\cleanup.bat", "");
		LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack " + Path.GetFileName(InputFile));
		return false;
	}

	public bool UnpackLogo(string InputFile, string OutputFolder)
	{
		LogInstance.Log(LogLineHead + "Unpacking logo..");
		if (InputFile.EndsWith(".PARTITION"))
		{
			if (!Directory.Exists(OutputFolder))
			{
				Directory.CreateDirectory(OutputFolder);
			}
			if (Settings.Default.NewLogoProcessingTools)
			{
				LogInstance.Log(LogLineHead + "Using new logo processing tools (amlogic)");
				BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "respack.py\" -d \"" + InputFile + "\" -o \"" + OutputFolder + "\" -C \"" + Path.GetDirectoryName(OutputFolder) + "\\.config\"");
				if (File.Exists(OutputFolder + "\\bootup.bmp"))
				{
					if (File.Exists(Path.GetDirectoryName(OutputFolder) + "\\.config\\" + Path.GetFileNameWithoutExtension(InputFile) + "_config"))
					{
						LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked logo");
						return true;
					}
					LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack logo");
					return false;
				}
				LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack logo");
				return false;
			}
			BackgroundShell(bin + "legacy\\imgpack.exe", " -d \"" + InputFile + "\" \"" + OutputFolder + "\"");
			if (File.Exists(OutputFolder + "\\logo.PARTITION"))
			{
				File.Delete(OutputFolder + "\\logo.PARTITION");
			}
			if (File.Exists(OutputFolder + "\\bootup"))
			{
				string logocfg = "";
				string[] files = Directory.GetFiles(OutputFolder);
				foreach (string file in files)
				{
					try
					{
						Image.FromFile(file).Dispose();
						logocfg = logocfg + Path.GetFileName(file) + "=bmp\n";
					}
					catch
					{
						File.Move(file, file + ".gz");
						BackgroundShell(bin + "gzip.exe", "-d \"" + file + ".gz");
						logocfg = logocfg + Path.GetFileName(file) + "=gz\n";
					}
					File.Move(file, file + ".bmp");
				}
				StreamWriter streamWriter = new StreamWriter(Path.GetDirectoryName(OutputFolder) + "\\.config\\" + Path.GetFileNameWithoutExtension(InputFile) + ".types");
				streamWriter.Write(logocfg);
				streamWriter.Close();
				streamWriter.Dispose();
				LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked logo");
				return true;
			}
			LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack logo");
			return false;
		}
		if (InputFile.EndsWith(".fex"))
		{
			debug = false;
			BackgroundShell(bin + "7z", "x \"" + InputFile + "\" -o\"" + OutputFolder + "\" bootlogo.bmp");
			debug = true;
			if (File.Exists(OutputFolder + "\\bootlogo.bmp"))
			{
				LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked logo");
				return true;
			}
			LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack logo");
			return false;
		}
		if (InputFile.EndsWith("second"))
		{
			BackgroundShell(bin + "imgRePackerRK.exe", "/2nd \"" + InputFile + "\"");
			File.Move(InputFile + ".cfg", InputFile + ".dump\\" + Path.GetFileNameWithoutExtension(InputFile) + ".cfg");
			if (Directory.Exists(OutputFolder))
			{
				DeleteDirectory(OutputFolder);
			}
			if (File.Exists(InputFile + ".dump\\logo.bmp"))
			{
				Directory.Move(InputFile + ".dump", OutputFolder);
				LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked logo");
				return true;
			}
			LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack logo");
			return false;
		}
		LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack logo");
		return false;
	}

	public bool UnpackDtb(string InputFile, string OutputFolder)
	{
		LogInstance.Log(LogLineHead + "Unpacking DTB..");
		if (!Directory.Exists(OutputFolder))
		{
			Directory.CreateDirectory(OutputFolder);
		}
		if (Settings.Default.NewDtbProcessingTools)
		{
			LogInstance.Log(LogLineHead + "Processing dtb with new tools");
			BackgroundShell(bin + "python3\\python.exe", "\"" + binpython + "splitdtb.py\" \"" + InputFile + "\" \"" + OutputFolder + "\" \"" + Path.GetDirectoryName(OutputFolder) + "\\.config\\dtb_unpack\"");
		}
		else
		{
			File.Copy(InputFile, InputFile + ".tmp.gz");
			BackgroundShell(bin + "gzip.exe", "-d \"" + InputFile + ".tmp.gz");
			if (File.Exists(InputFile + ".tmp.gz"))
			{
				File.Delete(InputFile + ".tmp.gz");
			}
			StreamWriter writer = new StreamWriter(Path.GetDirectoryName(OutputFolder) + "\\.config\\dtb_unpack");
			if (!File.Exists(InputFile + ".tmp"))
			{
				File.Copy(InputFile, InputFile + ".tmp");
				writer.Write("split");
			}
			else
			{
				writer.Write("gunzip|split");
			}
			writer.Close();
			writer.Dispose();
			BackgroundShell(bin + "legacy\\dtbsplit.exe", "\"" + InputFile + ".tmp\" \"" + OutputFolder + "\\\\\"");
			if (File.Exists(InputFile + ".tmp"))
			{
				File.Delete(InputFile + ".tmp");
			}
		}
		string[] files = Directory.GetFiles(OutputFolder);
		foreach (string file in files)
		{
			BackgroundShell(bin + "dtc.exe", " -I dtb -O dts -o \"" + file.Replace(".dtb", ".dts") + "\" \"" + file + "\"");
			File.Delete(file);
		}
		if (Directory.GetFiles(OutputFolder).Count() > 0)
		{
			LogInstance.Log(LogLineHead + "[SUCCESS] Unpacked DTB");
			return true;
		}
		DeleteDirectory(OutputFolder);
		LogInstance.Log(LogLineHead + "[ERROR] Failed to unpack DTB");
		return false;
	}

	public static Architecture ReadFileArchitecture(string filePath)
	{
		if (filePath == null)
		{
			throw new ArgumentNullException("filePath");
		}
		using FileStream stream = File.OpenRead(filePath);
		return ReadFileArchitecture(stream);
	}

	public static Architecture ReadFileArchitecture(Stream stream)
	{
		if (stream == null)
		{
			throw new ArgumentNullException("stream");
		}
		long length = stream.Length;
		if (length < 64)
		{
			return Architecture.Unknown;
		}
		BinaryReader reader = new BinaryReader(stream);
		stream.Position = 60L;
		uint peHeaderPtr = reader.ReadUInt32();
		if (peHeaderPtr == 0)
		{
			peHeaderPtr = 128u;
		}
		if (peHeaderPtr > length - 256)
		{
			return Architecture.Unknown;
		}
		stream.Position = peHeaderPtr;
		if (reader.ReadUInt32() != 17744)
		{
			return Architecture.Unknown;
		}
		return reader.ReadUInt16() switch
		{
			34404 => Architecture.x64, 
			332 => Architecture.x86, 
			43620 => Architecture.arm64, 
			_ => Architecture.Unknown, 
		};
	}

	private static Architecture GetArchitecture(ref SYSTEM_INFO si)
	{
		return si.wProcessorArchitecture switch
		{
			9 => Architecture.x64, 
			12 => Architecture.arm64, 
			0 => Architecture.x86, 
			_ => throw new PlatformNotSupportedException(), 
		};
	}

	[DllImport("kernel32")]
	private static extern void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);

	[DllImport("kernel32")]
	private static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);
}
