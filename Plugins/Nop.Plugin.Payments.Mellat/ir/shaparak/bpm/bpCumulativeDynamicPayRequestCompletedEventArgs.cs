using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x0200001B RID: 27
	[DesignerCategory("code"), DebuggerStepThrough, GeneratedCode("System.Web.Services", "4.6.79.0")]
	public class bpCumulativeDynamicPayRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000D6 RID: 214 RVA: 0x0000238C File Offset: 0x0000058C
		internal bpCumulativeDynamicPayRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x000052F4 File Offset: 0x000034F4
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x0400003F RID: 63
		private object[] results;
	}
}
