using System.Net.Mail;
using CustomerManagement.Logic.Common;

namespace CustomerManagement.Logic.Model;

public class EmailGateway : IEmailGateway
{
    public Result SendPromotionNotification(string email, CustomerStatus newStatus)
    {
        return TrySendEmail(email, "Congratulations!", "You've been promoted to " + newStatus);
    }

    private Result TrySendEmail(string to, string subject, string body)
    {
        var message = new MailMessage("noreply@northwind.com", to, subject, body);
        var client = new SmtpClient();

        try
        {
            client.Send(message);

            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail("Unable to send email");
        }
    }
}