using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Services
{
    public static class AuthMessageSender 
    {
        public static Task SendEmailAsync(string destinationMail, string subject, string message)
        {
            var mineMessage = new MimeMessage();
            mineMessage.From.Add(new MailboxAddress("Association Stolons", Configurations.ApplicationConfig.StolonsMailAdress));
            mineMessage.To.Add(new MailboxAddress(destinationMail, destinationMail));
            mineMessage.Subject = subject;

            mineMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, true);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(Configurations.ApplicationConfig.StolonsMailAdress, Configurations.ApplicationConfig.StolonsMailPassword);

                client.Send(mineMessage);
                client.Disconnect(true);
            }
            return Task.FromResult(0);
        }

        public static Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
