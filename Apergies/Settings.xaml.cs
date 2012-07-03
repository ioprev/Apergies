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
using System.IO.IsolatedStorage;
using Apergies.Model;

namespace Apergies
{
    public partial class Settings : PhoneApplicationPage
    {
        AppSettings settingsStorage;

        // Class constructor
        public Settings()
        {
            this.settingsStorage = new AppSettings();
            InitializeComponent();
        }


        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void toggleTile_Checked(object sender, RoutedEventArgs e)
        {
            toggleTile.SwitchForeground = new SolidColorBrush(Colors.Green);
            this.settingsStorage.UseTileToggleSwitchSetting = true;
            toggleTileUpdate.IsEnabled = true;
            updateInfoBlock.Opacity = 1;

        }

        private void toggleTile_Unchecked(object sender, RoutedEventArgs e)
        {
            toggleTile.SwitchForeground = new SolidColorBrush(Colors.Red);
            this.settingsStorage.UseTileToggleSwitchSetting = false;
            toggleTileUpdate.IsEnabled = false;
            updateInfoBlock.Opacity = 0.5;
        }

        private void toggleTileUpdate_Checked(object sender, RoutedEventArgs e)
        {
            toggleTileUpdate.SwitchForeground = new SolidColorBrush(Colors.Green);
            this.settingsStorage.UpdateTileToggleSwitchSetting = true;
        }

        private void toggleTileUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            toggleTileUpdate.SwitchForeground = new SolidColorBrush(Colors.Red);
            this.settingsStorage.UpdateTileToggleSwitchSetting = false;
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            bool useTile = settingsStorage.UseTileToggleSwitchSetting;
            toggleTile.IsChecked = useTile;
            toggleTileUpdate.IsChecked = settingsStorage.UpdateTileToggleSwitchSetting;
            if (useTile)
            {
                updateInfoBlock.Opacity = 1;
                toggleTileUpdate.IsEnabled = true;
            }
            else
            {
                updateInfoBlock.Opacity = 0.5;
                toggleTileUpdate.IsEnabled = false;
            }

        }
    }
}