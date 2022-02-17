using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_core.Common
{
    public class EmailService : IDisposable
    {
        private SmtpClient _client;
        public StringBuilder _body;

        public EmailService()
        {
            _body = new StringBuilder();
            _client = new SmtpClient();
        }
        
        public void Dispose()
        {
            _body.Clear();
            _client.Dispose();
        }

        public async Task<bool> SendEmailAsync(string fullname, string receiverEmail, string subject)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(receiverEmail);
                mail.From = new MailAddress("services@digital-tricks.com", "Dev Studio", Encoding.UTF8);
                mail.Subject = subject;
                mail.Body = _body.ToString();
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                _client.Host = "smtp.gmail.com";
                _client.Port = 587;
                _client.Credentials = new NetworkCredential("services@digital-tricks.com", "Password4digitaltricks15@!");
                _client.EnableSsl = true;
                _client.UseDefaultCredentials = false;
                await _client.SendMailAsync(mail);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}