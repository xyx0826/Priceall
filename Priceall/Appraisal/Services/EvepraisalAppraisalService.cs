﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Priceall.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Priceall.Appraisal
{
    class EvepraisalAppraisalService : IAppraisalService
    {
        public const string Endpoint = "https://evepraisal.com/appraisal.json";

        // private AppraisalSettings<bool> _persistSetting;

        // private AppraisalSettings[] _customSettings;

        private AppraisalMarket _market/* = AppraisalMarket.Jita*/;

        // private bool _isPersist;

        private readonly UriBuilder _uriBuilder;

        public EvepraisalAppraisalService()
        {
            // _persistSetting = new AppraisalSettings<bool>(
            //     "Persist", (isPersist) => { _isPersist = isPersist; });
            // _customSettings = new AppraisalSettings[] { _persistSetting };

            _uriBuilder = new UriBuilder(Endpoint);
        }

        private void BuildUrl()
        {
            var qs = HttpUtility.ParseQueryString(String.Empty);
            qs["market"] = _market.ToString().ToLower();
            //if (!_isPersist)
            //{
            //    qs["persist"] = "no";
            //}
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

            AppraisalResult res = new AppraisalResult(AppraisalStatus.Successful);
            res.Kind = j.SelectToken("appraisal.kind").ToObject<string>();
            res.BuyValue = j.SelectToken("appraisal.totals.buy").ToObject<double>();
            res.SellValue = j.SelectToken("appraisal.totals.sell").ToObject<double>();
            res.Volume = j.SelectToken("appraisal.totals.volume").ToObject<double>();
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

        public IReadOnlyCollection<AppraisalSettings> GetCustomSettings()
        {
            return Array.Empty<AppraisalSettings>();
            // return _customSettings;
        }

        public void SetCurrentMarket(AppraisalMarket market)
        {
            _market = market;
        }
    }
}
