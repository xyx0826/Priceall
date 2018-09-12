using Priceall.Properties;
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Priceall.Helper
{
    class AppraisalHelper
    {
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
        string MarketSystem
        {
            get { return _marketSystem; }
            set
            {
                _marketSystem = value;
                UpdateMarketSystem();
            }
        }

        /// <summary>
        /// Instantiates an appraisal helper class and set User-Agent.
        /// </summary>
        public AppraisalHelper(bool isPersist = false, string marketSystem = "jita")
        {
            var appVersion = Assembly.GetEntryAssembly().GetName().Version;
            _client.DefaultRequestHeaders.Add("User-Agent", 
                $"Priceall/{appVersion} (Github.com/xyx0826/Priceall)");

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

        async Task<string> QueryAppraisalAsync(string query)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var response = await _client.PostAsync(_uriBuilder.ToString(),
                        new StringContent(query));
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException) { return String.Empty; }
            });
        }

        /// <summary>
        /// Queries appraisal.
        /// </summary>
        /// <param name="clipboardContent">Content from clipboard to be parsed.</param>
        public async Task<string> QueryAppraisal(string query)
        {
            // If string is too long then trim it
            if (query.Length > Settings.Default.MaxStringLength)
                query = query.Take(Settings.Default.MaxStringLength).ToString();

            if (String.IsNullOrEmpty(query))
                return "{\"error_message\": \"No text found on clipboard.\"}";

            var response = await QueryAppraisalAsync(query);

            if (String.IsNullOrEmpty(response))
                return "{\"error_message\": \"Network error.\"}";
            else return response;
        }
    }
}
