using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Priceall.Services
{
    class UpdateService
    {
        static readonly string APPVEYOR_API_ROUTE = "https://ci.appveyor.com/api/projects/xyx0826/priceall/branch/master";

        public async Task<bool> CheckForUpdates()
        {
            Version remoteVersion;

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(APPVEYOR_API_ROUTE);
                    var content = await response.Content.ReadAsStringAsync();
                    remoteVersion = new Version(
                        (string)JObject.Parse(content)
                        .SelectToken("build.version"));
                }
                catch (HttpRequestException) { return false; }
            }
            return (Assembly.GetEntryAssembly().GetName().Version
                .CompareTo(remoteVersion) < 0);
        }
    }
}
