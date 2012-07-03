using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using Apergies.Model;
using HtmlAgilityPack;
using System.Xml.Linq;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Threading;

namespace Apergies
{
    public class AllStrikesLoader
    {
        WebClient webc;
        DataCache cache;
        private List<StrikeRecord> strikeList;
        public NotifyDataList dataNotifier;
        public NotifyAttn attnNotifier;

        public AllStrikesLoader()
        {
            //Constructor
            webc = new WebClient();
            webc.OpenReadCompleted += new OpenReadCompletedEventHandler(webc_OpenReadCompleted);
            this.strikeList = new List<StrikeRecord>();
        }
        public void LoadStrikes(string url)
        {
            cache = new DataCache(GetSHA1(url));
            webc.OpenReadAsync(new Uri(url));
        }
        public List<StrikeRecord> GetStrikes()
        {
            return strikeList;
        }
        void webc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    XDocument doc = XDocument.Load(e.Result);
                    var items = from item in doc.Descendants("item")
                                select new
                                {
                                    Title = item.Element("title").Value,
                                    Link = item.Element("link").Value,
                                };

                    foreach (var item in items)
                    {
                        this.strikeList.Add(new StrikeRecord(item.Title, item.Link));
                        cache.Marshall(this.strikeList);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Παρουσιάστηκε σφάλμα κατα την ανάκτηση των δεδομένων");
                    this.strikeList = cache.Unmarshall();
                } 
            }
            else
            {
                attnNotifier();
                this.strikeList = cache.Unmarshall();
            }
            dataNotifier();
        }

        private static String GetSHA1(String text)
        {
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            byte[] message = UE.GetBytes(text);

            SHA1Managed hashString = new SHA1Managed();
            string hex = "";

            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
    }
}
