using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CustomizationTool;

public class UnpackLevel : Form
{
	private IContainer components;

	private Label label4;

	private Button Unpack;

	private Button Cancel;

	public CheckBox Level1;

	public CheckBox Level2;

	public CheckBox Level3;

	private CheckBox systemcb;

	private CheckBox vendorcb;

	private CheckBox productcb;

	private CheckBox odmcb;

	private CheckBox oem;

	private CheckBox systemextcb;

	private CheckBox bootcb;

	private CheckBox recoverycb;

	private CheckBox dtbcb;

	private CheckBox launcher;

	private CheckBox bootvideocb;

	private CheckBox bootanimationcb;

	private CheckBox logocb;

	private CheckBox supercb;

	private CheckBox odmextcb;

	public string unpackLevel
	{
		get
		{
			return base.Tag.ToString();
		}
		set
		{
			base.Tag = value;
		}
	}

	public UnpackLevel()
	{
		InitializeComponent();
	}

	private void Unpack_Click(object sender, EventArgs e)
	{
		unpackLevel = "level1;";
		if (Level2.Checked)
		{
			unpackLevel += "level2;";
		}
		if (supercb.Checked)
		{
			unpackLevel += "super;";
		}
		if (systemcb.Checked)
		{
			unpackLevel += "system;";
		}
		if (vendorcb.Checked)
		{
			unpackLevel += "vendor;";
		}
		if (productcb.Checked)
		{
			unpackLevel += "product;";
		}
		if (oem.Checked)
		{
			unpackLevel += "oem;";
		}
		if (odmcb.Checked)
		{
			unpackLevel += "odm;";
		}
		if (systemextcb.Checked)
		{
			unpackLevel += "system_ext;";
		}
		if (odmextcb.Checked)
		{
			unpackLevel += "odm_ext;";
		}
		if (bootcb.Checked)
		{
			unpackLevel += "boot;";
		}
		if (recoverycb.Checked)
		{
			unpackLevel += "recovery;";
		}
		if (dtbcb.Checked)
		{
			unpackLevel += "dtb;";
		}
		if (Level3.Checked)
		{
			unpackLevel += "level3;";
		}
		if (odmextcb.Checked)
		{
			unpackLevel += "logo;";
		}
		if (logocb.Checked)
		{
			unpackLevel += "oem_ext;";
		}
		if (bootanimationcb.Checked)
		{
			unpackLevel += "bootanimation;";
		}
		if (bootvideocb.Checked)
		{
			unpackLevel += "bootvideo;";
		}
		if (launcher.Checked)
		{
			unpackLevel += "launcher;";
		}
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void Cancel_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void Level2_Click(object sender, EventArgs e)
	{
		if (!Level2.Checked)
		{
			Level3.Checked = false;
			supercb.Checked = false;
			systemcb.Checked = false;
			vendorcb.Checked = false;
			productcb.Checked = false;
			oem.Checked = false;
			odmcb.Checked = false;
			systemextcb.Checked = false;
			odmextcb.Checked = false;
			bootcb.Checked = false;
			recoverycb.Checked = false;
			dtbcb.Checked = false;
			logocb.Checked = false;
			bootanimationcb.Checked = false;
			bootvideocb.Checked = false;
			launcher.Checked = false;
		}
		else
		{
			supercb.Checked = true;
			systemcb.Checked = true;
			vendorcb.Checked = true;
			productcb.Checked = true;
			oem.Checked = true;
			odmcb.Checked = true;
			systemextcb.Checked = true;
			odmextcb.Checked = true;
			bootcb.Checked = true;
			recoverycb.Checked = true;
			dtbcb.Checked = true;
		}
	}

	private void Level2_Item_Click(object sender, EventArgs e)
	{
		if (!Level2.Checked)
		{
			Level2.Checked = true;
		}
		if (((CheckBox)sender).Name != "bootcb" && ((CheckBox)sender).Name != "recoverycb" && ((CheckBox)sender).Name != "dtbcb" && ((CheckBox)sender).Checked)
		{
			supercb.Checked = true;
		}
	}

	private void Level3_Click(object sender, EventArgs e)
	{
		if (Level3.Checked)
		{
			Level2.Checked = true;
			supercb.Checked = true;
			systemcb.Checked = true;
			vendorcb.Checked = true;
			productcb.Checked = true;
			oem.Checked = true;
			odmextcb.Checked = true;
			odmcb.Checked = true;
			systemextcb.Checked = true;
			bootcb.Checked = true;
			logocb.Checked = true;
			bootanimationcb.Checked = true;
			bootvideocb.Checked = true;
			launcher.Checked = true;
		}
		else
		{
			logocb.Checked = false;
			bootanimationcb.Checked = false;
			bootvideocb.Checked = false;
			launcher.Checked = false;
		}
	}

	private void Level3_Item_Click(object sender, EventArgs e)
	{
		if (!Level2.Checked)
		{
			Level2.Checked = true;
		}
		if (!Level3.Checked)
		{
			Level3.Checked = true;
		}
		Level2.Checked = true;
		supercb.Checked = true;
		systemcb.Checked = true;
		vendorcb.Checked = true;
		productcb.Checked = true;
		odmextcb.Checked = true;
		oem.Checked = true;
		odmcb.Checked = true;
		systemextcb.Checked = true;
	}

	private void logocb_Click(object sender, EventArgs e)
	{
		if (!Level2.Checked)
		{
			Level2.Checked = true;
			odmextcb.Checked = true;
		}
		if (!Level3.Checked)
		{
			Level3.Checked = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomizationTool.UnpackLevel));
		this.Level1 = new System.Windows.Forms.CheckBox();
		this.Level2 = new System.Windows.Forms.CheckBox();
		this.Level3 = new System.Windows.Forms.CheckBox();
		this.label4 = new System.Windows.Forms.Label();
		this.Unpack = new System.Windows.Forms.Button();
		this.Cancel = new System.Windows.Forms.Button();
		this.systemcb = new System.Windows.Forms.CheckBox();
		this.vendorcb = new System.Windows.Forms.CheckBox();
		this.productcb = new System.Windows.Forms.CheckBox();
		this.odmcb = new System.Windows.Forms.CheckBox();
		this.oem = new System.Windows.Forms.CheckBox();
		this.systemextcb = new System.Windows.Forms.CheckBox();
		this.bootcb = new System.Windows.Forms.CheckBox();
		this.recoverycb = new System.Windows.Forms.CheckBox();
		this.dtbcb = new System.Windows.Forms.CheckBox();
		this.launcher = new System.Windows.Forms.CheckBox();
		this.bootvideocb = new System.Windows.Forms.CheckBox();
		this.bootanimationcb = new System.Windows.Forms.CheckBox();
		this.logocb = new System.Windows.Forms.CheckBox();
		this.supercb = new System.Windows.Forms.CheckBox();
		this.odmextcb = new System.Windows.Forms.CheckBox();
		base.SuspendLayout();
		this.Level1.AutoSize = true;
		this.Level1.Checked = true;
		this.Level1.CheckState = System.Windows.Forms.CheckState.Checked;
		this.Level1.Enabled = false;
		this.Level1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.Level1.Location = new System.Drawing.Point(12, 39);
		this.Level1.Name = "Level1";
		this.Level1.Size = new System.Drawing.Size(351, 21);
		this.Level1.TabIndex = 0;
		this.Level1.Text = "Unpack level 1 - split upgrade package to partitions";
		this.Level1.UseVisualStyleBackColor = true;
		this.Level2.AutoSize = true;
		this.Level2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.Level2.Location = new System.Drawing.Point(12, 68);
		this.Level2.Name = "Level2";
		this.Level2.Size = new System.Drawing.Size(404, 21);
		this.Level2.TabIndex = 1;
		this.Level2.Text = "Unpack level 2 - unpack partitions to modify internal content";
		this.Level2.UseVisualStyleBackColor = true;
		this.Level2.Click += new System.EventHandler(Level2_Click);
		this.Level3.AutoSize = true;
		this.Level3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.Level3.Location = new System.Drawing.Point(12, 354);
		this.Level3.Name = "Level3";
		this.Level3.Size = new System.Drawing.Size(222, 21);
		this.Level3.TabIndex = 4;
		this.Level3.Text = "Unpack level 3 - Extract UI files";
		this.Level3.UseVisualStyleBackColor = true;
		this.Level3.Click += new System.EventHandler(Level3_Click);
		this.label4.AutoSize = true;
		this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f);
		this.label4.Location = new System.Drawing.Point(9, 9);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(523, 17);
		this.label4.TabIndex = 6;
		this.label4.Text = "Choose an unpack level to start the unpacking procedure or cancel the operation.";
		this.Unpack.Location = new System.Drawing.Point(470, 481);
		this.Unpack.Name = "Unpack";
		this.Unpack.Size = new System.Drawing.Size(75, 23);
		this.Unpack.TabIndex = 7;
		this.Unpack.Text = "Unpack";
		this.Unpack.UseVisualStyleBackColor = true;
		this.Unpack.Click += new System.EventHandler(Unpack_Click);
		this.Cancel.Location = new System.Drawing.Point(389, 481);
		this.Cancel.Name = "Cancel";
		this.Cancel.Size = new System.Drawing.Size(75, 23);
		this.Cancel.TabIndex = 8;
		this.Cancel.Text = "Cancel";
		this.Cancel.UseVisualStyleBackColor = true;
		this.Cancel.Click += new System.EventHandler(Cancel_Click);
		this.systemcb.AutoSize = true;
		this.systemcb.Location = new System.Drawing.Point(27, 116);
		this.systemcb.Name = "systemcb";
		this.systemcb.Size = new System.Drawing.Size(60, 17);
		this.systemcb.TabIndex = 9;
		this.systemcb.Text = "System";
		this.systemcb.UseVisualStyleBackColor = true;
		this.systemcb.Click += new System.EventHandler(Level2_Item_Click);
		this.vendorcb.AutoSize = true;
		this.vendorcb.Location = new System.Drawing.Point(27, 139);
		this.vendorcb.Name = "vendorcb";
		this.vendorcb.Size = new System.Drawing.Size(60, 17);
		this.vendorcb.TabIndex = 10;
		this.vendorcb.Text = "Vendor";
		this.vendorcb.UseVisualStyleBackColor = true;
		this.vendorcb.Click += new System.EventHandler(Level2_Item_Click);
		this.productcb.AutoSize = true;
		this.productcb.Location = new System.Drawing.Point(27, 162);
		this.productcb.Name = "productcb";
		this.productcb.Size = new System.Drawing.Size(63, 17);
		this.productcb.TabIndex = 11;
		this.productcb.Text = "Product";
		this.productcb.UseVisualStyleBackColor = true;
		this.productcb.Click += new System.EventHandler(Level2_Item_Click);
		this.odmcb.AutoSize = true;
		this.odmcb.Location = new System.Drawing.Point(27, 185);
		this.odmcb.Name = "odmcb";
		this.odmcb.Size = new System.Drawing.Size(48, 17);
		this.odmcb.TabIndex = 12;
		this.odmcb.Text = "Odm";
		this.odmcb.UseVisualStyleBackColor = true;
		this.odmcb.Click += new System.EventHandler(Level2_Item_Click);
		this.oem.AutoSize = true;
		this.oem.Location = new System.Drawing.Point(27, 208);
		this.oem.Name = "oem";
		this.oem.Size = new System.Drawing.Size(48, 17);
		this.oem.TabIndex = 13;
		this.oem.Text = "Oem";
		this.oem.UseVisualStyleBackColor = true;
		this.oem.Click += new System.EventHandler(Level2_Item_Click);
		this.systemextcb.AutoSize = true;
		this.systemextcb.Location = new System.Drawing.Point(27, 231);
		this.systemextcb.Name = "systemextcb";
		this.systemextcb.Size = new System.Drawing.Size(80, 17);
		this.systemextcb.TabIndex = 14;
		this.systemextcb.Text = "System_ext";
		this.systemextcb.UseVisualStyleBackColor = true;
		this.systemextcb.Click += new System.EventHandler(Level2_Item_Click);
		this.bootcb.AutoSize = true;
		this.bootcb.Location = new System.Drawing.Point(27, 277);
		this.bootcb.Name = "bootcb";
		this.bootcb.Size = new System.Drawing.Size(48, 17);
		this.bootcb.TabIndex = 15;
		this.bootcb.Text = "Boot";
		this.bootcb.UseVisualStyleBackColor = true;
		this.bootcb.Click += new System.EventHandler(Level2_Item_Click);
		this.recoverycb.AutoSize = true;
		this.recoverycb.Location = new System.Drawing.Point(27, 300);
		this.recoverycb.Name = "recoverycb";
		this.recoverycb.Size = new System.Drawing.Size(72, 17);
		this.recoverycb.TabIndex = 16;
		this.recoverycb.Text = "Recovery";
		this.recoverycb.UseVisualStyleBackColor = true;
		this.recoverycb.Click += new System.EventHandler(Level2_Item_Click);
		this.dtbcb.AutoSize = true;
		this.dtbcb.Location = new System.Drawing.Point(27, 323);
		this.dtbcb.Name = "dtbcb";
		this.dtbcb.Size = new System.Drawing.Size(80, 17);
		this.dtbcb.TabIndex = 17;
		this.dtbcb.Text = "Dtb/Meson";
		this.dtbcb.UseVisualStyleBackColor = true;
		this.dtbcb.Click += new System.EventHandler(Level2_Item_Click);
		this.launcher.AutoSize = true;
		this.launcher.Location = new System.Drawing.Point(27, 450);
		this.launcher.Name = "launcher";
		this.launcher.Size = new System.Drawing.Size(71, 17);
		this.launcher.TabIndex = 21;
		this.launcher.Text = "Launcher";
		this.launcher.UseVisualStyleBackColor = true;
		this.launcher.Click += new System.EventHandler(Level3_Item_Click);
		this.bootvideocb.AutoSize = true;
		this.bootvideocb.Location = new System.Drawing.Point(27, 427);
		this.bootvideocb.Name = "bootvideocb";
		this.bootvideocb.Size = new System.Drawing.Size(74, 17);
		this.bootvideocb.TabIndex = 20;
		this.bootvideocb.Text = "Bootvideo";
		this.bootvideocb.UseVisualStyleBackColor = true;
		this.bootvideocb.Click += new System.EventHandler(Level3_Item_Click);
		this.bootanimationcb.AutoSize = true;
		this.bootanimationcb.Location = new System.Drawing.Point(27, 404);
		this.bootanimationcb.Name = "bootanimationcb";
		this.bootanimationcb.Size = new System.Drawing.Size(93, 17);
		this.bootanimationcb.TabIndex = 19;
		this.bootanimationcb.Text = "Bootanimation";
		this.bootanimationcb.UseVisualStyleBackColor = true;
		this.bootanimationcb.Click += new System.EventHandler(Level3_Item_Click);
		this.logocb.AutoSize = true;
		this.logocb.Location = new System.Drawing.Point(27, 381);
		this.logocb.Name = "logocb";
		this.logocb.Size = new System.Drawing.Size(50, 17);
		this.logocb.TabIndex = 18;
		this.logocb.Text = "Logo";
		this.logocb.UseVisualStyleBackColor = true;
		this.logocb.Click += new System.EventHandler(logocb_Click);
		this.supercb.AutoSize = true;
		this.supercb.Location = new System.Drawing.Point(27, 93);
		this.supercb.Name = "supercb";
		this.supercb.Size = new System.Drawing.Size(54, 17);
		this.supercb.TabIndex = 22;
		this.supercb.Text = "Super";
		this.supercb.UseVisualStyleBackColor = true;
		this.supercb.Click += new System.EventHandler(Level2_Item_Click);
		this.odmextcb.AutoSize = true;
		this.odmextcb.Location = new System.Drawing.Point(27, 254);
		this.odmextcb.Name = "odmextcb";
		this.odmextcb.Size = new System.Drawing.Size(68, 17);
		this.odmextcb.TabIndex = 23;
		this.odmextcb.Text = "Odm_ext";
		this.odmextcb.UseVisualStyleBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(557, 516);
		base.Controls.Add(this.odmextcb);
		base.Controls.Add(this.supercb);
		base.Controls.Add(this.launcher);
		base.Controls.Add(this.bootvideocb);
		base.Controls.Add(this.bootanimationcb);
		base.Controls.Add(this.logocb);
		base.Controls.Add(this.dtbcb);
		base.Controls.Add(this.recoverycb);
		base.Controls.Add(this.bootcb);
		base.Controls.Add(this.systemextcb);
		base.Controls.Add(this.oem);
		base.Controls.Add(this.odmcb);
		base.Controls.Add(this.productcb);
		base.Controls.Add(this.vendorcb);
		base.Controls.Add(this.systemcb);
		base.Controls.Add(this.Cancel);
		base.Controls.Add(this.Unpack);
		base.Controls.Add(this.label4);
		base.Controls.Add(this.Level3);
		base.Controls.Add(this.Level2);
		base.Controls.Add(this.Level1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "UnpackLevel";
		this.Text = "Select unpack level";
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
