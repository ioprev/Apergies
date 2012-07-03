using System.Collections.Generic;
using System.Net;
using System;
using HtmlAgilityPack;
using System.Threading;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace ApergiesTileUpdate
{
    public class StrikeLoader
    {
        WebClient webc;
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
            webc.OpenReadAsync(new Uri(url));
        }
        public List<StrikeRecord> GetStrikes()
        {
            return strikeList;
        }
        void webc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            HtmlDocument doc = new HtmlDocument();
            bool errorExist = false;
            if (e.Error == null)
            {
                // Complete
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
             
                }
                catch (Exception ex)
                {
                    errorExist = true;
                    
                }
            }
            else
            {
                errorExist = true;
            }
            dataNotifier(errorExist);
        }
    }
}