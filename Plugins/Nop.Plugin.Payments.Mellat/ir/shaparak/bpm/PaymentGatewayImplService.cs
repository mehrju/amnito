using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Schema;
using System.Xml.Serialization;
using Nop.Plugin.Payments.Mellat.Properties;

namespace Nop.Plugin.Payments.Mellat.ir.shaparak.bpm
{
	// Token: 0x0200000D RID: 13
	[DebuggerStepThrough, GeneratedCode("System.Web.Services", "4.6.79.0"), WebServiceBinding(Name = "PaymentGatewayImplServiceSoapBinding", Namespace = "http://service.pgw.sw.bps.com/"), DesignerCategory("code")]
	public class PaymentGatewayImplService : SoapHttpClientProtocol
	{
		// Token: 0x0600005F RID: 95 RVA: 0x00003D70 File Offset: 0x00001F70
		public PaymentGatewayImplService()
		{
			this.Url = Settings.Default.Nop_Plugin_Payments_Mellat_ir_shaparak_bpm_PaymentGatewayImplService;
			if (this.IsLocalFileSystemWebService(this.Url))
			{
				this.UseDefaultCredentials = true;
				this.useDefaultCredentialsSetExplicitly = false;
			}
			else
			{
				this.useDefaultCredentialsSetExplicitly = true;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000060 RID: 96 RVA: 0x00003DB8 File Offset: 0x00001FB8
		// (set) Token: 0x06000061 RID: 97 RVA: 0x00003DD0 File Offset: 0x00001FD0
		public new string Url
		{
			get
			{
				return base.Url;
			}
			set
			{
				if (this.IsLocalFileSystemWebService(base.Url) && !this.useDefaultCredentialsSetExplicitly && !this.IsLocalFileSystemWebService(value))
				{
					base.UseDefaultCredentials = false;
				}
				base.Url = value;
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000062 RID: 98 RVA: 0x00003E10 File Offset: 0x00002010
		// (set) Token: 0x06000063 RID: 99 RVA: 0x00002285 File Offset: 0x00000485
		public new bool UseDefaultCredentials
		{
			get
			{
				return base.UseDefaultCredentials;
			}
			set
			{
				base.UseDefaultCredentials = value;
				this.useDefaultCredentialsSetExplicitly = true;
			}
		}

		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000064 RID: 100 RVA: 0x00003E24 File Offset: 0x00002024
		// (remove) Token: 0x06000065 RID: 101 RVA: 0x00003E5C File Offset: 0x0000205C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpVerifyRequestCompletedEventHandler bpVerifyRequestCompleted;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000066 RID: 102 RVA: 0x00003E94 File Offset: 0x00002094
		// (remove) Token: 0x06000067 RID: 103 RVA: 0x00003ECC File Offset: 0x000020CC
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpRefundInquiryRequestCompletedEventHandler bpRefundInquiryRequestCompleted;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000068 RID: 104 RVA: 0x00003F04 File Offset: 0x00002104
		// (remove) Token: 0x06000069 RID: 105 RVA: 0x00003F3C File Offset: 0x0000213C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpRefundVerifyRequestCompletedEventHandler bpRefundVerifyRequestCompleted;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x0600006A RID: 106 RVA: 0x00003F74 File Offset: 0x00002174
		// (remove) Token: 0x0600006B RID: 107 RVA: 0x00003FAC File Offset: 0x000021AC
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpSettleRequestCompletedEventHandler bpSettleRequestCompleted;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600006C RID: 108 RVA: 0x00003FE4 File Offset: 0x000021E4
		// (remove) Token: 0x0600006D RID: 109 RVA: 0x0000401C File Offset: 0x0000221C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpDynamicPayRequestCompletedEventHandler bpDynamicPayRequestCompleted;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x0600006E RID: 110 RVA: 0x00004054 File Offset: 0x00002254
		// (remove) Token: 0x0600006F RID: 111 RVA: 0x0000408C File Offset: 0x0000228C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpReversalRequestCompletedEventHandler bpReversalRequestCompleted;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000070 RID: 112 RVA: 0x000040C4 File Offset: 0x000022C4
		// (remove) Token: 0x06000071 RID: 113 RVA: 0x000040FC File Offset: 0x000022FC
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpCumulativeDynamicPayRequestCompletedEventHandler bpCumulativeDynamicPayRequestCompleted;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000072 RID: 114 RVA: 0x00004134 File Offset: 0x00002334
		// (remove) Token: 0x06000073 RID: 115 RVA: 0x0000416C File Offset: 0x0000236C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpPayRequestCompletedEventHandler bpPayRequestCompleted;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000074 RID: 116 RVA: 0x000041A4 File Offset: 0x000023A4
		// (remove) Token: 0x06000075 RID: 117 RVA: 0x000041DC File Offset: 0x000023DC
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpSaleReferenceIdRequestCompletedEventHandler bpSaleReferenceIdRequestCompleted;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000076 RID: 118 RVA: 0x00004214 File Offset: 0x00002414
		// (remove) Token: 0x06000077 RID: 119 RVA: 0x0000424C File Offset: 0x0000244C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpChargePayRequestCompletedEventHandler bpChargePayRequestCompleted;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000078 RID: 120 RVA: 0x00004284 File Offset: 0x00002484
		// (remove) Token: 0x06000079 RID: 121 RVA: 0x000042BC File Offset: 0x000024BC
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpInquiryRequestCompletedEventHandler bpInquiryRequestCompleted;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x0600007A RID: 122 RVA: 0x000042F4 File Offset: 0x000024F4
		// (remove) Token: 0x0600007B RID: 123 RVA: 0x0000432C File Offset: 0x0000252C
		[method: CompilerGenerated]
		[CompilerGenerated]
		public event bpRefundRequestCompletedEventHandler bpRefundRequestCompleted;

		// Token: 0x0600007C RID: 124 RVA: 0x00004364 File Offset: 0x00002564
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpVerifyRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long saleOrderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long saleReferenceId)
		{
			object[] array = base.Invoke("bpVerifyRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId,
				saleReferenceId
			});
			return (string)array[0];
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00002295 File Offset: 0x00000495
		public void bpVerifyRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId, long saleReferenceId)
		{
			this.bpVerifyRequestAsync(terminalId, userName, userPassword, orderId, saleOrderId, saleReferenceId, null);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000043BC File Offset: 0x000025BC
		public void bpVerifyRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId, long saleReferenceId, object userState)
		{
			if (this.bpVerifyRequestOperationCompleted == null)
			{
				this.bpVerifyRequestOperationCompleted = new SendOrPostCallback(this.OnbpVerifyRequestOperationCompleted);
			}
			base.InvokeAsync("bpVerifyRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId,
				saleReferenceId
			}, this.bpVerifyRequestOperationCompleted, userState);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00004430 File Offset: 0x00002630
		private void OnbpVerifyRequestOperationCompleted(object arg)
		{
			if (this.bpVerifyRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpVerifyRequestCompleted(this, new bpVerifyRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004478 File Offset: 0x00002678
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpRefundInquiryRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long refundOrderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long refundReferenceId)
		{
			object[] array = base.Invoke("bpRefundInquiryRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				refundOrderId,
				refundReferenceId
			});
			return (string)array[0];
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000022A7 File Offset: 0x000004A7
		public void bpRefundInquiryRequestAsync(long terminalId, string userName, string userPassword, long orderId, long refundOrderId, long refundReferenceId)
		{
			this.bpRefundInquiryRequestAsync(terminalId, userName, userPassword, orderId, refundOrderId, refundReferenceId, null);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000044D0 File Offset: 0x000026D0
		public void bpRefundInquiryRequestAsync(long terminalId, string userName, string userPassword, long orderId, long refundOrderId, long refundReferenceId, object userState)
		{
			if (this.bpRefundInquiryRequestOperationCompleted == null)
			{
				this.bpRefundInquiryRequestOperationCompleted = new SendOrPostCallback(this.OnbpRefundInquiryRequestOperationCompleted);
			}
			base.InvokeAsync("bpRefundInquiryRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				refundOrderId,
				refundReferenceId
			}, this.bpRefundInquiryRequestOperationCompleted, userState);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x00004544 File Offset: 0x00002744
		private void OnbpRefundInquiryRequestOperationCompleted(object arg)
		{
			if (this.bpRefundInquiryRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpRefundInquiryRequestCompleted(this, new bpRefundInquiryRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000458C File Offset: 0x0000278C
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpRefundVerifyRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long refundOrderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long refundReferenceId)
		{
			object[] array = base.Invoke("bpRefundVerifyRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				refundOrderId,
				refundReferenceId
			});
			return (string)array[0];
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000022B9 File Offset: 0x000004B9
		public void bpRefundVerifyRequestAsync(long terminalId, string userName, string userPassword, long orderId, long refundOrderId, long refundReferenceId)
		{
			this.bpRefundVerifyRequestAsync(terminalId, userName, userPassword, orderId, refundOrderId, refundReferenceId, null);
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000045E4 File Offset: 0x000027E4
		public void bpRefundVerifyRequestAsync(long terminalId, string userName, string userPassword, long orderId, long refundOrderId, long refundReferenceId, object userState)
		{
			if (this.bpRefundVerifyRequestOperationCompleted == null)
			{
				this.bpRefundVerifyRequestOperationCompleted = new SendOrPostCallback(this.OnbpRefundVerifyRequestOperationCompleted);
			}
			base.InvokeAsync("bpRefundVerifyRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				refundOrderId,
				refundReferenceId
			}, this.bpRefundVerifyRequestOperationCompleted, userState);
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004658 File Offset: 0x00002858
		private void OnbpRefundVerifyRequestOperationCompleted(object arg)
		{
			if (this.bpRefundVerifyRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpRefundVerifyRequestCompleted(this, new bpRefundVerifyRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000046A0 File Offset: 0x000028A0
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpSettleRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long saleOrderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long saleReferenceId)
		{
			object[] array = base.Invoke("bpSettleRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId,
				saleReferenceId
			});
			return (string)array[0];
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000022CB File Offset: 0x000004CB
		public void bpSettleRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId, long saleReferenceId)
		{
			this.bpSettleRequestAsync(terminalId, userName, userPassword, orderId, saleOrderId, saleReferenceId, null);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000046F8 File Offset: 0x000028F8
		public void bpSettleRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId, long saleReferenceId, object userState)
		{
			if (this.bpSettleRequestOperationCompleted == null)
			{
				this.bpSettleRequestOperationCompleted = new SendOrPostCallback(this.OnbpSettleRequestOperationCompleted);
			}
			base.InvokeAsync("bpSettleRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId,
				saleReferenceId
			}, this.bpSettleRequestOperationCompleted, userState);
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000476C File Offset: 0x0000296C
		private void OnbpSettleRequestOperationCompleted(object arg)
		{
			if (this.bpSettleRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpSettleRequestCompleted(this, new bpSettleRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000047B4 File Offset: 0x000029B4
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpDynamicPayRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long amount, [XmlElement(Form = XmlSchemaForm.Unqualified)] string localDate, [XmlElement(Form = XmlSchemaForm.Unqualified)] string localTime, [XmlElement(Form = XmlSchemaForm.Unqualified)] string additionalData, [XmlElement(Form = XmlSchemaForm.Unqualified)] string callBackUrl, [XmlElement(Form = XmlSchemaForm.Unqualified)] long payerId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long subServiceId)
		{
			object[] array = base.Invoke("bpDynamicPayRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				amount,
				localDate,
				localTime,
				additionalData,
				callBackUrl,
				payerId,
				subServiceId
			});
			return (string)array[0];
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00004830 File Offset: 0x00002A30
		public void bpDynamicPayRequestAsync(long terminalId, string userName, string userPassword, long orderId, long amount, string localDate, string localTime, string additionalData, string callBackUrl, long payerId, long subServiceId)
		{
			this.bpDynamicPayRequestAsync(terminalId, userName, userPassword, orderId, amount, localDate, localTime, additionalData, callBackUrl, payerId, subServiceId, null);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00004858 File Offset: 0x00002A58
		public void bpDynamicPayRequestAsync(long terminalId, string userName, string userPassword, long orderId, long amount, string localDate, string localTime, string additionalData, string callBackUrl, long payerId, long subServiceId, object userState)
		{
			if (this.bpDynamicPayRequestOperationCompleted == null)
			{
				this.bpDynamicPayRequestOperationCompleted = new SendOrPostCallback(this.OnbpDynamicPayRequestOperationCompleted);
			}
			base.InvokeAsync("bpDynamicPayRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				amount,
				localDate,
				localTime,
				additionalData,
				callBackUrl,
				payerId,
				subServiceId
			}, this.bpDynamicPayRequestOperationCompleted, userState);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000048EC File Offset: 0x00002AEC
		private void OnbpDynamicPayRequestOperationCompleted(object arg)
		{
			if (this.bpDynamicPayRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpDynamicPayRequestCompleted(this, new bpDynamicPayRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00004934 File Offset: 0x00002B34
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpReversalRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long saleOrderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long saleReferenceId)
		{
			object[] array = base.Invoke("bpReversalRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId,
				saleReferenceId
			});
			return (string)array[0];
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000022DD File Offset: 0x000004DD
		public void bpReversalRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId, long saleReferenceId)
		{
			this.bpReversalRequestAsync(terminalId, userName, userPassword, orderId, saleOrderId, saleReferenceId, null);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000498C File Offset: 0x00002B8C
		public void bpReversalRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId, long saleReferenceId, object userState)
		{
			if (this.bpReversalRequestOperationCompleted == null)
			{
				this.bpReversalRequestOperationCompleted = new SendOrPostCallback(this.OnbpReversalRequestOperationCompleted);
			}
			base.InvokeAsync("bpReversalRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId,
				saleReferenceId
			}, this.bpReversalRequestOperationCompleted, userState);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00004A00 File Offset: 0x00002C00
		private void OnbpReversalRequestOperationCompleted(object arg)
		{
			if (this.bpReversalRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpReversalRequestCompleted(this, new bpReversalRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00004A48 File Offset: 0x00002C48
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpCumulativeDynamicPayRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long amount, [XmlElement(Form = XmlSchemaForm.Unqualified)] string localDate, [XmlElement(Form = XmlSchemaForm.Unqualified)] string localTime, [XmlElement(Form = XmlSchemaForm.Unqualified)] string additionalData, [XmlElement(Form = XmlSchemaForm.Unqualified)] string callBackUrl)
		{
			object[] array = base.Invoke("bpCumulativeDynamicPayRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				amount,
				localDate,
				localTime,
				additionalData,
				callBackUrl
			});
			return (string)array[0];
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00004AAC File Offset: 0x00002CAC
		public void bpCumulativeDynamicPayRequestAsync(long terminalId, string userName, string userPassword, long orderId, long amount, string localDate, string localTime, string additionalData, string callBackUrl)
		{
			this.bpCumulativeDynamicPayRequestAsync(terminalId, userName, userPassword, orderId, amount, localDate, localTime, additionalData, callBackUrl, null);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00004AD0 File Offset: 0x00002CD0
		public void bpCumulativeDynamicPayRequestAsync(long terminalId, string userName, string userPassword, long orderId, long amount, string localDate, string localTime, string additionalData, string callBackUrl, object userState)
		{
			if (this.bpCumulativeDynamicPayRequestOperationCompleted == null)
			{
				this.bpCumulativeDynamicPayRequestOperationCompleted = new SendOrPostCallback(this.OnbpCumulativeDynamicPayRequestOperationCompleted);
			}
			base.InvokeAsync("bpCumulativeDynamicPayRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				amount,
				localDate,
				localTime,
				additionalData,
				callBackUrl
			}, this.bpCumulativeDynamicPayRequestOperationCompleted, userState);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004B50 File Offset: 0x00002D50
		private void OnbpCumulativeDynamicPayRequestOperationCompleted(object arg)
		{
			if (this.bpCumulativeDynamicPayRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpCumulativeDynamicPayRequestCompleted(this, new bpCumulativeDynamicPayRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00004B98 File Offset: 0x00002D98
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpPayRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long amount, [XmlElement(Form = XmlSchemaForm.Unqualified)] string localDate, [XmlElement(Form = XmlSchemaForm.Unqualified)] string localTime, [XmlElement(Form = XmlSchemaForm.Unqualified)] string additionalData, [XmlElement(Form = XmlSchemaForm.Unqualified)] string callBackUrl, [XmlElement(Form = XmlSchemaForm.Unqualified)] long payerId)
		{
			object[] array = base.Invoke("bpPayRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				amount,
				localDate,
				localTime,
				additionalData,
				callBackUrl,
				payerId
			});
			return (string)array[0];
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00004C08 File Offset: 0x00002E08
		public void bpPayRequestAsync(long terminalId, string userName, string userPassword, long orderId, long amount, string localDate, string localTime, string additionalData, string callBackUrl, long payerId)
		{
			this.bpPayRequestAsync(terminalId, userName, userPassword, orderId, amount, localDate, localTime, additionalData, callBackUrl, payerId, null);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00004C30 File Offset: 0x00002E30
		public void bpPayRequestAsync(long terminalId, string userName, string userPassword, long orderId, long amount, string localDate, string localTime, string additionalData, string callBackUrl, long payerId, object userState)
		{
			if (this.bpPayRequestOperationCompleted == null)
			{
				this.bpPayRequestOperationCompleted = new SendOrPostCallback(this.OnbpPayRequestOperationCompleted);
			}
			base.InvokeAsync("bpPayRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				amount,
				localDate,
				localTime,
				additionalData,
				callBackUrl,
				payerId
			}, this.bpPayRequestOperationCompleted, userState);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00004CB8 File Offset: 0x00002EB8
		private void OnbpPayRequestOperationCompleted(object arg)
		{
			if (this.bpPayRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpPayRequestCompleted(this, new bpPayRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004D00 File Offset: 0x00002F00
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpSaleReferenceIdRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long saleOrderId)
		{
			object[] array = base.Invoke("bpSaleReferenceIdRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId
			});
			return (string)array[0];
		}

		// Token: 0x0600009D RID: 157 RVA: 0x000022EF File Offset: 0x000004EF
		public void bpSaleReferenceIdRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId)
		{
			this.bpSaleReferenceIdRequestAsync(terminalId, userName, userPassword, orderId, saleOrderId, null);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004D50 File Offset: 0x00002F50
		public void bpSaleReferenceIdRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId, object userState)
		{
			if (this.bpSaleReferenceIdRequestOperationCompleted == null)
			{
				this.bpSaleReferenceIdRequestOperationCompleted = new SendOrPostCallback(this.OnbpSaleReferenceIdRequestOperationCompleted);
			}
			base.InvokeAsync("bpSaleReferenceIdRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId
			}, this.bpSaleReferenceIdRequestOperationCompleted, userState);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004DB8 File Offset: 0x00002FB8
		private void OnbpSaleReferenceIdRequestOperationCompleted(object arg)
		{
			if (this.bpSaleReferenceIdRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpSaleReferenceIdRequestCompleted(this, new bpSaleReferenceIdRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004E00 File Offset: 0x00003000
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpChargePayRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long amount, [XmlElement(Form = XmlSchemaForm.Unqualified)] string localDate, [XmlElement(Form = XmlSchemaForm.Unqualified)] string localTime, [XmlElement(Form = XmlSchemaForm.Unqualified)] string additionalData, [XmlElement(Form = XmlSchemaForm.Unqualified)] string callBackUrl, [XmlElement(Form = XmlSchemaForm.Unqualified)] long payerId)
		{
			object[] array = base.Invoke("bpChargePayRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				amount,
				localDate,
				localTime,
				additionalData,
				callBackUrl,
				payerId
			});
			return (string)array[0];
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00004E70 File Offset: 0x00003070
		public void bpChargePayRequestAsync(long terminalId, string userName, string userPassword, long orderId, long amount, string localDate, string localTime, string additionalData, string callBackUrl, long payerId)
		{
			this.bpChargePayRequestAsync(terminalId, userName, userPassword, orderId, amount, localDate, localTime, additionalData, callBackUrl, payerId, null);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004E98 File Offset: 0x00003098
		public void bpChargePayRequestAsync(long terminalId, string userName, string userPassword, long orderId, long amount, string localDate, string localTime, string additionalData, string callBackUrl, long payerId, object userState)
		{
			if (this.bpChargePayRequestOperationCompleted == null)
			{
				this.bpChargePayRequestOperationCompleted = new SendOrPostCallback(this.OnbpChargePayRequestOperationCompleted);
			}
			base.InvokeAsync("bpChargePayRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				amount,
				localDate,
				localTime,
				additionalData,
				callBackUrl,
				payerId
			}, this.bpChargePayRequestOperationCompleted, userState);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00004F20 File Offset: 0x00003120
		private void OnbpChargePayRequestOperationCompleted(object arg)
		{
			if (this.bpChargePayRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpChargePayRequestCompleted(this, new bpChargePayRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00004F68 File Offset: 0x00003168
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpInquiryRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long saleOrderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long saleReferenceId)
		{
			object[] array = base.Invoke("bpInquiryRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId,
				saleReferenceId
			});
			return (string)array[0];
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000022FF File Offset: 0x000004FF
		public void bpInquiryRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId, long saleReferenceId)
		{
			this.bpInquiryRequestAsync(terminalId, userName, userPassword, orderId, saleOrderId, saleReferenceId, null);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00004FC0 File Offset: 0x000031C0
		public void bpInquiryRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId, long saleReferenceId, object userState)
		{
			if (this.bpInquiryRequestOperationCompleted == null)
			{
				this.bpInquiryRequestOperationCompleted = new SendOrPostCallback(this.OnbpInquiryRequestOperationCompleted);
			}
			base.InvokeAsync("bpInquiryRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId,
				saleReferenceId
			}, this.bpInquiryRequestOperationCompleted, userState);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00005034 File Offset: 0x00003234
		private void OnbpInquiryRequestOperationCompleted(object arg)
		{
			if (this.bpInquiryRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpInquiryRequestCompleted(this, new bpInquiryRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000507C File Offset: 0x0000327C
		[SoapDocumentMethod("", RequestNamespace = "http://interfaces.core.sw.bps.com/", ResponseNamespace = "http://interfaces.core.sw.bps.com/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement("return", Form = XmlSchemaForm.Unqualified)]
		public string bpRefundRequest([XmlElement(Form = XmlSchemaForm.Unqualified)] long terminalId, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userName, [XmlElement(Form = XmlSchemaForm.Unqualified)] string userPassword, [XmlElement(Form = XmlSchemaForm.Unqualified)] long orderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long saleOrderId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long saleReferenceId, [XmlElement(Form = XmlSchemaForm.Unqualified)] long refundAmount)
		{
			object[] array = base.Invoke("bpRefundRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId,
				saleReferenceId,
				refundAmount
			});
			return (string)array[0];
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000050E0 File Offset: 0x000032E0
		public void bpRefundRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId, long saleReferenceId, long refundAmount)
		{
			this.bpRefundRequestAsync(terminalId, userName, userPassword, orderId, saleOrderId, saleReferenceId, refundAmount, null);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00005100 File Offset: 0x00003300
		public void bpRefundRequestAsync(long terminalId, string userName, string userPassword, long orderId, long saleOrderId, long saleReferenceId, long refundAmount, object userState)
		{
			if (this.bpRefundRequestOperationCompleted == null)
			{
				this.bpRefundRequestOperationCompleted = new SendOrPostCallback(this.OnbpRefundRequestOperationCompleted);
			}
			base.InvokeAsync("bpRefundRequest", new object[]
			{
				terminalId,
				userName,
				userPassword,
				orderId,
				saleOrderId,
				saleReferenceId,
				refundAmount
			}, this.bpRefundRequestOperationCompleted, userState);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000517C File Offset: 0x0000337C
		private void OnbpRefundRequestOperationCompleted(object arg)
		{
			if (this.bpRefundRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.bpRefundRequestCompleted(this, new bpRefundRequestCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00002311 File Offset: 0x00000511
		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000051C4 File Offset: 0x000033C4
		private bool IsLocalFileSystemWebService(string url)
		{
			bool result;
			if (url == null || url == string.Empty)
			{
				result = false;
			}
			else
			{
				Uri uri = new Uri(url);
				result = (uri.Port >= 1024 && string.Compare(uri.Host, "localHost", StringComparison.OrdinalIgnoreCase) == 0);
			}
			return result;
		}

		// Token: 0x04000020 RID: 32
		private SendOrPostCallback bpVerifyRequestOperationCompleted;

		// Token: 0x04000021 RID: 33
		private SendOrPostCallback bpRefundInquiryRequestOperationCompleted;

		// Token: 0x04000022 RID: 34
		private SendOrPostCallback bpRefundVerifyRequestOperationCompleted;

		// Token: 0x04000023 RID: 35
		private SendOrPostCallback bpSettleRequestOperationCompleted;

		// Token: 0x04000024 RID: 36
		private SendOrPostCallback bpDynamicPayRequestOperationCompleted;

		// Token: 0x04000025 RID: 37
		private SendOrPostCallback bpReversalRequestOperationCompleted;

		// Token: 0x04000026 RID: 38
		private SendOrPostCallback bpCumulativeDynamicPayRequestOperationCompleted;

		// Token: 0x04000027 RID: 39
		private SendOrPostCallback bpPayRequestOperationCompleted;

		// Token: 0x04000028 RID: 40
		private SendOrPostCallback bpSaleReferenceIdRequestOperationCompleted;

		// Token: 0x04000029 RID: 41
		private SendOrPostCallback bpChargePayRequestOperationCompleted;

		// Token: 0x0400002A RID: 42
		private SendOrPostCallback bpInquiryRequestOperationCompleted;

		// Token: 0x0400002B RID: 43
		private SendOrPostCallback bpRefundRequestOperationCompleted;

		// Token: 0x0400002C RID: 44
		private bool useDefaultCredentialsSetExplicitly;
	}
}
