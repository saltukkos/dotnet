using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Saltuk.Nsudotnet.Rss2Email
{
    class GmailSender : IDisposable, IForwardSender
    {
        private readonly SmtpClient _client;
        private readonly string _from;
        private readonly string _to;

        public GmailSender(string username, string password, string to)
        {
            _from = username;
            _to = to;
            _client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                Timeout = 20000,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials =
                    new System.Net.NetworkCredential(username, password)
            };
        }

        public bool SendMessage(IEnumerable<SendData> data)
        {
            try
            {
                StringBuilder message = new StringBuilder("<h1>List of recent news:</h1>");
                foreach (var item in data)
                {
                    message.AppendFormat("<br><h2>{0}</h2><h3>{1}<br>Full:{2}</h3>",
                        item.Title, item.Description, item.Link);
                }
                MailMessage sendMessage = new MailMessage(_from, _to)
                {
                    Subject = "Recent news",
                    Body = message.ToString(),
                    IsBodyHtml = true
                };
                _client.Send(sendMessage);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
