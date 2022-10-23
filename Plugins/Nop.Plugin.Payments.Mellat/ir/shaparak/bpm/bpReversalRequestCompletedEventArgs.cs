using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x02000019 RID: 25
	[GeneratedCode("System.Web.Services", "4.6.79.0"), DebuggerStepThrough, DesignerCategory("code")]
	public class bpReversalRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000D0 RID: 208 RVA: 0x00002379 File Offset: 0x00000579
		internal bpReversalRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x000052D0 File Offset: 0x000034D0
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x0400003E RID: 62
		private object[] results;
	}
}
