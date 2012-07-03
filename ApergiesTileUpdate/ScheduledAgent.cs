using System.Windows;
using Microsoft.Phone.Scheduler;
using System.Collections.Generic;
using Microsoft.Phone.Shell;
using System.Linq;

namespace ApergiesTileUpdate
{
    public delegate void NotifyDataList(bool errorExist);
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private static volatile bool _classInitialized;
        StrikeLoader sloader;
       
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;
                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            sloader = new StrikeLoader();
            sloader.dataNotifier = dataNotifier;
            sloader.LoadStrikes("http://apergia.gr/q/index.php?id=today");

        }
        public void dataNotifier(bool errorExist)
        {
            if (!errorExist)
            {
                List<StrikeRecord> strikes = sloader.GetStrikes();
                bool empty = false;
                int tileStrikesCount;
                string tileBackContent;

                foreach (StrikeRecord item in strikes)
                {
                    if (item.title.Contains("Δεν έχουν καταγραφεί"))
                    {
                        empty = true;
                    }
                    if (item.title.Contains("Δεν ήταν δυνατή η ανάκτηση"))
                    {
                        empty = true;
                    }
                }
                if (empty)
                {
                    tileStrikesCount = 0;
                }
                else
                {
                    tileStrikesCount = strikes.Count;
                }
                if (tileStrikesCount == 0)
                {
                    tileBackContent = "Καμμία απεργία σήμερα";
                }
                else if (tileStrikesCount == 1)
                {
                    tileBackContent = tileStrikesCount.ToString() + " απεργία σήμερα";
                }
                else
                {
                    tileBackContent = tileStrikesCount.ToString() + " απεργίες σήμερα";
                }
                // Update Tile
                ShellTile TileToFind = ShellTile.ActiveTiles.First();
                if (TileToFind != null)
                {
                    StandardTileData NewTileData = new StandardTileData
                    {
                        Title = "Απεργίες",
                        Count = tileStrikesCount,
                        BackTitle = "Απεργίες",
                        //BackBackgroundImage = new Uri(textBoxBackBackgroundImage.Text, UriKind.Relative),
                        BackContent = tileBackContent

                    };
                    // Update the Application Tile
                    TileToFind.Update(NewTileData);
                }
            }

            NotifyComplete();
        }
    }
}