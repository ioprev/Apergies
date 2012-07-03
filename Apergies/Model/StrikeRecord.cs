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

namespace Apergies.Model
{
    public class StrikeRecord
    {
        public String title { get; set; }
        public String link { get; set; }
        public StrikeRecord()
        {
            // Constructor
        }
        public StrikeRecord(string title, string link)
        {
            this.link = link;
            this.title = title;
        }
    }
}
