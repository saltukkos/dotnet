using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Saltuk.Nsudotnet.Rss2Email
{
    class RssForwarder
    {
        private readonly HashSet<string> _readNews;

        public RssForwarder()
        {
            _readNews = new HashSet<string>();
        }

        public RssForwarder(IEnumerable<string> savedHistory)
        {
            _readNews = new HashSet<string>(savedHistory);
        }

        public void StartForwarding(string url, IForwardSender sender)
        {
            Console.WriteLine("Press any key to stop");
            while (!Console.KeyAvailable)
            {
                try
                {
                    var res = WebRequest.Create(url).GetResponse();
                    var rss = XDocument.Load(res.GetResponseStream());

                    List<string> newGuids;
                    var sendData = getRecentNews(rss, out newGuids);

                    if (sendData.Count > 0 && sender.SendMessage(sendData))
                    {
                        _readNews.UnionWith(newGuids);
                    }

                    Thread.Sleep(1000 * 30);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(5000);
                }
 
            }
        }

        private List<SendData> getRecentNews(XDocument rss, out List<string> guids)
        {
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
            guids = new List<string>();
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

            return sendData;
        }
    }
}
