using System.Net.Mail;

namespace CustomerManagement.Logic.Model;

public class EmailGateway : IEmailGateway
{
    public bool SendPromotionNotification(string email, CustomerStatus newStatus)
    {
        return TrySendEmail(email, "Congratulations!", "You've been promoted to " + newStatus);
    }

    private bool TrySendEmail(string to, string subject, string body)
    {
        var message = new MailMessage("noreply@northwind.com", to, subject, body);
        var client = new SmtpClient();

        try
        {
            client.Send(message);

            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}