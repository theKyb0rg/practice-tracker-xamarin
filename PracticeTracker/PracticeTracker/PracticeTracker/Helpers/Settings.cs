// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace PracticeTracker.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;

        private const string FirstTimeUserKey = "FirstTimeUserKey";
        private static readonly bool FirstTimeUserKeyDefault = true;
        #endregion


        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(SettingsKey, value);
            }
        }

        public static bool FirstTimeUser
        {
            get
            {
                return AppSettings.GetValueOrDefault(FirstTimeUserKey, FirstTimeUserKeyDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(FirstTimeUserKey, value);
            }
        }

    }
}