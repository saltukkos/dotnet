using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Saltuk.Nsudotnet.Rss2Email
{
    class RssForwarder
    {
        private HashSet<string> _readNews;

        public RssForwarder()
        {
            _readNews = new HashSet<string>();
        }

        public RssForwarder(IEnumerable<string> savedHistory)
        {
            _readNews = new HashSet<string>(savedHistory);
        }

        public void StartForwarding(IForwardSender sender)
        {
            for (;;)
            {
                var rss = XDocument.Load("../../rssTest.xml");
                var items = from item in rss.Descendants()
                        where item.Name.LocalName == "item"
                        select new
                        {
                            title = item.Element("title")?.Value,
                            link = item.Element("link")?.Value,
                            description = item.Element("description")?.Value,
                            guid = item.Element("guid")?.Value
                        }
                    ;

                var sendData = new List<SendData>();
                var guids = new List<string>();
                foreach (var item in items)
                {
                    if (item.guid == null || _readNews.Contains(item.guid))
                        continue;

                    guids.Add(item.guid);
                    sendData.Add(new SendData()
                    {
                        Description = item.description,
                        Link = item.link,
                        Title = item.title
                    });
                }

                if (sendData.Count > 0 && sender.SendMessage(sendData))
                {
                    _readNews.UnionWith(guids);
                }

                Thread.Sleep(1000 * 30);
            }
        }
    }
}
