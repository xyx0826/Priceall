using Priceall.Properties;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Priceall.Http
{
    /// <summary>
    /// A client for interacting with appraisal services.
    /// </summary>
    static class PriceallClient
    {
        private static bool _initialized;

        private static HttpClient _client;

        /// <summary>
        /// Initialize the client by setting user agent and timeout.
        /// </summary>
        private static void Initialize()
        {
            if (_initialized)
            {
                throw new InvalidOperationException("PriceallClient is already initialized.");
            }

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("User-Agent", HttpUtilities.UserAgent);
            _client.Timeout = new TimeSpan(hours: 0, minutes: 0, seconds: 5);
            _initialized = true;
        }

        /// <summary>
        /// Sends a POST request to the specified endpoint with specified headers.
        /// </summary>
        /// <param name="endpoint">The endpoint of the request.</param>
        /// <param name="headers">Optional headers to send along the request.</param>
        /// <param name="content">Content to send in the request.</param>
        /// <returns>The server response.</returns>
        public static async Task<ServiceResponse> PostAsync(string endpoint, Dictionary<string, string> headers, string content)
        {
            if (!_initialized)
            {
                Initialize();
            }

            if (String.IsNullOrEmpty(content) || content.Length > Settings.Default.MaxStringLength)
            {
                // Content is empty or too long
                return ServiceResponse.FromError(ServiceStatus.ContentError);
            }

            HttpResponseMessage res;
            using (var req = new HttpRequestMessage(HttpMethod.Post, endpoint))
            {
                // Compose request
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        req.Headers.Add(header.Key, header.Value);
                    }
                }
                req.Content = new StringContent(content);

                // Parse response
                try
                {
                    res = await _client.SendAsync(req);
                }
                catch (Exception)
                {
                    // HttpRequestException, TaskCanceledException
                    return ServiceResponse.FromError(ServiceStatus.NetworkError);
                }
            }

            return new ServiceResponse(
                ServiceStatus.Successful, await res.Content.ReadAsStringAsync());
        }
    }
}
