using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Priceall.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Priceall.Appraisal
{
    class JaniceAppraisalService : IAppraisalService
    {
        public const string Endpoint = "https://janice.e-351.com/api/rest/v1/appraisal";

        private const string ApiKey = "A3lmazJdrn52ugFuVFrLd0mVqxfApYSx";

        private AppraisalMarket _market;

        private readonly UriBuilder _uriBuilder;

        public JaniceAppraisalService()
        {
            _uriBuilder = new UriBuilder(Endpoint);
        }

        private void BuildUrl()
        {
            var qs = HttpUtility.ParseQueryString(String.Empty);
            qs["key"] = ApiKey;
            qs["market"] = _market == AppraisalMarket.TheForge ? "1" : "2";
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
            return AppraisalMarket.Jita | AppraisalMarket.TheForge;
        }

        public IReadOnlyCollection<AppraisalSettings> GetCustomSettings()
        {
            return Array.Empty<AppraisalSettings>();
        }

        public void SetCurrentMarket(AppraisalMarket market)
        {
            _market = market;
        }
    }
}
