using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace CustomizationTool;

public class Form4 : Form
{
	private string download = "";

	private string target = "";

	private IContainer components;

	private ProgressBar UpdateProgress;

	private Label UpdateLabel;

	public Form4(string link, string savepath)
	{
		InitializeComponent();
		download = link;
		target = savepath;
		if (savepath.EndsWith(".zip"))
		{
			Text = "Downloading " + target.Split('/').Last().Split('\\')
				.Last()
				.Split('.')
				.First();
		}
	}

	private void Form4_Shown(object sender, EventArgs c)
	{
		new Thread((ThreadStart)delegate
		{
			WebClient obj = new WebClient
			{
				Headers = { { "a", "a" } }
			};
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			obj.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
			obj.Proxy = null;
			obj.DownloadProgressChanged += client_DownloadProgressChanged;
			obj.DownloadFileCompleted += client_DownloadFileCompleted;
			obj.DownloadFileAsync(new Uri(download), target);
		}).Start();
	}

	private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
	{
		BeginInvoke((MethodInvoker)delegate
		{
			double num = double.Parse(e.BytesReceived.ToString());
			double num2 = double.Parse(e.TotalBytesToReceive.ToString());
			double d = num / num2 * 100.0;
			UpdateLabel.Text = "Downloaded " + e.BytesReceived / 1024 / 1024 + "/" + e.TotalBytesToReceive / 1024 / 1024 + " MB";
			UpdateProgress.Value = int.Parse(Math.Truncate(d).ToString());
		});
	}

	private void BackgroundShell(string executable, string command)
	{
		Thread newThread = new Thread((ThreadStart)delegate
		{
			ProcessStartInfo startInfo = new ProcessStartInfo("\"" + executable + "\"", command)
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
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
		newThread.Abort();
	}

	private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
	{
		BeginInvoke((MethodInvoker)delegate
		{
			UpdateLabel.Text = "Unpacking " + target.Split('/').Last().Split('\\')
				.Last()
				.Split('.')
				.First() + "..";
			if (target.EndsWith(".zip"))
			{
				BackgroundShell(AppDomain.CurrentDomain.BaseDirectory + "bin\\7za", " x -y \"" + target + "\" -o\"" + AppDomain.CurrentDomain.BaseDirectory + "bin\\\"");
				File.Delete(target);
			}
			UpdateLabel.Text = "Completed";
			Close();
		});
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomizationTool.Form4));
		this.UpdateProgress = new System.Windows.Forms.ProgressBar();
		this.UpdateLabel = new System.Windows.Forms.Label();
		base.SuspendLayout();
		this.UpdateProgress.Location = new System.Drawing.Point(12, 12);
		this.UpdateProgress.Name = "UpdateProgress";
		this.UpdateProgress.Size = new System.Drawing.Size(282, 25);
		this.UpdateProgress.TabIndex = 0;
		this.UpdateLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.UpdateLabel.Location = new System.Drawing.Point(0, 40);
		this.UpdateLabel.Name = "UpdateLabel";
		this.UpdateLabel.Size = new System.Drawing.Size(306, 20);
		this.UpdateLabel.TabIndex = 1;
		this.UpdateLabel.Text = "Connecting to server..";
		this.UpdateLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(306, 60);
		base.ControlBox = false;
		base.Controls.Add(this.UpdateLabel);
		base.Controls.Add(this.UpdateProgress);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Form4";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Downloading update";
		base.Shown += new System.EventHandler(Form4_Shown);
		base.ResumeLayout(false);
	}
}
