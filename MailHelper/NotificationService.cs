using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MailHelper
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;
        public NotificationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> SendMailNotification(List<string> toEmailIds, string emailTemplate,
            string batchId)
        {
            var isMailSend = false;
            try
            {

                SmtpClient client = new SmtpClient();
                MimeMessage message = new MimeMessage();

                MailboxAddress from = new MailboxAddress(_configuration.GetSection("EmailNotification:FromName").Value,
                _configuration.GetSection("EmailNotification:FromEmail").Value);
                message.From.Add(from);

                MailboxAddress to = new MailboxAddress("User",
                "BhaweshDeepak@yahoo.com");

                List<MailboxAddress> toMailAddress = new List<MailboxAddress>();
                toEmailIds.ForEach(item =>
                {
                    var userName = item.Split("@")[0];
                    toMailAddress.Add(new MailboxAddress(userName, item.Trim()));
                });


                message.To.AddRange(toMailAddress);

                message.Subject = batchId;

                BodyBuilder bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = emailTemplate;


                message.Body = bodyBuilder.ToMessageBody();

                try
                {

                    client.Connect(_configuration.GetSection("EmailNotification:SMTPServer").Value,
                        Convert.ToInt32(_configuration.GetSection("EmailNotification:SMTPPort").Value), SecureSocketOptions.StartTls);

                    client.Authenticate("stcvat1@gmail.com", "vi@pra91");

                    client.Send(message);
                    isMailSend = true;

                }
                catch (Exception ex)
                {
                    //await WriteEmailLoger(ex);
                    isMailSend = false;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                await WriteEmailLoger(ex);
                isMailSend = false;
            }

            return await Task.Run(() => isMailSend);
        }

        public async Task<bool> SendMailWithAttachment(EmailAttachmentDetails model)
        {
            SmtpClient client = new SmtpClient();
            bool isMailSend = false;

            MimeMessage message = new MimeMessage();

            MailboxAddress from = new MailboxAddress(_configuration.GetSection("EmailNotification:FromName").Value,
            _configuration.GetSection("EmailNotification:FromEmail").Value);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress("User",
            "BhaweshDeepak@yahoo.com");
            List<MailboxAddress> toMailAddress = new List<MailboxAddress>();
            model.ToEmailIds.ForEach(item =>
            {
                var userName = item.Split("@")[0];
                toMailAddress.Add(new MailboxAddress(userName, item.Trim()));
            });


            message.To.AddRange(toMailAddress);

            message.Subject = "Request Details";

            BodyBuilder emailBodyBuilder = new BodyBuilder();

            byte[] attachmentFileByteArray;
            foreach (IFormFile attachmentFile in model.Attachments)
            {
                if (attachmentFile.Length > 0)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        attachmentFile.CopyTo(memoryStream);
                        attachmentFileByteArray = memoryStream.ToArray();
                    }
                    emailBodyBuilder.Attachments.Add(attachmentFile.FileName, attachmentFileByteArray, ContentType.Parse(attachmentFile.ContentType));
                }
            }

            emailBodyBuilder.HtmlBody = await GetTempHtmlData(model.Category, model.Priority, model.Description);
            message.Body = emailBodyBuilder.ToMessageBody();

            try
            {

                client.Connect(_configuration.GetSection("EmailNotification:SMTPServer").Value,
                    Convert.ToInt32(_configuration.GetSection("EmailNotification:SMTPPort").Value), SecureSocketOptions.StartTls);

                client.Authenticate("itbasserahbasserah@gmail.com", "vi@pra91");

                client.Send(message);
                isMailSend = true;

            }
            catch (Exception ex)
            {
                isMailSend = false;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
            return await Task.Run(() => isMailSend);
        }

        private async Task WriteEmailLoger(Exception ex)
        {

            string filePath = _configuration.GetSection("EmailNotification:LoggerPath").Value;

            using StreamWriter outputFile = new StreamWriter(filePath);
            await Task.Run(() => outputFile.WriteLine($"Message: {ex.Message} " +
                $"  inner exception {ex.InnerException}"));
        }

        private async Task<string> GetTempHtmlData(string categoryName, string priority, string description)
        {
            string tempHtml = $@" <h4>Catgory Name: {categoryName}</h4>
                                 <h4>Priority Name: {priority}</h4> 
                                 <h4>Description Name: {description}</h4>";
            return tempHtml;
        }
        private async Task<string> GethtmlTableForPriority(string categoryName, string priority, string description)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(@"<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Tempmlte</title>
</head>
<body>
    <div class='card' style='text-align:center; width:500px; margin-right:auto; margin-left:auto'>
        <div class=''>
            <h3 class='new-request-text tex-left'>New Request</h3>
            <hr style='background-color: orange; height: 1px; border: 0' />
        </div>

        <div class='card-body' style='text-align:center; width:400px; margin-right:auto; margin-left:auto'>");

            sb.Append(@"<table class='table table-bordered' border='3' style='box-sizing: border-box;border-collapse: collapse!important;width: 100%;max-width: 100%;margin-bottom: 1rem;background-color: transparent;border: 1px solid #dee2e6;'>
                <tbody style='border:2px solid green'>");

            sb.AppendFormat($@"<tbody style='border:2px solid green'> <tr'>
                <th>Category Name</th>
                <th>{categoryName}</th>
              </tr>
              <tr>
                <th>Priority Name</th>
                <th>{priority}</th>
              </tr> <tr>
                <th>Description</th>
                <th>{description}</th>
              </tr>");

            sb.Append(@"</tbody>
            </table>
            </div>
        </div>
</body>
</html>");

            return await Task.Run(() => sb.ToString());

        }
    }
}
