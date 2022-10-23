using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x02000015 RID: 21
	[GeneratedCode("System.Web.Services", "4.6.79.0"), DebuggerStepThrough, DesignerCategory("code")]
	public class bpSettleRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000C4 RID: 196 RVA: 0x00002353 File Offset: 0x00000553
		internal bpSettleRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00005288 File Offset: 0x00003488
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x0400003C RID: 60
		private object[] results;
	}
}
