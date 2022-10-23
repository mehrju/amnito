using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x02000025 RID: 37
	[DesignerCategory("code"), GeneratedCode("System.Web.Services", "4.6.79.0"), DebuggerStepThrough]
	public class bpRefundRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000F4 RID: 244 RVA: 0x000023EB File Offset: 0x000005EB
		internal bpRefundRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000F5 RID: 245 RVA: 0x000053A8 File Offset: 0x000035A8
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x04000044 RID: 68
		private object[] results;
	}
}
