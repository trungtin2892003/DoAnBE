using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _apiKey = "your-sendgrid-api-key";

    public async Task SendEmailAsync(string toEmail, string userName, string v)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress("shopcake@gmail.com", "Shop bánh ngọt");
        var subject = "Đăng ký thành công";
        var to = new EmailAddress(toEmail, userName);
        var plainTextContent = "Chào bạn, đăng ký của bạn đã thành công!";
        var htmlContent = $"<h1>Chào {userName},</h1><p>Đăng ký của bạn đã thành công!</p>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

        var response = await client.SendEmailAsync(msg);
        Console.WriteLine("Email sent successfully!");
    }
}
