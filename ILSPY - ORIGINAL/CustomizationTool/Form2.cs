using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CustomizationTool;

public class Form2 : Form
{
	private IContainer components;

	private Label label1;

	private TextBox textBox1;

	private Button button1;

	private LinkLabel linkLabel1;

	public Form2(string ID)
	{
		InitializeComponent();
		textBox1.Text = ID;
	}

	private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
	{
		Process.Start("https://buy.stripe.com/8wM4jicf8dFf7aU9AG");
	}

	private void button1_Click(object sender, EventArgs e)
	{
		Clipboard.SetText(textBox1.Text);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomizationTool.Form2));
		this.label1 = new System.Windows.Forms.Label();
		this.textBox1 = new System.Windows.Forms.TextBox();
		this.button1 = new System.Windows.Forms.Button();
		this.linkLabel1 = new System.Windows.Forms.LinkLabel();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(13, 13);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(293, 91);
		this.label1.TabIndex = 0;
		this.label1.Text = resources.GetString("label1.Text");
		this.textBox1.Location = new System.Drawing.Point(16, 114);
		this.textBox1.Name = "textBox1";
		this.textBox1.Size = new System.Drawing.Size(264, 20);
		this.textBox1.TabIndex = 1;
		this.button1.Location = new System.Drawing.Point(286, 111);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(58, 23);
		this.button1.TabIndex = 2;
		this.button1.Text = "Copy";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		this.linkLabel1.AutoSize = true;
		this.linkLabel1.Location = new System.Drawing.Point(215, 64);
		this.linkLabel1.Name = "linkLabel1";
		this.linkLabel1.Size = new System.Drawing.Size(53, 13);
		this.linkLabel1.TabIndex = 3;
		this.linkLabel1.TabStop = true;
		this.linkLabel1.Text = "click here";
		this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(linkLabel1_LinkClicked);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(351, 142);
		base.Controls.Add(this.linkLabel1);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.textBox1);
		base.Controls.Add(this.label1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Form2";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Product not activated";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
