using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Saltuk.Nsudotnet.Rss2Email
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var gmailSender = new GmailSender("rss2emailforwarder@gmail.com", "rss2emailforwarderpassworD", "saltukkos@gmail.com"))
            {
                RssForwarder forwarder = new RssForwarder();
                forwarder.StartForwarding(gmailSender);
            }
        }
    }
}