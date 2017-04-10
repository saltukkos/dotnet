using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
                RssForwarder forwarder = new RssForwarder(GetSavedInstance());
                var newState = forwarder.StartForwarding("https://meduza.io/rss/all", gmailSender);
                SaveInstance(newState);
            }
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