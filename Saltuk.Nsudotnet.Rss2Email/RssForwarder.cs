using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;

namespace Saltuk.Nsudotnet.Rss2Email
{
    class RssForwarder
    {
        private readonly HashSet<string> _readNews;

        private const int ButtonPressCheckPeriod = 100;

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
                    var sendData = GetRecentNews(rss, out newGuids);

                    if (sendData.Count > 0 && sender.SendMessage(sendData))
                    {
                        _readNews.UnionWith(newGuids);
                    }

                    SleepWatchingKey(30 * 1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SleepWatchingKey(5 * 1000);
                }
 
            }
        }

        private List<SendData> GetRecentNews(XDocument rss, out List<string> guids)
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

        private static void SleepWatchingKey(int msec)
        {
            while (msec > 0)
            {
                if (Console.KeyAvailable)
                    return;
                Thread.Sleep(ButtonPressCheckPeriod);
                msec -= ButtonPressCheckPeriod;
            }
        }
    }
}
