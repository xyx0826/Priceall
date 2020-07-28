using System.Reflection;

namespace Priceall.Http
{
    /// <summary>
    /// Utilities for making HTTP requests.
    /// </summary>
    static class HttpUtilities
    {
        private static string _appVersion;

        /// <summary>
        /// The default User-Agent value for the app.
        /// </summary>
        public static string UserAgent
        {
            get
            {
                if (_appVersion == null)
                {
                    _appVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
                }
                return $"Priceall/{_appVersion} (Github.com/xyx0826/Priceall)";
            }
        }
    }
}
