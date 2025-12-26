using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CustomizationTool.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.14.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

	public static Settings Default => defaultInstance;

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool PythonSuperUtils
	{
		get
		{
			return (bool)this["PythonSuperUtils"];
		}
		set
		{
			this["PythonSuperUtils"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool NewDtbProcessingTools
	{
		get
		{
			return (bool)this["NewDtbProcessingTools"];
		}
		set
		{
			this["NewDtbProcessingTools"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool NewLogoProcessingTools
	{
		get
		{
			return (bool)this["NewLogoProcessingTools"];
		}
		set
		{
			this["NewLogoProcessingTools"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool PythonImgextractor
	{
		get
		{
			return (bool)this["PythonImgextractor"];
		}
		set
		{
			this["PythonImgextractor"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool ArchMake_ext4fs
	{
		get
		{
			return (bool)this["ArchMake_ext4fs"];
		}
		set
		{
			this["ArchMake_ext4fs"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool PythonSimg2Img
	{
		get
		{
			return (bool)this["PythonSimg2Img"];
		}
		set
		{
			this["PythonSimg2Img"] = value;
		}
	}
}
