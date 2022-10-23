using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x02000017 RID: 23
	[DebuggerStepThrough, GeneratedCode("System.Web.Services", "4.6.79.0"), DesignerCategory("code")]
	public class bpDynamicPayRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000CA RID: 202 RVA: 0x00002366 File Offset: 0x00000566
		internal bpDynamicPayRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000CB RID: 203 RVA: 0x000052AC File Offset: 0x000034AC
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x0400003D RID: 61
		private object[] results;
	}
}
