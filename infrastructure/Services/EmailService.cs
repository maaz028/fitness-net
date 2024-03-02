using core.Models;
using infrastructure.Repositories;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Fitness_management_system.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendTestEmail(UserEmailOptions userEmailOptions)
        {
            userEmailOptions.Subject = "Reset Password Request";
            userEmailOptions.Body = UpdatePlaceholders(GetEmailBody("EmailBody"),userEmailOptions.KeyValuePairs);
            await SendEmail(userEmailOptions);
        }

        private readonly IOptions<SMTPConfigModel> _smtpConfig;

        public EmailService(IOptions<SMTPConfigModel> smtpConfig)
        {
            _smtpConfig = smtpConfig;
        }

        private const string templatePath = @"EmailTemplate/{0}.html";
      
        private async Task SendEmail(UserEmailOptions userEmailOptions)
        {
            MailMessage mail = new MailMessage()
            {
                Subject = userEmailOptions.Subject,
                Body = userEmailOptions.Body,
                From = new MailAddress(_smtpConfig.Value.SenderAddress, _smtpConfig.Value.SenderDisplayName),
                IsBodyHtml = _smtpConfig.Value.IsBodyHtml
            };

            foreach(var email in userEmailOptions.Email)         
                mail.To.Add(email);


            NetworkCredential networkCredentials = new NetworkCredential(_smtpConfig.Value.Username, _smtpConfig.Value.Password);

            SmtpClient smtpClient = new SmtpClient() {
                Host = _smtpConfig.Value.Host,
                Port = _smtpConfig.Value.Port,
                EnableSsl = _smtpConfig.Value.EnableSSL,
                UseDefaultCredentials = _smtpConfig.Value.EnableDefaultCredentials,
                Credentials = networkCredentials

            };
            mail.BodyEncoding = Encoding.Default;
            await smtpClient.SendMailAsync(mail);
        }

        private string GetEmailBody(string templateName)
        {
            var body = File.ReadAllText(string.Format(templatePath,templateName));
            return body;
        }

        private string UpdatePlaceholders(string text,List<KeyValuePair<string, string>> placeholders)
        {
            if(!string.IsNullOrEmpty(text) && placeholders != null) { 
               
                foreach(var item in placeholders)
                {
                    if(text.Contains(item.Key))
                    {
                        text = text.Replace(item.Key, item.Value);  
                    }
                }
            }
            
            return text;
        }
    }
   

}
