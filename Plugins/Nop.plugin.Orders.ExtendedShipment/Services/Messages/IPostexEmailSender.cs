using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using System.Collections.Generic;
using System.IO;

namespace Nop.plugin.Orders.ExtendedShipment.Services.Messages
{
    public interface IPostexEmailSender
    {
        void SendEmail(EmailAccount emailAccount,
            string subject, string body, string fromAddress, string fromName,
            string toAddress, string toName, string replyTo = null, string replyToName = null, string replyToAddress = null,
             IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            string[] attachmentFileNames = null, IEnumerable<Stream> attachments = null,
            int attachedDownloadId = 0, IDictionary<string, string> headers = null);
    }
}