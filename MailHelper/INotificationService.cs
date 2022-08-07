using System.Collections.Generic;
using System.Threading.Tasks;

namespace MailHelper
{
    public interface INotificationService
    {
        Task<bool> SendMailNotification(List<string> toEmailIds, string data, string batchId);
        Task<bool> SendMailWithAttachment(EmailAttachmentDetails model);

    }
}
