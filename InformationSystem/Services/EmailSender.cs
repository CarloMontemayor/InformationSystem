using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace InformationSystem.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var message = new MailMessage())
            {
                message.To.Add(new MailAddress(email, email));
                message.From = new MailAddress("InformationSystemtsu@gmail.com", "Information System");
                message.Subject = subject;
                message.Body = htmlMessage;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Port = 587;
                    client.UseDefaultCredentials = false;
                    var userName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Email")["UserName"];
                    var password = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("Email")["Password"];
                    client.Credentials = new NetworkCredential(userName, password);
                    client.EnableSsl = true;
                    client.Send(message);
                
                }
            }
            return Task.CompletedTask;
        }
    }
}
