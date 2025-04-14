using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Security;

namespace Service.EmailService
{
    public interface IEmailService
    {
        Task SendResetPasswordEmail(string toEmail, string resetlink);
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendResetPasswordEmail(string toEmail, string resetlink)
        {
            var stmp = _config.GetSection("SmtpSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(stmp["SenderName"], stmp["SenderEmail"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = "Reset Your Password";

            message.Body = new TextPart("html")
            {
                Text = $"<p>Click the link below to reset your password:</p>" +
                   $"<a href='{resetlink}'>Reset Password</a><br/><p>This link is valid for 1 hour.</p>"
            };


            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync(stmp["Server"], int.Parse(stmp["Port"]), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(stmp["Username"], stmp["Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);


            }
            catch (Exception ex)
            {
                throw new Exception("Error sending email", ex);
            }

        }
    }
}
