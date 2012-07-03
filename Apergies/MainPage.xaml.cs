using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using Apergies.Model;
using Microsoft.Phone.Tasks;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Scheduler;

namespace Apergies
{
    public delegate void NotifyDataList();
    public delegate void NotifyAttn();

    public partial class MainPage : PhoneApplicationPage
    {
        StrikeLoader today;
        StrikeLoader tomorrow;
        AllStrikesLoader allstrikes;
        DispatcherTimer overlayTimer;
        AppSettings storedSettings;
        int todayStrikes;
        int loadingCounter;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            storedSettings = new AppSettings();
            DataRefresh();
            RegisterScheduledTask();
        }

        private void RegisterScheduledTask()
        {
            // A unique name for your task. It is used to 
            // locate it in from the service.
            var taskName = "ApergiesTileUpdate";

            // If the task exists
            var oldTask = ScheduledActionService.Find(taskName) as PeriodicTask;
            if (oldTask != null)
            {
                ScheduledActionService.Remove(taskName);
            }
            if (this.storedSettings.UseTileToggleSwitchSetting && this.storedSettings.UpdateTileToggleSwitchSetting)
            { 
                // Create the Task
                PeriodicTask task = new PeriodicTask(taskName);

                // Description is required
                task.Description = "Ανανεώνει τις πληροφορίες για τις απεργίες στο πλακίδιο (tile) της εφαρμογής, όταν η εφαρμογή δε λειτουργεί";

                // Add it to the service to execute
                try
                {
                    ScheduledActionService.Add(task);
                }
                catch (Exception ex)
                {
                }
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                e.Cancel = true;
                NavigationService.GoBack();
            }
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            DataRefresh();
        }

        private void DataRefresh()
        {
            progressOverlay.Show();
            attnButton.Visibility = System.Windows.Visibility.Collapsed;
            loadingCounter = 3;
            today = new StrikeLoader();
            tomorrow = new StrikeLoader();
            allstrikes = new AllStrikesLoader();
            allstrikes.attnNotifier = attnNotifier;
            today.dataNotifier = todayNotifier;
            tomorrow.dataNotifier = tomorrowNotifier;
            allstrikes.dataNotifier = allstrikesNotifier;
            today.LoadStrikes("http://apergia.gr/q/index.php?id=today");
            tomorrow.LoadStrikes("http://apergia.gr/q/index.php?id=tomorrow");
            allstrikes.LoadStrikes("http://feeds.feedburner.com/apergiagr?format=xml"); 
        }
        private void todayNotifier()
        {
            bool empty = false;
            List<StrikeRecord> todayStrikesList = today.GetStrikes();
            
            todayList.ItemsSource = todayStrikesList;

            foreach (StrikeRecord item in todayStrikesList)
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
                todayStrikes = 0;
            }
            else
            {
                todayStrikes = todayStrikesList.Count;
            }
            if (this.storedSettings.UseTileToggleSwitchSetting) UpdateLiveTile();
            else RemoveLiveTile();
            loadingTaskComplete();
        }

        private void RemoveLiveTile()
        {
            ShellTile TileToFind = ShellTile.ActiveTiles.First();
            if (TileToFind != null)
            {
                StandardTileData data = new StandardTileData
                {
                    Count = 0,
                    BackBackgroundImage = new Uri("Empty", UriKind.Relative),
                    BackContent = string.Empty,
                    BackTitle = string.Empty
                };
                // Update the Application Tile
                TileToFind.Update(data);
            }

        }
        private void UpdateLiveTile()
        {
            String tilebackContent = "";
            if (todayStrikes == 0)
            {
                tilebackContent = "Καμμία απεργία σήμερα";
            }
            else if (todayStrikes == 1)
            {
                tilebackContent = todayStrikes.ToString() + " απεργία σήμερα";
            }
            else
            {
                tilebackContent = todayStrikes.ToString() + " απεργίες σήμερα";
            }
            // Update Tile
            ShellTile TileToFind = ShellTile.ActiveTiles.First();
            if (TileToFind != null)
            {
                StandardTileData NewTileData = new StandardTileData
                {
                    Title = "Απεργίες",
                    Count = todayStrikes,
                    BackTitle = "Απεργίες",
                    //BackBackgroundImage = new Uri(textBoxBackBackgroundImage.Text, UriKind.Relative),
                    BackContent = tilebackContent

                };
                // Update the Application Tile
                TileToFind.Update(NewTileData);
            }
        }
        private void tomorrowNotifier()
        {
            tomorrowList.ItemsSource = tomorrow.GetStrikes();
            loadingTaskComplete();
        }
        private void allstrikesNotifier()
        {
            allList.ItemsSource = allstrikes.GetStrikes();
            loadingTaskComplete();
        }
        private void attnNotifier()
        {
            attnButton.Visibility = System.Windows.Visibility.Visible;
        }
        private void loadingTaskComplete()
        {
            loadingCounter--;
            if (loadingCounter == 0) progressOverlay.Hide();
        }

        private void ApplicationBarItemAbout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        private void allList_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)e.OriginalSource;
            StrikeRecord item = (StrikeRecord)element.DataContext;
            WebBrowserTask browsertask = new WebBrowserTask();
            browsertask.Uri = new Uri(item.link);
            browsertask.Show();
        }


        private void attnButton_Click(object sender, RoutedEventArgs e)
        {
            overlayTimer = new System.Windows.Threading.DispatcherTimer();
            overlayTimer.Interval = new TimeSpan(0, 0, 0, 4, 0);
            overlayTimer.Tick += new EventHandler(overlayTimer_Tick);
            overlayTimer.Start();            
            noConnOverlay.Show();
        }

        void overlayTimer_Tick(object sender, EventArgs e)
        {
            overlayTimer.Stop();
            noConnOverlay.Hide();
        }

        private void ApplicationBarItemSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }
    }
}