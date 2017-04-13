using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NDesk.Options;

namespace Saltuk.Nsudotnet.Rss2Email
{
    class Program
    {

        static void Main(string[] args)
        {

            string url;
            string email;
            int checkPeriod;

            if (!ParseParams(args, out url, out email, out checkPeriod))
                return;

            using (var gmailSender = new GmailSender("rss2emailforwarder@gmail.com", "rss2emailforwarderpassworD", email))
            {
                RssForwarder forwarder = new RssForwarder(GetSavedInstance());
                var newState = forwarder.StartForwarding(new Uri(url), gmailSender, checkPeriod);
                SaveInstance(newState);
            }
        }

        private static bool ParseParams(string[] args, out string rss, out string email, out int checkPeriod)
        {
            string rssArg = null;
            string emailArg = null;
            int periodArg = 30;

            var p = new OptionSet()
            {
                { "r|rss=", "http address of {RSS} channel", s => rssArg = s },
                { "e|email=", "{EMAIL} address to forward {RSS} items", s => emailArg = s },
                { "p|period=", "{PERIOD}(in seconds) to check new {RSS} items - optional", n => int.TryParse(n, out periodArg) }
            };

            try
            {
                p.Parse(args);

            }
            catch (Exception e)
            {
                p.WriteOptionDescriptions(Console.Out);
                return false;
            }
            finally
            {
                rss = rssArg;
                email = emailArg;
                checkPeriod = periodArg;
            }

            if (rssArg == null || emailArg == null)
            {
                Console.WriteLine("Usage:");
                p.WriteOptionDescriptions(Console.Out);
                return false;
            }

            return true;
        }

        private static HashSet<string> GetSavedInstance()
        {
            try
            {
                using (FileStream file = new FileStream(ConfigurationManager.AppSettings["historyFile"], FileMode.Open))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return (HashSet<string>) binaryFormatter.Deserialize(file);
                }
            }
            catch
            {
                return null;
            }
        }

        private static void SaveInstance( HashSet<string> history)
        {
            try
            {
                using (FileStream file = new FileStream(ConfigurationManager.AppSettings["historyFile"], FileMode.Create))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    binaryFormatter.Serialize(file, history);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}