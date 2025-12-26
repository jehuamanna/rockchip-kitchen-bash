using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AMLogger;
using AxWMPLib;
using CustomizationTool.Properties;

namespace CustomizationTool;

public class Form1 : Form
{
	public enum ImgType
	{
		Amlogic,
		Rockchip,
		Allwinner,
		None
	}

	private bool activated;

	private string User = EasyMD5.Hash(SystemGuid.Value());

	private string AppVersion = "";

	private string sysdir = "";

	private string bin = AppDomain.CurrentDomain.BaseDirectory + "bin\\";

	public Logger LogInstance;

	public PatchClass patcher;

	private string LogLineHead = "[CUSTOMIZATION_TOOL] : ";

	private UnpackerV4 unpacker;

	private UnpackLevel levelUnpacker = new UnpackLevel();

	private bool loadingInfo = true;

	private bool paid;

	private bool loadinginfo = true;

	private string ExtPaths = "system,vendor,product,odm,oem,system_ext";

	private ImgType ImageType = ImgType.None;

	public string ShellOutput = "";

	private bool redirectOutput;

	public bool nulldat033786;

	public bool nulldat103743 = true;

	public bool nulldat210554 = true;

	public bool nulldat396908;

	private string ShellWorkDir = AppDomain.CurrentDomain.BaseDirectory;

	private bool playingBootanimation;

	private string oldMD5 = "";

	private int SearchIndex = 1;

	private IContainer components;

	private TabControl MainTab;

	private TabPage tabPage1;

	private Label label4;

	private Label label3;

	private Label label2;

	private Label label1;

	private TabPage tabPage2;

	private TabPage tabPage3;

	private TabPage tabPage4;

	private TabPage tabPage5;

	private GroupBox groupBox2;

	private TextBox SystemApps;

	private Label label9;

	private TextBox SystemFiles;

	private Label label8;

	private TextBox SystemSize;

	private Label label7;

	private GroupBox groupBox1;

	private TextBox Build;

	private TextBox SecurityPatch;

	private TextBox Product;

	private TextBox Vendor;

	private TextBox Model;

	private TextBox Version;

	private Label label6;

	private Label label5;

	private Panel panel1;

	private TextBox UpgradePackage;

	private Button UnpackImage;

	private MenuStrip menuStrip1;

	private ToolStripMenuItem AboutButton;

	private Panel panel2;

	private Button BootLogoButton;

	private Button BootanimationButton;

	private Panel BootAnimationPanel;

	private PictureBox BootanimationPicturebox;

	private Panel panel4;

	private Button BootanimationChange;

	private Button PlayBootanimation;

	private Panel BootlogoPanel;

	private PictureBox BootlogoPicturebox;

	private Label BootanimationFPS;

	private ImageList imageList1;

	private ListView AppsViewer;

	private Panel panel6;

	private Label label10;

	private ComboBox AppsDirectory;

	private Panel panel3;

	private Button AddApps;

	private Button RemoveApps;

	private SplitContainer splitContainer1;

	private Label label11;

	private Label label12;

	private Button OpenKernel;

	private Button OpenRecovery;

	private PictureBox UnpackingLoader;

	private Panel panel7;

	private Label StatusLabel;

	private Label label17;

	private new ToolStripMenuItem HelpButton;

	private ToolStripMenuItem supportTheProjectToolStripMenuItem;

	private ToolStripMenuItem threadToolStripMenuItem;

	private ToolStripMenuItem reportBugsToolStripMenuItem;

	private TabPage PackPage;

	private Panel panel10;

	private Button OpenLogos;

	private TextBox VendorApps;

	private Label label16;

	private TextBox VendorFiles;

	private Label label20;

	private TextBox VendorSize;

	private Label label22;

	private TextBox BuildDisplayID;

	private Label label26;

	private TextBox SDKVersion;

	private Label label25;

	private CheckBox MultiDtb;

	private CheckBox ContainsVendor;

	private CheckBox RamdiskSystem;

	private CheckBox BootupGunzipped;

	private Label label27;

	private Label label30;

	private Label label29;

	private Label label28;

	private Label CTVersionLabel;

	private Label UserLabel;

	private TabPage tabPage6;

	private GroupBox VendorGroupBox;

	private Button OpenVendorRemoteConf;

	private Button OpenVendor;

	private Button OpenVendorBuildProp;

	private GroupBox groupBox14;

	private Button OpenVendorKeymap;

	private Button OpenVendorGenericKl;

	private GroupBox groupBox9;

	private CheckedListBox KeysChecklist;

	private Button WriteKeys;

	private Button OpenWorking;

	private GroupBox SystemGroupBox;

	private Button OpenSystemRemoteConf;

	private Button OpenSystem;

	private Button OpenSystemBuildProp;

	private GroupBox groupBox4;

	private Button OpenSystemKeymap;

	private Button OpenSystemGenericKl;

	private ToolStripMenuItem supportToolStripMenuItem;

	private Panel DTBPanel;

	private RichTextBox DTBEditor;

	private Panel panel8;

	private Panel panel9;

	private Label label31;

	private Button BackOccurance;

	private TextBox SearchBox;

	private Button NextOccurance;

	private Button DiscardDTB;

	private Button SaveDTB;

	private Label label32;

	private ComboBox DtbComboBox;

	private TabPage tabPage7;

	private GroupBox groupBox8;

	private Label CTPCompressionStatus;

	private TrackBar CTPCompression;

	private Label label21;

	private Button ExportCTP;

	private Label label19;

	private GroupBox groupBox7;

	private CheckBox AddPlatformConfig;

	private Label label15;

	private Label label14;

	private Button RepackToImage;

	private GroupBox groupBox6;

	private Label CompressionStatus;

	private TrackBar CompressionLevel;

	private Label label18;

	private Button ZipRom;

	private Label label13;

	private Panel PatchesPanel;

	private GroupBox WifiGroupBox;

	private Button MultiWifiDrivers;

	private CheckBox SuperPartition;

	private Label label23;

	private Label label24;

	private Label PackageType;

	private Button BootvideoButton;

	private Panel BootvideoPanel;

	private Panel panel11;

	private Button ChangeBootvideo;

	private Button PlayBootvideo;

	private AxWindowsMediaPlayer BootVideoPlayer;

	private ComboBox LocalesComboBox;

	private Label label38;

	private Button RefreshLogo;

	private System.Windows.Forms.Timer bootlogotimer;

	private Button ReplaceKernel;

	private Button ReplaceRecovery;

	private Button RootFirmware;

	private Button RemoveRoot;

	private ToolStripMenuItem tutorialsToolStripMenuItem1;

	private CheckBox CompressBrotli;

	private Button RepackApps;

	private Button UnpackApps;

	private GroupBox ApkGroupbox;

	private GroupBox groupBox3;

	private Label AppVisbleToLauncherLabel;

	private Label AppLauncherAppLabel;

	private Label AppPackageLabel;

	private Label AppVisibleToTvLauncherLabel;

	private Button BootImgForMagisk;

	private ToolStripMenuItem logsToolStripMenuItem;

	private CheckBox SpecifyChipset;

	private TextBox RKChipset;

	private Label PatchDescription;

	private Label PatchVersion;

	private Label PatchAuthor;

	private Label label37;

	private Label label36;

	private Label label35;

	private Label label34;

	private RichTextBox PatchRTB;

	private LinkLabel AddPatchLabel;

	private Button ApplyPatchButton;

	private Label label33;

	private ComboBox PatchComboBox;

	private TabPage tabPage8;

	private Panel panel5;

	private Label label39;

	private TextBox SearchTextbox;

	private ContextMenuStrip SearchContextMenu;

	private ToolStripMenuItem copyToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ListView FilesList;

	private ColumnHeader columnHeader1;

	private ToolStripMenuItem experimentalFeaturesToolStripMenuItem1;

	private ToolStripMenuItem PythonSuperUtils;

	private ToolStripMenuItem NewDtbProcessingTools;

	private ToolStripMenuItem NewLogoProcessingTools;

	private ToolStripMenuItem PythonImgExtractor;

	private ToolStripMenuItem ArchMake_ext4fs;

	private ToolStripMenuItem NewPythonSimg2img;

	public Form1()
	{
		InitializeComponent();
		AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "bin\\resources\\" + SystemGuid.Value()))
		{
			File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "bin\\resources\\" + SystemGuid.Value(), "null");
		}
		CheckUpdateAsync();
		CTVersionLabel.Text = "v" + AppVersion.Substring(2);
		LogInstance = new Logger(AppDomain.CurrentDomain.BaseDirectory + "Logs\\CustomizationTool_" + DateTime.Now.ToString("dd-MM-yyyy HH.mm") + "_log.txt");
		LogInstance.Log(LogLineHead + "Checking network, Updates and Activation..");
		CheckActivationAsync();
		LogInstance.Log(LogLineHead + "Applying dev settings..");
		ApplyDevSettings();
		unpacker = new UnpackerV4(LogInstance);
		BootVideoPlayer.stretchToFit = true;
		BootVideoPlayer.uiMode = "None";
		BootVideoPlayer.Ctlenabled = false;
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "patches"))
		{
			if (Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "patches").Count() > 0)
			{
				string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "patches");
				foreach (string patch in files)
				{
					PatchComboBox.Items.Add(Path.GetFileNameWithoutExtension(patch));
				}
			}
			else
			{
				PatchesPanel.Enabled = false;
			}
		}
		else
		{
			PatchesPanel.Enabled = false;
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Documentation\\Tutorials"))
		{
			string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Documentation\\Tutorials");
			foreach (string file in files)
			{
				ToolStripMenuItem item = new ToolStripMenuItem();
				item.Text = Path.GetFileNameWithoutExtension(file);
				item.Tag = file;
				item.Click += Item_Click;
				tutorialsToolStripMenuItem1.DropDownItems.Add(item);
			}
		}
		if (!activated)
		{
			WifiGroupBox.Enabled = false;
			new Form2(SystemGuid.Value()).ShowDialog();
		}
		else
		{
			WifiGroupBox.Text = "Wifi";
			WifiGroupBox.Enabled = true;
			supportTheProjectToolStripMenuItem.Visible = false;
		}
	}

	private void testmethod()
	{
	}

	private void Item_Click(object sender, EventArgs e)
	{
		new TutorialForm(((ToolStripMenuItem)sender).Tag.ToString()).Show();
	}

	public string ConvertGithubRaw(string input)
	{
		return input.Replace("github.com", "raw.githubusercontent.com").Replace("/blob/", "/");
	}

	private void ApplyDevSettings()
	{
		PythonSuperUtils.Checked = Settings.Default.PythonSuperUtils;
		NewDtbProcessingTools.Checked = Settings.Default.NewDtbProcessingTools;
		NewLogoProcessingTools.Checked = Settings.Default.NewLogoProcessingTools;
		PythonImgExtractor.Checked = Settings.Default.PythonImgextractor;
		ArchMake_ext4fs.Checked = Settings.Default.ArchMake_ext4fs;
		NewPythonSimg2img.Checked = Settings.Default.PythonSimg2Img;
	}

	private void CheckResources()
	{
		LogInstance.Log(LogLineHead + "Checking resources exist..");
		string rsrcs = "python3,apktools,aik,e2fs";
		string[] array = rsrcs.Split(',');
		foreach (string rsrc in array)
		{
			if (!Directory.Exists(bin + rsrc))
			{
				UnpackingLoader.Visible = true;
				StatusLabel.Text = "Downloading resource " + rsrc + "..";
				LogInstance.Log(LogLineHead + "Downloaing resource " + rsrc + "..");
				new Form4("https://raw.githubusercontent.com/RickyDivjakovski/CustomizationTool_Resources/main/External/" + rsrc + ".zip", AppDomain.CurrentDomain.BaseDirectory + "bin\\" + rsrc + ".zip").ShowDialog();
			}
		}
		array = rsrcs.Split(',');
		foreach (string rsrc2 in array)
		{
			if (!Directory.Exists(bin + rsrc2))
			{
				LogInstance.Log(LogLineHead + "Failed to download resource, exiting..");
				Application.Exit();
			}
		}
		StatusLabel.Text = "Done.";
		UnpackingLoader.Visible = false;
	}

	private async Task CheckActivationAsync()
	{
		WebClient wc = new WebClient();
		wc.Headers.Add("a", "a");
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
		wc.Proxy = null;
		nulldat033786 = true;
		try
		{
			string LicenseData = wc.DownloadString("https://rdsoftwaregineering.wordpress.com/amlogic-licensing-info/").Split(new string[1] { "<code>" }, StringSplitOptions.None)[1].Split(new string[1] { "</code>" }, StringSplitOptions.None)[0];
			if (!string.IsNullOrWhiteSpace(LicenseData))
			{
				string[] array = LicenseData.Replace("\r\n", "\n").Split('\n');
				foreach (string line in array)
				{
					if (activated)
					{
						continue;
					}
					if (line.StartsWith(SystemGuid.Value() + "|"))
					{
						if (line.Split('|')[0] == SystemGuid.Value())
						{
							paid = true;
							nulldat103743 = false;
							activated = true;
							LogInstance.Log(LogLineHead + "Obsolete license found..");
							UserLabel.Text = "User: " + line.Split('|')[1];
						}
					}
					else if (line.StartsWith(TrueSystemGuid.Value() + "|") && line.Split('|')[0] == TrueSystemGuid.Value())
					{
						paid = true;
						nulldat103743 = false;
						activated = true;
						LogInstance.Log(LogLineHead + "New license found..");
						UserLabel.Text = "User: " + line.Split('|')[1];
					}
				}
				if (!activated)
				{
					new Form2(TrueSystemGuid.Value()).ShowDialog();
					Application.Exit();
				}
			}
		}
		catch (WebException ex)
		{
			if (ex.Message.Contains("could not be resolved"))
			{
				MessageBox.Show("No internet connection.\nResources online are required for operation.\n\nPlease connect to a network to run CustomizationTool", "No network connection");
			}
		}
		if (activated)
		{
			return;
		}
		LogInstance.Log(LogLineHead + "Using legacy..");
		try
		{
			string LicenseData2 = wc.DownloadString(ConvertGithubRaw("https://github.com/RickyDivjakovski/Rocket/blob/main/AMLogicTools/Keys/" + SystemGuid.Value()));
			if (string.IsNullOrWhiteSpace(LicenseData2))
			{
				wc.DownloadString(ConvertGithubRaw("https://github.com/RickyDivjakovski/Rocket/blob/main/AMLogicTools/Keys/" + TrueSystemGuid.Value()));
			}
			if (string.IsNullOrWhiteSpace(LicenseData2))
			{
				return;
			}
			string[] array = LicenseData2.Replace("\r\n", "\n").Split('\n');
			foreach (string line2 in array)
			{
				if (activated)
				{
					continue;
				}
				if (line2.StartsWith(SystemGuid.Value() + "|"))
				{
					if (line2.Split('|')[1] == EasyMD5.Hash(SystemGuid.Value()))
					{
						paid = true;
						nulldat103743 = false;
						activated = true;
						LogInstance.Log(LogLineHead + "Obsolete license found..");
						UserLabel.Text = "User: " + line2.Split('|')[1];
					}
				}
				else if (line2.StartsWith(TrueSystemGuid.Value() + "|") && line2.Split('|')[1] == EasyMD5.Hash(TrueSystemGuid.Value()))
				{
					paid = true;
					nulldat103743 = false;
					activated = true;
					LogInstance.Log(LogLineHead + "New license found..");
					UserLabel.Text = "User: " + line2.Split('|')[1];
				}
			}
			if (!activated)
			{
				new Form2(TrueSystemGuid.Value()).ShowDialog();
				Application.Exit();
			}
		}
		catch (WebException ex2)
		{
			if (ex2.Message.Contains("could not be resolved"))
			{
				MessageBox.Show("No internet connection.\nResources online are required for operation.\n\nPlease connect to a network to run CustomizationTool", "No network connection");
			}
		}
	}

	private void CheckUpdateAsync()
	{
		Assembly.GetExecutingAssembly().GetName().Version.ToString();
		string UpdateLink = ConvertGithubRaw("https://github.com/RickyDivjakovski/CustomizationTool_Resources/blob/main/Version");
		WebClient wc = new WebClient();
		wc.Headers.Add("a", "a");
		ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
		wc.Proxy = null;
		string LatestVersion = AppVersion;
		string LatestDownload = "";
		try
		{
			string[] array = wc.DownloadString(UpdateLink).Replace("\r\n", "\n").Split('\n');
			foreach (string line in array)
			{
				nulldat210554 = false;
				if (line.StartsWith("version|"))
				{
					LatestVersion = line.Split('|')[1];
				}
				if (line.StartsWith("download|"))
				{
					LatestDownload = line.Split('|')[1];
				}
			}
		}
		catch
		{
		}
		Version version = new Version(AppVersion);
		Version version2 = new Version(LatestVersion);
		if (version.CompareTo(version2) < 0 && new UpdateTool(wc.DownloadString(ConvertGithubRaw("https://github.com/RickyDivjakovski/CustomizationTool_Resources/blob/main/Changelog")).Replace("\r\n", "\n").Split(new string[1] { "\n\n" }, StringSplitOptions.None)
			.First()).ShowDialog() == DialogResult.Yes)
		{
			if (File.Exists(Path.GetTempPath() + "\\CustomizationToolInstaller.exe"))
			{
				File.Delete(Path.GetTempPath() + "\\CustomizationToolInstaller.exe");
			}
			try
			{
				new Form4(LatestDownload, Path.GetTempPath() + "\\CustomizationToolInstaller.exe").ShowDialog();
				Process.Start(Path.GetTempPath() + "\\CustomizationToolInstaller.exe", "\"" + AppDomain.CurrentDomain.BaseDirectory.Remove(AppDomain.CurrentDomain.BaseDirectory.Length - 1, 1));
				Environment.Exit(0);
			}
			catch
			{
				MessageBox.Show("An error has occured while updating CustomizationTools.\nPlease ensure you have a stable network connection.\nAlternatively you can download the updated installer from the website.", "Update error");
			}
		}
		nulldat396908 = true;
	}

	private string CheckSysDir()
	{
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system\\build.prop"))
		{
			return "system\\";
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system\\system\\build.prop"))
		{
			return "system\\system\\";
		}
		return "";
	}

	private void Form1_Shown(object sender, EventArgs e)
	{
		CheckResources();
		BootVideoPlayer.stretchToFit = true;
		BootVideoPlayer.uiMode = "None";
		BootVideoPlayer.Ctlenabled = false;
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\image.cfg"))
		{
			ImageType = ImgType.Amlogic;
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1"))
		{
			string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1");
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].EndsWith(".fex"))
				{
					ImageType = ImgType.Allwinner;
				}
			}
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\parameter.txt"))
		{
			ImageType = ImgType.Rockchip;
		}
		PackageType.Text = ImageType.ToString();
		sysdir = CheckSysDir();
		patcher = new PatchClass(LogInstance, PatchRTB);
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\").Count() > 0)
		{
			LoadRom();
			MainTab.Enabled = true;
		}
	}

	private void LoadRom()
	{
		LoadInfo();
		AppsViewer.Items.Clear();
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2"))
		{
			PatchesPanel.Enabled = true;
		}
		else
		{
			PatchesPanel.Enabled = false;
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "patches"))
		{
			if (Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "patches").Count() == 0)
			{
				PatchesPanel.Enabled = false;
			}
		}
		else
		{
			PatchesPanel.Enabled = false;
		}
		if (AppsDirectory.Items.Count > 0)
		{
			AppsDirectory.SelectedIndex = 0;
		}
		DtbComboBox.Items.Clear();
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb").Count() > 0)
		{
			DTBPanel.Enabled = true;
			DTBEditor.Text = "";
			string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb");
			foreach (string dtb in files)
			{
				DtbComboBox.Items.Add(Path.GetFileName(dtb));
			}
		}
		else
		{
			DtbComboBox.Items.Add(" ");
			DTBEditor.Text = "";
			DTBPanel.Enabled = false;
		}
		if (DtbComboBox.Items.Count > 0)
		{
			DtbComboBox.SelectedIndex = 0;
		}
		if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system"))
		{
			SystemGroupBox.Enabled = false;
		}
		else
		{
			SystemGroupBox.Enabled = true;
		}
		if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor"))
		{
			VendorGroupBox.Enabled = false;
		}
		else
		{
			VendorGroupBox.Enabled = true;
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\keys.conf"))
		{
			StreamReader KeyStream = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\keys.conf");
			string[] files = KeyStream.ReadToEnd().Replace("\r", "").Split('\n');
			foreach (string key in files)
			{
				for (int j = 0; j < KeysChecklist.Items.Count; j++)
				{
					if ((string)KeysChecklist.Items[j] == key)
					{
						KeysChecklist.SetItemChecked(j, value: true);
					}
				}
			}
			KeyStream.Dispose();
		}
		MainTab.Enabled = true;
		loadinginfo = false;
	}

	private void BackgroundShell(string executable, string command)
	{
		string shellOutput = "";
		LogInstance.Log("[SHELL] : " + Path.GetFileNameWithoutExtension(executable) + " " + command.Replace(AppDomain.CurrentDomain.BaseDirectory, ""));
		Thread newThread = new Thread((ThreadStart)delegate
		{
			ProcessStartInfo startInfo = new ProcessStartInfo(executable, command)
			{
				UseShellExecute = false,
				RedirectStandardOutput = redirectOutput,
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
			process.Dispose();
		});
		newThread.Start();
		while (newThread.IsAlive)
		{
			Application.DoEvents();
		}
		newThread.Abort();
		ShellOutput = shellOutput;
	}

	private void UnpackImage_Click(object sender, EventArgs e)
	{
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Filter = "Upgrade Packages/Project files (*.img; *.ctp; *.zip) | *.img; *.ctp; *.zip";
		if (ofd.ShowDialog() == DialogResult.OK && levelUnpacker.ShowDialog() == DialogResult.OK)
		{
			Unpack(ofd.FileName, levelUnpacker.Tag.ToString());
		}
	}

	private string GetProp(string property)
	{
		string combinedBuildProp = "";
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "build.prop"))
		{
			combinedBuildProp = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "build.prop");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\build.prop"))
		{
			combinedBuildProp = combinedBuildProp + "\n" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\build.prop");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\product\\build.prop"))
		{
			combinedBuildProp = combinedBuildProp + "\n" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\product\\build.prop");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm\\build.prop"))
		{
			combinedBuildProp = combinedBuildProp + "\n" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm\\build.prop");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\oem\\build.prop"))
		{
			combinedBuildProp = combinedBuildProp + "\n" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\oem\\build.prop");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system_ext\\build.prop"))
		{
			combinedBuildProp = combinedBuildProp + "\n" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system_ext\\build.prop");
		}
		string returnstring = "";
		string[] array = combinedBuildProp.Split('\n');
		foreach (string s in array)
		{
			if (s.StartsWith(property) && !string.IsNullOrWhiteSpace(s.Split('=').Last()))
			{
				returnstring = s.Split('=').Last();
			}
		}
		return returnstring;
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

	private void Ext4PartitionUnpack(string partition, string outputDirectory)
	{
		StatusLabel.Text = "Unpacking level2(Unpacking " + partition + ")..";
		LogInstance.Log(LogLineHead + "Attempting " + partition + " unpack");
		Thread level2Thread = new Thread((ThreadStart)delegate
		{
			if (File.Exists(outputDirectory + "\\level1\\" + partition + ".PARTITION") && !unpacker.UnpackExt4(outputDirectory + "\\level1\\" + partition + ".PARTITION", outputDirectory + "\\level2"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
				MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
			}
			if (File.Exists(outputDirectory + "\\level1\\" + partition + "_a.PARTITION") && !unpacker.UnpackExt4(outputDirectory + "\\level1\\" + partition + "_a.PARTITION", outputDirectory + "\\level2"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
				MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
			}
			if (File.Exists(outputDirectory + "\\level1\\" + partition + "_a.fex") && !unpacker.UnpackExt4(outputDirectory + "\\level1\\" + partition + "_a.fex", outputDirectory + "\\level2"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
				MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
			}
			if (File.Exists(outputDirectory + "\\level1\\" + partition + ".fex") && !unpacker.UnpackExt4(outputDirectory + "\\level1\\" + partition + "fex", outputDirectory + "\\level2"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
				MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
			}
			if (File.Exists(outputDirectory + "\\level2\\super\\" + partition + "_a.img") && !unpacker.UnpackExt4(outputDirectory + "\\level2\\super\\" + partition + "_a.img", outputDirectory + "\\level2"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
				MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
			}
			if (File.Exists(outputDirectory + "\\level1\\" + partition + ".img") && !unpacker.UnpackExt4(outputDirectory + "\\level1\\" + partition + ".img", outputDirectory + "\\level2"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
				MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
			}
			if (File.Exists(outputDirectory + "\\level2\\super\\" + partition + ".img") && !unpacker.UnpackExt4(outputDirectory + "\\level2\\super\\" + partition + ".img", outputDirectory + "\\level2"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
				MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
			}
		});
		level2Thread.Start();
		while (level2Thread.IsAlive)
		{
			Application.DoEvents();
		}
	}

	private void Unpack(string Input, string unpackitems)
	{
		MainTab.SelectedIndex = 0;
		MainTab.Refresh();
		if (BootlogoPicturebox.Image != null)
		{
			BootlogoPicturebox.Image.Dispose();
		}
		BootlogoPicturebox.Image = Resources.blank;
		BootanimationPicturebox.Image = Resources.blank;
		BootVideoPlayer.Ctlcontrols.stop();
		BootVideoPlayer.currentPlaylist.clear();
		BootVideoPlayer.close();
		UnpackImage.Enabled = false;
		UnpackingLoader.Visible = true;
		MainTab.Enabled = false;
		bool unpacked = true;
		LogInstance.Log(LogLineHead + "Unpacking " + Input + "..");
		UpgradePackage.Text = Input;
		string outputDirectory = AppDomain.CurrentDomain.BaseDirectory + "tmp";
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp");
		}
		Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp");
		if (!Path.GetExtension(Input).EndsWith("ctp"))
		{
			StatusLabel.Text = "Unpacking level1(Splitting package)..";
			LogInstance.Log(LogLineHead + "LEVEL 1 ---------------------------------------");
			LogInstance.Log(LogLineHead + "Splitting partitions from package..");
			Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1");
			if (Path.GetExtension(Input).EndsWith("zip"))
			{
				StatusLabel.Text = "Unpacking level1(Unpacking zip)..";
				Thread level1Thread = new Thread((ThreadStart)delegate
				{
					if (!unpacker.UnpackUpgradeZip(Input, outputDirectory + "\\level1"))
					{
						unpacked = false;
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack to unpack zip file.");
						MessageBox.Show("Failed to unpack zip file.\nThe zip file is not an Amlogic, Rockchip or Allwinner upgrade zip", "Could not unpack zip.");
					}
				});
				level1Thread.Start();
				while (level1Thread.IsAlive)
				{
					Application.DoEvents();
				}
			}
			else
			{
				StatusLabel.Text = "Unpacking level1(Unpacking img)..";
				Thread level1Thread = new Thread((ThreadStart)delegate
				{
					if (!unpacker.UnpackVendorImage(Input, outputDirectory + "\\level1"))
					{
						unpacked = false;
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack to unpack img file.");
						MessageBox.Show("Failed to unpack img file.\nThe img file is not an Amlogic, Rockchip or Allwinner image", "Could not unpack img.");
					}
				});
				level1Thread.Start();
				while (level1Thread.IsAlive)
				{
					Application.DoEvents();
				}
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\keys.conf"))
			{
				StreamReader KeyStream = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\keys.conf");
				string[] array = KeyStream.ReadToEnd().Replace("\r", "").Split('\n');
				foreach (string key in array)
				{
					for (int i = 0; i < KeysChecklist.Items.Count; i++)
					{
						if ((string)KeysChecklist.Items[i] == key)
						{
							KeysChecklist.SetItemChecked(i, value: true);
						}
					}
				}
				KeyStream.Dispose();
			}
			if (unpackitems.Contains(";level2;"))
			{
				LogInstance.Log(LogLineHead + "LEVEL 2 ---------------------------------------");
				Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2");
				if (!Directory.Exists(outputDirectory + "\\level2\\.config"))
				{
					Directory.CreateDirectory(outputDirectory + "\\level2\\.config").Attributes = FileAttributes.Hidden | FileAttributes.Directory;
				}
				string partition = "";
				if (unpackitems.Contains(";super;"))
				{
					partition = "super";
					StatusLabel.Text = "Unpacking level2(Unpacking " + partition + ")..";
					LogInstance.Log(LogLineHead + "Attempting " + partition + " unpack");
					Thread level2Thread = new Thread((ThreadStart)delegate
					{
						if (File.Exists(outputDirectory + "\\level1\\super.PARTITION") && !unpacker.UnpackSuper(outputDirectory + "\\level1\\super.PARTITION", outputDirectory + "\\level2\\super"))
						{
							LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
						}
						if (File.Exists(outputDirectory + "\\level1\\super.img") && !unpacker.UnpackSuper(outputDirectory + "\\level1\\super.img", outputDirectory + "\\level2\\super"))
						{
							LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
						}
						if (File.Exists(outputDirectory + "\\level1\\super.fex") && !unpacker.UnpackSuper(outputDirectory + "\\level1\\super.fex", outputDirectory + "\\level2\\super"))
						{
							LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
						}
					});
					level2Thread.Start();
					while (level2Thread.IsAlive)
					{
						Application.DoEvents();
					}
				}
				if (unpackitems.Contains(";system;"))
				{
					partition = "system";
					Ext4PartitionUnpack(partition, outputDirectory);
					sysdir = CheckSysDir();
				}
				if (nulldat396908 && !nulldat103743 && nulldat033786 && !nulldat210554)
				{
					if (unpackitems.Contains(";vendor;"))
					{
						partition = "vendor";
						Ext4PartitionUnpack(partition, outputDirectory);
					}
					if (!paid)
					{
						Application.Exit();
					}
					if (unpackitems.Contains(";product;"))
					{
						partition = "product";
						Ext4PartitionUnpack(partition, outputDirectory);
					}
					if (unpackitems.Contains(";odm;"))
					{
						partition = "odm";
						Ext4PartitionUnpack(partition, outputDirectory);
					}
					if (!paid)
					{
						Application.Exit();
					}
					if (unpackitems.Contains(";oem;"))
					{
						partition = "oem";
						Ext4PartitionUnpack(partition, outputDirectory);
					}
					if (unpackitems.Contains(";system_ext;"))
					{
						partition = "system_ext";
						Ext4PartitionUnpack(partition, outputDirectory);
					}
					if (unpackitems.Contains(";odm_ext;"))
					{
						partition = "odm_ext";
						Ext4PartitionUnpack(partition, outputDirectory);
					}
					if (!paid)
					{
						Application.Exit();
					}
					if (unpackitems.Contains(";boot;"))
					{
						partition = "boot";
						StatusLabel.Text = "Unpacking level2(Unpacking " + partition + ")..";
						LogInstance.Log(LogLineHead + "Attempting " + partition + " unpack");
						Thread level2Thread = new Thread((ThreadStart)delegate
						{
							if (File.Exists(outputDirectory + "\\level1\\boot.PARTITION") && !unpacker.UnpackKernel(outputDirectory + "\\level1\\boot.PARTITION", outputDirectory + "\\level2\\boot"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level1\\boot.img") && !unpacker.UnpackKernel(outputDirectory + "\\level1\\boot.img", outputDirectory + "\\level2\\boot"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level1\\boot.fex") && !unpacker.UnpackKernel(outputDirectory + "\\level1\\boot.fex", outputDirectory + "\\level2\\boot"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level1\\boot_a.fex") && !unpacker.UnpackKernel(outputDirectory + "\\level1\\boot_a.fex", outputDirectory + "\\level2\\boot"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level1\\boot_a.PARTITION") && !unpacker.UnpackKernel(outputDirectory + "\\level1\\boot_a.PARTITION", outputDirectory + "\\level2\\boot"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level2\\super\\boot_a.img") && !unpacker.UnpackKernel(outputDirectory + "\\level2\\super\\boot_a.img", outputDirectory + "\\level2\\boot"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level2\\super\\boot.img") && !unpacker.UnpackKernel(outputDirectory + "\\level2\\super\\boot.img", outputDirectory + "\\level2\\boot"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
						});
						level2Thread.Start();
						while (level2Thread.IsAlive)
						{
							Application.DoEvents();
						}
					}
					if (unpackitems.Contains(";recovery;"))
					{
						partition = "recovery";
						StatusLabel.Text = "Unpacking level2(Unpacking " + partition + ")..";
						LogInstance.Log(LogLineHead + "Attempting " + partition + " unpack");
						Thread level2Thread = new Thread((ThreadStart)delegate
						{
							if (File.Exists(outputDirectory + "\\level1\\recovery.PARTITION") && !unpacker.UnpackKernel(outputDirectory + "\\level1\\recovery.PARTITION", outputDirectory + "\\level2\\recovery"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level1\\recovery.img") && !unpacker.UnpackKernel(outputDirectory + "\\level1\\recovery.img", outputDirectory + "\\level2\\recovery"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level1\\recovery.fex") && !unpacker.UnpackKernel(outputDirectory + "\\level1\\recovery.fex", outputDirectory + "\\level2\\recovery"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level1\\recovery_a.fex") && !unpacker.UnpackKernel(outputDirectory + "\\level1\\recovery_a.fex", outputDirectory + "\\level2\\recovery"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level1\\recovery_a.PARTITION") && !unpacker.UnpackKernel(outputDirectory + "\\level1\\recovery_a.PARTITION", outputDirectory + "\\level2\\recovery"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level2\\super\\recovery_a.img") && !unpacker.UnpackKernel(outputDirectory + "\\level2\\super\\recovery_a.img", outputDirectory + "\\level2\\recovery"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
							if (File.Exists(outputDirectory + "\\level2\\super\\recovery.img") && !unpacker.UnpackKernel(outputDirectory + "\\level2\\super\\recovery.img", outputDirectory + "\\level2\\recovery"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
							}
						});
						level2Thread.Start();
						while (level2Thread.IsAlive)
						{
							Application.DoEvents();
						}
					}
					if (!paid)
					{
						Application.Exit();
					}
					if (unpackitems.Contains(";dtb;"))
					{
						partition = "dtb";
						StatusLabel.Text = "Unpacking level2(Unpacking " + partition + ")..";
						LogInstance.Log(LogLineHead + "Attempting " + partition + " unpack");
						Thread level2Thread = new Thread((ThreadStart)delegate
						{
							if (File.Exists(outputDirectory + "\\level1\\_aml_dtb.PARTITION") && !unpacker.UnpackDtb(outputDirectory + "\\level1\\_aml_dtb.PARTITION", outputDirectory + "\\level2\\dtb"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
								MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
							}
							if (File.Exists(outputDirectory + "\\level1\\_aml_dtb_a.PARTITION") && !unpacker.UnpackDtb(outputDirectory + "\\level1\\_aml_dtb_a.PARTITION", outputDirectory + "\\level2\\dtb"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
								MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
							}
							if (File.Exists(outputDirectory + "\\level2\\super\\_aml_dtb.img") && !unpacker.UnpackDtb(outputDirectory + "\\level2\\super\\_aml_dtb.img", outputDirectory + "\\level2\\dtb"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
								MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
							}
						});
						level2Thread.Start();
						while (level2Thread.IsAlive)
						{
							Application.DoEvents();
						}
					}
					if (unpackitems.Contains(";meson;"))
					{
						partition = "meson";
						StatusLabel.Text = "Unpacking level2(Unpacking " + partition + ")..";
						LogInstance.Log(LogLineHead + "Attempting " + partition + " unpack");
						Thread level2Thread = new Thread((ThreadStart)delegate
						{
							if (File.Exists(outputDirectory + "\\level1\\meson1.PARTITION") && !unpacker.UnpackDtb(outputDirectory + "\\level1\\meson1.PARTITION", outputDirectory + "\\level2\\meson1"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
								MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
							}
							if (File.Exists(outputDirectory + "\\level1\\meson1_a.PARTITION") && !unpacker.UnpackDtb(outputDirectory + "\\level1\\meson1_a.PARTITION", outputDirectory + "\\level2\\meson1"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
								MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
							}
							if (File.Exists(outputDirectory + "\\level2\\super\\meson1.img") && !unpacker.UnpackDtb(outputDirectory + "\\level2\\super\\meson1.img", outputDirectory + "\\level2\\meson1"))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition);
								MessageBox.Show("Failed to unpack the " + partition + " partition.\nOperation will still continue however modifications involving an unpacked " + partition + " will not be enabled.", "Could not unpack " + partition);
							}
						});
						level2Thread.Start();
						while (level2Thread.IsAlive)
						{
							Application.DoEvents();
						}
					}
				}
			}
			if (unpackitems.Contains(";level3;"))
			{
				LogInstance.Log(LogLineHead + "LEVEL 3 ---------------------------------------");
				if (!Directory.Exists(outputDirectory + "\\level3"))
				{
					Directory.CreateDirectory(outputDirectory + "\\level3");
				}
				if (!Directory.Exists(outputDirectory + "\\level3\\.config"))
				{
					Directory.CreateDirectory(outputDirectory + "\\level3\\.config").Attributes = FileAttributes.Hidden | FileAttributes.Directory;
				}
				string partition2 = "";
				if (unpackitems.Contains(";logo;"))
				{
					partition2 = "logo";
					StatusLabel.Text = "Unpacking level3(Unpacking " + partition2 + ")..";
					LogInstance.Log(LogLineHead + "Attempting " + partition2 + " unpack");
					if (File.Exists(outputDirectory + "\\level1\\logo.PARTITION"))
					{
						if (!unpacker.UnpackLogo(outputDirectory + "\\level1\\logo.PARTITION", outputDirectory + "\\level3\\bootlogo"))
						{
							LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition2);
							MessageBox.Show("Failed to unpack the " + partition2 + " partition.\nOperation will still continue however modifications involving an unpacked " + partition2 + " will not be enabled.", "Could not unpack " + partition2);
						}
					}
					else if (File.Exists(outputDirectory + "\\level1\\boot-resource.fex"))
					{
						if (!unpacker.UnpackLogo(outputDirectory + "\\level1\\boot-resource.fex", outputDirectory + "\\level3\\bootlogo"))
						{
							LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition2);
							MessageBox.Show("Failed to unpack the " + partition2 + " partition.\nOperation will still continue however modifications involving an unpacked " + partition2 + " will not be enabled.", "Could not unpack " + partition2);
						}
					}
					else if (File.Exists(outputDirectory + "\\level2\\boot\\split_img\\second") && !unpacker.UnpackLogo(outputDirectory + "\\level2\\boot\\split_img\\second", outputDirectory + "\\level3\\bootlogo"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition2);
						MessageBox.Show("Failed to unpack the " + partition2 + " partition.\nOperation will still continue however modifications involving an unpacked " + partition2 + " will not be enabled.", "Could not unpack " + partition2);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp"))
					{
						if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm_ext\\logo_files\\bootup.bmp"))
						{
							File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp");
							File.Copy(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm_ext\\logo_files\\bootup.bmp", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp");
						}
						FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp", FileMode.Open, FileAccess.Read);
						BootlogoPicturebox.Image = Image.FromStream(fs);
						fs.Close();
					}
					else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootlogo.bmp"))
					{
						FileStream fs2 = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootlogo.bmp", FileMode.Open, FileAccess.Read);
						BootlogoPicturebox.Image = Image.FromStream(fs2);
						fs2.Close();
					}
					else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\logo.bmp"))
					{
						FileStream fs3 = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\logo.bmp", FileMode.Open, FileAccess.Read);
						BootlogoPicturebox.Image = Image.FromStream(fs3);
						fs3.Close();
					}
				}
				if (unpackitems.Contains(";bootanimation;"))
				{
					partition2 = "bootanimation";
					StatusLabel.Text = "Unpacking level3(Unpacking " + partition2 + ")..";
					LogInstance.Log(LogLineHead + "Attempting " + partition2 + " unpack");
					string[] array = ExtPaths.Split(',');
					foreach (string extpath in array)
					{
						if (File.Exists(outputDirectory + "\\level2\\" + extpath + "\\media\\bootanimation.zip"))
						{
							if (!unpacker.UnpackBootanimation(outputDirectory + "\\level2\\" + extpath + "\\media\\bootanimation.zip", outputDirectory + "\\level3\\bootanimation\\" + extpath))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition2);
								MessageBox.Show("Failed to unpack the " + partition2 + " in " + extpath + ".\nOperation will still continue however modifications involving an unpacked " + partition2 + " in " + extpath + " will not be enabled.", "Could not unpack " + partition2);
							}
						}
						else if (File.Exists(outputDirectory + "\\level2\\" + extpath + "\\" + extpath + "\\media\\bootanimation.zip") && !unpacker.UnpackBootanimation(outputDirectory + "\\level2\\" + extpath + "\\" + extpath + "\\media\\bootanimation.zip", outputDirectory + "\\level3\\bootanimation\\" + extpath))
						{
							LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition2);
							MessageBox.Show("Failed to unpack the " + partition2 + " in " + extpath + ".\nOperation will still continue however modifications involving an unpacked " + partition2 + " in " + extpath + " will not be enabled.", "Could not unpack " + partition2);
						}
					}
				}
				if (unpackitems.Contains(";bootvideo;"))
				{
					partition2 = "bootvideo";
					StatusLabel.Text = "Unpacking level3(Unpacking " + partition2 + ")..";
					LogInstance.Log(LogLineHead + "Attempting " + partition2 + " unpack");
					string[] array = ExtPaths.Split(',');
					foreach (string extpath2 in array)
					{
						if (File.Exists(outputDirectory + "\\level2\\" + extpath2 + "\\etc\\bootvideo"))
						{
							if (!unpacker.UnpackBootVideo(outputDirectory + "\\level2\\" + extpath2 + "\\etc\\bootvideo", outputDirectory + "\\level3\\bootvideo\\" + extpath2))
							{
								LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition2);
								MessageBox.Show("Failed to unpack the " + partition2 + " in " + extpath2 + ".\nOperation will still continue however modifications involving an unpacked " + partition2 + " in " + extpath2 + " will not be enabled.", "Could not unpack " + partition2);
							}
						}
						else if (File.Exists(outputDirectory + "\\level2\\" + extpath2 + "\\" + extpath2 + "\\etc\\bootvideo") && !unpacker.UnpackBootVideo(outputDirectory + "\\level2\\" + extpath2 + "\\" + extpath2 + "\\etc\\bootvideo", outputDirectory + "\\level3\\bootvideo\\" + extpath2))
						{
							LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition2);
							MessageBox.Show("Failed to unpack the " + partition2 + " in " + extpath2 + ".\nOperation will still continue however modifications involving an unpacked " + partition2 + " in " + extpath2 + " will not be enabled.", "Could not unpack " + partition2);
						}
					}
				}
				if (unpackitems.Contains(";launcher;"))
				{
					partition2 = "launcher";
					StatusLabel.Text = "Unpacking level3(Unpacking " + partition2 + ")..";
					LogInstance.Log(LogLineHead + "Attempting " + partition2 + " unpack");
					if (Directory.Exists(outputDirectory + "\\level2") && !unpacker.UnpackLauncher(outputDirectory + "\\level2"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of " + partition2);
						MessageBox.Show("Failed to unpack the " + partition2 + ".\nOperation will still continue however modifications involving an unpacked " + partition2 + " will not be enabled.", "Could not unpack " + partition2);
					}
				}
			}
		}
		else if (Path.GetExtension(Input).EndsWith("ctp"))
		{
			LogInstance.Log(LogLineHead + "Unpacking customization tool project..");
			StatusLabel.Text = "Unpacking Customization Tool Project..";
			Thread ProjectThread = new Thread((ThreadStart)delegate
			{
				BackgroundShell(bin + "7za", " x \"" + Input + "\" -o\"" + AppDomain.CurrentDomain.BaseDirectory + "tmp\"");
			});
			ProjectThread.Start();
			while (ProjectThread.IsAlive)
			{
				Application.DoEvents();
			}
		}
		if (unpacked)
		{
			MainTab.Enabled = true;
			LoadRom();
			if (AppsDirectory.Items.Count > 0)
			{
				AppsDirectory.SelectedIndex = 0;
			}
			MessageBox.Show("Unpacked successfully..", "Unpack complete");
		}
		else
		{
			MessageBox.Show("Unpacking failed.", "Unpack complete");
		}
		UnpackImage.Enabled = true;
		StatusLabel.Text = "Done.";
		UnpackingLoader.Visible = false;
		LogInstance.Log(LogLineHead + "Unpack complete");
	}

	private void LoadInfo()
	{
		AppsDirectory.Items.Clear();
		Version.Text = "";
		Model.Text = "";
		Vendor.Text = "";
		Product.Text = "";
		SDKVersion.Text = "";
		SecurityPatch.Text = "";
		Build.Text = "";
		BuildDisplayID.Text = "";
		SystemSize.Text = "";
		SystemFiles.Text = "";
		SystemApps.Text = "";
		VendorSize.Text = "";
		VendorFiles.Text = "";
		VendorApps.Text = "";
		RamdiskSystem.Checked = false;
		ContainsVendor.Checked = false;
		MultiDtb.Checked = false;
		BootupGunzipped.Checked = false;
		SuperPartition.Checked = false;
		PackageType.Text = "";
		SpecifyChipset.Checked = false;
		SpecifyChipset.Enabled = false;
		RKChipset.Text = "";
		RKChipset.Enabled = false;
		AddPlatformConfig.Checked = false;
		AddPlatformConfig.Enabled = false;
		ImageType = ImgType.None;
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\image.cfg"))
		{
			ImageType = ImgType.Amlogic;
		}
		string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1");
		for (int i = 0; i < files.Length; i++)
		{
			if (files[i].EndsWith(".fex"))
			{
				ImageType = ImgType.Allwinner;
			}
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\parameter.txt"))
		{
			ImageType = ImgType.Rockchip;
		}
		PackageType.Text = ImageType.ToString();
		if (ImageType == ImgType.Rockchip)
		{
			SpecifyChipset.Enabled = true;
		}
		if (ImageType == ImgType.Amlogic)
		{
			AddPlatformConfig.Enabled = true;
		}
		if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\image") && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\platform.conf"))
		{
			AddPlatformConfig.Checked = true;
		}
		string sysdir = "";
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system\\build.prop"))
		{
			sysdir = "system\\";
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system\\system\\build.prop"))
		{
			sysdir = "system\\system\\";
		}
		if (sysdir == "system\\system\\")
		{
			RamdiskSystem.Checked = true;
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\vendor.PARTITION"))
		{
			ContainsVendor.Checked = true;
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\vendor.img"))
		{
			ContainsVendor.Checked = true;
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\vendor_a.img"))
		{
			ContainsVendor.Checked = true;
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\vendor_b.img"))
		{
			ContainsVendor.Checked = true;
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb").Count() > 1)
		{
			MultiDtb.Checked = true;
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\meson1") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\meson1").Count() > 1)
		{
			MultiDtb.Checked = true;
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super"))
		{
			SuperPartition.Checked = true;
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb") && Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\meson1") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb").Count() + Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\meson1").Count() > 1)
		{
			MultiDtb.Checked = true;
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\.config\\logo.types") && File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\.config\\logo.types").Contains("bootup=gz"))
		{
			BootupGunzipped.Checked = true;
		}
		files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1");
		for (int i = 0; i < files.Length; i++)
		{
			if (files[i].EndsWith(".br"))
			{
				CompressBrotli.Checked = true;
			}
		}
		string combinedBuildProp = "";
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "build.prop"))
		{
			combinedBuildProp = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "build.prop");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\build.prop"))
		{
			combinedBuildProp = combinedBuildProp + "\n" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\build.prop");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\product\\build.prop"))
		{
			combinedBuildProp = combinedBuildProp + "\n" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\product\\build.prop");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm\\build.prop"))
		{
			combinedBuildProp = combinedBuildProp + "\n" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm\\build.prop");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\oem\\build.prop"))
		{
			combinedBuildProp = combinedBuildProp + "\n" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\oem\\build.prop");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system_ext\\build.prop"))
		{
			combinedBuildProp = combinedBuildProp + "\n" + File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system_ext\\build.prop");
		}
		string currentLocale = "";
		Version.Text = GetProp("ro.build.version.release");
		Model.Text = GetProp("ro.product.model");
		Vendor.Text = GetProp("ro.product.brand");
		if (string.IsNullOrWhiteSpace(Vendor.Text))
		{
			Vendor.Text = GetProp("ro.product.system.brand");
		}
		Product.Text = GetProp("ro.product.device");
		if (string.IsNullOrWhiteSpace(Product.Text))
		{
			Product.Text = GetProp("ro.product.system.device");
		}
		Model.Text = GetProp("ro.product.model");
		if (string.IsNullOrWhiteSpace(Model.Text))
		{
			Model.Text = GetProp("ro.product.system.model");
		}
		SDKVersion.Text = GetProp("ro.build.version.sdk");
		SecurityPatch.Text = GetProp("ro.build.version.security_patch");
		Build.Text = GetProp("ro.build.id");
		BuildDisplayID.Text = GetProp("ro.build.display.id");
		currentLocale = GetProp("ro.product.locale").Replace(" ", "");
		LocalesComboBox.Items.Clear();
		if (string.IsNullOrWhiteSpace(currentLocale))
		{
			LocalesComboBox.Enabled = false;
		}
		else
		{
			LocalesComboBox.Enabled = true;
			StreamReader streamReader = new StreamReader(bin + "resources\\locales");
			string locales = streamReader.ReadToEnd();
			streamReader.Close();
			streamReader.Dispose();
			if (!string.IsNullOrWhiteSpace(currentLocale))
			{
				files = locales.Split('\n');
				foreach (string locale in files)
				{
					LocalesComboBox.Items.Add(locale);
					if (locale.StartsWith(currentLocale))
					{
						currentLocale = locale;
					}
				}
				if (LocalesComboBox.FindStringExact(currentLocale) >= 0)
				{
					LocalesComboBox.SelectedIndex = LocalesComboBox.FindStringExact(currentLocale);
				}
				else
				{
					LocalesComboBox.Enabled = false;
				}
			}
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir))
		{
			SystemSize.Text = (RomSize(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir) / 1024 / 1024).ToString();
			SystemFiles.Text = FilesAmount(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir).ToString();
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor"))
		{
			VendorSize.Text = (RomSize(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor") / 1024 / 1024).ToString();
			VendorFiles.Text = FilesAmount(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor").ToString();
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2"))
		{
			int sysappcount = 0;
			int venappcount = 0;
			foreach (string item in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2", "*.apk", SearchOption.AllDirectories))
			{
				if (item.StartsWith(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system"))
				{
					sysappcount++;
				}
				else
				{
					venappcount++;
				}
			}
			SystemApps.Text = sysappcount.ToString();
			VendorApps.Text = venappcount.ToString();
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2"))
		{
			foreach (string file in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2", "*.apk", SearchOption.AllDirectories))
			{
				if (file.EndsWith("\\" + Path.GetFileNameWithoutExtension(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".apk"))
				{
					string appsdir = "";
					try
					{
						appsdir = file.Replace(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\", "\\").Replace("\\" + Path.GetFileNameWithoutExtension(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".apk", "").Replace(sysdir, "system\\")
							.Replace("\\", "/");
					}
					catch
					{
					}
					if (!AppsDirectory.Items.Contains(appsdir))
					{
						AppsDirectory.Items.Add(appsdir);
					}
					continue;
				}
				try
				{
					if (!AppsDirectory.Items.Contains(Path.GetDirectoryName(file).Replace(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\", "\\").Replace(sysdir, "system\\")
						.Replace("\\", "/")))
					{
						string appsdir2 = "";
						appsdir2 = Path.GetDirectoryName(file).Replace(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\", "\\").Replace(sysdir, "system\\")
							.Replace("\\", "/");
						if (!AppsDirectory.Items.Contains(appsdir2))
						{
							AppsDirectory.Items.Add(appsdir2);
						}
					}
				}
				catch
				{
				}
			}
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\apps"))
		{
			AppsDirectory.Items.Add("Unpacked");
		}
		DtbComboBox.Items.Clear();
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb").Count() > 0)
		{
			DTBPanel.Enabled = true;
			DTBEditor.Text = "";
			files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb");
			foreach (string dtb in files)
			{
				DtbComboBox.Items.Add(Path.GetFileName(dtb));
			}
		}
		else
		{
			DtbComboBox.Items.Add(" ");
			DTBEditor.Text = "";
			DTBPanel.Enabled = false;
		}
		if (DtbComboBox.Items.Count > 0)
		{
			DtbComboBox.SelectedIndex = 0;
		}
		loadingInfo = false;
		UnpackingLoader.Visible = false;
	}

	public static long RomSize(string searchDirectory)
	{
		if (Directory.Exists(searchDirectory))
		{
			long num = (from file in Directory.EnumerateFiles(searchDirectory)
				let fileInfo = new FileInfo(file)
				select fileInfo.Length).Sum();
			long subDirSize = (from directory in Directory.EnumerateDirectories(searchDirectory)
				select RomSize(directory)).Sum();
			return num + subDirSize;
		}
		return 0L;
	}

	public static int FilesAmount(string searchDirectory)
	{
		if (Directory.Exists(searchDirectory))
		{
			return Directory.GetFiles(searchDirectory, "*", SearchOption.AllDirectories).Count();
		}
		return 0;
	}

	private void ChangeUIButtonBackcolor()
	{
		BootanimationButton.BackColor = Color.Transparent;
		BootvideoButton.BackColor = Color.Transparent;
		BootLogoButton.BackColor = Color.Transparent;
	}

	private async void BootanimationButton_Click(object sender, EventArgs e)
	{
		ChangeUIButtonBackcolor();
		BootanimationButton.BackColor = Color.SkyBlue;
		BootAnimationPanel.BringToFront();
		BootAnimationPanel.Dock = DockStyle.Fill;
		BootanimationPicturebox.Dock = DockStyle.Fill;
	}

	private async void PlayBootanimation_Click(object sender, EventArgs e)
	{
		if (playingBootanimation)
		{
			return;
		}
		string firstpath = "";
		string[] array = ExtPaths.Split(',');
		foreach (string extpath in array)
		{
			if (string.IsNullOrWhiteSpace(firstpath) && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootanimation\\" + extpath + "\\desc.txt"))
			{
				firstpath = extpath;
			}
		}
		if (!string.IsNullOrWhiteSpace(firstpath))
		{
			playingBootanimation = true;
			StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootanimation\\" + firstpath + "\\desc.txt");
			string fpsdata = reader.ReadLine();
			reader.Dispose();
			BootanimationFPS.Text = fpsdata.Split()[0] + "x" + fpsdata.Split()[1] + "@" + fpsdata.Split()[2] + "fps";
			string[] directories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootanimation\\" + firstpath);
			foreach (string s in directories)
			{
				string[] files = Directory.GetFiles(s);
				foreach (string path in files)
				{
					if (BootanimationPicturebox.Image != null)
					{
						BootanimationPicturebox.Image.Dispose();
					}
					FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
					BootanimationPicturebox.Image = Image.FromStream(fs);
					fs.Close();
					int rate;
					try
					{
						rate = 60 - Convert.ToInt32(fpsdata.Split()[2]) / 2;
					}
					catch
					{
						rate = 45;
					}
					await Task.Delay(rate);
				}
			}
			playingBootanimation = false;
		}
		else
		{
			MessageBox.Show("No bootanimation detected in rom. Cannot display.", "Error of bootanimation playback.");
		}
	}

	private void BootanimationChange_Click(object sender, EventArgs e)
	{
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Filter = "bootanimation files (*.zip) | *.zip";
		if (ofd.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		if (BootanimationPicturebox.Image != null)
		{
			BootanimationPicturebox.Image.Dispose();
		}
		UnpackingLoader.Visible = true;
		bool unpacked = true;
		string[] directories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootanimation");
		foreach (string dir in directories)
		{
			if (!File.Exists(dir + "\\desc.txt"))
			{
				continue;
			}
			if (!unpacker.UnpackBootanimation(ofd.FileName, dir))
			{
				unpacked = false;
			}
			else if (Path.GetFileName(dir) == "system")
			{
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "media\\bootanimation.zip"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "media\\bootanimation.zip");
					File.Copy(ofd.FileName, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "media\\bootanimation.zip");
				}
			}
			else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileName(dir) + "\\media\\bootanimation.zip"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileName(dir) + "\\media\\bootanimation.zip");
				File.Copy(ofd.FileName, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileName(dir) + "\\media\\bootanimation.zip");
			}
		}
		if (unpacked)
		{
			MessageBox.Show("Successfully imported bootanimation.", "Imported bootanimation.");
		}
		else
		{
			MessageBox.Show("Failed to import bootanimation.\nNo desc.txt found.", "Error of bootanimation unpack.");
		}
		UnpackingLoader.Visible = false;
	}

	private void BootvideoButton_Click(object sender, EventArgs e)
	{
		ChangeUIButtonBackcolor();
		BootvideoButton.BackColor = Color.SkyBlue;
		BootvideoPanel.BringToFront();
		BootvideoPanel.Dock = DockStyle.Fill;
	}

	private void PlayBootvideo_Click(object sender, EventArgs e)
	{
		BootVideoPlayer.BringToFront();
		BootVideoPlayer.Ctlcontrols.stop();
		BootVideoPlayer.currentPlaylist.clear();
		BootVideoPlayer.close();
		BootVideoPlayer.stretchToFit = true;
		BootVideoPlayer.Dock = DockStyle.Fill;
		string firstpath = "";
		string[] array = ExtPaths.Split(',');
		foreach (string extpath in array)
		{
			if (string.IsNullOrWhiteSpace(firstpath) && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootvideo\\" + extpath + "\\bootvideo.mp4"))
			{
				firstpath = extpath;
			}
		}
		if (!string.IsNullOrWhiteSpace(firstpath))
		{
			BootVideoPlayer.URL = AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootvideo\\" + firstpath + "\\bootvideo.mp4";
			BootVideoPlayer.Ctlcontrols.play();
		}
		else
		{
			MessageBox.Show("No bootvideo detected in rom. Cannot display.", "Error of bootvideo playback.");
		}
	}

	private void ChangeBootvideo_Click(object sender, EventArgs e)
	{
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Filter = "bootanimation video (*.mp4) | *.mp4";
		if (ofd.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		UnpackingLoader.Visible = true;
		BootVideoPlayer.Ctlcontrols.stop();
		BootVideoPlayer.currentPlaylist.clear();
		BootVideoPlayer.close();
		int count = 0;
		string[] directories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootvideo");
		foreach (string dir in directories)
		{
			if (!File.Exists(dir + "\\bootvideo.mp4"))
			{
				continue;
			}
			File.Delete(dir + "\\bootvideo.mp4");
			File.Copy(ofd.FileName, dir + "\\bootvideo.mp4");
			if (Path.GetFileName(dir) == "system")
			{
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\bootvideo.mp4"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\bootvideo.mp4");
					File.Copy(ofd.FileName, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\bootvideo.mp4");
				}
			}
			else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileName(dir) + "\\etc\\bootvideo.mp4"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileName(dir) + "\\etc\\bootvideo.mp4");
				File.Copy(ofd.FileName, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileName(dir) + "\\etc\\bootvideo.mp4");
			}
			count++;
		}
		if (count > 0)
		{
			MessageBox.Show("Successfully imported bootvideo.", "Imported bootvideo.");
		}
		else
		{
			MessageBox.Show("No bootvideo in ROM detected.", "Error of bootvideo import.");
		}
		UnpackingLoader.Visible = false;
	}

	private void BootLogoButton_Click(object sender, EventArgs e)
	{
		ChangeUIButtonBackcolor();
		BootLogoButton.BackColor = Color.SkyBlue;
		BootlogoPanel.BringToFront();
		BootlogoPanel.Dock = DockStyle.Fill;
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp"))
		{
			FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp", FileMode.Open, FileAccess.Read);
			BootlogoPicturebox.Image = Image.FromStream(fs);
			fs.Close();
		}
		else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootlogo.bmp"))
		{
			FileStream fs2 = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootlogo.bmp", FileMode.Open, FileAccess.Read);
			BootlogoPicturebox.Image = Image.FromStream(fs2);
			fs2.Close();
		}
		else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\logo.bmp"))
		{
			FileStream fs3 = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\logo.bmp", FileMode.Open, FileAccess.Read);
			BootlogoPicturebox.Image = Image.FromStream(fs3);
			fs3.Close();
		}
		else
		{
			MessageBox.Show("No bootlogo unpacked or detected in firmware. \nDisplaying is currently only supported for amlogic/allwinner devices.\nRepacking is currently only supported for amlogic devices.", "Error of bootlogo display.");
		}
	}

	private void AppsDirectory_SelectedIndexChanged(object sender, EventArgs e)
	{
		UpdateAppsView();
	}

	private void UpdateAppsView()
	{
		AppsViewer.Items.Clear();
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2"))
		{
			int sysappcount = 0;
			int venappcount = 0;
			foreach (string item in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2", "*.apk", SearchOption.AllDirectories))
			{
				if (item.StartsWith(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system"))
				{
					sysappcount++;
				}
				else
				{
					venappcount++;
				}
			}
			SystemApps.Text = sysappcount.ToString();
			VendorApps.Text = venappcount.ToString();
		}
		string realappsdir = AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2" + AppsDirectory.SelectedItem.ToString().Replace("/", "\\");
		if (AppsDirectory.SelectedItem.ToString().StartsWith("/system/"))
		{
			realappsdir = AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2" + AppsDirectory.SelectedItem.ToString().Replace("/", "\\").Replace("\\system\\", "\\" + sysdir);
		}
		if (AppsDirectory.SelectedItem != null && !string.IsNullOrWhiteSpace(AppsDirectory.SelectedItem.ToString()) && Directory.Exists(realappsdir))
		{
			string[] directories = Directory.GetDirectories(realappsdir);
			foreach (string subfolder in directories)
			{
				string[] files = Directory.GetFiles(subfolder);
				for (int j = 0; j < files.Length; j++)
				{
					if (files[j].EndsWith(".apk"))
					{
						ListViewItem lvi = new ListViewItem(Path.GetFileName(subfolder), 0);
						lvi.Tag = subfolder;
						AppsViewer.Items.Add(lvi);
					}
				}
			}
			directories = Directory.GetFiles(realappsdir);
			foreach (string app in directories)
			{
				if (app.EndsWith(".apk"))
				{
					ListViewItem lvi2 = new ListViewItem(Path.GetFileName(app), 0);
					lvi2.Tag = app;
					AppsViewer.Items.Add(lvi2);
				}
			}
		}
		else
		{
			if (!(AppsDirectory.SelectedItem.ToString() == "Unpacked"))
			{
				return;
			}
			foreach (string item2 in Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\apps", "*apktool.yml", SearchOption.AllDirectories))
			{
				ListViewItem lvi3 = new ListViewItem(Path.GetFileName(item2.Replace("\\apktool.yml", "")), 0);
				string app2 = item2.Replace("\\apktool.yml", "").Replace("\\tmp\\level3\\apps", "\\tmp\\level2");
				if (Path.GetDirectoryName(app2).EndsWith(Path.GetFileNameWithoutExtension(app2)))
				{
					lvi3.Tag = Path.GetDirectoryName(app2);
				}
				else
				{
					lvi3.Tag = app2 + ".apk";
				}
				AppsViewer.Items.Add(lvi3);
			}
			MessageBox.Show("Unpacked APKs will not be repacked while repacking firmware.\nTo repack, click the 'Repack' button.", "APK repacking");
		}
	}

	private void RemoveApps_Click(object sender, EventArgs e)
	{
		if (AppsViewer.SelectedItems == null || AppsDirectory.SelectedItem == null || string.IsNullOrWhiteSpace(AppsDirectory.SelectedItem.ToString()) || AppsViewer.SelectedItems.Count <= 0)
		{
			return;
		}
		foreach (ListViewItem selectedItem in AppsViewer.SelectedItems)
		{
			string appfile = selectedItem.Tag.ToString();
			if (File.Exists(appfile))
			{
				File.Delete(appfile);
			}
			else if (Directory.Exists(appfile))
			{
				DeleteDirectory(appfile);
			}
		}
		UpdateAppsView();
	}

	private void AddApps_Click(object sender, EventArgs e)
	{
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Filter = "android apps (*.apk) | *.apk";
		if (ofd.ShowDialog() == DialogResult.OK)
		{
			string realappsdir = AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2" + AppsDirectory.SelectedItem.ToString().Replace("/", "\\");
			if (AppsDirectory.SelectedItem.ToString().StartsWith("/system/"))
			{
				realappsdir = AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2" + AppsDirectory.SelectedItem.ToString().Replace("/", "\\").Replace("\\system\\", "\\" + sysdir);
			}
			File.Copy(ofd.FileName, realappsdir + "\\" + Path.GetFileName(ofd.FileName));
			UpdateAppsView();
		}
	}

	private void AppsViewer_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (AppsViewer.SelectedItems == null)
		{
			ResetManifestInfo();
		}
		if (AppsViewer.SelectedItems.Count > 0)
		{
			ResetManifestInfo();
		}
		if (AppsViewer.SelectedItems != null && AppsDirectory.SelectedItem != null && !string.IsNullOrWhiteSpace(AppsDirectory.SelectedItem.ToString()) && AppsViewer.SelectedItems.Count == 1)
		{
			ApkGroupbox.Enabled = true;
			string appfile = AppsViewer.SelectedItems[0].Tag.ToString();
			string outputFolder = "";
			if (File.Exists(appfile))
			{
				outputFolder = AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\apps\\" + appfile.Replace(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2", "");
			}
			else if (Directory.Exists(appfile))
			{
				_ = appfile + "\\" + Path.GetFileNameWithoutExtension(appfile);
				outputFolder = AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\apps\\" + appfile.Replace(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2", "") + "\\" + Path.GetFileNameWithoutExtension(appfile).Replace(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2", "");
			}
			if (Directory.Exists(outputFolder))
			{
				UnpackApps.Enabled = false;
				RepackApps.Enabled = true;
				AppManifestInfo(outputFolder);
			}
			else
			{
				UnpackApps.Enabled = true;
				RepackApps.Enabled = false;
				AppPackageLabel.Text = "";
			}
		}
	}

	private void ResetManifestInfo()
	{
		ApkGroupbox.Enabled = false;
		AppPackageLabel.Text = "";
		AppLauncherAppLabel.Text = "";
		AppVisbleToLauncherLabel.Text = "";
		AppVisibleToTvLauncherLabel.Text = "";
		UnpackApps.Enabled = false;
		RepackApps.Enabled = false;
	}

	private void AppManifestInfo(string inputDirectory)
	{
		if (!File.Exists(inputDirectory + "\\AndroidManifest.xml"))
		{
			return;
		}
		AppPackageLabel.Text = "";
		StreamReader streamReader = new StreamReader(inputDirectory + "\\AndroidManifest.xml");
		string data = streamReader.ReadToEnd();
		streamReader.Close();
		streamReader.Dispose();
		string processedData = "";
		bool copy = false;
		bool isLauncher = false;
		string[] array = data.Split(' ');
		foreach (string line in array)
		{
			if (line.StartsWith("package"))
			{
				AppPackageLabel.Text = line.Split('"')[1];
			}
		}
		array = data.Replace("\r\n", "\n").Split('\n');
		foreach (string line2 in array)
		{
			if (line2.Contains("<intent-filter"))
			{
				copy = true;
			}
			if (copy)
			{
				processedData = processedData + line2 + "\n";
			}
			if (line2.Contains("</intent-filter"))
			{
				copy = false;
				if (processedData.Contains("android:name=\"android.intent.category.HOME\"") && processedData.Contains("android:name=\"android.intent.category.DEFAULT\""))
				{
					isLauncher = true;
				}
				processedData = "";
			}
		}
		if (isLauncher)
		{
			AppLauncherAppLabel.Text = "Launcher app: True";
		}
		else
		{
			AppLauncherAppLabel.Text = "Launcher app: False";
		}
		if (data.Contains("android:name=\"android.intent.category.LAUNCHER\""))
		{
			AppVisbleToLauncherLabel.Text = "Visible to launcher: True";
		}
		else
		{
			AppVisbleToLauncherLabel.Text = "Visible to launcher: False";
		}
		if (data.Contains("android:name=\"android.intent.category.LEANBACK_LAUNCHER\""))
		{
			AppVisibleToTvLauncherLabel.Text = "Visible to TV launcher: True";
		}
		else
		{
			AppVisibleToTvLauncherLabel.Text = "Visible to TV launcher: False";
		}
	}

	private void UnpackApps_Click(object sender, EventArgs e)
	{
		if (AppsViewer.SelectedItems == null || AppsDirectory.SelectedItem == null || string.IsNullOrWhiteSpace(AppsDirectory.SelectedItem.ToString()))
		{
			return;
		}
		if (AppsViewer.SelectedItems.Count == 1)
		{
			string appfile = AppsViewer.SelectedItems[0].Tag.ToString();
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
			if (Directory.Exists(outputFolder))
			{
				MessageBox.Show(Path.GetFileNameWithoutExtension(inputFile) + " is already unpacked.", "Already unpacked");
			}
			else
			{
				UnpackingLoader.Visible = true;
				StatusLabel.Text = "Decompiling " + Path.GetFileNameWithoutExtension(appfile) + "..";
				if (Directory.Exists(bin + "apktools"))
				{
					Thread packThread = new Thread((ThreadStart)delegate
					{
						if (unpacker.UnpackAPK(inputFile, outputFolder))
						{
							LogInstance.Log(LogLineHead + "Successfully unpacked " + Path.GetFileNameWithoutExtension(appfile));
							MessageBox.Show("Unpacked APKs will not be repacked while repacking firmware.\nTo repack, click the 'Repack' button.");
						}
						else
						{
							LogInstance.Log(LogLineHead + "Failed to unpack " + Path.GetFileNameWithoutExtension(appfile));
							MessageBox.Show("Failed to unpack", "ERROR");
						}
					});
					packThread.Start();
					while (packThread.IsAlive)
					{
						Application.DoEvents();
					}
				}
				if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\apps") && Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\apps", "*apktool.yml", SearchOption.AllDirectories).Count() > 0)
				{
					bool isUnpacked = false;
					foreach (string item in AppsDirectory.Items)
					{
						if (item.ToString() == "Unpacked")
						{
							isUnpacked = true;
						}
					}
					if (!isUnpacked)
					{
						AppsDirectory.Items.Add("Unpacked");
					}
				}
				StatusLabel.Text = "Done.";
				UnpackingLoader.Visible = false;
			}
			if (Directory.Exists(outputFolder))
			{
				UnpackApps.Enabled = false;
				RepackApps.Enabled = true;
				AppManifestInfo(outputFolder);
			}
			else
			{
				UnpackApps.Enabled = true;
				RepackApps.Enabled = false;
				AppPackageLabel.Text = "";
			}
		}
		else
		{
			MessageBox.Show("Please select one APK file at a time.", "Too many selections");
		}
	}

	private void RepackApps_Click(object sender, EventArgs e)
	{
		if (AppsViewer.SelectedItems == null || AppsDirectory.SelectedItem == null || string.IsNullOrWhiteSpace(AppsDirectory.SelectedItem.ToString()))
		{
			return;
		}
		if (AppsViewer.SelectedItems.Count == 1)
		{
			string appfile = AppsViewer.SelectedItems[0].Tag.ToString();
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
			if (!Directory.Exists(outputFolder))
			{
				return;
			}
			UnpackingLoader.Visible = true;
			StatusLabel.Text = "Compiling " + Path.GetFileNameWithoutExtension(appfile) + "..";
			Thread packThread = new Thread((ThreadStart)delegate
			{
				if (unpacker.RepackAPK(outputFolder, inputFile))
				{
					LogInstance.Log(LogLineHead + "Successfully repacked " + Path.GetFileNameWithoutExtension(appfile));
				}
				else
				{
					LogInstance.Log(LogLineHead + "Failed to repack " + Path.GetFileNameWithoutExtension(appfile));
					MessageBox.Show("Failed to repack", "ERROR");
				}
			});
			packThread.Start();
			while (packThread.IsAlive)
			{
				Application.DoEvents();
			}
			StatusLabel.Text = "Done.";
			UnpackingLoader.Visible = false;
		}
		else
		{
			MessageBox.Show("Please select one APK file at a time.", "Too many selections");
		}
	}

	private void AppsViewer_DoubleClick(object sender, EventArgs e)
	{
		if (AppsViewer.SelectedItems != null && AppsDirectory.SelectedItem != null && !string.IsNullOrWhiteSpace(AppsDirectory.SelectedItem.ToString()) && AppsViewer.SelectedItems.Count == 1)
		{
			string appfile = AppsViewer.SelectedItems[0].Tag.ToString();
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
			if (File.Exists(outputFolder + "\\AndroidManifest.xml"))
			{
				Process.Start(outputFolder);
			}
			else
			{
				Process.Start(inputFile + ".apk");
			}
		}
	}

	private void OpenKernel_Click(object sender, EventArgs e)
	{
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot\\ramdisk"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot\\ramdisk");
		}
	}

	private void BootImgForMagisk_Click(object sender, EventArgs e)
	{
		LogInstance.Log(LogLineHead + "Generating boot.img for Magisk..");
		string outputDirectory = AppDomain.CurrentDomain.BaseDirectory + "tmp";
		string bootfile = "";
		if (File.Exists(outputDirectory + "\\level1\\boot.PARTITION"))
		{
			bootfile = outputDirectory + "\\level1\\boot.PARTITION";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot.img"))
		{
			bootfile = outputDirectory + "\\level1\\boot.img";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot.fex"))
		{
			bootfile = outputDirectory + "\\level1\\boot.fex";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot_a.PARTITION"))
		{
			bootfile = outputDirectory + "\\level1\\boot_a.PARTITION";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot_a.img"))
		{
			bootfile = outputDirectory + "\\level1\\boot_a.img";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot_a.fex"))
		{
			bootfile = outputDirectory + "\\level1\\boot_a.fex";
		}
		if (File.Exists(outputDirectory + "\\level2\\super\\boot.img"))
		{
			bootfile = outputDirectory + "\\level2\\super\\boot.img";
		}
		if (File.Exists(outputDirectory + "\\level2\\super\\boot_a.img"))
		{
			bootfile = outputDirectory + "\\level2\\super\\boot_a.img";
		}
		if (!string.IsNullOrWhiteSpace(bootfile))
		{
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\magiskimg"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\magiskimg");
			}
			if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\magiskimg"))
			{
				Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\magiskimg");
			}
			File.Copy(bootfile, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\magiskimg\\boot.img");
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\magiskimg\\boot.img"))
			{
				Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\magiskimg");
			}
		}
		else
		{
			MessageBox.Show("The kernel was not found in the firmware", "Failed to generate boot.img for magisk");
		}
	}

	private void ReplaceKernel_Click(object sender, EventArgs e)
	{
		OpenFileDialog ofd = new OpenFileDialog();
		if (ofd.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		LogInstance.Log(LogLineHead + "Replacing kernel..");
		string outputDirectory = AppDomain.CurrentDomain.BaseDirectory + "tmp";
		string bootfile = "";
		if (File.Exists(outputDirectory + "\\level1\\boot.PARTITION"))
		{
			bootfile = outputDirectory + "\\level1\\boot.PARTITION";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot.img"))
		{
			bootfile = outputDirectory + "\\level1\\boot.img";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot.fex"))
		{
			bootfile = outputDirectory + "\\level1\\boot.fex";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot_a.PARTITION"))
		{
			bootfile = outputDirectory + "\\level1\\boot_a.PARTITION";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot_a.img"))
		{
			bootfile = outputDirectory + "\\level1\\boot_a.img";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot_a.fex"))
		{
			bootfile = outputDirectory + "\\level1\\boot_a.fex";
		}
		if (File.Exists(outputDirectory + "\\level2\\super\\boot.img"))
		{
			bootfile = outputDirectory + "\\level2\\super\\boot.img";
		}
		if (File.Exists(outputDirectory + "\\level2\\super\\boot_a.img"))
		{
			bootfile = outputDirectory + "\\level2\\super\\boot_a.img";
		}
		if (!string.IsNullOrWhiteSpace(bootfile))
		{
			File.Delete(bootfile);
			File.Copy(ofd.FileName, bootfile);
			if (Directory.Exists(outputDirectory + "\\level2"))
			{
				bool unpackboot = false;
				if (Directory.Exists(outputDirectory + "\\level2\\boot"))
				{
					unpackboot = true;
				}
				else if (MessageBox.Show("Would you like to unpack the kernel?", "Unpack kernel", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					unpackboot = true;
				}
				if (unpackboot)
				{
					if (Directory.Exists(outputDirectory + "\\level2\\boot"))
					{
						DeleteDirectory(outputDirectory + "\\level2\\boot");
					}
					Thread level2Thread = new Thread((ThreadStart)delegate
					{
						if (!unpacker.UnpackKernel(bootfile, outputDirectory + "\\level2\\boot"))
						{
							LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of kernel");
							MessageBox.Show("Failed to unpack the boot partition.\nOperation will still continue however modifications involving an unpacked boot will not be enabled.", "Could not unpack boot");
						}
					});
					level2Thread.Start();
					while (level2Thread.IsAlive)
					{
						Application.DoEvents();
					}
				}
			}
			LogInstance.Log(LogLineHead + "Replacing kernel..");
			MessageBox.Show("The kernel has been replaced", "Kernel replaced");
		}
		else
		{
			MessageBox.Show("The kernel was not found in the firmware", "Failed to replace kernel");
		}
	}

	private void OpenRecovery_Click(object sender, EventArgs e)
	{
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\recovery\\ramdisk"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\recovery\\ramdisk");
		}
	}

	private void ReplaceRecovery_Click(object sender, EventArgs e)
	{
		OpenFileDialog ofd = new OpenFileDialog();
		if (ofd.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		LogInstance.Log(LogLineHead + "Replacing recovery..");
		string outputDirectory = AppDomain.CurrentDomain.BaseDirectory + "tmp";
		string recoveryfile = "";
		if (File.Exists(outputDirectory + "\\level1\\boot.PARTITION"))
		{
			recoveryfile = outputDirectory + "\\level1\\boot.PARTITION";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot.img"))
		{
			recoveryfile = outputDirectory + "\\level1\\boot.img";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot.fex"))
		{
			recoveryfile = outputDirectory + "\\level1\\boot.fex";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot_a.PARTITION"))
		{
			recoveryfile = outputDirectory + "\\level1\\boot_a.PARTITION";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot_a.img"))
		{
			recoveryfile = outputDirectory + "\\level1\\boot_a.img";
		}
		if (File.Exists(outputDirectory + "\\level1\\boot_a.fex"))
		{
			recoveryfile = outputDirectory + "\\level1\\boot_a.fex";
		}
		if (File.Exists(outputDirectory + "\\level2\\super\\boot.img"))
		{
			recoveryfile = outputDirectory + "\\level2\\super\\boot.img";
		}
		if (File.Exists(outputDirectory + "\\level2\\super\\boot_a.img"))
		{
			recoveryfile = outputDirectory + "\\level2\\super\\boot_a.img";
		}
		if (!string.IsNullOrWhiteSpace(recoveryfile))
		{
			File.Delete(recoveryfile);
			File.Copy(ofd.FileName, recoveryfile);
			if (Directory.Exists(outputDirectory + "\\level2"))
			{
				bool unpackrecovery = false;
				if (Directory.Exists(outputDirectory + "\\level2\\recovery"))
				{
					unpackrecovery = true;
				}
				else if (MessageBox.Show("Would you like to unpack the recovery?", "Unpack recovery", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					unpackrecovery = true;
				}
				if (unpackrecovery)
				{
					if (Directory.Exists(outputDirectory + "\\level2\\recovery"))
					{
						DeleteDirectory(outputDirectory + "\\level2\\recovery");
					}
					Thread level2Thread = new Thread((ThreadStart)delegate
					{
						if (!unpacker.UnpackKernel(recoveryfile, outputDirectory + "\\level2\\recovery"))
						{
							LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - unpack of recovery");
							MessageBox.Show("Failed to unpack the recovery partition.\nOperation will still continue however modifications involving an unpacked recovery will not be enabled.", "Could not unpack recovery");
						}
					});
					level2Thread.Start();
					while (level2Thread.IsAlive)
					{
						Application.DoEvents();
					}
				}
			}
			LogInstance.Log(LogLineHead + "Replacing recovery..");
			MessageBox.Show("The recovery has been replaced", "Recovery replaced");
		}
		else
		{
			MessageBox.Show("The recovery was not found in the firmware", "Failed to replace recovery");
		}
	}

	private void OpenWorking_Click(object sender, EventArgs e)
	{
		Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp");
	}

	private void RootFirmware_Click(object sender, EventArgs e)
	{
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "build.prop"))
		{
			if (MessageBox.Show("Are you sure you want to root the firmware with SuperSU?", "Root firmware", MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			int api = 19;
			try
			{
				api = Convert.ToInt32(GetProp("ro.build.version.sdk"));
			}
			catch
			{
				if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app") && Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app").Count() > 3)
				{
					api = 21;
				}
			}
			string abi = "";
			string abilong = "";
			string abi2 = "";
			if (string.IsNullOrWhiteSpace("ro.product.cpu.abilist"))
			{
				abi = GetProp("ro.product.cpu.abi").Substring(0, 3);
				abilong = GetProp("ro.product.cpu.abi").Split(',')[0];
				abi2 = GetProp("ro.product.cpu.abi2").Split(',')[0];
			}
			else
			{
				abi = GetProp("ro.product.cpu.abilist").Split(',')[0].Substring(0, 3);
				abilong = GetProp("ro.product.cpu.abilist").Split(',')[0];
				if (GetProp("ro.product.cpu.abilist").Split(',').Length == 2)
				{
					abi2 = GetProp("ro.product.cpu.abilist").Split(',')[1];
				}
			}
			bool sugote = false;
			bool supolicy = false;
			string arch = "arm";
			bool appprocess64 = false;
			string systemlib = "lib";
			string mksh = "mksh";
			if (abi == "x86")
			{
				arch = "x86";
			}
			if (abi2 == "x86")
			{
				arch = "x86";
			}
			if (api >= 17)
			{
				sugote = true;
				if (abilong == "armeabi-v7a")
				{
					arch = "armv7";
				}
				if (abi == "mip")
				{
					arch = "mips";
				}
				if (abilong == "mips")
				{
					arch = "mips";
				}
			}
			_ = 18;
			if (api >= 20)
			{
				if (abilong == "arm64-v8a")
				{
					arch = "arm64";
					systemlib = "lib64";
					appprocess64 = true;
				}
				if (abilong == "mips64")
				{
					arch = "mips64";
					systemlib = "lib64";
					appprocess64 = true;
				}
				if (abilong == "x86_64")
				{
					arch = "x64";
					systemlib = "lib64";
					appprocess64 = true;
				}
			}
			if (api >= 19)
			{
				supolicy = true;
				File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\toolbox");
			}
			_ = 21;
			if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\" + mksh))
			{
				mksh = "sh";
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery.sh"))
			{
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery_original.sh"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery.sh");
				}
				else
				{
					File.Move(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery.sh", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery_original.sh");
				}
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery.sh"))
			{
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery_original.sh"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery.sh");
				}
				else
				{
					File.Move(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery.sh", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery_original.sh");
				}
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32_original"))
			{
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32");
				}
				File.Move(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32_original", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64_original"))
			{
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64");
				}
				File.Move(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64_original", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process_init"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process_init");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\su"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\su");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\su"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\su");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "sbin\\su"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "sbin\\su");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\sbin\\su"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\sbin\\su");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\bin\\su"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\bin\\su");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\xbin\\su"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\xbin\\su");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\daemonsu"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\daemonsu");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\sugote"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\sugote");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\sugote-mksh"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\sugote-mksh");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\supolicy"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\supolicy");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "lib\\libsupol.so"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "lib\\libsupol.so");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "lib64\\libsupol.so"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "lib64\\libsupol.so");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\.ext\\.su"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\.ext\\.su");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery.sh"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery.sh");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery.sh"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery.sh");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\init.d\\99SuperSUDaemon"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\init.d\\99SuperSUDaemon");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\.installed_su_daemon"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\.installed_su_daemon");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.apk"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.apk");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.apk"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.apk");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.odex"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.odex");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.apk"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.apk");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.apk"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.apk");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.odex"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.odex");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.apk"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.apk");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.apk"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.apk");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.odex"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.odex");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.apk"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.apk");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.apk"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.apk");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.odex"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.odex");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.apk"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.apk");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.apk"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.apk");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.odex"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.odex");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.apk"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.apk");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.apk"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.apk");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.odex"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.odex");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser.apk"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser.apk");
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser"))
			{
				DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser.apk"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser.apk");
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser.odex");
			}
			if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\.ext"))
			{
				Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\.ext");
			}
			File.Copy(bin + "resources\\su\\" + arch + "\\su", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\.ext\\.su");
			File.Copy(bin + "resources\\su\\" + arch + "\\su", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\su");
			File.Copy(bin + "resources\\su\\" + arch + "\\su", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\daemonsu");
			if (sugote)
			{
				File.Copy(bin + "resources\\su\\" + arch + "\\su", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\sugote");
				File.Copy(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\" + mksh, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\sugote-mksh");
			}
			if (supolicy)
			{
				File.Copy(bin + "resources\\su\\" + arch + "\\supolicy", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\supolicy");
				File.Copy(bin + "resources\\su\\" + arch + "\\libsupol.so", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + systemlib + "\\libsupol.so");
			}
			if (api < 20)
			{
				File.Copy(bin + "resources\\su\\common\\Superuser.apk", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.apk");
			}
			else
			{
				Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU");
				File.Copy(bin + "resources\\su\\common\\Superuser.apk", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU\\SuperSU.apk");
			}
			File.Copy(bin + "resources\\su\\common\\install-recovery.sh", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery.sh");
			File.Copy(bin + "resources\\su\\common\\install-recovery.sh", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery.sh");
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process");
			}
			File.Copy(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\daemonsu", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process");
			if (appprocess64)
			{
				if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64_original"))
				{
					File.Move(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64_original");
				}
				else
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64_original");
				}
				File.Copy(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\daemonsu", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64");
				if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process_init"))
				{
					File.Copy(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64_original", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process_init");
				}
			}
			else
			{
				if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32_original"))
				{
					File.Move(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32_original");
				}
				else
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32_original");
				}
				File.Copy(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\daemonsu", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32");
				if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process_init"))
				{
					File.Copy(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32_original", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process_init");
				}
			}
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\init.d"))
			{
				File.Copy(bin + "resources\\su\\common\\99SuperSUDaemon", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\init.d\\99SuperSUDaemon");
				File.Copy(bin + "resources\\su\\common\\.installed_su_daemon", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\.installed_su_daemon");
			}
			MessageBox.Show("Firmware has been successfully rooted.", "Success");
		}
		else
		{
			MessageBox.Show("Cannot detect system partition.", "Error");
		}
	}

	private void RemoveRoot_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("Are you sure you want to remove SuperSU root access from the firmware?", "Remove root", MessageBoxButtons.YesNo) != DialogResult.Yes)
		{
			return;
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery.sh"))
		{
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery_original.sh"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery.sh");
			}
			else
			{
				File.Move(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery.sh", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery_original.sh");
			}
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery.sh"))
		{
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery_original.sh"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery.sh");
			}
			else
			{
				File.Move(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery.sh", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery_original.sh");
			}
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32_original"))
		{
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32");
			}
			File.Move(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32_original", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process32");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64_original"))
		{
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64");
			}
			File.Move(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64_original", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process64");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process_init"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\app_process_init");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\su"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\su");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\su"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\su");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "sbin\\su"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "sbin\\su");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\sbin\\su"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\sbin\\su");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\bin\\su"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\bin\\su");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\xbin\\su"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\xbin\\su");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\daemonsu"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\daemonsu");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\sugote"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\sugote");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\sugote-mksh"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\sugote-mksh");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\supolicy"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "xbin\\supolicy");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "lib\\libsupol.so"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "lib\\libsupol.so");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "lib64\\libsupol.so"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "lib64\\libsupol.so");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\.ext\\.su"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\.ext\\.su");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery.sh"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "bin\\install-recovery.sh");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery.sh"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\install-recovery.sh");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\init.d\\99SuperSUDaemon"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\init.d\\99SuperSUDaemon");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\.installed_su_daemon"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\.installed_su_daemon");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.apk"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.apk");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.apk"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.apk");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.odex"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Superuser.odex");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.apk"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.apk");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.apk"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.apk");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.odex"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperUser.odex");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.apk"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.apk");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.apk"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.apk");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.odex"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\superuser.odex");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.apk"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.apk");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.apk"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.apk");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.odex"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\Supersu.odex");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.apk"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.apk");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.apk"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.apk");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.odex"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\SuperSU.odex");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.apk"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.apk");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.apk"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.apk");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.odex"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\supersu.odex");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser.apk"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser.apk");
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser.apk"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser.apk");
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser"))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "app\\VenomSuperUser.odex");
		}
		MessageBox.Show("SuperSU root access has been removed from the firmware.", "Success");
	}

	private void OpenSystem_Click(object sender, EventArgs e)
	{
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir);
		}
	}

	private void OpenBuildProp_Click(object sender, EventArgs e)
	{
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "build.prop"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "build.prop");
		}
	}

	private void OpenKeymap_Click(object sender, EventArgs e)
	{
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "usr\\keylayout"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "usr\\keylayout");
		}
	}

	private void OpenGenericKl_Click(object sender, EventArgs e)
	{
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "usr\\keylayout\\Generic.kl"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "usr\\keylayout\\Generic.kl");
		}
	}

	private void OpenRemoteConf_Click(object sender, EventArgs e)
	{
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\remote.conf"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\remote.conf");
		}
		else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\remote.cfg"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "etc\\remote.cfg");
		}
	}

	private void OpenVendor_Click(object sender, EventArgs e)
	{
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor");
		}
	}

	private void OpenVendorBuildProp_Click(object sender, EventArgs e)
	{
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\build.prop"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\build.prop");
		}
	}

	private void OpenVendorKeymap_Click(object sender, EventArgs e)
	{
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\usr\\keylayout"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\usr\\keylayout");
		}
	}

	private void OpenVendorGenericKl_Click(object sender, EventArgs e)
	{
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\usr\\keylayout\\Generic.kl"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\usr\\keylayout\\Generic.kl");
		}
	}

	private void OpenVendorRemoteConf_Click(object sender, EventArgs e)
	{
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\etc\\remote.conf"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\etc\\remote.conf");
		}
		else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\etc\\remote.cfg"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\etc\\remote.cfg");
		}
	}

	private void Ext4PartitionRepack(string partition)
	{
		StatusLabel.Text = "Repacking " + partition + "..";
		Thread packThread = new Thread((ThreadStart)delegate
		{
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\" + partition + ".PARTITION") && !unpacker.RepackExt4(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + partition, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\" + partition + ".PARTITION"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\" + partition + ".img") && !unpacker.RepackExt4(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + partition, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\" + partition + ".img"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\" + partition + ".fex") && !unpacker.RepackExt4(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + partition, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\" + partition + ".fex"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\" + partition + "_a.fex") && !unpacker.RepackExt4_super(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + partition, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\" + partition + "_a.fex"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\" + partition + "_a.PARTITION") && !unpacker.RepackExt4_super(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + partition, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\" + partition + "_a.PARTITION"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\" + partition + "_a.img") && !unpacker.RepackExt4_super(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + partition, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\" + partition + "_a.img"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\" + partition + ".img") && !unpacker.RepackExt4_super(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + partition, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\" + partition + ".img"))
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
			}
		});
		packThread.Start();
		while (packThread.IsAlive)
		{
			Application.DoEvents();
		}
	}

	private void StandardRepackFunction()
	{
		string partition = "";
		LogInstance.Log(LogLineHead + "LEVEL 3 ---------------------------------------");
		if (nulldat396908 && !nulldat103743 && nulldat033786 && !nulldat210554)
		{
			partition = "logo";
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp"))
			{
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm_ext\\logo_files\\bootup.bmp"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm_ext\\logo_files\\bootup.bmp");
					File.Copy(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm_ext\\logo_files\\bootup.bmp");
				}
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\logo.PARTITION"))
				{
					StatusLabel.Text = "Repacking " + partition + "..";
					if (!unpacker.RepackLogo(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\logo.PARTITION"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
				}
			}
			else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\logo.bmp") && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot\\split_img\\second"))
			{
				StatusLabel.Text = "Repacking " + partition + "..";
				if (!unpacker.RepackLogo(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot\\split_img\\second"))
				{
					LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
				}
			}
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootvideo"))
		{
			partition = "bootvideo";
			StatusLabel.Text = "Repacking bootvideo..";
			if (nulldat396908 && !nulldat103743 && nulldat033786 && !nulldat210554)
			{
				string[] directories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootvideo");
				foreach (string dir in directories)
				{
					if (!File.Exists(dir + "\\bootvideo.mp4"))
					{
						continue;
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileName(dir) + "\\etc\\bootvideo"))
					{
						if (!unpacker.RepackBootVideo(dir + "\\bootvideo.mp4", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileName(dir) + "\\etc\\bootvideo"))
						{
							LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
						}
					}
					else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileName(dir) + "\\" + Path.GetFileName(dir) + "\\etc\\bootvideo") && !unpacker.RepackBootVideo(dir + "\\bootvideo.mp4", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileName(dir) + "\\" + Path.GetFileName(dir) + "\\etc\\bootvideo"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
				}
			}
		}
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootanimation"))
		{
			partition = "bootanimation";
			StatusLabel.Text = "Repacking bootanimation..";
			string[] directories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootanimation");
			foreach (string dir2 in directories)
			{
				if (!File.Exists(dir2 + "\\desc.txt"))
				{
					continue;
				}
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileNameWithoutExtension(dir2) + "\\media\\bootanimation.zip"))
				{
					if (!unpacker.RepackBootanimation(dir2, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileNameWithoutExtension(dir2) + "\\media\\bootanimation.zip"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
				}
				else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileNameWithoutExtension(dir2) + "\\" + Path.GetFileNameWithoutExtension(dir2) + "\\media\\bootanimation.zip") && !unpacker.RepackBootanimation(dir2, AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + Path.GetFileNameWithoutExtension(dir2) + "\\" + Path.GetFileNameWithoutExtension(dir2) + "\\media\\bootanimation.zip"))
				{
					LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
				}
			}
		}
		LogInstance.Log(LogLineHead + "LEVEL 2 ---------------------------------------");
		Thread packThread;
		if (nulldat396908 && !nulldat103743 && nulldat033786 && !nulldat210554)
		{
			if (!paid)
			{
				Environment.Exit(0);
			}
			partition = "dtb";
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb"))
			{
				StatusLabel.Text = "Repacking " + partition + "..";
				packThread = new Thread((ThreadStart)delegate
				{
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\_aml_dtb.PARTITION") && !unpacker.RepackDtb(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\_aml_dtb.PARTITION"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\_aml_dtb_a.PARTITION") && !unpacker.RepackDtb(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\_aml_dtb_a.PARTITION"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\dtb_a.img") && !unpacker.RepackDtb(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\dtb_a.img"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
				});
				packThread.Start();
				while (packThread.IsAlive)
				{
					Application.DoEvents();
				}
			}
			partition = "meson1";
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\meson1"))
			{
				StatusLabel.Text = "Repacking " + partition + "..";
				packThread = new Thread((ThreadStart)delegate
				{
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\meson1.PARTITION") && !unpacker.RepackDtb(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\meson1", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\meson1.PARTITION"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\meson1_a.PARTITION") && !unpacker.RepackDtb(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\meson1", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\meson1_a.PARTITION"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\meson1_a.img") && !unpacker.RepackDtb(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\meson1", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\meson1_a.img"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
				});
				packThread.Start();
				while (packThread.IsAlive)
				{
					Application.DoEvents();
				}
			}
			partition = "boot";
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot"))
			{
				StatusLabel.Text = "Repacking " + partition + "..";
				packThread = new Thread((ThreadStart)delegate
				{
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\boot.PARTITION") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\boot.PARTITION"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\boot.img") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\boot.img"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\boot.fex") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\boot.fex"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\boot_a.fex") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\boot_a.fex"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\boot_a.PARTITION") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\boot_a.PARTITION"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\boot_a.img") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\boot_a.img"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\boot.img") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\boot", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\boot.img"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
				});
				packThread.Start();
				while (packThread.IsAlive)
				{
					Application.DoEvents();
				}
			}
			if (!paid)
			{
				Application.Exit();
			}
			partition = "recovery";
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\recovery"))
			{
				StatusLabel.Text = "Repacking " + partition + "..";
				packThread = new Thread((ThreadStart)delegate
				{
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\recovery.PARTITION") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\recovery", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\recovery.PARTITION"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\recovery.img") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\recovery", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\recovery.img"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\recovery.fex") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\recovery", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\recovery.fex"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\recovery_a.fex") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\recovery", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\recovery_a.fex"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\recovery_a.PARTITION") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\recovery", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\recovery_a.PARTITION"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\recovery_a.img") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\recovery", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\recovery_a.img"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
					if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\recovery.img") && !unpacker.RepackKernel(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\recovery", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super\\recovery.img"))
					{
						LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
					}
				});
				packThread.Start();
				while (packThread.IsAlive)
				{
					Application.DoEvents();
				}
			}
			partition = "system_ext";
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system_ext"))
			{
				Ext4PartitionRepack(partition);
			}
			partition = "odm_ext";
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm_ext"))
			{
				Ext4PartitionRepack(partition);
			}
			partition = "oem";
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\oem"))
			{
				Ext4PartitionRepack(partition);
			}
			if (!paid)
			{
				Environment.Exit(0);
			}
			partition = "odm";
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm"))
			{
				Ext4PartitionRepack(partition);
			}
			partition = "product";
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\product"))
			{
				Ext4PartitionRepack(partition);
			}
			partition = "vendor";
			if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor"))
			{
				Ext4PartitionRepack(partition);
			}
		}
		partition = "system";
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system"))
		{
			Ext4PartitionRepack(partition);
		}
		partition = "super";
		if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super"))
		{
			return;
		}
		StatusLabel.Text = "Repacking " + partition + "..";
		packThread = new Thread((ThreadStart)delegate
		{
			bool flag = false;
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\super.PARTITION") && !unpacker.RepackSuper(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\super.PARTITION"))
			{
				flag = true;
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\super.img") && !unpacker.RepackSuper(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\super.img"))
			{
				flag = true;
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\super.fex") && !unpacker.RepackSuper(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\super", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\super.fex"))
			{
				flag = true;
			}
			if (flag)
			{
				LogInstance.Log(LogLineHead + "IMGUnpacker: FAILED - packing of " + partition);
				MessageBox.Show("Failed to repack Super partition.\nThis could be a system size issue.\n\nThe original will be used.");
			}
		});
		packThread.Start();
		while (packThread.IsAlive)
		{
			Application.DoEvents();
		}
	}

	private void CompressionLevel_Scroll(object sender, EventArgs e)
	{
		if (CompressionLevel.Value <= 3)
		{
			CompressionStatus.Text = CompressionLevel.Value + " - larger package size, faster processing time";
		}
		if (CompressionLevel.Value > 3 && CompressionLevel.Value <= 6)
		{
			CompressionStatus.Text = CompressionLevel.Value + " - medium package size, medium processing time";
		}
		if (CompressionLevel.Value >= 7)
		{
			CompressionStatus.Text = CompressionLevel.Value + " - smaller package size, slower processing time";
		}
	}

	private void ZipRom_Click(object sender, EventArgs e)
	{
		if (!Directory.Exists(bin + "python3"))
		{
			return;
		}
		SaveFileDialog saver = new SaveFileDialog();
		saver.Filter = "flashable zip (*.zip) | *.zip";
		if (saver.ShowDialog() == DialogResult.OK)
		{
			MainTab.SelectedIndex = 0;
			MainTab.Refresh();
			MainTab.Enabled = false;
			UnpackingLoader.Visible = true;
			StatusLabel.Text = "Starting creation of upgrade zip";
			LogInstance.Log(LogLineHead + "Creating zip package " + saver.FileName);
			StandardRepackFunction();
			LogInstance.Log(LogLineHead + "LEVEL 1 ---------------------------------------");
			LogInstance.Log(LogLineHead + "Building upgrade zip..");
			_ = CompressionLevel.Value;
			StatusLabel.Text = "(level1)Repacking upgrade zip..";
			if (unpacker.RepackZipFile(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1", saver.FileName, CompressionLevel.Value.ToString()))
			{
				LogInstance.Log(LogLineHead + "Successfully built upgrade zip");
			}
			else
			{
				LogInstance.Log(LogLineHead + "Failed to build upgrade zip");
				MessageBox.Show("Failed to repack", "ERROR");
			}
			MessageBox.Show("Packing completed", "Complete");
			StatusLabel.Text = "Done.";
			RepackToImage.Enabled = true;
			MainTab.Enabled = true;
			UnpackingLoader.Visible = false;
		}
	}

	private void RepackToImage_Click(object sender, EventArgs e)
	{
		SaveFileDialog saver = new SaveFileDialog();
		saver.Filter = "burn tool image (*.img) | *.img";
		if (saver.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		MainTab.SelectedIndex = 0;
		MainTab.Refresh();
		MainTab.Enabled = false;
		UnpackingLoader.Visible = true;
		StatusLabel.Text = "Starting creation of upgrade package";
		LogInstance.Log(LogLineHead + "Creating upgrade package " + saver.FileName);
		StandardRepackFunction();
		UnpackingLoader.Visible = true;
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\magiskimg"))
		{
			DeleteDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\magiskimg");
		}
		LogInstance.Log(LogLineHead + "LEVEL 1 ---------------------------------------");
		LogInstance.Log(LogLineHead + "Building upgrade package structured img");
		StatusLabel.Text = "(level1)Repacking upgrade package..";
		Thread packThread = new Thread((ThreadStart)delegate
		{
			if (SpecifyChipset.Checked)
			{
				if (!string.IsNullOrWhiteSpace(RKChipset.Text) && RKChipset.Text != null)
				{
					if (RKChipset.Text.Length > 5)
					{
						if (RKChipset.Text.ToUpper().StartsWith("RK"))
						{
							if (unpacker.RepackVendorImage(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1", saver.FileName))
							{
								LogInstance.Log(LogLineHead + "Successfully built upgrade package");
							}
							else
							{
								LogInstance.Log(LogLineHead + "Failed to build upgrade package");
								MessageBox.Show("Failed to repack", "ERROR");
							}
						}
						else
						{
							LogInstance.Log(LogLineHead + "Failed to pack, specified chip does not start with RK prefix");
							MessageBox.Show("Chip does not start with \"RK\"", "Error");
						}
					}
					else
					{
						LogInstance.Log(LogLineHead + "Failed to pack, length of specified chip must be 6 or more chars");
						MessageBox.Show("The length of the chip must be 6 or more chars long", "Error");
					}
				}
				else
				{
					LogInstance.Log(LogLineHead + "Failed to pack, chipset not specified");
					MessageBox.Show("Chipset is not specified.", "ERROR");
				}
			}
			else if (unpacker.RepackVendorImage(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1", saver.FileName))
			{
				LogInstance.Log(LogLineHead + "Successfully built upgrade package");
			}
			else
			{
				LogInstance.Log(LogLineHead + "Failed to build upgrade package");
				MessageBox.Show("Failed to repack", "ERROR");
			}
		});
		packThread.Start();
		while (packThread.IsAlive)
		{
			Application.DoEvents();
		}
		MessageBox.Show("Packing process completed", "Complete");
		StatusLabel.Text = "Done.";
		MainTab.Enabled = true;
		UnpackingLoader.Visible = false;
	}

	private void CTPCompression_Scroll(object sender, EventArgs e)
	{
		if (CTPCompression.Value == 1)
		{
			CTPCompressionStatus.Text = "Fastest compression, fastest but big file";
		}
		if (CTPCompression.Value == 2)
		{
			CTPCompressionStatus.Text = "Fast compression, fast but medium file";
		}
		if (CTPCompression.Value == 3)
		{
			CTPCompressionStatus.Text = "Optimal compression, longer but smaller file";
		}
	}

	private void ExportCTP_Click(object sender, EventArgs e)
	{
		SaveFileDialog saver = new SaveFileDialog();
		saver.Filter = "Customization Tool Project (*.ctp) | *.ctp";
		if (saver.ShowDialog() == DialogResult.OK)
		{
			UnpackingLoader.Visible = true;
			ExportCTP.Enabled = false;
			StatusLabel.Text = "Exporting Customization Tool Project..";
			CompressionLevel ctpCompression = System.IO.Compression.CompressionLevel.Optimal;
			if (CTPCompression.Value == 1)
			{
				ctpCompression = System.IO.Compression.CompressionLevel.NoCompression;
			}
			if (CTPCompression.Value == 2)
			{
				ctpCompression = System.IO.Compression.CompressionLevel.Fastest;
			}
			if (CTPCompression.Value == 3)
			{
				ctpCompression = System.IO.Compression.CompressionLevel.Optimal;
			}
			LogInstance.Log(LogLineHead + "Exporting Customization Tool Project");
			Thread ExportThread = new Thread((ThreadStart)delegate
			{
				ZipFile.CreateFromDirectory(AppDomain.CurrentDomain.BaseDirectory + "tmp", saver.FileName, ctpCompression, includeBaseDirectory: false);
			});
			ExportThread.Start();
			while (ExportThread.IsAlive)
			{
				Application.DoEvents();
			}
			LogInstance.Log(LogLineHead + "Exporting project complete");
			MessageBox.Show("Exporting completed", "Complete");
			StatusLabel.Text = "Done.";
			ExportCTP.Enabled = true;
			UnpackingLoader.Visible = false;
		}
	}

	private void AboutButton_Click(object sender, EventArgs e)
	{
		MessageBox.Show("A new improved build of CustomizationTool built from scratch.\nAndroid 5.x.x > 14.x.x support.\nAmlogic, Rockchip & Allwinner chipset support\n\nThis is the release they could never produce.\nProject by Ricky Divjakovski.", "About Customization Tool");
	}

	private void supportTheProjectToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Process.Start("https://rd-soft.net/customization-tool");
	}

	private void WriteKeys_Click(object sender, EventArgs e)
	{
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\keys.conf"))
		{
			string newKeys = "";
			foreach (string key in KeysChecklist.CheckedItems)
			{
				newKeys = newKeys + key + "\r\n";
			}
			File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level1\\keys.conf", newKeys);
			LogInstance.Log(LogLineHead + "The following keys have been written\n" + newKeys);
			MessageBox.Show("The following keys have been written\n\n" + newKeys, "Operation complete");
		}
		else
		{
			MessageBox.Show("Only supported on AMLogic firmware", "Error");
		}
	}

	private void threadToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Process.Start("https://forum.xda-developers.com/android-stick--console-computers/amlogic/opensource-amlogic-tools-t3786991");
	}

	private void reportBugsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		MessageBox.Show("To report a bug please copy the log of the crash inside the logs file and send it to\nsupport@rd-soft.net");
	}

	private void OpenLogos_Click(object sender, EventArgs e)
	{
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\tmp\\level3\\bootlogo"))
		{
			Process.Start(AppDomain.CurrentDomain.BaseDirectory + "\\tmp\\level3\\bootlogo");
		}
	}

	private void RefreshLogo_Click(object sender, EventArgs e)
	{
		if (BootlogoPicturebox.Image != null)
		{
			BootlogoPicturebox.Image.Dispose();
		}
		BootlogoPicturebox.Image = Resources.blank;
		BootlogoPanel.BringToFront();
		BootlogoPanel.Dock = DockStyle.Fill;
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp"))
		{
			FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp", FileMode.Open, FileAccess.Read);
			BootlogoPicturebox.Image = Image.FromStream(fs);
			fs.Close();
		}
		else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootlogo.bmp"))
		{
			FileStream fs2 = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootlogo.bmp", FileMode.Open, FileAccess.Read);
			BootlogoPicturebox.Image = Image.FromStream(fs2);
			fs2.Close();
		}
		else
		{
			MessageBox.Show("No bootlogo unpacked or detected in firmware. \nDisplaying is currently only supported for amlogic/allwinner devices.\nRepacking is currently only supported for amlogic devices.", "Error of bootlogo display.");
		}
	}

	private void bootlogotimer_Tick(object sender, EventArgs e)
	{
		if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo"))
		{
			return;
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp"))
		{
			if (oldMD5 != CalculateMD5(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp"))
			{
				FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootup.bmp", FileMode.Open, FileAccess.Read);
				BootlogoPicturebox.Image = Image.FromStream(fs);
				fs.Close();
			}
		}
		else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootlogo.bmp") && oldMD5 != CalculateMD5(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootlogo.bmp"))
		{
			FileStream fs2 = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level3\\bootlogo\\bootlogo.bmp", FileMode.Open, FileAccess.Read);
			BootlogoPicturebox.Image = Image.FromStream(fs2);
			fs2.Close();
		}
	}

	private static string CalculateMD5(string filename)
	{
		using MD5 md5 = MD5.Create();
		using FileStream stream = File.OpenRead(filename);
		return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
	}

	private void UpdateProp(string property, string value)
	{
		if (string.IsNullOrWhiteSpace(BuildDisplayID.Text))
		{
			return;
		}
		string buildprops = "";
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "build.prop"))
		{
			buildprops = buildprops + AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + sysdir + "build.prop;";
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\build.prop"))
		{
			buildprops = buildprops + AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor\\build.prop;";
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\product\\build.prop"))
		{
			buildprops = buildprops + AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\product\\build.prop;";
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\oem\\build.prop"))
		{
			buildprops = buildprops + AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\oem\\build.prop;";
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm\\build.prop"))
		{
			buildprops = buildprops + AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\odm\\build.prop;";
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system_ext\\build.prop"))
		{
			buildprops = buildprops + AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\system_ext\\build.prop;";
		}
		string[] array = buildprops.Split(';');
		foreach (string buildprop in array)
		{
			if (!string.IsNullOrWhiteSpace(buildprop))
			{
				string newBuildProp = "";
				string[] array2 = File.ReadAllText(buildprop).Split('\n');
				foreach (string line in array2)
				{
					newBuildProp = ((!line.StartsWith(property + "=")) ? (newBuildProp + line + "\n") : (newBuildProp + property + "=" + value + "\n"));
				}
				try
				{
					File.WriteAllText(buildprop, newBuildProp.Remove(newBuildProp.Length - 1));
				}
				catch
				{
				}
			}
		}
	}

	private void BuildDisplayID_TextChanged(object sender, EventArgs e)
	{
		UpdateProp("ro.build.display.id", BuildDisplayID.Text);
	}

	private void supportToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Process.Start("https://rd-soft.net/contact");
	}

	private void DtbComboBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb") && DtbComboBox.SelectedItem != null && !string.IsNullOrWhiteSpace(DtbComboBox.SelectedItem.ToString()))
		{
			StreamReader DTBReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb\\" + DtbComboBox.SelectedItem.ToString());
			DTBEditor.Text = DTBReader.ReadToEnd();
			DTBReader.Dispose();
			LoadHighlighting();
		}
	}

	private void DiscardDTB_Click(object sender, EventArgs e)
	{
		if (DtbComboBox.SelectedItem != null && !string.IsNullOrWhiteSpace(DtbComboBox.SelectedItem.ToString()))
		{
			StreamReader DTBReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb\\" + DtbComboBox.SelectedItem.ToString());
			DTBEditor.Text = DTBReader.ReadToEnd();
			DTBReader.Dispose();
			LoadHighlighting();
		}
	}

	private void SaveDTB_Click(object sender, EventArgs e)
	{
		if (DtbComboBox.SelectedItem != null && !string.IsNullOrWhiteSpace(DtbComboBox.SelectedItem.ToString()))
		{
			File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb\\" + DtbComboBox.SelectedItem.ToString());
			StreamWriter streamWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\dtb\\" + DtbComboBox.SelectedItem.ToString());
			streamWriter.Write(DTBEditor.Text.Replace("\r", ""));
			streamWriter.Close();
			streamWriter.Dispose();
			LoadHighlighting();
		}
	}

	private void SearchBox_TextChanged(object sender, EventArgs e)
	{
		if (SearchBox.Text.Length > 0)
		{
			SearchIndex = 1;
			int lineNumber = 0;
			string[] array = DTBEditor.Text.Split('\n');
			foreach (string obj in array)
			{
				lineNumber++;
				if (obj.Contains(SearchBox.Text))
				{
					break;
				}
			}
			DTBEditor.HideSelection = false;
			DTBEditor.SelectionStart = DTBEditor.GetFirstCharIndexFromLine(lineNumber - 1);
			DTBEditor.SelectionLength = DTBEditor.Lines[lineNumber - 1].Length;
			DTBEditor.ScrollToCaret();
		}
		else
		{
			LoadHighlighting();
		}
	}

	private void NextOccurance_Click(object sender, EventArgs e)
	{
		SearchIndex++;
		int CurrentIndex = 2;
		int lineNumber = 0;
		string[] array = DTBEditor.Text.Split('\n');
		foreach (string obj in array)
		{
			lineNumber++;
			if (obj.Contains(SearchBox.Text))
			{
				CurrentIndex++;
				if (CurrentIndex == SearchIndex)
				{
					break;
				}
			}
		}
		DTBEditor.HideSelection = false;
		DTBEditor.SelectionStart = DTBEditor.GetFirstCharIndexFromLine(lineNumber - 1);
		DTBEditor.SelectionLength = DTBEditor.Lines[lineNumber - 1].Length;
		DTBEditor.ScrollToCaret();
	}

	private void BackOccurance_Click(object sender, EventArgs e)
	{
		SearchIndex--;
		int CurrentIndex = 1;
		int lineNumber = 0;
		string[] array = DTBEditor.Text.Split('\n');
		foreach (string obj in array)
		{
			lineNumber++;
			if (obj.Contains(SearchBox.Text))
			{
				CurrentIndex++;
				if (CurrentIndex == SearchIndex)
				{
					break;
				}
			}
		}
		DTBEditor.HideSelection = false;
		DTBEditor.SelectionStart = DTBEditor.GetFirstCharIndexFromLine(lineNumber - 1);
		DTBEditor.SelectionLength = DTBEditor.Lines[lineNumber - 1].Length;
		DTBEditor.ScrollToCaret();
	}

	private void LoadHighlighting()
	{
		string keywords = "(.+?=|.+? =)";
		MatchCollection matchCollection = Regex.Matches(DTBEditor.Text, keywords);
		string comments = "(#.+?$|#)";
		MatchCollection commentMatches = Regex.Matches(DTBEditor.Text, comments, RegexOptions.Multiline);
		string strings = ".+?{|}";
		MatchCollection stringMatches = Regex.Matches(DTBEditor.Text, strings);
		string headers = ".+?/;|}";
		MatchCollection headersMatches = Regex.Matches(DTBEditor.Text, headers);
		int originalIndex = DTBEditor.SelectionStart;
		int originalLength = DTBEditor.SelectionLength;
		menuStrip1.Focus();
		DTBEditor.SelectionStart = 0;
		DTBEditor.SelectionLength = DTBEditor.Text.Length;
		DTBEditor.SelectionColor = Color.DimGray;
		foreach (Match m in matchCollection)
		{
			DTBEditor.SelectionStart = m.Index;
			DTBEditor.SelectionLength = m.Length;
			DTBEditor.SelectionColor = SystemColors.WindowText;
		}
		foreach (Match m2 in headersMatches)
		{
			DTBEditor.SelectionStart = m2.Index;
			DTBEditor.SelectionLength = m2.Length;
			DTBEditor.SelectionColor = Color.DarkOrange;
		}
		foreach (Match m3 in stringMatches)
		{
			DTBEditor.SelectionStart = m3.Index;
			DTBEditor.SelectionLength = m3.Length;
			DTBEditor.SelectionColor = Color.DarkOrange;
		}
		foreach (Match m4 in commentMatches)
		{
			DTBEditor.SelectionStart = m4.Index;
			DTBEditor.SelectionLength = m4.Length;
			DTBEditor.SelectionColor = Color.FromArgb(147, 199, 99);
		}
		DTBEditor.SelectionStart = originalIndex;
		DTBEditor.SelectionLength = originalLength;
		DTBEditor.SelectionColor = Color.DimGray;
		DTBEditor.Focus();
	}

	private void PatchComboBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (PatchComboBox.SelectedItem != null && !string.IsNullOrWhiteSpace(PatchComboBox.SelectedItem.ToString()) && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "patches\\" + PatchComboBox.SelectedItem.ToString() + ".patch"))
		{
			PatchAuthor.Text = patcher.PatchAuthor(AppDomain.CurrentDomain.BaseDirectory + "patches\\" + PatchComboBox.SelectedItem.ToString() + ".patch");
			PatchVersion.Text = patcher.PatchVersion(AppDomain.CurrentDomain.BaseDirectory + "patches\\" + PatchComboBox.SelectedItem.ToString() + ".patch");
			PatchDescription.Text = patcher.PatchDescription(AppDomain.CurrentDomain.BaseDirectory + "patches\\" + PatchComboBox.SelectedItem.ToString() + ".patch");
		}
	}

	private void ApplyPatchButton_Click(object sender, EventArgs e)
	{
		if (PatchComboBox.SelectedItem != null && !string.IsNullOrWhiteSpace(PatchComboBox.SelectedItem.ToString()) && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "patches\\" + PatchComboBox.SelectedItem.ToString() + ".patch"))
		{
			patcher.ImageType = ImageType.ToString();
			UnpackingLoader.Visible = true;
			StatusLabel.Text = "Running patch " + PatchComboBox.SelectedItem.ToString();
			if (patcher.RunPatch(AppDomain.CurrentDomain.BaseDirectory + "patches\\" + PatchComboBox.SelectedItem.ToString() + ".patch", AppDomain.CurrentDomain.BaseDirectory + "tmp\\"))
			{
				UnpackingLoader.Visible = false;
				MessageBox.Show("Successfully ran patch " + PatchComboBox.SelectedItem.ToString() + ".", "Patching complete");
			}
			else
			{
				UnpackingLoader.Visible = false;
				MessageBox.Show("Failed to run patch " + PatchComboBox.SelectedItem.ToString() + ".", "Patching failed");
			}
			StatusLabel.Text = "Done.";
		}
	}

	private void AddPatchLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		OpenFileDialog ofd = new OpenFileDialog();
		ofd.Filter = "Patch files (*.patch; | *.patch;";
		if (ofd.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "patches\\" + Path.GetFileName(ofd.FileName)))
		{
			if (MessageBox.Show("Patch " + Path.GetFileName(ofd.FileName) + " already exists, overwrite?", "Patch already exists", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "patches\\" + Path.GetFileName(ofd.FileName));
				File.Copy(ofd.FileName, AppDomain.CurrentDomain.BaseDirectory + "patches\\" + Path.GetFileName(ofd.FileName));
				MessageBox.Show("Patch " + Path.GetFileNameWithoutExtension(ofd.FileName) + " has been installed", "Patch installed");
			}
		}
		else
		{
			File.Copy(ofd.FileName, AppDomain.CurrentDomain.BaseDirectory + "patches\\" + Path.GetFileName(ofd.FileName));
			MessageBox.Show("Patch " + Path.GetFileNameWithoutExtension(ofd.FileName) + " has been installed", "Patch installed");
		}
		PatchComboBox.Items.Clear();
		PatchAuthor.Text = "";
		PatchVersion.Text = "";
		PatchDescription.Text = "";
		PatchRTB.Text = "";
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "patches"))
		{
			if (Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "patches").Count() > 0)
			{
				string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "patches");
				foreach (string patch in files)
				{
					PatchComboBox.Items.Add(Path.GetFileNameWithoutExtension(patch));
				}
			}
			else
			{
				PatchesPanel.Enabled = false;
			}
		}
		else
		{
			PatchesPanel.Enabled = false;
		}
	}

	private void MultiWifiDrivers_Click(object sender, EventArgs e)
	{
		if (ImageType != ImgType.Amlogic)
		{
			return;
		}
		StatusLabel.Text = "Downloading multi-wifi drivers";
		string wifidir = sysdir;
		if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\vendor"))
		{
			wifidir = "vendor\\";
		}
		if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "etc\\wifi\\Multiwifi.mark"))
		{
			UnpackingLoader.Visible = true;
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\drivers.zip"))
			{
				File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\drivers.zip");
			}
			Thread DLThread = new Thread((ThreadStart)delegate
			{
				WebClient obj = new WebClient
				{
					Headers = { { "a", "a" } }
				};
				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
				obj.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
				obj.Proxy = null;
				obj.DownloadFile("https://raw.githubusercontent.com/RickyDivjakovski/CT8.0.0-patch-building/main/PatchResources/MWP", AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\drivers.zip");
			});
			DLThread.Start();
			StatusLabel.Text = "Unpacking multi-wifi drivers";
			while (DLThread.IsAlive)
			{
				Application.DoEvents();
			}
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\drivers.zip"))
			{
				Thread newThread = new Thread((ThreadStart)delegate
				{
					BackgroundShell(bin + "7za", " x -y \"" + AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\drivers.zip\" -o\"" + AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "\"");
				});
				newThread.Start();
				while (newThread.IsAlive)
				{
					Application.DoEvents();
				}
				StreamReader streamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "AppendContexts");
				string appendstr = streamReader.ReadToEnd();
				streamReader.Close();
				streamReader.Dispose();
				StreamReader streamReader2 = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\.config\\" + wifidir.Split('\\').First() + "_file_contexts");
				string contexts = streamReader2.ReadToEnd();
				streamReader2.Close();
				streamReader2.Dispose();
				contexts = contexts + "\n" + appendstr.Replace("$installdir/", wifidir.Replace("\\", "/"));
				StreamWriter streamWriter = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\.config\\" + wifidir.Split('\\').First() + "_file_contexts", append: false);
				streamWriter.WriteLine(contexts);
				streamWriter.Close();
				streamWriter.Dispose();
				StreamReader streamReader3 = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "AppendProps");
				string propappendstr = streamReader3.ReadToEnd();
				streamReader3.Close();
				streamReader3.Dispose();
				StreamReader streamReader4 = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "build.prop");
				string props = streamReader4.ReadToEnd();
				streamReader4.Close();
				streamReader4.Dispose();
				props = props + "\n" + propappendstr;
				StreamWriter streamWriter2 = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "build.prop", append: false);
				streamWriter2.WriteLine(props);
				streamWriter2.Close();
				streamWriter2.Dispose();
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\drivers.zip"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\drivers.zip");
				}
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "AppendContexts"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "AppendContexts");
				}
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "AppendProps"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "AppendProps");
				}
				MessageBox.Show("Multi wifi drivers successfully installed", "Complete");
				UnpackingLoader.Visible = false;
			}
			else
			{
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\drivers.zip"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\drivers.zip");
				}
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "AppendContexts"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "AppendContexts");
				}
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "AppendProps"))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + wifidir + "AppendProps");
				}
				MessageBox.Show("Failed to download drivers", "Error");
			}
		}
		else
		{
			MessageBox.Show("Multi wifi drivers already installed", "Complete");
		}
		StatusLabel.Text = "Done.";
	}

	private void CTVersionLabel_Click(object sender, EventArgs e)
	{
		testmethod();
	}

	private void LocalesComboBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		UpdateProp("ro.product.locale", LocalesComboBox.Text.Split(' ').First());
	}

	private void BuildDisplayID_Click(object sender, EventArgs e)
	{
		BuildDisplayID.SelectAll();
	}

	private void logsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		Process.Start(AppDomain.CurrentDomain.BaseDirectory + "\\Logs");
	}

	private void SpecifyChipset_CheckedChanged(object sender, EventArgs e)
	{
		if (SpecifyChipset.Checked)
		{
			RKChipset.Enabled = true;
		}
		else
		{
			RKChipset.Enabled = false;
		}
	}

	private void MainTab_TabIndexChanged(object sender, EventArgs e)
	{
	}

	private void MainTab_Selected(object sender, TabControlEventArgs e)
	{
		if (MainTab.SelectedTab.Text.Contains("Search") && Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2"))
		{
			redirectOutput = false;
			BackgroundShell("cmd.exe", "/C \"dir /s /b \"" + AppDomain.CurrentDomain.BaseDirectory + "\\tmp\\level2\" > \"" + AppDomain.CurrentDomain.BaseDirectory + "\\tmp\\level2\\.config\\file_list\"\"");
			redirectOutput = true;
		}
	}

	private void SearchTextbox_TextChanged(object sender, EventArgs e)
	{
		FilesList.Items.Clear();
		if (!(SearchTextbox.Text != "") || string.IsNullOrWhiteSpace(SearchTextbox.Text) || SearchTextbox.Text.Length <= 2)
		{
			return;
		}
		StreamReader reader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "\\tmp\\level2\\.config\\file_list");
		string obj = reader.ReadToEnd().Replace(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\", "").Replace("\\", "/")
			.Replace("\r\n", "\n");
		reader.Close();
		reader.Dispose();
		string[] array = obj.Split('\n');
		foreach (string line in array)
		{
			if (!line.Contains(".config/") && line.Contains(SearchTextbox.Text))
			{
				FilesList.Items.Add(line);
			}
		}
	}

	private void FilesList_DoubleClick(object sender, EventArgs e)
	{
		if (FilesList.SelectedItems.Count > 0 && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + FilesList.SelectedItems[0].Text.Replace("/", "\\")))
		{
			string argument = "/select, \"" + AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + FilesList.SelectedItems[0].Text.Replace("/", "\\") + "\"";
			Process.Start("explorer.exe", argument);
		}
	}

	private void FilesList_MouseDown(object sender, MouseEventArgs e)
	{
		if (FilesList.SelectedItems.Count > 0 && e.Button == MouseButtons.Right)
		{
			SearchContextMenu.Show(Cursor.Position);
		}
	}

	private void copyToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (FilesList.SelectedItems.Count <= 0)
		{
			return;
		}
		FolderBrowserDialog fbd = new FolderBrowserDialog();
		fbd.Description = "Select where to copy the files";
		if (fbd.ShowDialog() != DialogResult.OK)
		{
			return;
		}
		if (Directory.Exists(fbd.SelectedPath))
		{
			foreach (ListViewItem file in FilesList.SelectedItems)
			{
				if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + file.Text.Replace("/", "\\")))
				{
					continue;
				}
				if (!Directory.Exists(Path.GetDirectoryName(fbd.SelectedPath + "\\" + file.Text.Replace("/", "\\"))))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(fbd.SelectedPath + "\\" + file.Text.Replace("/", "\\")));
				}
				if (File.Exists(fbd.SelectedPath + "\\" + file.Text.Replace("/", "\\")))
				{
					if (MessageBox.Show("File \"" + file.Text + "\" already exists in copy path.\nOverwrite?", "Caution", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						File.Delete(fbd.SelectedPath + "\\" + file.Text.Replace("/", "\\"));
						File.Copy(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + file.Text.Replace("/", "\\"), fbd.SelectedPath + "\\" + file.Text.Replace("/", "\\"));
					}
				}
				else
				{
					File.Copy(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + file.Text.Replace("/", "\\"), fbd.SelectedPath + "\\" + file.Text.Replace("/", "\\"));
				}
			}
		}
		MessageBox.Show("Copied all files retaining directory structure.", "Success");
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("Are you sure you would like to delete these files?", "Caution", MessageBoxButtons.YesNo) != DialogResult.Yes)
		{
			return;
		}
		foreach (ListViewItem file in FilesList.SelectedItems)
		{
			try
			{
				if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + file.Text.Replace("/", "\\")))
				{
					File.Delete(AppDomain.CurrentDomain.BaseDirectory + "tmp\\level2\\" + file.Text.Replace("/", "\\"));
				}
			}
			catch
			{
			}
		}
		foreach (ListViewItem file2 in FilesList.SelectedItems)
		{
			FilesList.Items.Remove(file2);
		}
		MessageBox.Show("Deleted all selected files.", "Success");
	}

	private void PythonSuperUtils_Click(object sender, EventArgs e)
	{
		if (Settings.Default.PythonSuperUtils)
		{
			Settings.Default.PythonSuperUtils = false;
			PythonSuperUtils.Checked = false;
		}
		else
		{
			Settings.Default.PythonSuperUtils = true;
			PythonSuperUtils.Checked = true;
		}
		Settings.Default.Save();
	}

	private void NewDtbProcessingTools_Click(object sender, EventArgs e)
	{
		if (Settings.Default.NewDtbProcessingTools)
		{
			Settings.Default.NewDtbProcessingTools = false;
			NewDtbProcessingTools.Checked = false;
		}
		else
		{
			Settings.Default.NewDtbProcessingTools = true;
			NewDtbProcessingTools.Checked = true;
		}
		Settings.Default.Save();
	}

	private void NewLogoProcessingTools_Click(object sender, EventArgs e)
	{
		if (Settings.Default.NewLogoProcessingTools)
		{
			Settings.Default.NewLogoProcessingTools = false;
			NewLogoProcessingTools.Checked = false;
		}
		else
		{
			Settings.Default.NewLogoProcessingTools = true;
			NewLogoProcessingTools.Checked = true;
		}
		Settings.Default.Save();
	}

	private void PythonImgExtractor_Click(object sender, EventArgs e)
	{
		if (Settings.Default.PythonImgextractor)
		{
			Settings.Default.PythonImgextractor = false;
			PythonImgExtractor.Checked = false;
		}
		else
		{
			Settings.Default.PythonImgextractor = true;
			PythonImgExtractor.Checked = true;
		}
		Settings.Default.Save();
	}

	private void ArchMake_ext4fs_Click(object sender, EventArgs e)
	{
		if (Settings.Default.ArchMake_ext4fs)
		{
			Settings.Default.ArchMake_ext4fs = false;
			ArchMake_ext4fs.Checked = false;
		}
		else
		{
			Settings.Default.ArchMake_ext4fs = true;
			ArchMake_ext4fs.Checked = true;
		}
		Settings.Default.Save();
	}

	private void PythonSimg2Img_Click(object sender, EventArgs e)
	{
		if (Settings.Default.PythonSimg2Img)
		{
			Settings.Default.PythonSimg2Img = false;
			NewPythonSimg2img.Checked = false;
		}
		else
		{
			Settings.Default.PythonSimg2Img = true;
			NewPythonSimg2img.Checked = true;
		}
		Settings.Default.Save();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomizationTool.Form1));
		this.MainTab = new System.Windows.Forms.TabControl();
		this.tabPage1 = new System.Windows.Forms.TabPage();
		this.groupBox2 = new System.Windows.Forms.GroupBox();
		this.VendorApps = new System.Windows.Forms.TextBox();
		this.label16 = new System.Windows.Forms.Label();
		this.VendorFiles = new System.Windows.Forms.TextBox();
		this.label20 = new System.Windows.Forms.Label();
		this.VendorSize = new System.Windows.Forms.TextBox();
		this.label22 = new System.Windows.Forms.Label();
		this.SystemApps = new System.Windows.Forms.TextBox();
		this.label9 = new System.Windows.Forms.Label();
		this.SystemFiles = new System.Windows.Forms.TextBox();
		this.label8 = new System.Windows.Forms.Label();
		this.SystemSize = new System.Windows.Forms.TextBox();
		this.label7 = new System.Windows.Forms.Label();
		this.groupBox1 = new System.Windows.Forms.GroupBox();
		this.LocalesComboBox = new System.Windows.Forms.ComboBox();
		this.label38 = new System.Windows.Forms.Label();
		this.PackageType = new System.Windows.Forms.Label();
		this.label24 = new System.Windows.Forms.Label();
		this.SuperPartition = new System.Windows.Forms.CheckBox();
		this.label23 = new System.Windows.Forms.Label();
		this.label30 = new System.Windows.Forms.Label();
		this.label29 = new System.Windows.Forms.Label();
		this.label28 = new System.Windows.Forms.Label();
		this.label27 = new System.Windows.Forms.Label();
		this.BootupGunzipped = new System.Windows.Forms.CheckBox();
		this.MultiDtb = new System.Windows.Forms.CheckBox();
		this.ContainsVendor = new System.Windows.Forms.CheckBox();
		this.RamdiskSystem = new System.Windows.Forms.CheckBox();
		this.BuildDisplayID = new System.Windows.Forms.TextBox();
		this.label26 = new System.Windows.Forms.Label();
		this.SDKVersion = new System.Windows.Forms.TextBox();
		this.label25 = new System.Windows.Forms.Label();
		this.Build = new System.Windows.Forms.TextBox();
		this.SecurityPatch = new System.Windows.Forms.TextBox();
		this.Product = new System.Windows.Forms.TextBox();
		this.Vendor = new System.Windows.Forms.TextBox();
		this.Model = new System.Windows.Forms.TextBox();
		this.Version = new System.Windows.Forms.TextBox();
		this.label1 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.label5 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.label4 = new System.Windows.Forms.Label();
		this.tabPage2 = new System.Windows.Forms.TabPage();
		this.BootAnimationPanel = new System.Windows.Forms.Panel();
		this.BootanimationPicturebox = new System.Windows.Forms.PictureBox();
		this.panel4 = new System.Windows.Forms.Panel();
		this.BootanimationChange = new System.Windows.Forms.Button();
		this.PlayBootanimation = new System.Windows.Forms.Button();
		this.BootanimationFPS = new System.Windows.Forms.Label();
		this.BootvideoPanel = new System.Windows.Forms.Panel();
		this.BootVideoPlayer = new AxWMPLib.AxWindowsMediaPlayer();
		this.panel11 = new System.Windows.Forms.Panel();
		this.ChangeBootvideo = new System.Windows.Forms.Button();
		this.PlayBootvideo = new System.Windows.Forms.Button();
		this.panel2 = new System.Windows.Forms.Panel();
		this.BootvideoButton = new System.Windows.Forms.Button();
		this.BootLogoButton = new System.Windows.Forms.Button();
		this.BootanimationButton = new System.Windows.Forms.Button();
		this.BootlogoPanel = new System.Windows.Forms.Panel();
		this.panel10 = new System.Windows.Forms.Panel();
		this.RefreshLogo = new System.Windows.Forms.Button();
		this.OpenLogos = new System.Windows.Forms.Button();
		this.BootlogoPicturebox = new System.Windows.Forms.PictureBox();
		this.tabPage3 = new System.Windows.Forms.TabPage();
		this.AppsViewer = new System.Windows.Forms.ListView();
		this.imageList1 = new System.Windows.Forms.ImageList(this.components);
		this.panel6 = new System.Windows.Forms.Panel();
		this.label10 = new System.Windows.Forms.Label();
		this.AppsDirectory = new System.Windows.Forms.ComboBox();
		this.panel3 = new System.Windows.Forms.Panel();
		this.ApkGroupbox = new System.Windows.Forms.GroupBox();
		this.AppVisibleToTvLauncherLabel = new System.Windows.Forms.Label();
		this.AppVisbleToLauncherLabel = new System.Windows.Forms.Label();
		this.AppLauncherAppLabel = new System.Windows.Forms.Label();
		this.AppPackageLabel = new System.Windows.Forms.Label();
		this.UnpackApps = new System.Windows.Forms.Button();
		this.RepackApps = new System.Windows.Forms.Button();
		this.groupBox3 = new System.Windows.Forms.GroupBox();
		this.AddApps = new System.Windows.Forms.Button();
		this.RemoveApps = new System.Windows.Forms.Button();
		this.tabPage4 = new System.Windows.Forms.TabPage();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.BootImgForMagisk = new System.Windows.Forms.Button();
		this.ReplaceKernel = new System.Windows.Forms.Button();
		this.OpenKernel = new System.Windows.Forms.Button();
		this.label11 = new System.Windows.Forms.Label();
		this.ReplaceRecovery = new System.Windows.Forms.Button();
		this.OpenRecovery = new System.Windows.Forms.Button();
		this.label12 = new System.Windows.Forms.Label();
		this.tabPage5 = new System.Windows.Forms.TabPage();
		this.DTBPanel = new System.Windows.Forms.Panel();
		this.DTBEditor = new System.Windows.Forms.RichTextBox();
		this.panel8 = new System.Windows.Forms.Panel();
		this.panel9 = new System.Windows.Forms.Panel();
		this.label31 = new System.Windows.Forms.Label();
		this.BackOccurance = new System.Windows.Forms.Button();
		this.SearchBox = new System.Windows.Forms.TextBox();
		this.NextOccurance = new System.Windows.Forms.Button();
		this.DiscardDTB = new System.Windows.Forms.Button();
		this.SaveDTB = new System.Windows.Forms.Button();
		this.label32 = new System.Windows.Forms.Label();
		this.DtbComboBox = new System.Windows.Forms.ComboBox();
		this.PackPage = new System.Windows.Forms.TabPage();
		this.OpenWorking = new System.Windows.Forms.Button();
		this.VendorGroupBox = new System.Windows.Forms.GroupBox();
		this.OpenVendor = new System.Windows.Forms.Button();
		this.OpenVendorBuildProp = new System.Windows.Forms.Button();
		this.groupBox14 = new System.Windows.Forms.GroupBox();
		this.OpenVendorRemoteConf = new System.Windows.Forms.Button();
		this.OpenVendorKeymap = new System.Windows.Forms.Button();
		this.OpenVendorGenericKl = new System.Windows.Forms.Button();
		this.groupBox9 = new System.Windows.Forms.GroupBox();
		this.KeysChecklist = new System.Windows.Forms.CheckedListBox();
		this.WriteKeys = new System.Windows.Forms.Button();
		this.SystemGroupBox = new System.Windows.Forms.GroupBox();
		this.WifiGroupBox = new System.Windows.Forms.GroupBox();
		this.MultiWifiDrivers = new System.Windows.Forms.Button();
		this.RemoveRoot = new System.Windows.Forms.Button();
		this.RootFirmware = new System.Windows.Forms.Button();
		this.OpenSystem = new System.Windows.Forms.Button();
		this.OpenSystemBuildProp = new System.Windows.Forms.Button();
		this.groupBox4 = new System.Windows.Forms.GroupBox();
		this.OpenSystemRemoteConf = new System.Windows.Forms.Button();
		this.OpenSystemKeymap = new System.Windows.Forms.Button();
		this.OpenSystemGenericKl = new System.Windows.Forms.Button();
		this.tabPage6 = new System.Windows.Forms.TabPage();
		this.FilesList = new System.Windows.Forms.ListView();
		this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
		this.panel5 = new System.Windows.Forms.Panel();
		this.label39 = new System.Windows.Forms.Label();
		this.SearchTextbox = new System.Windows.Forms.TextBox();
		this.tabPage7 = new System.Windows.Forms.TabPage();
		this.PatchesPanel = new System.Windows.Forms.Panel();
		this.PatchDescription = new System.Windows.Forms.Label();
		this.PatchVersion = new System.Windows.Forms.Label();
		this.PatchAuthor = new System.Windows.Forms.Label();
		this.label37 = new System.Windows.Forms.Label();
		this.label36 = new System.Windows.Forms.Label();
		this.label35 = new System.Windows.Forms.Label();
		this.label34 = new System.Windows.Forms.Label();
		this.PatchRTB = new System.Windows.Forms.RichTextBox();
		this.AddPatchLabel = new System.Windows.Forms.LinkLabel();
		this.ApplyPatchButton = new System.Windows.Forms.Button();
		this.label33 = new System.Windows.Forms.Label();
		this.PatchComboBox = new System.Windows.Forms.ComboBox();
		this.tabPage8 = new System.Windows.Forms.TabPage();
		this.groupBox8 = new System.Windows.Forms.GroupBox();
		this.CTPCompressionStatus = new System.Windows.Forms.Label();
		this.CTPCompression = new System.Windows.Forms.TrackBar();
		this.label21 = new System.Windows.Forms.Label();
		this.ExportCTP = new System.Windows.Forms.Button();
		this.label19 = new System.Windows.Forms.Label();
		this.groupBox7 = new System.Windows.Forms.GroupBox();
		this.RKChipset = new System.Windows.Forms.TextBox();
		this.SpecifyChipset = new System.Windows.Forms.CheckBox();
		this.AddPlatformConfig = new System.Windows.Forms.CheckBox();
		this.label15 = new System.Windows.Forms.Label();
		this.label14 = new System.Windows.Forms.Label();
		this.RepackToImage = new System.Windows.Forms.Button();
		this.groupBox6 = new System.Windows.Forms.GroupBox();
		this.CompressBrotli = new System.Windows.Forms.CheckBox();
		this.CompressionStatus = new System.Windows.Forms.Label();
		this.CompressionLevel = new System.Windows.Forms.TrackBar();
		this.label18 = new System.Windows.Forms.Label();
		this.ZipRom = new System.Windows.Forms.Button();
		this.label13 = new System.Windows.Forms.Label();
		this.panel1 = new System.Windows.Forms.Panel();
		this.CTVersionLabel = new System.Windows.Forms.Label();
		this.UnpackingLoader = new System.Windows.Forms.PictureBox();
		this.UpgradePackage = new System.Windows.Forms.TextBox();
		this.UnpackImage = new System.Windows.Forms.Button();
		this.menuStrip1 = new System.Windows.Forms.MenuStrip();
		this.AboutButton = new System.Windows.Forms.ToolStripMenuItem();
		this.tutorialsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.HelpButton = new System.Windows.Forms.ToolStripMenuItem();
		this.threadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.reportBugsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.supportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.logsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.experimentalFeaturesToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.PythonSuperUtils = new System.Windows.Forms.ToolStripMenuItem();
		this.NewDtbProcessingTools = new System.Windows.Forms.ToolStripMenuItem();
		this.NewLogoProcessingTools = new System.Windows.Forms.ToolStripMenuItem();
		this.PythonImgExtractor = new System.Windows.Forms.ToolStripMenuItem();
		this.ArchMake_ext4fs = new System.Windows.Forms.ToolStripMenuItem();
		this.NewPythonSimg2img = new System.Windows.Forms.ToolStripMenuItem();
		this.supportTheProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.panel7 = new System.Windows.Forms.Panel();
		this.UserLabel = new System.Windows.Forms.Label();
		this.StatusLabel = new System.Windows.Forms.Label();
		this.label17 = new System.Windows.Forms.Label();
		this.bootlogotimer = new System.Windows.Forms.Timer(this.components);
		this.SearchContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.MainTab.SuspendLayout();
		this.tabPage1.SuspendLayout();
		this.groupBox2.SuspendLayout();
		this.groupBox1.SuspendLayout();
		this.tabPage2.SuspendLayout();
		this.BootAnimationPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.BootanimationPicturebox).BeginInit();
		this.panel4.SuspendLayout();
		this.BootvideoPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.BootVideoPlayer).BeginInit();
		this.panel11.SuspendLayout();
		this.panel2.SuspendLayout();
		this.BootlogoPanel.SuspendLayout();
		this.panel10.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.BootlogoPicturebox).BeginInit();
		this.tabPage3.SuspendLayout();
		this.panel6.SuspendLayout();
		this.panel3.SuspendLayout();
		this.ApkGroupbox.SuspendLayout();
		this.groupBox3.SuspendLayout();
		this.tabPage4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		this.tabPage5.SuspendLayout();
		this.DTBPanel.SuspendLayout();
		this.panel8.SuspendLayout();
		this.panel9.SuspendLayout();
		this.PackPage.SuspendLayout();
		this.VendorGroupBox.SuspendLayout();
		this.groupBox14.SuspendLayout();
		this.groupBox9.SuspendLayout();
		this.SystemGroupBox.SuspendLayout();
		this.WifiGroupBox.SuspendLayout();
		this.groupBox4.SuspendLayout();
		this.tabPage6.SuspendLayout();
		this.panel5.SuspendLayout();
		this.tabPage7.SuspendLayout();
		this.PatchesPanel.SuspendLayout();
		this.tabPage8.SuspendLayout();
		this.groupBox8.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.CTPCompression).BeginInit();
		this.groupBox7.SuspendLayout();
		this.groupBox6.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.CompressionLevel).BeginInit();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.UnpackingLoader).BeginInit();
		this.menuStrip1.SuspendLayout();
		this.panel7.SuspendLayout();
		this.SearchContextMenu.SuspendLayout();
		base.SuspendLayout();
		this.MainTab.Controls.Add(this.tabPage1);
		this.MainTab.Controls.Add(this.tabPage2);
		this.MainTab.Controls.Add(this.tabPage3);
		this.MainTab.Controls.Add(this.tabPage4);
		this.MainTab.Controls.Add(this.tabPage5);
		this.MainTab.Controls.Add(this.PackPage);
		this.MainTab.Controls.Add(this.tabPage6);
		this.MainTab.Controls.Add(this.tabPage7);
		this.MainTab.Controls.Add(this.tabPage8);
		this.MainTab.Dock = System.Windows.Forms.DockStyle.Fill;
		this.MainTab.Enabled = false;
		this.MainTab.Location = new System.Drawing.Point(0, 73);
		this.MainTab.Name = "MainTab";
		this.MainTab.SelectedIndex = 0;
		this.MainTab.Size = new System.Drawing.Size(840, 494);
		this.MainTab.TabIndex = 2;
		this.MainTab.Selected += new System.Windows.Forms.TabControlEventHandler(MainTab_Selected);
		this.tabPage1.Controls.Add(this.groupBox2);
		this.tabPage1.Controls.Add(this.groupBox1);
		this.tabPage1.Location = new System.Drawing.Point(4, 22);
		this.tabPage1.Name = "tabPage1";
		this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage1.Size = new System.Drawing.Size(832, 468);
		this.tabPage1.TabIndex = 0;
		this.tabPage1.Text = "Product Info";
		this.tabPage1.UseVisualStyleBackColor = true;
		this.groupBox2.Controls.Add(this.VendorApps);
		this.groupBox2.Controls.Add(this.label16);
		this.groupBox2.Controls.Add(this.VendorFiles);
		this.groupBox2.Controls.Add(this.label20);
		this.groupBox2.Controls.Add(this.VendorSize);
		this.groupBox2.Controls.Add(this.label22);
		this.groupBox2.Controls.Add(this.SystemApps);
		this.groupBox2.Controls.Add(this.label9);
		this.groupBox2.Controls.Add(this.SystemFiles);
		this.groupBox2.Controls.Add(this.label8);
		this.groupBox2.Controls.Add(this.SystemSize);
		this.groupBox2.Controls.Add(this.label7);
		this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.groupBox2.Location = new System.Drawing.Point(3, 313);
		this.groupBox2.Name = "groupBox2";
		this.groupBox2.Size = new System.Drawing.Size(826, 152);
		this.groupBox2.TabIndex = 7;
		this.groupBox2.TabStop = false;
		this.groupBox2.Text = "Other properties";
		this.VendorApps.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.VendorApps.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.VendorApps.Location = new System.Drawing.Point(604, 102);
		this.VendorApps.Name = "VendorApps";
		this.VendorApps.ReadOnly = true;
		this.VendorApps.Size = new System.Drawing.Size(191, 20);
		this.VendorApps.TabIndex = 18;
		this.label16.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label16.AutoSize = true;
		this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label16.Location = new System.Drawing.Point(504, 105);
		this.label16.Name = "label16";
		this.label16.Size = new System.Drawing.Size(67, 13);
		this.label16.TabIndex = 17;
		this.label16.Text = "Vendor apps";
		this.VendorFiles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.VendorFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.VendorFiles.Location = new System.Drawing.Point(604, 73);
		this.VendorFiles.Name = "VendorFiles";
		this.VendorFiles.ReadOnly = true;
		this.VendorFiles.Size = new System.Drawing.Size(191, 20);
		this.VendorFiles.TabIndex = 16;
		this.label20.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label20.AutoSize = true;
		this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label20.Location = new System.Drawing.Point(504, 76);
		this.label20.Name = "label20";
		this.label20.Size = new System.Drawing.Size(62, 13);
		this.label20.TabIndex = 15;
		this.label20.Text = "Vendor files";
		this.VendorSize.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.VendorSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.VendorSize.Location = new System.Drawing.Point(604, 44);
		this.VendorSize.Name = "VendorSize";
		this.VendorSize.ReadOnly = true;
		this.VendorSize.Size = new System.Drawing.Size(191, 20);
		this.VendorSize.TabIndex = 14;
		this.label22.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label22.AutoSize = true;
		this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label22.Location = new System.Drawing.Point(504, 47);
		this.label22.Name = "label22";
		this.label22.Size = new System.Drawing.Size(87, 13);
		this.label22.TabIndex = 13;
		this.label22.Text = "Vendor size (MB)";
		this.SystemApps.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.SystemApps.Location = new System.Drawing.Point(103, 101);
		this.SystemApps.Name = "SystemApps";
		this.SystemApps.ReadOnly = true;
		this.SystemApps.Size = new System.Drawing.Size(191, 20);
		this.SystemApps.TabIndex = 12;
		this.label9.AutoSize = true;
		this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label9.Location = new System.Drawing.Point(6, 104);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(67, 13);
		this.label9.TabIndex = 11;
		this.label9.Text = "System apps";
		this.SystemFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.SystemFiles.Location = new System.Drawing.Point(103, 72);
		this.SystemFiles.Name = "SystemFiles";
		this.SystemFiles.ReadOnly = true;
		this.SystemFiles.Size = new System.Drawing.Size(191, 20);
		this.SystemFiles.TabIndex = 10;
		this.label8.AutoSize = true;
		this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label8.Location = new System.Drawing.Point(6, 75);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(62, 13);
		this.label8.TabIndex = 9;
		this.label8.Text = "System files";
		this.SystemSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.SystemSize.Location = new System.Drawing.Point(103, 43);
		this.SystemSize.Name = "SystemSize";
		this.SystemSize.ReadOnly = true;
		this.SystemSize.Size = new System.Drawing.Size(191, 20);
		this.SystemSize.TabIndex = 8;
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label7.Location = new System.Drawing.Point(6, 46);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(87, 13);
		this.label7.TabIndex = 7;
		this.label7.Text = "System size (MB)";
		this.groupBox1.Controls.Add(this.LocalesComboBox);
		this.groupBox1.Controls.Add(this.label38);
		this.groupBox1.Controls.Add(this.PackageType);
		this.groupBox1.Controls.Add(this.label24);
		this.groupBox1.Controls.Add(this.SuperPartition);
		this.groupBox1.Controls.Add(this.label23);
		this.groupBox1.Controls.Add(this.label30);
		this.groupBox1.Controls.Add(this.label29);
		this.groupBox1.Controls.Add(this.label28);
		this.groupBox1.Controls.Add(this.label27);
		this.groupBox1.Controls.Add(this.BootupGunzipped);
		this.groupBox1.Controls.Add(this.MultiDtb);
		this.groupBox1.Controls.Add(this.ContainsVendor);
		this.groupBox1.Controls.Add(this.RamdiskSystem);
		this.groupBox1.Controls.Add(this.BuildDisplayID);
		this.groupBox1.Controls.Add(this.label26);
		this.groupBox1.Controls.Add(this.SDKVersion);
		this.groupBox1.Controls.Add(this.label25);
		this.groupBox1.Controls.Add(this.Build);
		this.groupBox1.Controls.Add(this.SecurityPatch);
		this.groupBox1.Controls.Add(this.Product);
		this.groupBox1.Controls.Add(this.Vendor);
		this.groupBox1.Controls.Add(this.Model);
		this.groupBox1.Controls.Add(this.Version);
		this.groupBox1.Controls.Add(this.label1);
		this.groupBox1.Controls.Add(this.label6);
		this.groupBox1.Controls.Add(this.label2);
		this.groupBox1.Controls.Add(this.label5);
		this.groupBox1.Controls.Add(this.label3);
		this.groupBox1.Controls.Add(this.label4);
		this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
		this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.groupBox1.Location = new System.Drawing.Point(3, 3);
		this.groupBox1.Name = "groupBox1";
		this.groupBox1.Size = new System.Drawing.Size(826, 310);
		this.groupBox1.TabIndex = 6;
		this.groupBox1.TabStop = false;
		this.groupBox1.Text = "System properties";
		this.LocalesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.LocalesComboBox.FormattingEnabled = true;
		this.LocalesComboBox.Location = new System.Drawing.Point(104, 272);
		this.LocalesComboBox.Name = "LocalesComboBox";
		this.LocalesComboBox.Size = new System.Drawing.Size(267, 21);
		this.LocalesComboBox.TabIndex = 30;
		this.LocalesComboBox.SelectedIndexChanged += new System.EventHandler(LocalesComboBox_SelectedIndexChanged);
		this.label38.AutoSize = true;
		this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label38.Location = new System.Drawing.Point(8, 275);
		this.label38.Name = "label38";
		this.label38.Size = new System.Drawing.Size(72, 13);
		this.label38.TabIndex = 29;
		this.label38.Text = "Default locale";
		this.PackageType.AutoSize = true;
		this.PackageType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.PackageType.ForeColor = System.Drawing.Color.Black;
		this.PackageType.Location = new System.Drawing.Point(100, 26);
		this.PackageType.Name = "PackageType";
		this.PackageType.Size = new System.Drawing.Size(0, 13);
		this.PackageType.TabIndex = 28;
		this.label24.AutoSize = true;
		this.label24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label24.Location = new System.Drawing.Point(7, 26);
		this.label24.Name = "label24";
		this.label24.Size = new System.Drawing.Size(73, 13);
		this.label24.TabIndex = 27;
		this.label24.Text = "Package type";
		this.SuperPartition.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.SuperPartition.AutoSize = true;
		this.SuperPartition.Enabled = false;
		this.SuperPartition.Location = new System.Drawing.Point(801, 274);
		this.SuperPartition.Name = "SuperPartition";
		this.SuperPartition.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.SuperPartition.Size = new System.Drawing.Size(15, 14);
		this.SuperPartition.TabIndex = 26;
		this.SuperPartition.UseVisualStyleBackColor = true;
		this.label23.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label23.AutoSize = true;
		this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label23.Location = new System.Drawing.Point(720, 275);
		this.label23.Name = "label23";
		this.label23.Size = new System.Drawing.Size(75, 13);
		this.label23.TabIndex = 25;
		this.label23.Text = "Super partition";
		this.label30.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label30.AutoSize = true;
		this.label30.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label30.Location = new System.Drawing.Point(711, 190);
		this.label30.Name = "label30";
		this.label30.Size = new System.Drawing.Size(84, 13);
		this.label30.TabIndex = 24;
		this.label30.Text = "Contains vendor";
		this.label29.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label29.AutoSize = true;
		this.label29.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label29.Location = new System.Drawing.Point(713, 219);
		this.label29.Name = "label29";
		this.label29.Size = new System.Drawing.Size(82, 13);
		this.label29.TabIndex = 23;
		this.label29.Text = "Multiple dtb files";
		this.label28.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label28.AutoSize = true;
		this.label28.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label28.Location = new System.Drawing.Point(678, 248);
		this.label28.Name = "label28";
		this.label28.Size = new System.Drawing.Size(117, 13);
		this.label28.TabIndex = 22;
		this.label28.Text = "Gunzipped bootup.bmp";
		this.label27.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label27.AutoSize = true;
		this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label27.Location = new System.Drawing.Point(668, 161);
		this.label27.Name = "label27";
		this.label27.Size = new System.Drawing.Size(127, 13);
		this.label27.TabIndex = 21;
		this.label27.Text = "Ramdisk in level2/system";
		this.BootupGunzipped.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.BootupGunzipped.AutoSize = true;
		this.BootupGunzipped.Enabled = false;
		this.BootupGunzipped.Location = new System.Drawing.Point(801, 248);
		this.BootupGunzipped.Name = "BootupGunzipped";
		this.BootupGunzipped.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.BootupGunzipped.Size = new System.Drawing.Size(15, 14);
		this.BootupGunzipped.TabIndex = 20;
		this.BootupGunzipped.UseVisualStyleBackColor = true;
		this.MultiDtb.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.MultiDtb.AutoSize = true;
		this.MultiDtb.Enabled = false;
		this.MultiDtb.Location = new System.Drawing.Point(801, 219);
		this.MultiDtb.Name = "MultiDtb";
		this.MultiDtb.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.MultiDtb.Size = new System.Drawing.Size(15, 14);
		this.MultiDtb.TabIndex = 19;
		this.MultiDtb.UseVisualStyleBackColor = true;
		this.ContainsVendor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.ContainsVendor.AutoSize = true;
		this.ContainsVendor.Enabled = false;
		this.ContainsVendor.Location = new System.Drawing.Point(801, 190);
		this.ContainsVendor.Name = "ContainsVendor";
		this.ContainsVendor.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.ContainsVendor.Size = new System.Drawing.Size(15, 14);
		this.ContainsVendor.TabIndex = 18;
		this.ContainsVendor.UseVisualStyleBackColor = true;
		this.RamdiskSystem.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.RamdiskSystem.AutoSize = true;
		this.RamdiskSystem.Enabled = false;
		this.RamdiskSystem.Location = new System.Drawing.Point(801, 161);
		this.RamdiskSystem.Name = "RamdiskSystem";
		this.RamdiskSystem.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.RamdiskSystem.Size = new System.Drawing.Size(15, 14);
		this.RamdiskSystem.TabIndex = 17;
		this.RamdiskSystem.UseVisualStyleBackColor = true;
		this.BuildDisplayID.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.BuildDisplayID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.BuildDisplayID.Location = new System.Drawing.Point(640, 23);
		this.BuildDisplayID.Name = "BuildDisplayID";
		this.BuildDisplayID.Size = new System.Drawing.Size(181, 20);
		this.BuildDisplayID.TabIndex = 16;
		this.BuildDisplayID.Click += new System.EventHandler(BuildDisplayID_Click);
		this.BuildDisplayID.TextChanged += new System.EventHandler(BuildDisplayID_TextChanged);
		this.label26.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label26.AutoSize = true;
		this.label26.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label26.Location = new System.Drawing.Point(553, 26);
		this.label26.Name = "label26";
		this.label26.Size = new System.Drawing.Size(81, 13);
		this.label26.TabIndex = 15;
		this.label26.Text = "Build Display ID";
		this.SDKVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.SDKVersion.Location = new System.Drawing.Point(103, 185);
		this.SDKVersion.Name = "SDKVersion";
		this.SDKVersion.ReadOnly = true;
		this.SDKVersion.Size = new System.Drawing.Size(268, 20);
		this.SDKVersion.TabIndex = 14;
		this.label25.AutoSize = true;
		this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label25.Location = new System.Drawing.Point(7, 188);
		this.label25.Name = "label25";
		this.label25.Size = new System.Drawing.Size(67, 13);
		this.label25.TabIndex = 13;
		this.label25.Text = "SDK Version";
		this.Build.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.Build.Location = new System.Drawing.Point(103, 243);
		this.Build.Name = "Build";
		this.Build.ReadOnly = true;
		this.Build.Size = new System.Drawing.Size(268, 20);
		this.Build.TabIndex = 11;
		this.SecurityPatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.SecurityPatch.Location = new System.Drawing.Point(103, 214);
		this.SecurityPatch.Name = "SecurityPatch";
		this.SecurityPatch.ReadOnly = true;
		this.SecurityPatch.Size = new System.Drawing.Size(268, 20);
		this.SecurityPatch.TabIndex = 10;
		this.Product.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.Product.Location = new System.Drawing.Point(102, 157);
		this.Product.Name = "Product";
		this.Product.ReadOnly = true;
		this.Product.Size = new System.Drawing.Size(268, 20);
		this.Product.TabIndex = 9;
		this.Vendor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.Vendor.Location = new System.Drawing.Point(102, 128);
		this.Vendor.Name = "Vendor";
		this.Vendor.ReadOnly = true;
		this.Vendor.Size = new System.Drawing.Size(268, 20);
		this.Vendor.TabIndex = 8;
		this.Model.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.Model.Location = new System.Drawing.Point(102, 99);
		this.Model.Name = "Model";
		this.Model.ReadOnly = true;
		this.Model.Size = new System.Drawing.Size(268, 20);
		this.Model.TabIndex = 7;
		this.Version.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.Version.Location = new System.Drawing.Point(102, 70);
		this.Version.Name = "Version";
		this.Version.ReadOnly = true;
		this.Version.Size = new System.Drawing.Size(268, 20);
		this.Version.TabIndex = 6;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label1.Location = new System.Drawing.Point(6, 73);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(42, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "Version";
		this.label6.AutoSize = true;
		this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label6.Location = new System.Drawing.Point(7, 246);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(30, 13);
		this.label6.TabIndex = 5;
		this.label6.Text = "Build";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label2.Location = new System.Drawing.Point(6, 102);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(36, 13);
		this.label2.TabIndex = 1;
		this.label2.Text = "Model";
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label5.Location = new System.Drawing.Point(7, 217);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(75, 13);
		this.label5.TabIndex = 4;
		this.label5.Text = "Security patch";
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label3.Location = new System.Drawing.Point(6, 131);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(41, 13);
		this.label3.TabIndex = 2;
		this.label3.Text = "Vendor";
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label4.Location = new System.Drawing.Point(6, 160);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(44, 13);
		this.label4.TabIndex = 3;
		this.label4.Text = "Product";
		this.tabPage2.Controls.Add(this.BootAnimationPanel);
		this.tabPage2.Controls.Add(this.BootvideoPanel);
		this.tabPage2.Controls.Add(this.panel2);
		this.tabPage2.Controls.Add(this.BootlogoPanel);
		this.tabPage2.Location = new System.Drawing.Point(4, 22);
		this.tabPage2.Name = "tabPage2";
		this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage2.Size = new System.Drawing.Size(832, 468);
		this.tabPage2.TabIndex = 1;
		this.tabPage2.Text = "User Interface";
		this.tabPage2.UseVisualStyleBackColor = true;
		this.BootAnimationPanel.Controls.Add(this.BootanimationPicturebox);
		this.BootAnimationPanel.Controls.Add(this.panel4);
		this.BootAnimationPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.BootAnimationPanel.Location = new System.Drawing.Point(3, 27);
		this.BootAnimationPanel.Name = "BootAnimationPanel";
		this.BootAnimationPanel.Size = new System.Drawing.Size(826, 438);
		this.BootAnimationPanel.TabIndex = 1;
		this.BootanimationPicturebox.BackColor = System.Drawing.Color.Black;
		this.BootanimationPicturebox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.BootanimationPicturebox.Location = new System.Drawing.Point(0, 0);
		this.BootanimationPicturebox.Name = "BootanimationPicturebox";
		this.BootanimationPicturebox.Size = new System.Drawing.Size(826, 414);
		this.BootanimationPicturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.BootanimationPicturebox.TabIndex = 4;
		this.BootanimationPicturebox.TabStop = false;
		this.panel4.Controls.Add(this.BootanimationChange);
		this.panel4.Controls.Add(this.PlayBootanimation);
		this.panel4.Controls.Add(this.BootanimationFPS);
		this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel4.Location = new System.Drawing.Point(0, 414);
		this.panel4.Name = "panel4";
		this.panel4.Size = new System.Drawing.Size(826, 24);
		this.panel4.TabIndex = 3;
		this.BootanimationChange.Location = new System.Drawing.Point(91, 2);
		this.BootanimationChange.Name = "BootanimationChange";
		this.BootanimationChange.Size = new System.Drawing.Size(85, 23);
		this.BootanimationChange.TabIndex = 7;
		this.BootanimationChange.Text = "Change";
		this.BootanimationChange.UseVisualStyleBackColor = true;
		this.BootanimationChange.Click += new System.EventHandler(BootanimationChange_Click);
		this.PlayBootanimation.Location = new System.Drawing.Point(0, 2);
		this.PlayBootanimation.Name = "PlayBootanimation";
		this.PlayBootanimation.Size = new System.Drawing.Size(85, 23);
		this.PlayBootanimation.TabIndex = 8;
		this.PlayBootanimation.Text = "Play";
		this.PlayBootanimation.UseVisualStyleBackColor = true;
		this.PlayBootanimation.Click += new System.EventHandler(PlayBootanimation_Click);
		this.BootanimationFPS.Dock = System.Windows.Forms.DockStyle.Right;
		this.BootanimationFPS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.BootanimationFPS.Location = new System.Drawing.Point(626, 0);
		this.BootanimationFPS.Name = "BootanimationFPS";
		this.BootanimationFPS.Size = new System.Drawing.Size(200, 24);
		this.BootanimationFPS.TabIndex = 9;
		this.BootanimationFPS.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
		this.BootvideoPanel.Controls.Add(this.BootVideoPlayer);
		this.BootvideoPanel.Controls.Add(this.panel11);
		this.BootvideoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.BootvideoPanel.Location = new System.Drawing.Point(3, 27);
		this.BootvideoPanel.Name = "BootvideoPanel";
		this.BootvideoPanel.Size = new System.Drawing.Size(826, 438);
		this.BootvideoPanel.TabIndex = 6;
		this.BootVideoPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.BootVideoPlayer.Enabled = true;
		this.BootVideoPlayer.Location = new System.Drawing.Point(0, 0);
		this.BootVideoPlayer.Name = "BootVideoPlayer";
		this.BootVideoPlayer.OcxState = (System.Windows.Forms.AxHost.State)resources.GetObject("BootVideoPlayer.OcxState");
		this.BootVideoPlayer.Size = new System.Drawing.Size(826, 414);
		this.BootVideoPlayer.TabIndex = 8;
		this.panel11.Controls.Add(this.ChangeBootvideo);
		this.panel11.Controls.Add(this.PlayBootvideo);
		this.panel11.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel11.Location = new System.Drawing.Point(0, 414);
		this.panel11.Name = "panel11";
		this.panel11.Size = new System.Drawing.Size(826, 24);
		this.panel11.TabIndex = 7;
		this.ChangeBootvideo.Location = new System.Drawing.Point(91, 2);
		this.ChangeBootvideo.Name = "ChangeBootvideo";
		this.ChangeBootvideo.Size = new System.Drawing.Size(85, 23);
		this.ChangeBootvideo.TabIndex = 7;
		this.ChangeBootvideo.Text = "Change";
		this.ChangeBootvideo.UseVisualStyleBackColor = true;
		this.ChangeBootvideo.Click += new System.EventHandler(ChangeBootvideo_Click);
		this.PlayBootvideo.Location = new System.Drawing.Point(0, 2);
		this.PlayBootvideo.Name = "PlayBootvideo";
		this.PlayBootvideo.Size = new System.Drawing.Size(85, 23);
		this.PlayBootvideo.TabIndex = 8;
		this.PlayBootvideo.Text = "Play";
		this.PlayBootvideo.UseVisualStyleBackColor = true;
		this.PlayBootvideo.Click += new System.EventHandler(PlayBootvideo_Click);
		this.panel2.Controls.Add(this.BootvideoButton);
		this.panel2.Controls.Add(this.BootLogoButton);
		this.panel2.Controls.Add(this.BootanimationButton);
		this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel2.Location = new System.Drawing.Point(3, 3);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(826, 24);
		this.panel2.TabIndex = 0;
		this.BootvideoButton.Location = new System.Drawing.Point(91, 0);
		this.BootvideoButton.Name = "BootvideoButton";
		this.BootvideoButton.Size = new System.Drawing.Size(85, 23);
		this.BootvideoButton.TabIndex = 10;
		this.BootvideoButton.Text = "Bootvideo";
		this.BootvideoButton.UseVisualStyleBackColor = true;
		this.BootvideoButton.Click += new System.EventHandler(BootvideoButton_Click);
		this.BootLogoButton.Location = new System.Drawing.Point(182, 0);
		this.BootLogoButton.Name = "BootLogoButton";
		this.BootLogoButton.Size = new System.Drawing.Size(85, 23);
		this.BootLogoButton.TabIndex = 9;
		this.BootLogoButton.Text = "Boot logo";
		this.BootLogoButton.UseVisualStyleBackColor = true;
		this.BootLogoButton.Click += new System.EventHandler(BootLogoButton_Click);
		this.BootanimationButton.BackColor = System.Drawing.Color.SkyBlue;
		this.BootanimationButton.Location = new System.Drawing.Point(0, 0);
		this.BootanimationButton.Name = "BootanimationButton";
		this.BootanimationButton.Size = new System.Drawing.Size(85, 23);
		this.BootanimationButton.TabIndex = 7;
		this.BootanimationButton.Text = "Bootanimation";
		this.BootanimationButton.UseVisualStyleBackColor = false;
		this.BootanimationButton.Click += new System.EventHandler(BootanimationButton_Click);
		this.BootlogoPanel.Controls.Add(this.panel10);
		this.BootlogoPanel.Controls.Add(this.BootlogoPicturebox);
		this.BootlogoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.BootlogoPanel.Location = new System.Drawing.Point(3, 3);
		this.BootlogoPanel.Name = "BootlogoPanel";
		this.BootlogoPanel.Size = new System.Drawing.Size(826, 462);
		this.BootlogoPanel.TabIndex = 5;
		this.panel10.Controls.Add(this.RefreshLogo);
		this.panel10.Controls.Add(this.OpenLogos);
		this.panel10.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel10.Location = new System.Drawing.Point(0, 438);
		this.panel10.Name = "panel10";
		this.panel10.Size = new System.Drawing.Size(826, 24);
		this.panel10.TabIndex = 5;
		this.RefreshLogo.Location = new System.Drawing.Point(83, 1);
		this.RefreshLogo.Name = "RefreshLogo";
		this.RefreshLogo.Size = new System.Drawing.Size(75, 23);
		this.RefreshLogo.TabIndex = 6;
		this.RefreshLogo.Text = "Refresh";
		this.RefreshLogo.UseVisualStyleBackColor = true;
		this.RefreshLogo.Click += new System.EventHandler(RefreshLogo_Click);
		this.OpenLogos.Location = new System.Drawing.Point(2, 1);
		this.OpenLogos.Name = "OpenLogos";
		this.OpenLogos.Size = new System.Drawing.Size(75, 23);
		this.OpenLogos.TabIndex = 0;
		this.OpenLogos.Text = "Open logo's";
		this.OpenLogos.UseVisualStyleBackColor = true;
		this.OpenLogos.Click += new System.EventHandler(OpenLogos_Click);
		this.BootlogoPicturebox.BackColor = System.Drawing.Color.Transparent;
		this.BootlogoPicturebox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.BootlogoPicturebox.Location = new System.Drawing.Point(0, 0);
		this.BootlogoPicturebox.Name = "BootlogoPicturebox";
		this.BootlogoPicturebox.Size = new System.Drawing.Size(826, 462);
		this.BootlogoPicturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
		this.BootlogoPicturebox.TabIndex = 4;
		this.BootlogoPicturebox.TabStop = false;
		this.tabPage3.Controls.Add(this.AppsViewer);
		this.tabPage3.Controls.Add(this.panel6);
		this.tabPage3.Controls.Add(this.panel3);
		this.tabPage3.Location = new System.Drawing.Point(4, 22);
		this.tabPage3.Name = "tabPage3";
		this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage3.Size = new System.Drawing.Size(832, 468);
		this.tabPage3.TabIndex = 2;
		this.tabPage3.Text = "Applications";
		this.tabPage3.UseVisualStyleBackColor = true;
		this.AppsViewer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.AppsViewer.HideSelection = false;
		this.AppsViewer.LargeImageList = this.imageList1;
		this.AppsViewer.Location = new System.Drawing.Point(3, 34);
		this.AppsViewer.Name = "AppsViewer";
		this.AppsViewer.Size = new System.Drawing.Size(606, 431);
		this.AppsViewer.TabIndex = 2;
		this.AppsViewer.UseCompatibleStateImageBehavior = false;
		this.AppsViewer.SelectedIndexChanged += new System.EventHandler(AppsViewer_SelectedIndexChanged);
		this.AppsViewer.DoubleClick += new System.EventHandler(AppsViewer_DoubleClick);
		this.imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageList1.ImageStream");
		this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
		this.imageList1.Images.SetKeyName(0, "Default_applicationIcon.png");
		this.panel6.Controls.Add(this.label10);
		this.panel6.Controls.Add(this.AppsDirectory);
		this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel6.Location = new System.Drawing.Point(3, 3);
		this.panel6.Name = "panel6";
		this.panel6.Size = new System.Drawing.Size(606, 31);
		this.panel6.TabIndex = 1;
		this.label10.AutoSize = true;
		this.label10.Location = new System.Drawing.Point(5, 8);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(93, 13);
		this.label10.TabIndex = 1;
		this.label10.Text = "Show apps inside:";
		this.AppsDirectory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.AppsDirectory.FormattingEnabled = true;
		this.AppsDirectory.Location = new System.Drawing.Point(104, 5);
		this.AppsDirectory.Name = "AppsDirectory";
		this.AppsDirectory.Size = new System.Drawing.Size(496, 21);
		this.AppsDirectory.TabIndex = 0;
		this.AppsDirectory.SelectedIndexChanged += new System.EventHandler(AppsDirectory_SelectedIndexChanged);
		this.panel3.Controls.Add(this.ApkGroupbox);
		this.panel3.Controls.Add(this.groupBox3);
		this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
		this.panel3.Location = new System.Drawing.Point(609, 3);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(220, 462);
		this.panel3.TabIndex = 0;
		this.ApkGroupbox.Controls.Add(this.AppVisibleToTvLauncherLabel);
		this.ApkGroupbox.Controls.Add(this.AppVisbleToLauncherLabel);
		this.ApkGroupbox.Controls.Add(this.AppLauncherAppLabel);
		this.ApkGroupbox.Controls.Add(this.AppPackageLabel);
		this.ApkGroupbox.Controls.Add(this.UnpackApps);
		this.ApkGroupbox.Controls.Add(this.RepackApps);
		this.ApkGroupbox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ApkGroupbox.Location = new System.Drawing.Point(0, 59);
		this.ApkGroupbox.Name = "ApkGroupbox";
		this.ApkGroupbox.Size = new System.Drawing.Size(220, 403);
		this.ApkGroupbox.TabIndex = 4;
		this.ApkGroupbox.TabStop = false;
		this.ApkGroupbox.Text = "APK Modification";
		this.AppVisibleToTvLauncherLabel.AutoSize = true;
		this.AppVisibleToTvLauncherLabel.Location = new System.Drawing.Point(6, 93);
		this.AppVisibleToTvLauncherLabel.Name = "AppVisibleToTvLauncherLabel";
		this.AppVisibleToTvLauncherLabel.Size = new System.Drawing.Size(0, 13);
		this.AppVisibleToTvLauncherLabel.TabIndex = 7;
		this.AppVisbleToLauncherLabel.AutoSize = true;
		this.AppVisbleToLauncherLabel.Location = new System.Drawing.Point(6, 75);
		this.AppVisbleToLauncherLabel.Name = "AppVisbleToLauncherLabel";
		this.AppVisbleToLauncherLabel.Size = new System.Drawing.Size(0, 13);
		this.AppVisbleToLauncherLabel.TabIndex = 6;
		this.AppLauncherAppLabel.AutoSize = true;
		this.AppLauncherAppLabel.Location = new System.Drawing.Point(6, 56);
		this.AppLauncherAppLabel.Name = "AppLauncherAppLabel";
		this.AppLauncherAppLabel.Size = new System.Drawing.Size(0, 13);
		this.AppLauncherAppLabel.TabIndex = 5;
		this.AppPackageLabel.AutoSize = true;
		this.AppPackageLabel.Location = new System.Drawing.Point(6, 385);
		this.AppPackageLabel.Name = "AppPackageLabel";
		this.AppPackageLabel.Size = new System.Drawing.Size(0, 13);
		this.AppPackageLabel.TabIndex = 4;
		this.UnpackApps.Enabled = false;
		this.UnpackApps.Location = new System.Drawing.Point(6, 19);
		this.UnpackApps.Name = "UnpackApps";
		this.UnpackApps.Size = new System.Drawing.Size(102, 23);
		this.UnpackApps.TabIndex = 2;
		this.UnpackApps.Text = "Unpack";
		this.UnpackApps.UseVisualStyleBackColor = true;
		this.UnpackApps.Click += new System.EventHandler(UnpackApps_Click);
		this.RepackApps.Enabled = false;
		this.RepackApps.Location = new System.Drawing.Point(112, 19);
		this.RepackApps.Name = "RepackApps";
		this.RepackApps.Size = new System.Drawing.Size(102, 23);
		this.RepackApps.TabIndex = 3;
		this.RepackApps.Text = "Repack";
		this.RepackApps.UseVisualStyleBackColor = true;
		this.RepackApps.Click += new System.EventHandler(RepackApps_Click);
		this.groupBox3.Controls.Add(this.AddApps);
		this.groupBox3.Controls.Add(this.RemoveApps);
		this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
		this.groupBox3.Location = new System.Drawing.Point(0, 0);
		this.groupBox3.Name = "groupBox3";
		this.groupBox3.Size = new System.Drawing.Size(220, 59);
		this.groupBox3.TabIndex = 5;
		this.groupBox3.TabStop = false;
		this.groupBox3.Text = "Add/Remove apps";
		this.AddApps.Location = new System.Drawing.Point(6, 19);
		this.AddApps.Name = "AddApps";
		this.AddApps.Size = new System.Drawing.Size(102, 23);
		this.AddApps.TabIndex = 1;
		this.AddApps.Text = "Add";
		this.AddApps.UseVisualStyleBackColor = true;
		this.AddApps.Click += new System.EventHandler(AddApps_Click);
		this.RemoveApps.Location = new System.Drawing.Point(112, 19);
		this.RemoveApps.Name = "RemoveApps";
		this.RemoveApps.Size = new System.Drawing.Size(102, 23);
		this.RemoveApps.TabIndex = 0;
		this.RemoveApps.Text = "Remove";
		this.RemoveApps.UseVisualStyleBackColor = true;
		this.RemoveApps.Click += new System.EventHandler(RemoveApps_Click);
		this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
		this.tabPage4.Controls.Add(this.splitContainer1);
		this.tabPage4.Location = new System.Drawing.Point(4, 22);
		this.tabPage4.Name = "tabPage4";
		this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage4.Size = new System.Drawing.Size(832, 468);
		this.tabPage4.TabIndex = 3;
		this.tabPage4.Text = "Kernel/Recovery";
		this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainer1.Location = new System.Drawing.Point(3, 3);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Window;
		this.splitContainer1.Panel1.Controls.Add(this.BootImgForMagisk);
		this.splitContainer1.Panel1.Controls.Add(this.ReplaceKernel);
		this.splitContainer1.Panel1.Controls.Add(this.OpenKernel);
		this.splitContainer1.Panel1.Controls.Add(this.label11);
		this.splitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Window;
		this.splitContainer1.Panel2.Controls.Add(this.ReplaceRecovery);
		this.splitContainer1.Panel2.Controls.Add(this.OpenRecovery);
		this.splitContainer1.Panel2.Controls.Add(this.label12);
		this.splitContainer1.Size = new System.Drawing.Size(826, 462);
		this.splitContainer1.SplitterDistance = 413;
		this.splitContainer1.SplitterWidth = 1;
		this.splitContainer1.TabIndex = 0;
		this.BootImgForMagisk.Location = new System.Drawing.Point(117, 60);
		this.BootImgForMagisk.Name = "BootImgForMagisk";
		this.BootImgForMagisk.Size = new System.Drawing.Size(109, 23);
		this.BootImgForMagisk.TabIndex = 3;
		this.BootImgForMagisk.Text = "Boot.img for Magisk";
		this.BootImgForMagisk.UseVisualStyleBackColor = true;
		this.BootImgForMagisk.Click += new System.EventHandler(BootImgForMagisk_Click);
		this.ReplaceKernel.Location = new System.Drawing.Point(5, 60);
		this.ReplaceKernel.Name = "ReplaceKernel";
		this.ReplaceKernel.Size = new System.Drawing.Size(106, 23);
		this.ReplaceKernel.TabIndex = 2;
		this.ReplaceKernel.Text = "Replace kernel";
		this.ReplaceKernel.UseVisualStyleBackColor = true;
		this.ReplaceKernel.Click += new System.EventHandler(ReplaceKernel_Click);
		this.OpenKernel.Location = new System.Drawing.Point(5, 31);
		this.OpenKernel.Name = "OpenKernel";
		this.OpenKernel.Size = new System.Drawing.Size(405, 23);
		this.OpenKernel.TabIndex = 1;
		this.OpenKernel.Text = "Open kernel ramdisk";
		this.OpenKernel.UseVisualStyleBackColor = true;
		this.OpenKernel.Click += new System.EventHandler(OpenKernel_Click);
		this.label11.Dock = System.Windows.Forms.DockStyle.Top;
		this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.label11.Location = new System.Drawing.Point(0, 0);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(413, 17);
		this.label11.TabIndex = 0;
		this.label11.Text = "Kernel";
		this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.ReplaceRecovery.Location = new System.Drawing.Point(3, 60);
		this.ReplaceRecovery.Name = "ReplaceRecovery";
		this.ReplaceRecovery.Size = new System.Drawing.Size(106, 23);
		this.ReplaceRecovery.TabIndex = 6;
		this.ReplaceRecovery.Text = "Replace recovery";
		this.ReplaceRecovery.UseVisualStyleBackColor = true;
		this.ReplaceRecovery.Click += new System.EventHandler(ReplaceRecovery_Click);
		this.OpenRecovery.Location = new System.Drawing.Point(3, 31);
		this.OpenRecovery.Name = "OpenRecovery";
		this.OpenRecovery.Size = new System.Drawing.Size(404, 23);
		this.OpenRecovery.TabIndex = 5;
		this.OpenRecovery.Text = "Open recovery ramdisk";
		this.OpenRecovery.UseVisualStyleBackColor = true;
		this.OpenRecovery.Click += new System.EventHandler(OpenRecovery_Click);
		this.label12.Dock = System.Windows.Forms.DockStyle.Top;
		this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.label12.Location = new System.Drawing.Point(0, 0);
		this.label12.Name = "label12";
		this.label12.Size = new System.Drawing.Size(412, 17);
		this.label12.TabIndex = 1;
		this.label12.Text = "Recovery";
		this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
		this.tabPage5.Controls.Add(this.DTBPanel);
		this.tabPage5.Location = new System.Drawing.Point(4, 22);
		this.tabPage5.Name = "tabPage5";
		this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage5.Size = new System.Drawing.Size(832, 468);
		this.tabPage5.TabIndex = 4;
		this.tabPage5.Text = "DTB Editor";
		this.tabPage5.UseVisualStyleBackColor = true;
		this.DTBPanel.Controls.Add(this.DTBEditor);
		this.DTBPanel.Controls.Add(this.panel8);
		this.DTBPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.DTBPanel.Location = new System.Drawing.Point(3, 3);
		this.DTBPanel.Name = "DTBPanel";
		this.DTBPanel.Size = new System.Drawing.Size(826, 462);
		this.DTBPanel.TabIndex = 3;
		this.DTBEditor.BackColor = System.Drawing.SystemColors.Window;
		this.DTBEditor.Dock = System.Windows.Forms.DockStyle.Fill;
		this.DTBEditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.DTBEditor.ForeColor = System.Drawing.SystemColors.WindowText;
		this.DTBEditor.Location = new System.Drawing.Point(0, 31);
		this.DTBEditor.Name = "DTBEditor";
		this.DTBEditor.Size = new System.Drawing.Size(826, 431);
		this.DTBEditor.TabIndex = 4;
		this.DTBEditor.Text = "";
		this.DTBEditor.WordWrap = false;
		this.panel8.Controls.Add(this.panel9);
		this.panel8.Controls.Add(this.DiscardDTB);
		this.panel8.Controls.Add(this.SaveDTB);
		this.panel8.Controls.Add(this.label32);
		this.panel8.Controls.Add(this.DtbComboBox);
		this.panel8.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel8.Location = new System.Drawing.Point(0, 0);
		this.panel8.Name = "panel8";
		this.panel8.Size = new System.Drawing.Size(826, 31);
		this.panel8.TabIndex = 3;
		this.panel9.Controls.Add(this.label31);
		this.panel9.Controls.Add(this.BackOccurance);
		this.panel9.Controls.Add(this.SearchBox);
		this.panel9.Controls.Add(this.NextOccurance);
		this.panel9.Dock = System.Windows.Forms.DockStyle.Right;
		this.panel9.Location = new System.Drawing.Point(541, 0);
		this.panel9.Name = "panel9";
		this.panel9.Size = new System.Drawing.Size(285, 31);
		this.panel9.TabIndex = 2;
		this.label31.AutoSize = true;
		this.label31.Location = new System.Drawing.Point(4, 8);
		this.label31.Name = "label31";
		this.label31.Size = new System.Drawing.Size(27, 13);
		this.label31.TabIndex = 5;
		this.label31.Text = "Find";
		this.BackOccurance.Location = new System.Drawing.Point(171, 4);
		this.BackOccurance.Name = "BackOccurance";
		this.BackOccurance.Size = new System.Drawing.Size(51, 23);
		this.BackOccurance.TabIndex = 7;
		this.BackOccurance.Text = "Back";
		this.BackOccurance.UseVisualStyleBackColor = true;
		this.BackOccurance.Click += new System.EventHandler(BackOccurance_Click);
		this.SearchBox.Location = new System.Drawing.Point(37, 5);
		this.SearchBox.Name = "SearchBox";
		this.SearchBox.Size = new System.Drawing.Size(128, 20);
		this.SearchBox.TabIndex = 4;
		this.SearchBox.Click += new System.EventHandler(SearchBox_TextChanged);
		this.NextOccurance.Location = new System.Drawing.Point(228, 4);
		this.NextOccurance.Name = "NextOccurance";
		this.NextOccurance.Size = new System.Drawing.Size(51, 23);
		this.NextOccurance.TabIndex = 6;
		this.NextOccurance.Text = "Next";
		this.NextOccurance.UseVisualStyleBackColor = true;
		this.NextOccurance.Click += new System.EventHandler(NextOccurance_Click);
		this.DiscardDTB.Location = new System.Drawing.Point(357, 4);
		this.DiscardDTB.Name = "DiscardDTB";
		this.DiscardDTB.Size = new System.Drawing.Size(97, 23);
		this.DiscardDTB.TabIndex = 3;
		this.DiscardDTB.Text = "Discard changes";
		this.DiscardDTB.UseVisualStyleBackColor = true;
		this.DiscardDTB.Click += new System.EventHandler(DiscardDTB_Click);
		this.SaveDTB.Location = new System.Drawing.Point(264, 4);
		this.SaveDTB.Name = "SaveDTB";
		this.SaveDTB.Size = new System.Drawing.Size(87, 23);
		this.SaveDTB.TabIndex = 2;
		this.SaveDTB.Text = "Save changes";
		this.SaveDTB.UseVisualStyleBackColor = true;
		this.SaveDTB.Click += new System.EventHandler(SaveDTB_Click);
		this.label32.AutoSize = true;
		this.label32.Location = new System.Drawing.Point(6, 8);
		this.label32.Name = "label32";
		this.label32.Size = new System.Drawing.Size(74, 13);
		this.label32.TabIndex = 1;
		this.label32.Text = "Selected DTB";
		this.DtbComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.DtbComboBox.FormattingEnabled = true;
		this.DtbComboBox.Location = new System.Drawing.Point(86, 5);
		this.DtbComboBox.Name = "DtbComboBox";
		this.DtbComboBox.Size = new System.Drawing.Size(160, 21);
		this.DtbComboBox.TabIndex = 0;
		this.DtbComboBox.SelectedIndexChanged += new System.EventHandler(DtbComboBox_SelectedIndexChanged);
		this.PackPage.Controls.Add(this.OpenWorking);
		this.PackPage.Controls.Add(this.VendorGroupBox);
		this.PackPage.Controls.Add(this.groupBox9);
		this.PackPage.Controls.Add(this.SystemGroupBox);
		this.PackPage.Location = new System.Drawing.Point(4, 22);
		this.PackPage.Name = "PackPage";
		this.PackPage.Padding = new System.Windows.Forms.Padding(3);
		this.PackPage.Size = new System.Drawing.Size(832, 468);
		this.PackPage.TabIndex = 6;
		this.PackPage.Text = "Advanced";
		this.PackPage.UseVisualStyleBackColor = true;
		this.OpenWorking.Location = new System.Drawing.Point(6, 439);
		this.OpenWorking.Name = "OpenWorking";
		this.OpenWorking.Size = new System.Drawing.Size(147, 23);
		this.OpenWorking.TabIndex = 4;
		this.OpenWorking.Text = "Open work folder";
		this.OpenWorking.UseVisualStyleBackColor = true;
		this.OpenWorking.Click += new System.EventHandler(OpenWorking_Click);
		this.VendorGroupBox.Controls.Add(this.OpenVendor);
		this.VendorGroupBox.Controls.Add(this.OpenVendorBuildProp);
		this.VendorGroupBox.Controls.Add(this.groupBox14);
		this.VendorGroupBox.Location = new System.Drawing.Point(190, 6);
		this.VendorGroupBox.Name = "VendorGroupBox";
		this.VendorGroupBox.Size = new System.Drawing.Size(173, 181);
		this.VendorGroupBox.TabIndex = 14;
		this.VendorGroupBox.TabStop = false;
		this.VendorGroupBox.Text = " Vendor";
		this.OpenVendor.Location = new System.Drawing.Point(6, 19);
		this.OpenVendor.Name = "OpenVendor";
		this.OpenVendor.Size = new System.Drawing.Size(147, 23);
		this.OpenVendor.TabIndex = 0;
		this.OpenVendor.Text = "Open vendor folder";
		this.OpenVendor.UseVisualStyleBackColor = true;
		this.OpenVendor.Click += new System.EventHandler(OpenVendor_Click);
		this.OpenVendorBuildProp.Location = new System.Drawing.Point(6, 48);
		this.OpenVendorBuildProp.Name = "OpenVendorBuildProp";
		this.OpenVendorBuildProp.Size = new System.Drawing.Size(147, 23);
		this.OpenVendorBuildProp.TabIndex = 1;
		this.OpenVendorBuildProp.Text = "Open build.prop";
		this.OpenVendorBuildProp.UseVisualStyleBackColor = true;
		this.OpenVendorBuildProp.Click += new System.EventHandler(OpenVendorBuildProp_Click);
		this.groupBox14.Controls.Add(this.OpenVendorRemoteConf);
		this.groupBox14.Controls.Add(this.OpenVendorKeymap);
		this.groupBox14.Controls.Add(this.OpenVendorGenericKl);
		this.groupBox14.Location = new System.Drawing.Point(0, 77);
		this.groupBox14.Name = "groupBox14";
		this.groupBox14.Size = new System.Drawing.Size(200, 104);
		this.groupBox14.TabIndex = 7;
		this.groupBox14.TabStop = false;
		this.groupBox14.Text = "Keyboard + Remote";
		this.OpenVendorRemoteConf.Location = new System.Drawing.Point(6, 77);
		this.OpenVendorRemoteConf.Name = "OpenVendorRemoteConf";
		this.OpenVendorRemoteConf.Size = new System.Drawing.Size(147, 23);
		this.OpenVendorRemoteConf.TabIndex = 2;
		this.OpenVendorRemoteConf.Text = "Open remote config";
		this.OpenVendorRemoteConf.UseVisualStyleBackColor = true;
		this.OpenVendorRemoteConf.Click += new System.EventHandler(OpenVendorRemoteConf_Click);
		this.OpenVendorKeymap.Location = new System.Drawing.Point(6, 19);
		this.OpenVendorKeymap.Name = "OpenVendorKeymap";
		this.OpenVendorKeymap.Size = new System.Drawing.Size(147, 23);
		this.OpenVendorKeymap.TabIndex = 2;
		this.OpenVendorKeymap.Text = "Open keyboard map folder";
		this.OpenVendorKeymap.UseVisualStyleBackColor = true;
		this.OpenVendorKeymap.Click += new System.EventHandler(OpenVendorKeymap_Click);
		this.OpenVendorGenericKl.Location = new System.Drawing.Point(6, 48);
		this.OpenVendorGenericKl.Name = "OpenVendorGenericKl";
		this.OpenVendorGenericKl.Size = new System.Drawing.Size(147, 23);
		this.OpenVendorGenericKl.TabIndex = 3;
		this.OpenVendorGenericKl.Text = "Open Generic.kl (keymap)";
		this.OpenVendorGenericKl.UseVisualStyleBackColor = true;
		this.OpenVendorGenericKl.Click += new System.EventHandler(OpenVendorGenericKl_Click);
		this.groupBox9.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.groupBox9.Controls.Add(this.KeysChecklist);
		this.groupBox9.Controls.Add(this.WriteKeys);
		this.groupBox9.Location = new System.Drawing.Point(567, 6);
		this.groupBox9.Name = "groupBox9";
		this.groupBox9.Size = new System.Drawing.Size(259, 456);
		this.groupBox9.TabIndex = 13;
		this.groupBox9.TabStop = false;
		this.groupBox9.Text = "Keys";
		this.KeysChecklist.Dock = System.Windows.Forms.DockStyle.Fill;
		this.KeysChecklist.FormattingEnabled = true;
		this.KeysChecklist.Items.AddRange(new object[19]
		{
			"mac", "mac_bt", "mac_wifi", "usid", "hdcp", "hdcp2", "hdcp2_tx", "hdcp2_rx", "secure_boot_set", "widevinekeybox",
			"deviceid", "hdcp22_fw_private", "hdcp22_rx_private", "hdcp22_rx_fw", "playreadykeybox", "hdcp14_rx", "prpubkeybox", "prprivkeybox", "attestationkeybox"
		});
		this.KeysChecklist.Location = new System.Drawing.Point(3, 16);
		this.KeysChecklist.Name = "KeysChecklist";
		this.KeysChecklist.Size = new System.Drawing.Size(253, 414);
		this.KeysChecklist.TabIndex = 15;
		this.WriteKeys.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.WriteKeys.Location = new System.Drawing.Point(3, 430);
		this.WriteKeys.Name = "WriteKeys";
		this.WriteKeys.Size = new System.Drawing.Size(253, 23);
		this.WriteKeys.TabIndex = 10;
		this.WriteKeys.Text = "Write keys";
		this.WriteKeys.UseVisualStyleBackColor = true;
		this.WriteKeys.Click += new System.EventHandler(WriteKeys_Click);
		this.SystemGroupBox.Controls.Add(this.WifiGroupBox);
		this.SystemGroupBox.Controls.Add(this.RemoveRoot);
		this.SystemGroupBox.Controls.Add(this.RootFirmware);
		this.SystemGroupBox.Controls.Add(this.OpenSystem);
		this.SystemGroupBox.Controls.Add(this.OpenSystemBuildProp);
		this.SystemGroupBox.Controls.Add(this.groupBox4);
		this.SystemGroupBox.Location = new System.Drawing.Point(6, 6);
		this.SystemGroupBox.Name = "SystemGroupBox";
		this.SystemGroupBox.Size = new System.Drawing.Size(172, 286);
		this.SystemGroupBox.TabIndex = 11;
		this.SystemGroupBox.TabStop = false;
		this.SystemGroupBox.Text = "System";
		this.WifiGroupBox.Controls.Add(this.MultiWifiDrivers);
		this.WifiGroupBox.Enabled = false;
		this.WifiGroupBox.Location = new System.Drawing.Point(0, 239);
		this.WifiGroupBox.Name = "WifiGroupBox";
		this.WifiGroupBox.Size = new System.Drawing.Size(256, 47);
		this.WifiGroupBox.TabIndex = 15;
		this.WifiGroupBox.TabStop = false;
		this.WifiGroupBox.Text = "Amlogic Wifi (paid feature)";
		this.MultiWifiDrivers.Location = new System.Drawing.Point(6, 19);
		this.MultiWifiDrivers.Name = "MultiWifiDrivers";
		this.MultiWifiDrivers.Size = new System.Drawing.Size(147, 23);
		this.MultiWifiDrivers.TabIndex = 2;
		this.MultiWifiDrivers.Text = "Add multi wifi drivers";
		this.MultiWifiDrivers.UseVisualStyleBackColor = true;
		this.MultiWifiDrivers.Click += new System.EventHandler(MultiWifiDrivers_Click);
		this.RemoveRoot.Location = new System.Drawing.Point(6, 46);
		this.RemoveRoot.Name = "RemoveRoot";
		this.RemoveRoot.Size = new System.Drawing.Size(147, 23);
		this.RemoveRoot.TabIndex = 11;
		this.RemoveRoot.Text = "Remove root (SuperSU)";
		this.RemoveRoot.UseVisualStyleBackColor = true;
		this.RemoveRoot.Click += new System.EventHandler(RemoveRoot_Click);
		this.RootFirmware.Location = new System.Drawing.Point(6, 17);
		this.RootFirmware.Name = "RootFirmware";
		this.RootFirmware.Size = new System.Drawing.Size(147, 23);
		this.RootFirmware.TabIndex = 10;
		this.RootFirmware.Text = "Root firmware (SuperSU)";
		this.RootFirmware.UseVisualStyleBackColor = true;
		this.RootFirmware.Click += new System.EventHandler(RootFirmware_Click);
		this.OpenSystem.Location = new System.Drawing.Point(6, 75);
		this.OpenSystem.Name = "OpenSystem";
		this.OpenSystem.Size = new System.Drawing.Size(147, 23);
		this.OpenSystem.TabIndex = 0;
		this.OpenSystem.Text = "Open system folder";
		this.OpenSystem.UseVisualStyleBackColor = true;
		this.OpenSystem.Click += new System.EventHandler(OpenSystem_Click);
		this.OpenSystemBuildProp.Location = new System.Drawing.Point(6, 104);
		this.OpenSystemBuildProp.Name = "OpenSystemBuildProp";
		this.OpenSystemBuildProp.Size = new System.Drawing.Size(147, 23);
		this.OpenSystemBuildProp.TabIndex = 1;
		this.OpenSystemBuildProp.Text = "Open build.prop";
		this.OpenSystemBuildProp.UseVisualStyleBackColor = true;
		this.OpenSystemBuildProp.Click += new System.EventHandler(OpenBuildProp_Click);
		this.groupBox4.Controls.Add(this.OpenSystemRemoteConf);
		this.groupBox4.Controls.Add(this.OpenSystemKeymap);
		this.groupBox4.Controls.Add(this.OpenSystemGenericKl);
		this.groupBox4.Location = new System.Drawing.Point(0, 133);
		this.groupBox4.Name = "groupBox4";
		this.groupBox4.Size = new System.Drawing.Size(172, 100);
		this.groupBox4.TabIndex = 7;
		this.groupBox4.TabStop = false;
		this.groupBox4.Text = "Keyboard + Remote";
		this.OpenSystemRemoteConf.Location = new System.Drawing.Point(6, 77);
		this.OpenSystemRemoteConf.Name = "OpenSystemRemoteConf";
		this.OpenSystemRemoteConf.Size = new System.Drawing.Size(147, 23);
		this.OpenSystemRemoteConf.TabIndex = 2;
		this.OpenSystemRemoteConf.Text = "Open remote config";
		this.OpenSystemRemoteConf.UseVisualStyleBackColor = true;
		this.OpenSystemRemoteConf.Click += new System.EventHandler(OpenRemoteConf_Click);
		this.OpenSystemKeymap.Location = new System.Drawing.Point(6, 19);
		this.OpenSystemKeymap.Name = "OpenSystemKeymap";
		this.OpenSystemKeymap.Size = new System.Drawing.Size(147, 23);
		this.OpenSystemKeymap.TabIndex = 2;
		this.OpenSystemKeymap.Text = "Open keyboard map folder";
		this.OpenSystemKeymap.UseVisualStyleBackColor = true;
		this.OpenSystemKeymap.Click += new System.EventHandler(OpenKeymap_Click);
		this.OpenSystemGenericKl.Location = new System.Drawing.Point(6, 48);
		this.OpenSystemGenericKl.Name = "OpenSystemGenericKl";
		this.OpenSystemGenericKl.Size = new System.Drawing.Size(147, 23);
		this.OpenSystemGenericKl.TabIndex = 3;
		this.OpenSystemGenericKl.Text = "Open Generic.kl (keymap)";
		this.OpenSystemGenericKl.UseVisualStyleBackColor = true;
		this.OpenSystemGenericKl.Click += new System.EventHandler(OpenGenericKl_Click);
		this.tabPage6.Controls.Add(this.FilesList);
		this.tabPage6.Controls.Add(this.panel5);
		this.tabPage6.Location = new System.Drawing.Point(4, 22);
		this.tabPage6.Name = "tabPage6";
		this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage6.Size = new System.Drawing.Size(832, 468);
		this.tabPage6.TabIndex = 7;
		this.tabPage6.Text = "Search";
		this.tabPage6.UseVisualStyleBackColor = true;
		this.FilesList.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.FilesList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[1] { this.columnHeader1 });
		this.FilesList.Dock = System.Windows.Forms.DockStyle.Fill;
		this.FilesList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
		this.FilesList.HideSelection = false;
		this.FilesList.LabelWrap = false;
		this.FilesList.Location = new System.Drawing.Point(3, 30);
		this.FilesList.Name = "FilesList";
		this.FilesList.Size = new System.Drawing.Size(826, 435);
		this.FilesList.TabIndex = 1;
		this.FilesList.UseCompatibleStateImageBehavior = false;
		this.FilesList.View = System.Windows.Forms.View.Details;
		this.FilesList.DoubleClick += new System.EventHandler(FilesList_DoubleClick);
		this.FilesList.MouseDown += new System.Windows.Forms.MouseEventHandler(FilesList_MouseDown);
		this.columnHeader1.Text = "File";
		this.columnHeader1.Width = 500;
		this.panel5.Controls.Add(this.label39);
		this.panel5.Controls.Add(this.SearchTextbox);
		this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel5.Location = new System.Drawing.Point(3, 3);
		this.panel5.Name = "panel5";
		this.panel5.Size = new System.Drawing.Size(826, 27);
		this.panel5.TabIndex = 0;
		this.label39.AutoSize = true;
		this.label39.Location = new System.Drawing.Point(3, 7);
		this.label39.Name = "label39";
		this.label39.Size = new System.Drawing.Size(47, 13);
		this.label39.TabIndex = 1;
		this.label39.Text = "Search: ";
		this.SearchTextbox.Location = new System.Drawing.Point(48, 4);
		this.SearchTextbox.Name = "SearchTextbox";
		this.SearchTextbox.Size = new System.Drawing.Size(773, 20);
		this.SearchTextbox.TabIndex = 0;
		this.SearchTextbox.TextChanged += new System.EventHandler(SearchTextbox_TextChanged);
		this.tabPage7.Controls.Add(this.PatchesPanel);
		this.tabPage7.Location = new System.Drawing.Point(4, 22);
		this.tabPage7.Name = "tabPage7";
		this.tabPage7.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage7.Size = new System.Drawing.Size(832, 468);
		this.tabPage7.TabIndex = 8;
		this.tabPage7.Text = "Patches";
		this.tabPage7.UseVisualStyleBackColor = true;
		this.PatchesPanel.Controls.Add(this.PatchDescription);
		this.PatchesPanel.Controls.Add(this.PatchVersion);
		this.PatchesPanel.Controls.Add(this.PatchAuthor);
		this.PatchesPanel.Controls.Add(this.label37);
		this.PatchesPanel.Controls.Add(this.label36);
		this.PatchesPanel.Controls.Add(this.label35);
		this.PatchesPanel.Controls.Add(this.label34);
		this.PatchesPanel.Controls.Add(this.PatchRTB);
		this.PatchesPanel.Controls.Add(this.AddPatchLabel);
		this.PatchesPanel.Controls.Add(this.ApplyPatchButton);
		this.PatchesPanel.Controls.Add(this.label33);
		this.PatchesPanel.Controls.Add(this.PatchComboBox);
		this.PatchesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.PatchesPanel.Location = new System.Drawing.Point(3, 3);
		this.PatchesPanel.Name = "PatchesPanel";
		this.PatchesPanel.Size = new System.Drawing.Size(826, 462);
		this.PatchesPanel.TabIndex = 0;
		this.PatchDescription.AutoSize = true;
		this.PatchDescription.Location = new System.Drawing.Point(70, 102);
		this.PatchDescription.Name = "PatchDescription";
		this.PatchDescription.Size = new System.Drawing.Size(0, 13);
		this.PatchDescription.TabIndex = 27;
		this.PatchVersion.AutoSize = true;
		this.PatchVersion.Location = new System.Drawing.Point(51, 80);
		this.PatchVersion.Name = "PatchVersion";
		this.PatchVersion.Size = new System.Drawing.Size(0, 13);
		this.PatchVersion.TabIndex = 26;
		this.PatchAuthor.AutoSize = true;
		this.PatchAuthor.Location = new System.Drawing.Point(47, 58);
		this.PatchAuthor.Name = "PatchAuthor";
		this.PatchAuthor.Size = new System.Drawing.Size(0, 13);
		this.PatchAuthor.TabIndex = 25;
		this.label37.AutoSize = true;
		this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label37.Location = new System.Drawing.Point(9, 124);
		this.label37.Name = "label37";
		this.label37.Size = new System.Drawing.Size(42, 13);
		this.label37.TabIndex = 24;
		this.label37.Text = "Output:";
		this.label36.AutoSize = true;
		this.label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label36.Location = new System.Drawing.Point(9, 102);
		this.label36.Name = "label36";
		this.label36.Size = new System.Drawing.Size(63, 13);
		this.label36.TabIndex = 23;
		this.label36.Text = "Description:";
		this.label35.AutoSize = true;
		this.label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label35.Location = new System.Drawing.Point(9, 80);
		this.label35.Name = "label35";
		this.label35.Size = new System.Drawing.Size(45, 13);
		this.label35.TabIndex = 22;
		this.label35.Text = "Version:";
		this.label34.AutoSize = true;
		this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label34.Location = new System.Drawing.Point(9, 58);
		this.label34.Name = "label34";
		this.label34.Size = new System.Drawing.Size(41, 13);
		this.label34.TabIndex = 21;
		this.label34.Text = "Author:";
		this.PatchRTB.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.PatchRTB.Location = new System.Drawing.Point(7, 149);
		this.PatchRTB.Name = "PatchRTB";
		this.PatchRTB.ReadOnly = true;
		this.PatchRTB.Size = new System.Drawing.Size(813, 281);
		this.PatchRTB.TabIndex = 20;
		this.PatchRTB.Text = "";
		this.PatchRTB.WordWrap = false;
		this.AddPatchLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.AddPatchLabel.AutoSize = true;
		this.AddPatchLabel.Location = new System.Drawing.Point(9, 439);
		this.AddPatchLabel.Name = "AddPatchLabel";
		this.AddPatchLabel.Size = new System.Drawing.Size(95, 13);
		this.AddPatchLabel.TabIndex = 19;
		this.AddPatchLabel.TabStop = true;
		this.AddPatchLabel.Text = "install a new patch";
		this.AddPatchLabel.VisitedLinkColor = System.Drawing.Color.Blue;
		this.AddPatchLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(AddPatchLabel_LinkClicked);
		this.ApplyPatchButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.ApplyPatchButton.Location = new System.Drawing.Point(745, 434);
		this.ApplyPatchButton.Name = "ApplyPatchButton";
		this.ApplyPatchButton.Size = new System.Drawing.Size(75, 23);
		this.ApplyPatchButton.TabIndex = 17;
		this.ApplyPatchButton.Text = "Apply patch";
		this.ApplyPatchButton.UseVisualStyleBackColor = true;
		this.ApplyPatchButton.Click += new System.EventHandler(ApplyPatchButton_Click);
		this.label33.AutoSize = true;
		this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label33.Location = new System.Drawing.Point(9, 22);
		this.label33.Name = "label33";
		this.label33.Size = new System.Drawing.Size(35, 13);
		this.label33.TabIndex = 16;
		this.label33.Text = "Patch";
		this.PatchComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.PatchComboBox.FormattingEnabled = true;
		this.PatchComboBox.Location = new System.Drawing.Point(44, 19);
		this.PatchComboBox.Name = "PatchComboBox";
		this.PatchComboBox.Size = new System.Drawing.Size(369, 21);
		this.PatchComboBox.TabIndex = 0;
		this.PatchComboBox.SelectedIndexChanged += new System.EventHandler(PatchComboBox_SelectedIndexChanged);
		this.tabPage8.Controls.Add(this.groupBox8);
		this.tabPage8.Controls.Add(this.groupBox7);
		this.tabPage8.Controls.Add(this.groupBox6);
		this.tabPage8.Location = new System.Drawing.Point(4, 22);
		this.tabPage8.Name = "tabPage8";
		this.tabPage8.Padding = new System.Windows.Forms.Padding(3);
		this.tabPage8.Size = new System.Drawing.Size(832, 468);
		this.tabPage8.TabIndex = 9;
		this.tabPage8.Text = "Packaging";
		this.tabPage8.UseVisualStyleBackColor = true;
		this.groupBox8.Controls.Add(this.CTPCompressionStatus);
		this.groupBox8.Controls.Add(this.CTPCompression);
		this.groupBox8.Controls.Add(this.label21);
		this.groupBox8.Controls.Add(this.ExportCTP);
		this.groupBox8.Controls.Add(this.label19);
		this.groupBox8.Location = new System.Drawing.Point(8, 308);
		this.groupBox8.Name = "groupBox8";
		this.groupBox8.Size = new System.Drawing.Size(304, 107);
		this.groupBox8.TabIndex = 11;
		this.groupBox8.TabStop = false;
		this.groupBox8.Text = "Customization Tool project file";
		this.CTPCompressionStatus.AutoSize = true;
		this.CTPCompressionStatus.Location = new System.Drawing.Point(6, 62);
		this.CTPCompressionStatus.Name = "CTPCompressionStatus";
		this.CTPCompressionStatus.Size = new System.Drawing.Size(208, 13);
		this.CTPCompressionStatus.TabIndex = 18;
		this.CTPCompressionStatus.Text = "Optimal compression, longer but smaller file";
		this.CTPCompression.AutoSize = false;
		this.CTPCompression.BackColor = System.Drawing.Color.White;
		this.CTPCompression.LargeChange = 1;
		this.CTPCompression.Location = new System.Drawing.Point(104, 43);
		this.CTPCompression.Maximum = 3;
		this.CTPCompression.Minimum = 1;
		this.CTPCompression.Name = "CTPCompression";
		this.CTPCompression.Size = new System.Drawing.Size(194, 20);
		this.CTPCompression.TabIndex = 17;
		this.CTPCompression.TickStyle = System.Windows.Forms.TickStyle.None;
		this.CTPCompression.Value = 3;
		this.CTPCompression.Scroll += new System.EventHandler(CTPCompression_Scroll);
		this.label21.AutoSize = true;
		this.label21.Location = new System.Drawing.Point(6, 44);
		this.label21.Name = "label21";
		this.label21.Size = new System.Drawing.Size(92, 13);
		this.label21.TabIndex = 16;
		this.label21.Text = "Compression level";
		this.ExportCTP.Location = new System.Drawing.Point(6, 82);
		this.ExportCTP.Name = "ExportCTP";
		this.ExportCTP.Size = new System.Drawing.Size(292, 22);
		this.ExportCTP.TabIndex = 15;
		this.ExportCTP.Text = "Export";
		this.ExportCTP.UseVisualStyleBackColor = true;
		this.ExportCTP.Click += new System.EventHandler(ExportCTP_Click);
		this.label19.AutoSize = true;
		this.label19.Location = new System.Drawing.Point(7, 20);
		this.label19.Name = "label19";
		this.label19.Size = new System.Drawing.Size(238, 13);
		this.label19.TabIndex = 0;
		this.label19.Text = "Produces a file that can be re-opened by this tool";
		this.groupBox7.Controls.Add(this.RKChipset);
		this.groupBox7.Controls.Add(this.SpecifyChipset);
		this.groupBox7.Controls.Add(this.AddPlatformConfig);
		this.groupBox7.Controls.Add(this.label15);
		this.groupBox7.Controls.Add(this.label14);
		this.groupBox7.Controls.Add(this.RepackToImage);
		this.groupBox7.Location = new System.Drawing.Point(8, 6);
		this.groupBox7.Name = "groupBox7";
		this.groupBox7.Size = new System.Drawing.Size(304, 139);
		this.groupBox7.TabIndex = 10;
		this.groupBox7.TabStop = false;
		this.groupBox7.Text = "USB burning tool package";
		this.RKChipset.Location = new System.Drawing.Point(162, 113);
		this.RKChipset.Name = "RKChipset";
		this.RKChipset.Size = new System.Drawing.Size(132, 20);
		this.RKChipset.TabIndex = 14;
		this.SpecifyChipset.AutoSize = true;
		this.SpecifyChipset.Location = new System.Drawing.Point(12, 116);
		this.SpecifyChipset.Name = "SpecifyChipset";
		this.SpecifyChipset.Size = new System.Drawing.Size(144, 17);
		this.SpecifyChipset.TabIndex = 13;
		this.SpecifyChipset.Text = "Specify chipset (RK only)";
		this.SpecifyChipset.UseVisualStyleBackColor = true;
		this.SpecifyChipset.CheckedChanged += new System.EventHandler(SpecifyChipset_CheckedChanged);
		this.AddPlatformConfig.AutoSize = true;
		this.AddPlatformConfig.Location = new System.Drawing.Point(12, 96);
		this.AddPlatformConfig.Name = "AddPlatformConfig";
		this.AddPlatformConfig.Size = new System.Drawing.Size(203, 17);
		this.AddPlatformConfig.TabIndex = 12;
		this.AddPlatformConfig.Text = "Add platform configuration to firmware";
		this.AddPlatformConfig.UseVisualStyleBackColor = true;
		this.label15.AutoSize = true;
		this.label15.Location = new System.Drawing.Point(9, 62);
		this.label15.Name = "label15";
		this.label15.Size = new System.Drawing.Size(285, 26);
		this.label15.TabIndex = 11;
		this.label15.Text = "If the following is not enabled by default, only enable\r\nif you are getting \"error creating workflow\" in USB burn tool";
		this.label14.AutoSize = true;
		this.label14.Location = new System.Drawing.Point(4, 15);
		this.label14.Name = "label14";
		this.label14.Size = new System.Drawing.Size(161, 13);
		this.label14.TabIndex = 10;
		this.label14.Text = "Produces a USB burn tool image";
		this.RepackToImage.Location = new System.Drawing.Point(6, 33);
		this.RepackToImage.Name = "RepackToImage";
		this.RepackToImage.Size = new System.Drawing.Size(292, 22);
		this.RepackToImage.TabIndex = 9;
		this.RepackToImage.Text = "Repack to upgrade package";
		this.RepackToImage.UseVisualStyleBackColor = true;
		this.RepackToImage.Click += new System.EventHandler(RepackToImage_Click);
		this.groupBox6.Controls.Add(this.CompressBrotli);
		this.groupBox6.Controls.Add(this.CompressionStatus);
		this.groupBox6.Controls.Add(this.CompressionLevel);
		this.groupBox6.Controls.Add(this.label18);
		this.groupBox6.Controls.Add(this.ZipRom);
		this.groupBox6.Controls.Add(this.label13);
		this.groupBox6.Location = new System.Drawing.Point(8, 151);
		this.groupBox6.Name = "groupBox6";
		this.groupBox6.Size = new System.Drawing.Size(304, 136);
		this.groupBox6.TabIndex = 9;
		this.groupBox6.TabStop = false;
		this.groupBox6.Text = "Recovery flashable zip";
		this.CompressBrotli.AutoSize = true;
		this.CompressBrotli.Location = new System.Drawing.Point(12, 108);
		this.CompressBrotli.Name = "CompressBrotli";
		this.CompressBrotli.Size = new System.Drawing.Size(119, 17);
		this.CompressBrotli.TabIndex = 15;
		this.CompressBrotli.Text = "Compress with brotli";
		this.CompressBrotli.UseVisualStyleBackColor = true;
		this.CompressionStatus.AutoSize = true;
		this.CompressionStatus.Location = new System.Drawing.Point(3, 60);
		this.CompressionStatus.Name = "CompressionStatus";
		this.CompressionStatus.Size = new System.Drawing.Size(242, 13);
		this.CompressionStatus.TabIndex = 14;
		this.CompressionStatus.Text = "5 - medium package size, medium processing time";
		this.CompressionLevel.AutoSize = false;
		this.CompressionLevel.BackColor = System.Drawing.Color.White;
		this.CompressionLevel.LargeChange = 1;
		this.CompressionLevel.Location = new System.Drawing.Point(101, 39);
		this.CompressionLevel.Maximum = 9;
		this.CompressionLevel.Minimum = 1;
		this.CompressionLevel.Name = "CompressionLevel";
		this.CompressionLevel.Size = new System.Drawing.Size(197, 20);
		this.CompressionLevel.TabIndex = 13;
		this.CompressionLevel.TickStyle = System.Windows.Forms.TickStyle.None;
		this.CompressionLevel.Value = 5;
		this.CompressionLevel.Scroll += new System.EventHandler(CompressionLevel_Scroll);
		this.label18.AutoSize = true;
		this.label18.Location = new System.Drawing.Point(3, 42);
		this.label18.Name = "label18";
		this.label18.Size = new System.Drawing.Size(92, 13);
		this.label18.TabIndex = 12;
		this.label18.Text = "Compression level";
		this.ZipRom.Location = new System.Drawing.Point(6, 79);
		this.ZipRom.Name = "ZipRom";
		this.ZipRom.Size = new System.Drawing.Size(292, 22);
		this.ZipRom.TabIndex = 8;
		this.ZipRom.Text = "Repack to zip";
		this.ZipRom.UseVisualStyleBackColor = true;
		this.ZipRom.Click += new System.EventHandler(ZipRom_Click);
		this.label13.AutoSize = true;
		this.label13.Location = new System.Drawing.Point(3, 16);
		this.label13.Name = "label13";
		this.label13.Size = new System.Drawing.Size(261, 13);
		this.label13.TabIndex = 0;
		this.label13.Text = "Produces a zip file that can be flashed inside recovery";
		this.panel1.Controls.Add(this.CTVersionLabel);
		this.panel1.Controls.Add(this.UnpackingLoader);
		this.panel1.Controls.Add(this.UpgradePackage);
		this.panel1.Controls.Add(this.UnpackImage);
		this.panel1.Controls.Add(this.menuStrip1);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel1.Location = new System.Drawing.Point(0, 0);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(840, 73);
		this.panel1.TabIndex = 3;
		this.CTVersionLabel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.CTVersionLabel.AutoSize = true;
		this.CTVersionLabel.BackColor = System.Drawing.SystemColors.Window;
		this.CTVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.CTVersionLabel.Location = new System.Drawing.Point(779, 4);
		this.CTVersionLabel.Name = "CTVersionLabel";
		this.CTVersionLabel.Size = new System.Drawing.Size(47, 17);
		this.CTVersionLabel.TabIndex = 9;
		this.CTVersionLabel.Text = "v1.0.0";
		this.CTVersionLabel.Click += new System.EventHandler(CTVersionLabel_Click);
		this.UnpackingLoader.Dock = System.Windows.Forms.DockStyle.Right;
		this.UnpackingLoader.Image = CustomizationTool.Properties.Resources.loading;
		this.UnpackingLoader.Location = new System.Drawing.Point(785, 24);
		this.UnpackingLoader.Name = "UnpackingLoader";
		this.UnpackingLoader.Size = new System.Drawing.Size(55, 49);
		this.UnpackingLoader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
		this.UnpackingLoader.TabIndex = 8;
		this.UnpackingLoader.TabStop = false;
		this.UnpackingLoader.Visible = false;
		this.UpgradePackage.Enabled = false;
		this.UpgradePackage.Location = new System.Drawing.Point(84, 31);
		this.UpgradePackage.Name = "UpgradePackage";
		this.UpgradePackage.Size = new System.Drawing.Size(405, 20);
		this.UpgradePackage.TabIndex = 5;
		this.UpgradePackage.Text = "Select Amlogic upgrade package or customization tool project file..";
		this.UnpackImage.Location = new System.Drawing.Point(3, 29);
		this.UnpackImage.Name = "UnpackImage";
		this.UnpackImage.Size = new System.Drawing.Size(75, 23);
		this.UnpackImage.TabIndex = 4;
		this.UnpackImage.Text = "Unpack";
		this.UnpackImage.UseVisualStyleBackColor = true;
		this.UnpackImage.Click += new System.EventHandler(UnpackImage_Click);
		this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.AboutButton, this.tutorialsToolStripMenuItem1, this.HelpButton, this.experimentalFeaturesToolStripMenuItem1, this.supportTheProjectToolStripMenuItem });
		this.menuStrip1.Location = new System.Drawing.Point(0, 0);
		this.menuStrip1.Name = "menuStrip1";
		this.menuStrip1.Size = new System.Drawing.Size(840, 24);
		this.menuStrip1.TabIndex = 7;
		this.menuStrip1.Text = "menuStrip1";
		this.AboutButton.Name = "AboutButton";
		this.AboutButton.Size = new System.Drawing.Size(52, 20);
		this.AboutButton.Text = "About";
		this.AboutButton.Click += new System.EventHandler(AboutButton_Click);
		this.tutorialsToolStripMenuItem1.Name = "tutorialsToolStripMenuItem1";
		this.tutorialsToolStripMenuItem1.Size = new System.Drawing.Size(65, 20);
		this.tutorialsToolStripMenuItem1.Text = "Tutorials";
		this.HelpButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.threadToolStripMenuItem, this.reportBugsToolStripMenuItem, this.supportToolStripMenuItem, this.logsToolStripMenuItem });
		this.HelpButton.Name = "HelpButton";
		this.HelpButton.Size = new System.Drawing.Size(44, 20);
		this.HelpButton.Text = "Help";
		this.threadToolStripMenuItem.Name = "threadToolStripMenuItem";
		this.threadToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.threadToolStripMenuItem.Text = "Thread";
		this.threadToolStripMenuItem.Click += new System.EventHandler(threadToolStripMenuItem_Click);
		this.reportBugsToolStripMenuItem.Name = "reportBugsToolStripMenuItem";
		this.reportBugsToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.reportBugsToolStripMenuItem.Text = "Report bugs";
		this.reportBugsToolStripMenuItem.Click += new System.EventHandler(reportBugsToolStripMenuItem_Click);
		this.supportToolStripMenuItem.Name = "supportToolStripMenuItem";
		this.supportToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.supportToolStripMenuItem.Text = "Support";
		this.supportToolStripMenuItem.Click += new System.EventHandler(supportToolStripMenuItem_Click);
		this.logsToolStripMenuItem.Name = "logsToolStripMenuItem";
		this.logsToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
		this.logsToolStripMenuItem.Text = "Logs";
		this.logsToolStripMenuItem.Click += new System.EventHandler(logsToolStripMenuItem_Click);
		this.experimentalFeaturesToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[6] { this.PythonSuperUtils, this.NewDtbProcessingTools, this.NewLogoProcessingTools, this.PythonImgExtractor, this.ArchMake_ext4fs, this.NewPythonSimg2img });
		this.experimentalFeaturesToolStripMenuItem1.Name = "experimentalFeaturesToolStripMenuItem1";
		this.experimentalFeaturesToolStripMenuItem1.Size = new System.Drawing.Size(117, 20);
		this.experimentalFeaturesToolStripMenuItem1.Text = "Developer Options";
		this.PythonSuperUtils.Name = "PythonSuperUtils";
		this.PythonSuperUtils.Size = new System.Drawing.Size(288, 22);
		this.PythonSuperUtils.Text = "Use python super utils";
		this.PythonSuperUtils.Click += new System.EventHandler(PythonSuperUtils_Click);
		this.NewDtbProcessingTools.Name = "NewDtbProcessingTools";
		this.NewDtbProcessingTools.Size = new System.Drawing.Size(288, 22);
		this.NewDtbProcessingTools.Text = "Use new dtb processing tools";
		this.NewDtbProcessingTools.Click += new System.EventHandler(NewDtbProcessingTools_Click);
		this.NewLogoProcessingTools.Name = "NewLogoProcessingTools";
		this.NewLogoProcessingTools.Size = new System.Drawing.Size(288, 22);
		this.NewLogoProcessingTools.Text = "Use new logo processing tools (amlogic)";
		this.NewLogoProcessingTools.Click += new System.EventHandler(NewLogoProcessingTools_Click);
		this.PythonImgExtractor.Name = "PythonImgExtractor";
		this.PythonImgExtractor.Size = new System.Drawing.Size(288, 22);
		this.PythonImgExtractor.Text = "Use new ext4 extractor";
		this.PythonImgExtractor.Click += new System.EventHandler(PythonImgExtractor_Click);
		this.ArchMake_ext4fs.Name = "ArchMake_ext4fs";
		this.ArchMake_ext4fs.Size = new System.Drawing.Size(288, 22);
		this.ArchMake_ext4fs.Text = "Use $ARCH dedicated make_ext4fs";
		this.ArchMake_ext4fs.Click += new System.EventHandler(ArchMake_ext4fs_Click);
		this.NewPythonSimg2img.Name = "NewPythonSimg2img";
		this.NewPythonSimg2img.Size = new System.Drawing.Size(288, 22);
		this.NewPythonSimg2img.Text = "Use new python simg2img";
		this.NewPythonSimg2img.Click += new System.EventHandler(PythonSimg2Img_Click);
		this.supportTheProjectToolStripMenuItem.Name = "supportTheProjectToolStripMenuItem";
		this.supportTheProjectToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
		this.supportTheProjectToolStripMenuItem.Text = "Get full version";
		this.supportTheProjectToolStripMenuItem.Click += new System.EventHandler(supportTheProjectToolStripMenuItem_Click);
		this.panel7.Controls.Add(this.UserLabel);
		this.panel7.Controls.Add(this.StatusLabel);
		this.panel7.Controls.Add(this.label17);
		this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel7.Location = new System.Drawing.Point(0, 567);
		this.panel7.Name = "panel7";
		this.panel7.Size = new System.Drawing.Size(840, 25);
		this.panel7.TabIndex = 10;
		this.UserLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.UserLabel.BackColor = System.Drawing.SystemColors.Control;
		this.UserLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.UserLabel.Location = new System.Drawing.Point(602, 3);
		this.UserLabel.Name = "UserLabel";
		this.UserLabel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
		this.UserLabel.Size = new System.Drawing.Size(231, 17);
		this.UserLabel.TabIndex = 10;
		this.UserLabel.Text = "User: Free user";
		this.StatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.StatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.StatusLabel.Location = new System.Drawing.Point(41, 0);
		this.StatusLabel.Name = "StatusLabel";
		this.StatusLabel.Size = new System.Drawing.Size(799, 25);
		this.StatusLabel.TabIndex = 1;
		this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.label17.Dock = System.Windows.Forms.DockStyle.Left;
		this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.label17.Location = new System.Drawing.Point(0, 0);
		this.label17.Name = "label17";
		this.label17.Size = new System.Drawing.Size(41, 25);
		this.label17.TabIndex = 0;
		this.label17.Text = "Status: ";
		this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.bootlogotimer.Interval = 3000;
		this.bootlogotimer.Tick += new System.EventHandler(bootlogotimer_Tick);
		this.SearchContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.copyToolStripMenuItem, this.deleteToolStripMenuItem });
		this.SearchContextMenu.Name = "SearchContextMenu";
		this.SearchContextMenu.Size = new System.Drawing.Size(108, 48);
		this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
		this.copyToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.copyToolStripMenuItem.Text = "Copy";
		this.copyToolStripMenuItem.Click += new System.EventHandler(copyToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(deleteToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(840, 592);
		base.Controls.Add(this.MainTab);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.panel7);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MainMenuStrip = this.menuStrip1;
		base.Name = "Form1";
		this.Text = "Customization Tool";
		base.Shown += new System.EventHandler(Form1_Shown);
		this.MainTab.ResumeLayout(false);
		this.tabPage1.ResumeLayout(false);
		this.groupBox2.ResumeLayout(false);
		this.groupBox2.PerformLayout();
		this.groupBox1.ResumeLayout(false);
		this.groupBox1.PerformLayout();
		this.tabPage2.ResumeLayout(false);
		this.BootAnimationPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.BootanimationPicturebox).EndInit();
		this.panel4.ResumeLayout(false);
		this.BootvideoPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.BootVideoPlayer).EndInit();
		this.panel11.ResumeLayout(false);
		this.panel2.ResumeLayout(false);
		this.BootlogoPanel.ResumeLayout(false);
		this.panel10.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.BootlogoPicturebox).EndInit();
		this.tabPage3.ResumeLayout(false);
		this.panel6.ResumeLayout(false);
		this.panel6.PerformLayout();
		this.panel3.ResumeLayout(false);
		this.ApkGroupbox.ResumeLayout(false);
		this.ApkGroupbox.PerformLayout();
		this.groupBox3.ResumeLayout(false);
		this.tabPage4.ResumeLayout(false);
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		this.tabPage5.ResumeLayout(false);
		this.DTBPanel.ResumeLayout(false);
		this.panel8.ResumeLayout(false);
		this.panel8.PerformLayout();
		this.panel9.ResumeLayout(false);
		this.panel9.PerformLayout();
		this.PackPage.ResumeLayout(false);
		this.VendorGroupBox.ResumeLayout(false);
		this.groupBox14.ResumeLayout(false);
		this.groupBox9.ResumeLayout(false);
		this.SystemGroupBox.ResumeLayout(false);
		this.WifiGroupBox.ResumeLayout(false);
		this.groupBox4.ResumeLayout(false);
		this.tabPage6.ResumeLayout(false);
		this.panel5.ResumeLayout(false);
		this.panel5.PerformLayout();
		this.tabPage7.ResumeLayout(false);
		this.PatchesPanel.ResumeLayout(false);
		this.PatchesPanel.PerformLayout();
		this.tabPage8.ResumeLayout(false);
		this.groupBox8.ResumeLayout(false);
		this.groupBox8.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.CTPCompression).EndInit();
		this.groupBox7.ResumeLayout(false);
		this.groupBox7.PerformLayout();
		this.groupBox6.ResumeLayout(false);
		this.groupBox6.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.CompressionLevel).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.UnpackingLoader).EndInit();
		this.menuStrip1.ResumeLayout(false);
		this.menuStrip1.PerformLayout();
		this.panel7.ResumeLayout(false);
		this.SearchContextMenu.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
