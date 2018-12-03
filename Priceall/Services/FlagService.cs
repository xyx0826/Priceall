using Priceall.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Priceall.Services
{
    /// <summary>
    /// Service class for managing application flags.
    /// Flags are "marker files" in Priceall's code repository.
    /// They are sometimes used to show advice or information regarding Priceall's features.
    /// </summary>
    static class FlagService
    {
        static readonly string _flagsDirectory = "https://raw.githubusercontent.com/xyx0826/Priceall/master/Flags/";

        static readonly string[] _knownFlags = new string[]
        {
            "AUTO_REFRESH_OFF"
        };

        /// <summary>
        /// Checks the existence of every flag in the repo.
        /// If a flag is present, its value in settings will be set to true.
        /// </summary>
        public static async Task CheckAllFlags()
        {
            using (var client = new HttpClient())
            {
                foreach (var flag in _knownFlags)
                {
                    // for every flag, check if it exists on the repo.
                    // if found, set the corresponding setting to true.
                    // if error occurs, keep it false.
                    try
                    {
                        var response = await client.GetAsync(_flagsDirectory + flag);
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                            Settings.Default["FLAG_" + flag] = false;
                        else Settings.Default["FLAG_" + flag] = true;
                    }
                    catch (HttpRequestException)
                    {
                        Settings.Default["FLAG_" + flag] = false;
                    }
                }
                Debug.WriteLine("Flags are checked.");
            }
        }
    }
}
