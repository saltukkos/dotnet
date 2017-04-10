using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saltuk.Nsudotnet.Rss2Email
{

    class SendData
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }

    }

    interface IForwardSender
    {
        bool SendMessage(IEnumerable<SendData> news);
    }
}
