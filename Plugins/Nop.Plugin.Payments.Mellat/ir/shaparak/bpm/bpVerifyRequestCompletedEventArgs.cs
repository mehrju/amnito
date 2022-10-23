using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x0200000F RID: 15
	[DesignerCategory("code"), GeneratedCode("System.Web.Services", "4.6.79.0"), DebuggerStepThrough]
	public class bpVerifyRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000B2 RID: 178 RVA: 0x0000231A File Offset: 0x0000051A
		internal bpVerifyRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x0000521C File Offset: 0x0000341C
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x04000039 RID: 57
		private object[] results;
	}
}
