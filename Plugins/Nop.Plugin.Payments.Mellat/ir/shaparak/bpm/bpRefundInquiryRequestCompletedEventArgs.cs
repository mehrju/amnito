using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x02000011 RID: 17
	[GeneratedCode("System.Web.Services", "4.6.79.0"), DebuggerStepThrough, DesignerCategory("code")]
	public class bpRefundInquiryRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x0000232D File Offset: 0x0000052D
		internal bpRefundInquiryRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00005240 File Offset: 0x00003440
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x0400003A RID: 58
		private object[] results;
	}
}
