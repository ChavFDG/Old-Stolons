using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Stolons.Services
{
    public static class AuthMessageSender 
    {
        /// <summary>
        /// Info : http://www.elanderson.net/2016/02/emails-using-mailgun-in-asp-net-core/
        /// </summary>
        public static async Task SendEmailAsync(string email, string name, string subject, string message)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(Configurations.ApplicationConfig.MailBaseUri) })
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.Unicode.GetBytes(Configurations.ApplicationConfig.MailApiKey)));

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("from", Configurations.ApplicationConfig.MailApiFrom),
                    new KeyValuePair<string, string>("to", name+"<"+email+">"),
                    new KeyValuePair<string, string>("subject", subject),
                    new KeyValuePair<string, string>("text", message)
                });

                await client.PostAsync(Configurations.ApplicationConfig.MailRequestUri, content).ConfigureAwait(false);
            }
        }

        public static Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
   
}
