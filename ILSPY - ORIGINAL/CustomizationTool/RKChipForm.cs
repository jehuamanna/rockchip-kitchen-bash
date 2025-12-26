using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CustomizationTool;

public class RKChipForm : Form
{
	private IContainer components;

	private Label label1;

	private TextBox chip;

	private Button button1;

	public RKChipForm()
	{
		InitializeComponent();
	}

	private void button1_Click(object sender, EventArgs e)
	{
		if (chip.Text.Length > 5)
		{
			if (chip.Text.ToUpper().StartsWith("RK"))
			{
				base.Tag = chip.Text.ToUpper();
				base.DialogResult = DialogResult.OK;
				Close();
			}
			else
			{
				MessageBox.Show("Chip does not start with \"RK\"", "Error");
			}
		}
		else
		{
			MessageBox.Show("The length of the chip must be 6 or more chars long", "Error");
		}
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
		this.label1 = new System.Windows.Forms.Label();
		this.chip = new System.Windows.Forms.TextBox();
		this.button1 = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(3, 2);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(332, 26);
		this.label1.TabIndex = 0;
		this.label1.Text = "Please enter RK chip, this should be 6 characters long and start with \r\nRK. E.G. RK3200, RK2877";
		this.chip.Location = new System.Drawing.Point(6, 31);
		this.chip.Name = "chip";
		this.chip.Size = new System.Drawing.Size(322, 20);
		this.chip.TabIndex = 1;
		this.button1.Location = new System.Drawing.Point(253, 57);
		this.button1.Name = "button1";
		this.button1.Size = new System.Drawing.Size(75, 23);
		this.button1.TabIndex = 2;
		this.button1.Text = "Submit";
		this.button1.UseVisualStyleBackColor = true;
		this.button1.Click += new System.EventHandler(button1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(334, 84);
		base.Controls.Add(this.button1);
		base.Controls.Add(this.chip);
		base.Controls.Add(this.label1);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "RKChipForm";
		this.Text = "Enter RK chip";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
