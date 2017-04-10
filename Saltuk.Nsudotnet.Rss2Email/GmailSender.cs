using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

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
                StringBuilder message = new StringBuilder("<b><h1>List of recent news:</h1></b><br>");
                foreach (var item in data)
                {
                    message.AppendFormat("<br><h2><b>{0}</b></h2><br><h3>{1}<br><b>Full:</b>{2}<br></h3>",
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
            catch (Exception)
            {
                return false;
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
