using Priceall.Properties;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Priceall.Services
{
    /// <summary>
    /// Service class for interacting with Evepraisal.com.
    /// Provides persistance and market system customization.
    /// </summary>
    static class AppraisalService
    {
        #region Properties
        static string _userAgent;
        static readonly HttpClient _client = new HttpClient();
        static readonly UriBuilder _uriBuilder = 
            new UriBuilder("https://evepraisal.com/appraisal.json");

        static bool _isPersist;
        public static bool IsPersist
        {
            get { return _isPersist; }
            set
            {
                _isPersist = value;
                UpdatePersist();
            }
        }

        static string _marketSystem;
        public static string MarketSystem
        {
            get { return _marketSystem; }
            set
            {
                _marketSystem = value;
                UpdateMarketSystem();
            }
        }
        #endregion
        
        /// <summary>
        /// Initializes Evepraisal service and set appropriate User-Agent and params.
        /// </summary>
        /// <param name="isPersist">Whether appraisal requests shall persist on serverside. Defaulted to no.</param>
        /// <param name="marketSystem">The system for price checking. Defaulted to Jita.</param>
        public static void Initialize(bool isPersist = false, string marketSystem = "jita")
        {
            var appVersion = Assembly.GetEntryAssembly().GetName().Version;
            _userAgent = $"Priceall/{appVersion} (Github.com/xyx0826/Priceall)";
            _client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            IsPersist = isPersist;
            MarketSystem = marketSystem;
        }

        /// <summary>
        /// Updates persistance of future Evepraisal queries.
        /// </summary>
        static void UpdatePersist()
        {
            var queryParams = HttpUtility.ParseQueryString(_uriBuilder.Query);
            if (!IsPersist) queryParams["persist"] = "no";
            else queryParams.Remove("persist");
            _uriBuilder.Query = queryParams.ToString();
        }

        /// <summary>
        /// Updates the target market system of future Evepraisal queries.
        /// </summary>
        static void UpdateMarketSystem()
        {
            var queryParams = HttpUtility.ParseQueryString(_uriBuilder.Query);
            queryParams["market"] = MarketSystem;
            _uriBuilder.Query = queryParams.ToString();
        }

        /// <summary>
        /// Queries appraisal.
        /// </summary>
        /// <param name="clipboardContent">Content from clipboard to be parsed.</param>
        public static async Task<string> QueryAppraisal(string query)
        {
            // if string is too long, do not proceed
            if (query.Length > Settings.Default.MaxStringLength)
                return "{\"error_message\": \"Clipboard text too long.\"}";
            // if no string in clipboard, do not proceed
            if (String.IsNullOrEmpty(query))
                return "{\"error_message\": \"No text found on clipboard.\"}";

            // initial checks passed, ask server for response
            var jsonResponse = String.Empty;

            try
            {
                var httpResponse = await _client.PostAsync(_uriBuilder.ToString(),
                    new StringContent(query));
                jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e) { Debug.WriteLine("HttpRequestException: " + e.Message); }

            // empty response is a network error
            if (String.IsNullOrEmpty(jsonResponse))
                return "{\"error_message\": \"Network error.\"}";

            return jsonResponse;
        }
    }
}
