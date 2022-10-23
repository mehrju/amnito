using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x02000013 RID: 19
	[DesignerCategory("code"), DebuggerStepThrough, GeneratedCode("System.Web.Services", "4.6.79.0")]
	public class bpRefundVerifyRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000BE RID: 190 RVA: 0x00002340 File Offset: 0x00000540
		internal bpRefundVerifyRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00005264 File Offset: 0x00003464
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x0400003B RID: 59
		private object[] results;
	}
}
