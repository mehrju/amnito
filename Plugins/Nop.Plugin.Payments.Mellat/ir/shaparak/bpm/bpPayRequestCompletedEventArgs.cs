using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x0200001D RID: 29
	[GeneratedCode("System.Web.Services", "4.6.79.0"), DebuggerStepThrough, DesignerCategory("code")]
	public class bpPayRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000DC RID: 220 RVA: 0x0000239F File Offset: 0x0000059F
		internal bpPayRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000DD RID: 221 RVA: 0x00005318 File Offset: 0x00003518
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x04000040 RID: 64
		private object[] results;
	}
}
