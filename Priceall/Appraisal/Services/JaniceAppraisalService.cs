using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Priceall.Http;
using Priceall.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Priceall.Appraisal
{
    class JaniceAppraisalService : IAppraisalService
    {
        private const string Endpoint = "https://janice.e-351.com/api/rest/v1/appraisal";

        private const string ApiKey = "A3lmazJdrn52ugFuVFrLd0mVqxfApYSx";

        private readonly UriBuilder _uriBuilder;

        private AppraisalSetting<AppraisalMarket> _marketSetting;

        public JaniceAppraisalService()
        {
            _uriBuilder = new UriBuilder(Endpoint);
            _marketSetting = new AppraisalSetting<AppraisalMarket>(
                JsonSettingsService.CreateSetting("Market", AppraisalMarket.TheForge));
        }

        private void BuildUrl()
        {
            var qs = HttpUtility.ParseQueryString(String.Empty);
            qs["key"] = ApiKey;
            qs["market"] = _marketSetting.Value switch
            {
                AppraisalMarket.Jita => "2",
                AppraisalMarket.SystemR1OGn => "3",
                AppraisalMarket.Perimeter => "4",
                AppraisalMarket.TheForge => "5",
                AppraisalMarket.NPC => "6",
                _ => throw new ArgumentOutOfRangeException()
            };
            qs["designation"] = "100";  // appraisal
            qs["pricing"] = "200";  // split
            _uriBuilder.Query = qs.ToString();
        }

        public async Task<AppraisalResult> AppraiseAsync(string content)
        {
            BuildUrl();
            var res = await PriceallClient.PostAsync(_uriBuilder.ToString(), null, content);
            switch (res.Status)
            {
                case ServiceStatus.ContentError:
                    return new AppraisalResult(AppraisalStatus.ContentError, "Text too long or empty.");
                case ServiceStatus.NetworkError:
                    return new AppraisalResult(AppraisalStatus.NetworkError, "Network error.");
                default:
                    // parse total value
                    return ParseResponse(res.Response);
            }
        }

        private AppraisalResult ParseResponse(string response)
        {
            // Parse JSON
            JObject j;
            try
            {
                j = JObject.Parse(response);
            }
            catch (JsonReaderException)
            {
                return new AppraisalResult(AppraisalStatus.InternalError, "Invalid server response.");
            }

            JToken i;
            if (!j.TryGetValue("items", out i) || !(i is JArray items))
            {
                // Shouldn't happen
                return new AppraisalResult(AppraisalStatus.InternalError, "Unexpected server response.");
            }

            if (items.Count == 0)
            {
                // No items
                return new AppraisalResult(AppraisalStatus.ContentError, "Text is invalid.");
            }

            AppraisalResult res = new AppraisalResult(AppraisalStatus.Successful);
            res.BuyValue = j.SelectToken("totalBuyPrice").ToObject<double>();
            res.SellValue = j.SelectToken("totalSellPrice").ToObject<double>();
            res.Volume = j.SelectToken("totalVolume").ToObject<double>();
            return res;
        }

        public AppraisalMarket GetAvailableMarkets()
        {
            return AppraisalMarket.Jita | AppraisalMarket.SystemR1OGn | AppraisalMarket.Perimeter |
                   AppraisalMarket.TheForge | AppraisalMarket.NPC;
        }

        public IReadOnlyCollection<JsonSetting> GetCustomSettings()
            => Array.Empty<JsonSetting>();

        public void SetCurrentMarket(AppraisalMarket market)
        {
            _marketSetting.Value = market;
        }
    }
}
