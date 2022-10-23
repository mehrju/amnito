using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Messages;
using Nop.Services.Media;

namespace Nop.plugin.Orders.ExtendedShipment.Services.Messages
{
    public class PostexEmailSender : IPostexEmailSender
    {
        private readonly IDownloadService _downloadService;

        public PostexEmailSender(IDownloadService downloadService)
        {
            _downloadService = downloadService;
        }

        public void SendEmail(EmailAccount emailAccount,
            string subject, string body, string fromAddress, string fromName,
            string toAddress, string toName, string replyTo = null, string replyToName = null, string replyToAddress = null,
             IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
            string[] attachmentFileNames = null, IEnumerable<Stream> attachments = null,
            int attachedDownloadId = 0, IDictionary<string, string> headers = null)
        {
            var message = new MailMessage
            {
                //from, to, reply to
                From = new MailAddress(fromAddress, fromName)
            };
            message.To.Add(new MailAddress(toAddress, toName));
            if (!string.IsNullOrEmpty(replyTo))
            {
                message.ReplyToList.Add(new MailAddress(replyTo, replyToName));
            }

            //BCC
            if (bcc != null)
            {
                foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }

            //CC
            if (cc != null)
            {
                foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                {
                    message.CC.Add(address.Trim());
                }
            }

            //content
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            //headers
            if (headers != null)
                foreach (var header in headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }

            //create the file attachment for this e-mail message
            int i = 0;
            foreach (var attachStream in attachments)
            {
                var attachment = new Attachment(attachStream, attachmentFileNames[i++]);
                attachment.NameEncoding = Encoding.Unicode;
                //attachment.ContentDisposition.CreationDate = File.GetCreationTime(attachmentFilePath);
                //attachment.ContentDisposition.ModificationDate = File.GetLastWriteTime(attachmentFilePath);
                //attachment.ContentDisposition.ReadDate = File.GetLastAccessTime(attachmentFilePath);
                message.Attachments.Add(attachment);
            }

            //another attachment?
            if (attachedDownloadId > 0)
            {
                var download = _downloadService.GetDownloadById(attachedDownloadId);
                if (download != null)
                {
                    //we do not support URLs as attachments
                    if (!download.UseDownloadUrl)
                    {
                        var fileName = !string.IsNullOrWhiteSpace(download.Filename) ? download.Filename : download.Id.ToString();
                        fileName += download.Extension;


                        var ms = new MemoryStream(download.DownloadBinary);
                        var attachment = new Attachment(ms, fileName);
                        //string contentType = !string.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : "application/octet-stream";
                        //var attachment = new Attachment(ms, fileName, contentType);
                        attachment.ContentDisposition.CreationDate = DateTime.UtcNow;
                        attachment.ContentDisposition.ModificationDate = DateTime.UtcNow;
                        attachment.ContentDisposition.ReadDate = DateTime.UtcNow;
                        message.Attachments.Add(attachment);
                    }
                }
            }

            //send email
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.UseDefaultCredentials = emailAccount.UseDefaultCredentials;
                smtpClient.Host = emailAccount.Host;
                smtpClient.Port = emailAccount.Port;
                smtpClient.EnableSsl = emailAccount.EnableSsl;
                smtpClient.Credentials = emailAccount.UseDefaultCredentials ?
                    CredentialCache.DefaultNetworkCredentials :
                    new NetworkCredential(emailAccount.Username, emailAccount.Password);
                smtpClient.Send(message);
            }
        }
    }
}
