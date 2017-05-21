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

        public RssForwarder(IEnumerable<string> savedHistory)
        {
            _readNews = savedHistory == null ? new HashSet<string>() : new HashSet<string>(savedHistory);
        }

        public HashSet<string> StartForwarding(Uri uri, IForwardSender sender, int checkPeriod)
        {
            Console.WriteLine("Press any key to stop");
            while (!Console.KeyAvailable)
            {
                try
                {
                    var res = WebRequest.Create(uri).GetResponse();
                    var rss = XDocument.Load(res.GetResponseStream());

                    IEnumerable<string> newGuids;
                    var sendData = GetRecentNews(rss, out newGuids);

                    if (sendData != null && sender.SendMessage(sendData))
                    {
                        _readNews.UnionWith(newGuids);
                    }

                    SleepWatchingKey(checkPeriod * 1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    SleepWatchingKey(5 * 1000);
                }
 
            }

            return _readNews;
        }

        private IEnumerable<SendData> GetRecentNews(XDocument rss, out IEnumerable<string> guids)
        {
            var news = 
            (
                from item in rss.Descendants()
                where item.Name.LocalName == "item"
                where item.Element("guid") != null
                where !_readNews.Contains(item.Element("guid")?.Value)
                select new
                {
                    title = item.Element("title")?.Value,
                    link = item.Element("link")?.Value,
                    description = item.Element("description")?.Value,
                    guid = item.Element("guid")?.Value
                }
            ).ToDictionary(
                item => item.guid, 
                item => new SendData
                {
                    Title = item.title,
                    Description = item.description,
                    Link = item.link
                }
            );

            if (news.Count > 0)
            {
                guids = news.Keys;
                return news.Values;
            }

            guids = null;
            return null;
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
