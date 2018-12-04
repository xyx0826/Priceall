using Priceall.Services;
using System;
using System.Windows.Media;

namespace Priceall.Helpers
{
    static class ColorHelper
    {
        private static BrushConverter _converter = new BrushConverter();

        /// <summary>
        /// Attempts to convert the given hex color to a SolidColorBrush.
        /// </summary>
        /// <param name="hexColor">The color represented in hex.</param>
        /// <returns>A corresponding SolidColorBrush, 
        /// or white SolidColorBrush if given color is invalid.</returns>
        public static SolidColorBrush ConvertHexToColorBrush(string hexColor)
        {
            try
            {
                return _converter.ConvertFrom("#" + hexColor) as SolidColorBrush;
            }
            catch (FormatException)
            {
                return new SolidColorBrush(Colors.White);
            }
        }

        /// <summary>
        /// Attempts to find the hex color stored in a setting 
        /// and convert it to a SolidColorBrush.
        /// </summary>
        /// <param name="settingKey">The key to the setting containing a hex color.</param>
        /// <returns>A corresponding SolidColorBrush, 
        /// or white SolidColorBrush if given color is invalid.</returns>
        public static SolidColorBrush ConvertSettingToColorBrush(string settingKey)
        {
            return ConvertHexToColorBrush(
                SettingsService.GetSetting<string>(settingKey));
        }
    }
}
