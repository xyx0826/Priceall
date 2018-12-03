using Priceall.Properties;

namespace Priceall.Services
{
    static class SettingsService
    {
        /// <summary>
        /// Attempts to get the value of a setting by its key.
        /// </summary>
        /// <typeparam name="T">The type of the setting's value.</typeparam>
        /// <param name="key">The key of the setting to be found.</param>
        /// <returns>The value of the specified setting, 
        /// or null if the setting is not found.</returns>
        public static T GetSetting<T>(string key)
        {
            var setting = Settings.Default[key];
            return (T)setting;
        }

        /// <summary>
        /// Attempts to modify the value of a setting by its key.
        /// </summary>
        /// <typeparam name="T">The type of the setting's value.</typeparam>
        /// <param name="key">The key of the setting to be modifed.</param>
        /// <param name="newValue">The new value for the specified setting.</param>
        /// <returns>The previous value of the specified settings,
        /// or null if the setting is not found.</returns>
        public static T SetSetting<T>(string key, T newValue)
        {
            var oldSettings = Settings.Default[key];
            Settings.Default[key] = newValue;

            Settings.Default.Save();
            return (T)oldSettings;
        }

        /// <summary>
        /// Checks for settings of older Priceall versions
        /// and migrate them over.
        /// </summary>
        public static void UpdateSettings()
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }
        }
    }
}
