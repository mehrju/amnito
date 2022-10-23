using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x0200001F RID: 31
	[GeneratedCode("System.Web.Services", "4.6.79.0"), DebuggerStepThrough, DesignerCategory("code")]
	public class bpSaleReferenceIdRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000E2 RID: 226 RVA: 0x000023B2 File Offset: 0x000005B2
		internal bpSaleReferenceIdRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000E3 RID: 227 RVA: 0x0000533C File Offset: 0x0000353C
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x04000041 RID: 65
		private object[] results;
	}
}
