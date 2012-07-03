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
using System.IO.IsolatedStorage;

namespace Apergies.Model
{
    public class AppSettings
    {
        // Isolated Storage Settings
        IsolatedStorageSettings settings;

        // The key names of our settings
        const string UseLiveTileSettingKey = "UseLiveTileSetting";
        const string UpdateLiveTileSettingKey = "UpdateLiveTileSetting";

        // The default values of our settings
        const bool UseLiveTileSettingDefault = false;
        const bool UpdateLiveTileSettingDefault = true;

        // Class constructor
        public AppSettings()
        {
            // Get the settings
            settings = IsolatedStorageSettings.ApplicationSettings;

        }

        public bool AddOrUpdateValue(string Key, Object value)
        {
            bool valueChanged = false;

            // If the key exists
            if (settings.Contains(Key))
            {
                // If the value has changed
                if (settings[Key] != value)
                {
                    // Store the new value
                    settings[Key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                settings.Add(Key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        public T GetValueOrDefault<T>(string Key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.
            if (settings.Contains(Key))
            {
                value = (T)settings[Key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        public void Save()
        {
            settings.Save();
        }

        public bool UseTileToggleSwitchSetting
        {
            get
            {
                return GetValueOrDefault<bool>(UseLiveTileSettingKey, UseLiveTileSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(UseLiveTileSettingKey, value))
                {
                    Save();
                }
            }
        }

        public bool UpdateTileToggleSwitchSetting
        {
            get
            {
                return GetValueOrDefault<bool>(UpdateLiveTileSettingKey, UpdateLiveTileSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(UpdateLiveTileSettingKey, value))
                {
                   
                    Save();
                }
            }
        }
    }
}
