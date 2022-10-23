using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x02000021 RID: 33
	[GeneratedCode("System.Web.Services", "4.6.79.0"), DebuggerStepThrough, DesignerCategory("code")]
	public class bpChargePayRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000E8 RID: 232 RVA: 0x000023C5 File Offset: 0x000005C5
		internal bpChargePayRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000E9 RID: 233 RVA: 0x00005360 File Offset: 0x00003560
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x04000042 RID: 66
		private object[] results;
	}
}
