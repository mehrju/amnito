using Nop.Core;
using System;
using System.IO;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Logger
{
	public class FNSLogger
	{
		private readonly bool bool_0;

		private string string_0;

		private string string_1;

		public FNSLogger(bool showDebugInfo = false)
		{
			this.bool_0 = showDebugInfo;
			this.string_0 = PluginLog.SystemName;
			this.string_0.Replace(" ", "_");
			if (this.string_0.Length == 0)
			{
				this.string_0 = "foxnetsoft_log";
			}
			this.string_1 = CommonHelper.MapPath(string.Concat("~/App_Data/", this.string_0, "_log.txt"));
		}

		public void ClearLogFile()
		{
			try
			{
				if (File.Exists(this.string_1))
				{
					File.Delete(this.string_1);
				}
			}
			catch
			{
			}
		}

		public string GetLogFilePath()
		{
			return this.string_1;
		}

		public void LogMessage(string message)
		{
			try
			{
				if (this.bool_0)
				{
					DateTime now = DateTime.Now;
					message = string.Format("{0}*******{1}{2}", now.ToString("yyyy.MM.dd HH:mm:ss:ffff"), Environment.NewLine, message);
					try
					{
						if (File.Exists(this.string_1))
						{
							FileInfo fileInfo = new FileInfo(this.string_1);
							if (fileInfo.Length > 10485760L)
							{
								fileInfo.Delete();
							}
						}
					}
					catch
					{
					}
					using (FileStream fileStream = new FileStream(this.string_1, FileMode.Append, FileAccess.Write, FileShare.Read))
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream))
						{
							streamWriter.WriteLine(message);
						}
					}
				}
			}
			catch
			{
			}
		}
	}
}