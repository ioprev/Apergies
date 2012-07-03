using Apergies.Model;
using System.Collections.Generic;
using System.Net;
using System;
using HtmlAgilityPack;
using System.Threading;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace Apergies
{
    public class StrikeLoader
    {
        WebClient webc;
        DataCache cache;
        private List<StrikeRecord> strikeList;
        public NotifyDataList dataNotifier;
        
        
        public StrikeLoader()
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
            HtmlDocument doc = new HtmlDocument();

            if (e.Error == null)
            {
                try
                {
                    doc.Load(e.Result);
                    var items = from item in doc.DocumentNode.Descendants("li")
                                select new
                                {
                                    Title = item.InnerText
                                };
                    foreach (var item in items)
                    {
                        this.strikeList.Add(new StrikeRecord(HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(item.Title)), ""));
                    }
                    cache.Marshall(this.strikeList);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Παρουσιάστηκε σφάλμα κατα την ανάκτηση των δεδομένων");
                    this.strikeList = cache.Unmarshall();
                }
            }
            else
            {
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