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
    /// Services class for interacting with Evepraisal.com.
    /// Provides persistance and market system customization.
    /// </summary>
    class AppraisalService
    {
        #region Properties
        static readonly HttpClient _client = new HttpClient();
        static readonly UriBuilder _uriBuilder = 
            new UriBuilder("https://evepraisal.com/appraisal.json");

        bool _isPersist;
        public bool IsPersist
        {
            get { return _isPersist; }
            set
            {
                _isPersist = value;
                UpdatePersist();
            }
        }

        string _marketSystem;
        public string MarketSystem
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
        /// Instantiates an Evepraisal service and set appropriate User-Agent and params.
        /// </summary>
        /// <param name="isPersist">Whether appraisal requests shall persist on serverside. Defaulted to no.</param>
        /// <param name="marketSystem">The system for price checking. Defaulted to Jita.</param>
        public AppraisalService(bool isPersist = false, string marketSystem = "jita")
        {
            // set request user-agent
            var appVersion = Assembly.GetEntryAssembly().GetName().Version;
            _client.DefaultRequestHeaders.Add("User-Agent", 
                $"Priceall/{appVersion} (Github.com/xyx0826/Priceall)");
            // set request params
            IsPersist = isPersist;
            MarketSystem = marketSystem;
        }

        /// <summary>
        /// Updates the persist status of HttpClient.
        /// </summary>
        void UpdatePersist()
        {
            var queryParams = HttpUtility.ParseQueryString(_uriBuilder.Query);
            if (!IsPersist) queryParams["persist"] = "no";
            else queryParams.Remove("persist");
            _uriBuilder.Query = queryParams.ToString();
        }

        /// <summary>
        /// Updates the target market system of HttpClient.
        /// </summary>
        void UpdateMarketSystem()
        {
            var queryParams = HttpUtility.ParseQueryString(_uriBuilder.Query);
            queryParams["market"] = MarketSystem;
            _uriBuilder.Query = queryParams.ToString();
        }

        /// <summary>
        /// Queries appraisal.
        /// </summary>
        /// <param name="clipboardContent">Content from clipboard to be parsed.</param>
        public async Task<string> QueryAppraisal(string query)
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
