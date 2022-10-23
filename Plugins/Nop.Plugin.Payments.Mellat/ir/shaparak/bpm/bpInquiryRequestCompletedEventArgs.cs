using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x02000023 RID: 35
	[DesignerCategory("code"), GeneratedCode("System.Web.Services", "4.6.79.0"), DebuggerStepThrough]
	public class bpInquiryRequestCompletedEventArgs : AsyncCompletedEventArgs
	{
		// Token: 0x060000EE RID: 238 RVA: 0x000023D8 File Offset: 0x000005D8
		internal bpInquiryRequestCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
		{
			this.results = results;
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000EF RID: 239 RVA: 0x00005384 File Offset: 0x00003584
		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return (string)this.results[0];
			}
		}

		// Token: 0x04000043 RID: 67
		private object[] results;
	}
}
