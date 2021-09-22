using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Priceall.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Priceall.Services;

namespace Priceall.Appraisal
{
    class EvepraisalAppraisalService : IAppraisalService
    {
        private const string Endpoint = "https://evepraisal.com/appraisal.json";

        private readonly UriBuilder _uriBuilder;

        private readonly AppraisalSetting<bool> _persistSetting;

        private readonly AppraisalSetting<AppraisalMarket> _marketSetting;

        public EvepraisalAppraisalService()
        {
            _uriBuilder = new UriBuilder(Endpoint);
            _persistSetting =
                new AppraisalSetting<bool>(
                    JsonSettingsService.CreateSetting("Persist", false));
            _marketSetting =
                new AppraisalSetting<AppraisalMarket>(
                    JsonSettingsService.CreateSetting("Market", AppraisalMarket.Jita));
        }

        private void BuildUrl()
        {
            var qs = HttpUtility.ParseQueryString(String.Empty);
            qs["market"] = _marketSetting.Value.ToString().ToLower();
            if (!_persistSetting.Value)
            {
                qs["persist"] = "no";
            }
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

            // Check error message
            if (j.TryGetValue("error_message", out var error))
            {
                return new AppraisalResult(AppraisalStatus.ContentError, error.ToObject<string>());
            }

            var res = new AppraisalResult(AppraisalStatus.Successful)
            {
                Kind = j.SelectToken("appraisal.kind").ToObject<string>(),
                BuyValue = j.SelectToken("appraisal.totals.buy").ToObject<double>(),
                SellValue = j.SelectToken("appraisal.totals.sell").ToObject<double>(),
                Volume = j.SelectToken("appraisal.totals.volume").ToObject<double>()
            };
            return res;
        }

        public AppraisalMarket GetAvailableMarkets()
        {
            return AppraisalMarket.Amarr
                | AppraisalMarket.Dodixie
                | AppraisalMarket.Hek
                | AppraisalMarket.Jita
                | AppraisalMarket.Rens
                | AppraisalMarket.Universe;
        }

        public IReadOnlyCollection<JsonSetting> GetCustomSettings()
            => Array.Empty<JsonSetting>();
            // => new JsonSetting[] { _persistSetting };

        public void SetCurrentMarket(AppraisalMarket market)
        {
            _marketSetting.Value = market;
        }
    }
}
