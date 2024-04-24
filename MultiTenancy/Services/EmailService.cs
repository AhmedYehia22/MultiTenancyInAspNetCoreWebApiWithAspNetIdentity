using System.Net;
using System.Net.Mail;

namespace MultiTenancy.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML)
        {
            string MailServer = _config["EmailSettings:MailServer"];
            string FromEmail = _config["EmailSettings:FromEmail"];
            string Password = _config["EmailSettings:Password"];
            int Port = int.Parse(_config["EmailSettings:MailPort"]);

            var client = new SmtpClient(MailServer, Port)
            {
                Credentials = new NetworkCredential(FromEmail, Password),
                EnableSsl = true,
            };

            MailMessage mailMessage = new MailMessage(FromEmail, toEmail, subject, body)
            {
                IsBodyHtml = isBodyHTML
            };
            try
            {

                // return client.SendMailAsync(mailMessage);
              await  client.SendMailAsync(mailMessage);
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
    }
}
