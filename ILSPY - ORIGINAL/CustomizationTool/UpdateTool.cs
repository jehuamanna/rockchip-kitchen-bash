using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CustomizationTool;

public class UpdateTool : Form
{
	private IContainer components;

	private Label label1;

	private RichTextBox richTextBox1;

	private Button button1;

	private Button button2;

	public UpdateTool(string changelog)
	{
		InitializeComponent();
		base.DialogResult = DialogResult.No;
		richTextBox1.Text = changelog;
	}

	private void button2_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.No;
		Close();
	}

	private void button1_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Yes;
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomizationTool.UpdateTool));
		this.label1 = new System.Windows.Forms.Label();
		this.richTextBox1 = new System.Windows.Forms.RichTextBox();
		this.button1 = new System.Windows.Forms.Button();
		this.button2 = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.label1.Dock = System.Windows.Forms.DockStyle.Top;
		this.label1.Location = new System.Drawing.Point(0, 0);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(595, 53);
		this.label1.TabIndex = 0;
		this.label1.Text = "An update is available for CustomizationTool. Would you like to update now?\r\n\r\nChanges:";
		this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
		this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Top;
		this.richTextBox1.Location = new System.Drawing.Point(0, 53);
		this.richTextBox1.Name = "richTextBox1";
		this.richTextBox1.ReadOnly = true;
		this.richTextBox1.Size = new System.Drawing.Size(595, 238);
		this.richTextBox1.TabIndex = 1;
		this.richTextBox1.Text = "";
		this.button1.Location = new System.Drawing.Point(515, 297);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 2;
		this.button1.Text = "Download";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.button2.Location = new System.Drawing.Point(4, 297);
		this.button2.Name = "button2";
		this.button2.Size = new System.Drawing.Size(75, 23);
		this.button2.TabIndex = 3;
		this.button2.Text = "Ignore";
		this.button2.UseVisualStyleBackColor = true;
		this.button2.Click += new System.EventHandler(button2_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(595, 325);
		base.Controls.Add(this.button2);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.richTextBox1);
		base.Controls.Add(this.label1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "UpdateTool";
		this.Text = "CustomizationTool update available";
		base.ResumeLayout(false);
	}
}
